//using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClienteSincrono
{

    public class TCPCliente
    {

        TcpClient socket = null;
        NetworkStream network = null;
        StreamWriter writer = null;
        StreamReader reader = null;
        public static int Main(String[] args)
        {
            TCPCliente appcliente = new TCPCliente();
            string servidor = "127.0.0.1";
            Int32 port = 13000;
            appcliente.Connect(servidor, port);
            appcliente.ControlDatos();
            appcliente.Cerrar();
            Console.Read();
            return 0;
        }
        public TCPCliente()
        {

        }
        private void Connect(String server, Int32 port)
        {
            try
            {

                this.socket = new TcpClient(server, port);
                Console.WriteLine("Socket Cliente creado.");
                network = socket.GetStream();
                Console.WriteLine(network.Socket.ToString());
                this.writer = new StreamWriter(network);
                this.reader = new StreamReader(network);
                Console.WriteLine("Buffer de escritura y lectura creados.");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private void ControlDatos()
        {
            string datouser = string.Empty;
            int contador = 0;
            Console.WriteLine(reader.ReadLine());
            while (true)
            {
                try
                {
                    //Envia y recibe texto

                    Console.WriteLine("Indica un numero:\n");
                    datouser = Console.ReadLine();
                    contador++;
                    writer.WriteLine(datouser);
                    writer.Flush();
                    datouser = reader.ReadLine();
                    Console.WriteLine(datouser);

                    if (datouser == "Has acertado")
                    {
                        Console.WriteLine("Eres el ganador del juego");
                        writer.WriteLine(contador);
                        writer.Flush();
                        break;
                    }
                    if (datouser == "Partida terminada")
                    {
                        datouser = reader.ReadLine();
                        Console.WriteLine("El ganador es el jugador: " + datouser);
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        private void Cerrar()
        {
            this.socket.Close();
            this.writer.Close();
            this.network.Close();
            this.socket.Close();
        }


    }
}
