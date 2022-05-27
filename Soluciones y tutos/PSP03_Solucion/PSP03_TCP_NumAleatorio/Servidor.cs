using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClienteMultiple
{

    public class TCPServidor
    {
        
        public int jugadas = 0;
        private Object o = new object();

        private string datotratar = string.Empty;
        public static int Main(String[] args)
        {
            TCPServidor appserver = new TCPServidor();
            Partida p = new Partida();

            int random = p.getNumAleatorio();
            //Inicialización de variables

            TcpClient socketcliente = null;
            NetworkStream network = null;
            StreamWriter writer = null;
            StreamReader reader = null;
            

            //Preparación de datos para el listener
            
            string servidor = "127.0.0.1";
            IPAddress ipserver = IPAddress.Parse(servidor);
            Int32 port = 13000;

            //appserver.NumAleatorio();
            Console.WriteLine("El numero aleatorio es:{0}", p.random.ToString());


            TcpListener listener = new TcpListener(ipserver, port);
            Console.WriteLine("Socket lister creado");
            listener.Start(10);
            

            while (true)
            {
                socketcliente = listener.AcceptTcpClient(); //linea bloqueante
                Console.WriteLine("Conexión con cliente establecida.");
               
                Thread t = new Thread(() => appserver.ControlDatos(socketcliente, p));
                t.Start();
            }

            socketcliente.Close();

            Console.WriteLine("\n Fin del juego");
            Console.Read();
            return 0;
        }
        public TCPServidor()
        {
            
        }
        
        private void ControlDatos(TcpClient socket, Partida partida)
        {
  
            NetworkStream network = socket.GetStream();
            StreamWriter writer = new StreamWriter(network);
            StreamReader reader = new StreamReader(network);
            Console.WriteLine("Buffer de entrada y salida creados.");
            partida.setParticipante(socket.Client.Handle);
            Console.WriteLine("IdentificadorCliente: {0}.", socket.Client.Handle);
            datotratar = socket.Client.Handle.ToString();
            writer.WriteLine(datotratar);
            writer.Flush();
            int judadas = partida.getJugadasTotales();
           

            try
            {
                writer.WriteLine("Intenta adivinar mi número:");
                writer.Flush();
                
                while (true)
                {
                    try
                    {
                        datotratar = reader.ReadLine(); //linea bloqueante
                        Console.WriteLine(datotratar);
         
                        lock (o)
                        {

                            if (!(partida.getGanador().Equals(IntPtr.Zero)))
                            {

                                Console.WriteLine(partida.numCliente(partida.getGanador()));
                                Console.WriteLine("GANADOR");
                                Console.WriteLine("Identificador: {0}", partida.getGanador());
                                Console.WriteLine("Cliente: {0}", partida.numCliente(partida.getGanador()));
                                writer.WriteLine("GANADOR");
                                writer.Flush();
                                writer.WriteLine(partida.getGanador().ToString());
                                writer.Flush();
                                break;
                            }
                            else 
                            {
                                int i;
                                if (Int32.Parse(datotratar) == partida.random)
                                {
                                    writer.WriteLine("ACERTADO");
                                    writer.Flush();
                                    judadas = partida.getJugadasTotales();
                                    partida.setJugadas(judadas++);
                                    partida.setGanador(socket.Client.Handle);
                                    break;
                                }
                                else if (Int32.Parse(datotratar) > partida.random)
                                {
                                    writer.WriteLine("MENOR");
                                    writer.Flush();
                                    judadas = partida.getJugadasTotales();
                                    partida.setJugadas(judadas++);
                                }

                                else if (Int32.Parse(datotratar) < partida.random)
                                {
                                    writer.WriteLine("MAYOR");
                                    writer.Flush();
                                    judadas = partida.getJugadasTotales();
                                    partida.setJugadas(judadas++);
                                }
                                else if (!Int32.TryParse(datotratar, out i))
                                {
                                    writer.WriteLine("NUMEROS");
                                    writer.Flush();
                                    break;
                                }

                            }

                        }
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.Out.Flush();
                    }
                    
                    
                }
                datotratar = reader.ReadLine();

                Console.WriteLine("Has acertado!!Zorionak!");
                writer.WriteLine(jugadas.ToString());


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                writer.Close();
                network.Close();
                reader.Close();
            }
           

        }


    }
}