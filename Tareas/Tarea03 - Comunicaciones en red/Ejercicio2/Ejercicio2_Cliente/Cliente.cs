using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PSP03_Socket_TCP
{
    internal class Cliente
    {
        public static int Main(String[] args)
        {

            Cliente servidor = new Cliente();
            servidor.FuncionServidor();

            Console.WriteLine("Pulse intro para continuar");
            Console.ReadLine();

            return 0;
        }
        private void FuncionServidor()
        {
            Socket sender = null;

            try
            {
                int port = 12000;
                string data = null;
                byte[] bytes = new Byte[4096];

                Random random = new Random();

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[5];
                sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine("Programa cliente iniciando.\n");

                //Conexión de socket al servidor
                IPEndPoint iPEndPoint = new IPEndPoint(ipAddress.Address, port); //Indicamos IP de servidor y puerto del servidor
                sender.Connect(iPEndPoint); //Se establece la conexión
                Console.WriteLine("Socket conectado a servidor {0}\n", sender.RemoteEndPoint.ToString()); //Mostramos por pantalla que todo ha ido correcto



                //Recepción de información
                Console.WriteLine("Cliente transfiriendo datos.\n");




                //Esperamos la respuesta del servidor
                while (true)
                {
                    Console.WriteLine("1.-FotoMonte");
                    Console.WriteLine("2.-FotoPlaya");
                    Console.WriteLine("3.-FotoCiudad");
                    Console.WriteLine("4.-Salir");

                    data = string.Empty;
                    data = Console.ReadLine();

                    if (data == "4")
                    {
                        break;
                    }

                    byte[] msg = Encoding.ASCII.GetBytes(data); //Añadimos fin de fichero al texto
                    sender.Send(msg); //Enviamos el texto

                    sender.Receive(bytes);
                    Console.WriteLine("Recibiendo imagen");
                    Console.WriteLine("¿Que nombre le quieres dar a la imagen descargada?");
                    string nombre = Console.ReadLine();
                    string path = @"../../../../fotos/";
                    string filename = path + nombre + ".jpg";


                    File.WriteAllBytes(filename, bytes);
                    Console.WriteLine("El directorio donde se ha guardado es {0}", Path.GetFullPath(filename));
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                //Cerramos el socket
                sender.Close();

            }



        }
    }
}