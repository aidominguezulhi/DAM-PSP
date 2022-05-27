using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Common
{
    public class Utils
    {

        public static byte[] EncryptOperacion(Operacion op, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(op.ToXml());
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        public static Operacion DecryptBytes(byte[] data, byte[] Key, byte[] IV)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            string xml = srDecrypt.ReadToEnd();
                            StringReader sr = new StringReader(xml);
                            Operacion op = (Operacion)new XmlSerializer(typeof(Operacion)).Deserialize(sr);
                            return op;
                        }
                    }
                }
            }
        }

        public static string GetMd5HashHex(MD5 md5, string input)
        {
            return GetMd5HashHex(md5, Encoding.UTF8.GetBytes(input));
        }

        public static string GetMd5HashHex(MD5 md5, byte[] sign)
        {
            byte[] data = md5.ComputeHash(sign);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
