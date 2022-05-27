using Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Servidor
{
    class Servidor
    {

        public static DataBase db = new DataBase();
        public static Bloque bloque = new Bloque();
        public static ConexionFTP ftp;
        private readonly TcpListener listener;

        private static readonly object db_lock = new object();
        private static readonly object bloque_lock = new object();
        private static readonly object ftp_lock = new object();

        private static byte[] aesKey;
        private static byte[] aesIV;

        public Servidor(IPAddress address, int port, Bloque bloque_comienzo, byte[] key, byte[] IV)
        {
            listener = new TcpListener(address, port);
            bloque = bloque_comienzo;
            aesIV = IV;
            aesKey = key;
        }

        public void StartAsync(ConexionFTP ftp_conectado)
        {
            ftp = ftp_conectado;

            if (ftp.IsConnected())
            {
                ftp.CambiaDirectorio(Constants.ftpBloquesPath);
                try
                {
                    // Arranca servidor TCP:
                    listener.Start();
                    while (true)
                    {
                        var client = listener.AcceptTcpClient();
                        Task t = Task.Factory.StartNew(GestionarOperacion, client);
                        // Si descomentamos la siguiente línea convertimos el servidor en síncrono:
                        //t.Wait();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                }
                ftp.Desconecta();
            }
        }

        Action<object> GestionarOperacion = (object client) =>
        {
            using (var clienteTCP = (TcpClient)client)
            using (var stream = clienteTCP.GetStream())
            {
                // Cliente conectado.
                Operacion op = null;

                XmlSerializer serializer = new XmlSerializer(typeof(byte[]));
                if (stream.CanRead)
                {
                    using (var xr = XmlReader.Create(stream))
                    {
                        Console.WriteLine($" ...Petición de operación (+1s delay) en marcha (id={Task.CurrentId})");
                        Thread.Sleep(1000);
                        // Recepción 
                        byte[] op_enc = (byte[])serializer.Deserialize(xr);
                        op = Utils.DecryptBytes(op_enc, aesKey, aesIV);
                        Operar(op);
                        Registrar(op);
                        Console.WriteLine($" ...Operación realizada (id={Task.CurrentId}).");
                    }
                }

                // Envia respuesta.
                if (stream.CanWrite)
                {
                    byte[] op_enc = Utils.EncryptOperacion(op, aesKey, aesIV);
                    serializer.Serialize(stream, op_enc);
                    clienteTCP.Client.Shutdown(SocketShutdown.Both);
                }
            }
        };


        public static void Operar(Operacion op)
        {
            lock (db_lock)
            {
                if (db.Transferir(op))
                {
                    op.Estado = true;
                }
                else
                {
                    op.Estado = false;
                }
            }
        }

        public static void Registrar(Operacion op)
        {
            if (op.Estado)
            {
                Bloque bup = null;
                lock (bloque_lock)
                {
                    if (!bloque.IsLleno())
                    {
                        bloque.AddOp(op);
                        if (bloque.IsLleno())
                        {
                            bloque.Minar();//síncrono
                            bup = bloque.Clone();
                            bloque.Next();//nueva instancia de bloque
                        }
                    }
                }
                //------- ALMACENAR -------
                //Sube un clon del bloque para que otros hilos puedan utilizar la referencia al bloque mientras se sube al FTP
                if (bup != null)
                {
                    lock (ftp_lock)
                    {
                        ftp.SubirBloque(bup);
                        Console.WriteLine($" + Bloque {bup.Numero} cargado al FTP.\n");
                    }
                }
                //-------------------------
            }
        }

    }
}