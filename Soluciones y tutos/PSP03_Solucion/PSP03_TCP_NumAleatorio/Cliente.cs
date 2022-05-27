using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClienteMultiple
{

    public class TCPCliente
    {

        TcpClient socket = null;
        NetworkStream network = null;
        StreamWriter writer = null;
        StreamReader reader = null;    
        public static int Main(String[] args)
        {
            PartidaCliente partidaCliente = new PartidaCliente();
            TCPCliente appcliente = new TCPCliente();
            string servidor = "127.0.0.1";
            Int32 port = 13000;

            appcliente.Connect(servidor, port, partidaCliente);
            appcliente.ControlDatos(partidaCliente);
            appcliente.Cerrar();

            Console.WriteLine("\n Fin del juego");
            Console.Read();
            return 0;
        }
        public TCPCliente()
        {

        }
        private void Connect(String server, Int32 port, PartidaCliente partidaC)
        {
            try
            {
               
                this.socket = new TcpClient(server, port);
                Console.WriteLine("Socket Cliente creado.");
                network = socket.GetStream();
                this.writer = new StreamWriter(network);
                this.reader = new StreamReader(network);
                Console.WriteLine("Buffer de escritura y lectura creados.");

                string datouser = string.Empty;

                datouser = reader.ReadLine();
                partidaC.setIdentificador(Int32.Parse(datouser));
                Console.WriteLine("Identificador de cliente: {0} ", datouser);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private void ControlDatos(PartidaCliente partidaC)
        {
            string datouser = string.Empty;
            int num = int.MaxValue;
            string[] subs = null;
            try
            {
                //Muestra el inicio del juego
                datouser = reader.ReadLine();
                Console.WriteLine(datouser);
                datouser = Console.ReadLine();

                //Recoge la primera cifra
                writer.WriteLine(datouser);
                writer.Flush();
                int judadas = partidaC.getJugadas();
                judadas++;
                partidaC.setJugadas(judadas);
                Console.WriteLine(judadas);

                do
                {
                    datouser = reader.ReadLine();
                    //subs = datouser.Split(' ');
                    if (datouser.Equals("MAYOR"))
                    {
                        Console.WriteLine("El número es mayor:");
                        datouser = Console.ReadLine();
                        
                        writer.WriteLine(datouser);
                        writer.Flush();
                        judadas = partidaC.getJugadas();
                        judadas++;
                        partidaC.setJugadas(judadas);
                        Console.WriteLine(judadas);
                    }
                    else if (datouser.Equals("MENOR"))
                    {
                        Console.WriteLine("El número es menor:");
                        datouser = Console.ReadLine();
                        writer.WriteLine(datouser);
                        writer.Flush();
                        judadas = partidaC.getJugadas();
                        judadas++;
                        partidaC.setJugadas(judadas);
                        Console.WriteLine(judadas);
                    }
                    else if (datouser.Contains("GANADOR"))
                    {
                        writer.WriteLine(datouser);
                        writer.Flush();
                        datouser = reader.ReadLine();
                        writer.WriteLine(datouser);
                        writer.Flush();
                        Console.WriteLine("El ganador es: {0}. ", datouser);

                        break;
                    }
                    else if (datouser.Equals("ACERTADO"))
                    {
                        
                        datouser = "ACERTADO";
                        Console.WriteLine("Has acertado!!Zorionak!");
                        writer.WriteLine(datouser);
                        writer.Flush();
                        Console.WriteLine("Numero de jugadas realizadas por ti: {0}. ", partidaC.getJugadas());

                        break;
                    }
                   
                    else if (subs[0].Equals("NUMERO"))
                    {
                        Console.WriteLine("Sólo se permiten números");
                        break;
                    }
                } while (true); //!datouser.Equals("ACERTADO") || !datouser.Equals("NUMERO")
                Console.WriteLine("*****************");
                Console.WriteLine("Fin de la partida.");
                Console.WriteLine("*****************");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Out.Flush();
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