using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Common
{
    [Serializable]
    public class Bloque
    {

        public List<Operacion> Operaciones { get; set; }
        public string Relleno { get; set; }
        public string Resumen { get; set; }
        public string Previo { get; set; }

        [NonSerialized]
        public int Numero;

        public Bloque()
        {
            this.Numero = 1;
            this.Previo = Constants.resumenInicial;
            Operaciones = new List<Operacion>();
        }

        public Bloque(int numero, string previo)
        {
            this.Numero = numero;
            this.Previo = previo;
            Operaciones = new List<Operacion>();
        }

        // Constructor que crea una instancia apartir del fichero en el FTP deserializándolo:
        public Bloque(FtpClient client,  string ftpPath)
        {
            MemoryStream stream = new MemoryStream();
            client.Download(stream, ftpPath);
            string xml = client.Encoding.GetString(stream.GetBuffer());
            Bloque bloqueftp;
            using (TextReader reader = new StringReader(xml))
            {
                bloqueftp = (Bloque)new XmlSerializer(typeof(Bloque)).Deserialize(reader);
            }

            this.Numero = bloqueftp.Numero;
            this.Previo = bloqueftp.Previo;
            this.Operaciones = bloqueftp.Operaciones;
            this.Relleno = bloqueftp.Relleno;
            this.Resumen = bloqueftp.Resumen;
        }

        public Bloque Clone()
        {
            Bloque bloque = new Bloque();
            bloque.Numero = Numero;
            bloque.Relleno = Relleno;
            bloque.Resumen = Resumen;
            bloque.Previo = Previo;
            bloque.Operaciones = new List<Operacion>();
            int n = Operaciones.Count;
            for (int i = 0; i < n; i++)
            {
                bloque.Operaciones.Add(Operaciones[i].clone());
            }
            return bloque;
        }

        public int GetSize()
        {
            return Operaciones.Count;
        }
        public bool IsLleno()
        {
            return (Constants.opsXblock <= Operaciones.Count);
        }

        public void AddOp(Operacion op)
        {
            Operaciones.Add(op);
        }


        public void Minar()
        {
            Console.WriteLine($" +x+x+ Bloque {Numero} minando.");
            StringBuilder sb = new StringBuilder(Previo);
            for (int i = 0; i < Constants.opsXblock; i++)
            {
                sb.Append(Operaciones[i]);
            }
            string s = sb.ToString();
            MD5 md5 = MD5.Create();
            long r = 0;
            string hash;
            do
            {
                string str = s + r.ToString();
                hash = Utils.GetMd5HashHex(md5, str);
                r++;
            } while (!hash.StartsWith(Constants.minadoMinimo));
            Relleno = r.ToString();
            Resumen = hash;
            Console.WriteLine($" -x-x- Bloque {Numero} minado.");
        }

        public void Next()
        {
            Operaciones = new List<Operacion>();
            Previo = Resumen;
            Relleno = "";
            Resumen = "";
            Numero = Numero + 1;
        }
        public string ToXml()
        {
            using (StringWriter writer = new StringWriter())
            {
                XmlSerializer xml = new XmlSerializer(typeof(Bloque));
                xml.Serialize(writer, this);
                return writer.ToString();
            }
        }
    }

}