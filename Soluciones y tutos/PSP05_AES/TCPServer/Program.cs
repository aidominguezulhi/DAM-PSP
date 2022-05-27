using Common;
using System;
using System.IO;
using System.IO.Pipes;
using System.Net;

namespace Servidor
{
    class Program
    {
        private static readonly ConexionFTP ftp = new ConexionFTP(Constants.ftpHost, Constants.ftpUser, Constants.ftpPass);
        public static int Main()
        {
            byte[] aesKey, aesIV;
            if (ftp.IsConnected() )
            {
                if (RecivirAES_pipe(out aesKey, out aesIV)) {
                    Bloque bloque_comienzo = GetPrimerBloque(ftp);
                    //Escucha de TCP:  
                    Servidor srv = new Servidor(IPAddress.Any, Constants.tcpPort, bloque_comienzo, aesKey, aesIV);
                    srv.StartAsync(ftp);

                    ftp.Desconecta();
                }
                else
                {
                    Console.WriteLine("*** ERROR: No se ha conseguido crear y compartir AES (key+IV) con el servidor TCP.");
                }
            }
            else
            {
                Console.WriteLine("*** ERROR: Compruebe la conexión FTP al servidor {0} al directorio {1} con el usuario {2}", Constants.ftpHost, Constants.ftpBloquesPath, Constants.ftpUser);
            }
            Console.WriteLine("FIN PROGRAMA SERVIDOR");
            return 0;
        }

        private static Bloque GetPrimerBloque(ConexionFTP ftp)
        {
            Bloque bloque = null;
            //Obtención de la información del servidor FTP
            if (ftp.IsConnected())
            {
                if (ftp.CambiaDirectorio(Constants.ftpBloquesPath))
                {
                    int n = ftp.NumeroUltimoBloque();
                    string resumen = ftp.ResumenBloque(n);
                    bloque = new Bloque(n + 1, resumen);
                    Console.WriteLine($"Último bloque en el FTP, {n} con resumen {resumen}");
                }
                else
                {
                    Console.WriteLine("*** ERROR: No se puede cambiar al directorio {0}", Constants.ftpBloquesPath);
                }
            }
            else
            {
                Console.WriteLine("*** ERROR: No se puede conectar al servidor FTP {0}", Constants.ftpHost);
            }

            return bloque;
        }


        private static bool RecivirAES_pipe(out byte[] aesKey, out byte[] aesIV)
        {
            aesIV = null;
            aesKey = null;
            bool communication_ok = false;
            using (NamedPipeServerStream pipe_srv = new NamedPipeServerStream(Constants.msgPipe))
            {
                pipe_srv.WaitForConnection();
                string msg_in = new StreamReader(pipe_srv).ReadLine();
                string msg_out = Constants.msgServerAccepts;
                Console.WriteLine("\n---------------------------------------------------------------");
                Console.WriteLine(msg_in);
                StreamWriter writer = new StreamWriter(pipe_srv);
                if (msg_in.Equals(Constants.msgAesShared))
                {
                    aesKey = ftp.DownloadBytes(Constants.remotePathKey);
                    aesIV = ftp.DownloadBytes(Constants.remotePathIV);
                    communication_ok = true;
                }
                else
                {
                    msg_out = Constants.msgServerNotAllowed;
                }
                writer.WriteLine(msg_out);
                writer.Flush();
                Console.WriteLine("---------------------------------------------------------------\n");
            }
            return communication_ok;
        }
    }
}
