using Common;
using FluentFTP;
using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Cliente
{
    public class Cliente
    {
        private static byte[] aesKey;
        private static byte[] aesIV;

        static void Main(string[] args)
        {
            GenerarAES();

            ComunicarAESsubido();


            //Simula diez clientes
            int N = 10;
            Task[] tasks = new Task[N];
            for (int j = 0; j < N; j++)
            {
                tasks[j] = Task.Run(() =>
                {
                    Cliente cliente = new Cliente();
                    //Los diez clientes envían diez operaciones cada uno
                    for (int i = 0; i < 10; i++)
                    {
                        Operacion op = new Operacion("uno", "dos", 100, "");
                        try
                        {
                            cliente.Transaccion(op);
                        } catch (Exception e)
                        {
                            Console.WriteLine(" !! ERROR durante la transacción de la operación: \n\t"+ op.ToString());
                        }
                    }
                });
            }
            //Esperar a terminar los diez clientes (para ver el resultado de los mensajes mejor)
            for (int j = 0; j < N; j++)
            {
                tasks[j].Wait();
            }
            //Un nuevo cliente que da "problemas", no hay saldo suficiente.
            Task.Run(() =>
            {
                Cliente cliente = new Cliente();
                //Este envío produce un error si el saldo de "uno" era inicialmente 10000 y ya se había transferido a "dos" esa cantidad
                Operacion opX = new Operacion("uno", "dos", 100, "");
                try
                {
                    cliente.Transaccion(opX);
                }
                catch (Exception e)
                {
                    Console.WriteLine(" !! ERROR durante la transacción de la operación: \n\t" + opX.ToString());
                }
                //Este otro es para ver la continuidad del programa y los saldos
                Operacion op = new Operacion("dos", "uno", 100, "");
                try
                {
                    cliente.Transaccion(op);
                }
                catch (Exception e)
                {
                    Console.WriteLine(" !! ERROR durante la transacción de la operación: \n\t" + op.ToString());
                }
                cliente.Transaccion(op);
            });
            //Espera pulsar enter
            Console.Read();
        }

        private static bool ComunicarAESsubido()
        {
            bool communication_ok = false;
            using (NamedPipeClientStream pipe_client = new NamedPipeClientStream(Constants.msgPipe))
            {
                pipe_client.Connect();
                StreamWriter writer = new StreamWriter(pipe_client);
                writer.WriteLine(Constants.msgAesShared);
                writer.Flush();
                string msg_in = new StreamReader(pipe_client).ReadLine();
                if (msg_in.Equals(Constants.msgServerAccepts))
                {
                    communication_ok = true;
                }
            }
            return communication_ok;
        }

        private static void GenerarAES()
        {
            using (Aes aes = Aes.Create())
            {
                ConexionFTP ftp = new ConexionFTP(Constants.ftpHost, Constants.ftpUser, Constants.ftpPass);
                aesKey = aes.Key;
                ftp.UploadBytes(aesKey, Constants.remotePathKey);
                aesIV = aes.IV;
                ftp.UploadBytes(aesIV, Constants.remotePathIV);
                ftp.Desconecta();
            }
        }

        private void Transaccion(Operacion op)
        {

            using (var client = new TcpClient())
            {
                // Conexión  
                client.Connect(Constants.tcpHost, Constants.tcpPort);
                using (var stream = client.GetStream())
                {
                    // Envío  
                    XmlSerializer serializer = new XmlSerializer(typeof(byte[]));
                    if (stream.CanWrite)
                    {
                        byte[] op_enc = Utils.EncryptOperacion(op, aesKey, aesIV);
                        serializer.Serialize(stream, op_enc);
                        // Termina el envio:
                        client.Client.Shutdown(SocketShutdown.Send);
                    }

                    if (stream.CanRead)
                    {
                        // Recepción 
                        //byte[] opr_enc = Utils.ReceiveFromNStream(stream);
                        using (var xr = XmlReader.Create(stream))
                        {
                            // Recepción 
                            byte[] opr_enc = (byte[])serializer.Deserialize(xr);
                            Operacion opr = Utils.DecryptBytes(opr_enc, aesKey, aesIV);

                            // Salida  
                            if (!opr.Estado)
                            {
                                Console.WriteLine($"\nOperación fallida: {opr.ToString()}");
                            }
                            else
                            {
                                Console.Write(".");

                            }
                        }
                    }


                    stream.Close();
                    client.Close();
                }
            }
        }
    }
}
