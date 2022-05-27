using FluentFTP;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Common
{
    public class ConexionFTP
    {
        private FtpClient client;

        public string FtpServerAddress { get; set; }
        public string WorkingDir { get; set; }

        public ConexionFTP(string ftpServerAddress, string userName, string pass)
        {
            if (Conecta(ftpServerAddress, userName, pass))
            {
                Console.WriteLine($"+ Conectado al FTP: {userName}@{ftpServerAddress}");
            }
        }
        public bool Conecta(string ftpServerAddress, string userName, string pass)
        {
            FtpServerAddress = ftpServerAddress;
            client = new FtpClient(ftpServerAddress);
            client.Credentials = new NetworkCredential(userName, pass);
            client.Connect();
            return client.IsConnected;
        }

        public bool IsConnected()
        {
            return client.IsConnected;
        }

        public void Desconecta()
        {
            client.Disconnect();
            Console.WriteLine("- FTP desconectado.");
        }
        public bool CambiaDirectorio(string dirName)
        {
            bool b = false;
            if (client.IsConnected)
            {
                client.CreateDirectory(dirName);
                b = client.DirectoryExists(dirName);
                if (b)
                {
                    client.SetWorkingDirectory(dirName);
                    WorkingDir = client.GetWorkingDirectory();
                    b = (WorkingDir == dirName);
                }
            }
            return b;
        }
        public int NumeroUltimoBloque()
        {
            var listado = client.GetNameListing();
            return listado.Length;
        }
        public string ResumenBloque(int n)
        {
            string resumen = Constants.resumenInicial;
            if (n > 0)
            {
                string remotePath = WorkingDir + "/" + Constants.BloqueFileIni + n.ToString() + Constants.BloqueFileExt;
                Bloque bloque = new Bloque(client, remotePath);
                resumen = bloque.Resumen;
            }
            return resumen;
        }
        public bool SubirBloque(Bloque bloque)
        {
            string remotePath = WorkingDir + "/" + Constants.BloqueFileIni + bloque.Numero.ToString() + Constants.BloqueFileExt;
            byte[] byteArray = Encoding.ASCII.GetBytes(bloque.ToXml());
            return UploadBytes(byteArray, remotePath);
        }

        public bool UploadBytes(byte[] bytes, string remotePath)
        {
            MemoryStream stream = new MemoryStream(bytes);
            return client.Upload(stream, remotePath);
        }

        public byte[] DownloadBytes(string ftpPath)
        {
            var memoryStream = new MemoryStream();
            client.Download(memoryStream, ftpPath);
            return memoryStream.ToArray();
        }
    }
}
