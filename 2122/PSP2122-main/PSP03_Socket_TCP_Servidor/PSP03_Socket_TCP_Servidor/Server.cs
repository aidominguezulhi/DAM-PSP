using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace PSP03_SocketClass_TCP_Servidor
{
    internal class Server
    {
        //ATRIBUTOS

        private Socket listener = null;
        private int port = 12000;
        private IPAddress ipAddress = null;
        private Socket handler = null;

        //CONSTRUCTOR

        public Server()
        {

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            this.ipAddress = ipHostInfo.AddressList[1];
            this.listener = new Socket(this.ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Programa servidor iniciando.");
        }

        //MÉTODOS
        //establecerConexión establece la conexión con el equipo remoto
        public void esperandoConexion()
        {
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(this.ipAddress, this.port);
                listener.Bind(localEndPoint);
                listener.Listen(10);
                Console.WriteLine("Puerto escuchando a peticiones de cliente");
                Socket handler1 = listener.Accept();
                this.handler = handler1;
                Console.WriteLine("Aceptada la conexión con el  cliente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error abriendo puerto de escucha y realizando conexión {0}", ex.ToString());

            }
            

        }

        public void transfiendoinfo()
        {

            try
            {
                byte[] msg = Encoding.ASCII.GetBytes("El servidor ha recibido el mensaje correctamente.");
                this.handler.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando datos al cliente {0}", ex.ToString());
                
            }
            


        }
        public string recibiendoInfo()
        {
            string data = null;
            byte[] bytes = new Byte[1024];

            try
            {
                while (true)
                {
                    int bytesRec = this.handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error recibiendo datos al cliente {0}", ex.ToString());
                
            }
            
            return data;
        }
        public void cerrarServidor()
        {
            //Deja de enviar y recibir datos
            this.handler.Shutdown(SocketShutdown.Both);

            //Cierra la conexión de socket.
            this.handler.Close();

        }
    }
}
