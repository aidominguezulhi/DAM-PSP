using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClienteSincrono
{
    public class Partida
    {
        public bool enJuego;
        public int ganador;

        public Partida()
        {
            this.enJuego = true;

        }

    }
    public class TCPServidor
    {
        //ublic static Jugador[] jugador = new Jugador[10];
        public static int num;

        private Object o = new object();
        public static int Main(String[] args)
        {
            int cnt = 0;
            //Inicialización de variables

            TcpClient socketcliente = null;
            NetworkStream network = null;
            StreamWriter writer = null;
            StreamReader reader = null;


            //int id = 0;
            //Preparación de datos para el listener
            TCPServidor appserver = new TCPServidor();
            string servidor = "127.0.0.1";
            IPAddress ipserver = IPAddress.Parse(servidor);
            Int32 port = 13000;




            TcpListener listener = new TcpListener(ipserver, port);
            Console.WriteLine("Socket listener creado");
            listener.Start(10);

            Random rnd = new Random();
            num = rnd.Next(1, 101);
            Partida partida = new Partida();
            Console.WriteLine("Numero a acertar es {0}", num);

            //Se crea un thread por cada cliente que se conecte
            while (cnt < 10)
            {
                int id = 1;
                socketcliente = listener.AcceptTcpClient(); //linea bloqueante
                Console.WriteLine("Conexión con cliente establecida.");
                Thread t = new Thread(() => appserver.ControlDatos(socketcliente, id, partida));

                t.Start();
                cnt++;
            }

            socketcliente.Close();

            Console.Read();
            return 0;
        }
        public TCPServidor()
        {

        }

        private void ControlDatos(TcpClient socket, int id, Partida partida)
        {
            //Random rnd = new Random();
            int numId = socket.Client.Handle.ToInt32();
            string contador;

            NetworkStream network = socket.GetStream();
            StreamWriter writer = new StreamWriter(network);
            StreamReader reader = new StreamReader(network);
            
            Console.WriteLine("Buffer de entrada y salida creados");
            Console.WriteLine("id Jugador = {0}", numId);
            writer.WriteLine("Soy el jugador numero " + numId);
            writer.Flush();

            string data = string.Empty;

            int cNum;
            try
            {

                while (true)
                {
                    data = reader.ReadLine();
                    cNum = Int32.Parse(data);
                    Console.WriteLine(cNum);

                    if (partida.enJuego == false)
                    {
                        writer.WriteLine("Partida terminada");
                        writer.Flush();
                        writer.WriteLine(partida.ganador);
                        writer.Flush();
                    }
                    else
                    {
                        lock (o)
                        {

                            if (cNum > num)
                            {

                                writer.WriteLine("El numero es mas pequeño");
                                writer.Flush();

                            }
                            else if (cNum < num)
                            {
                                writer.WriteLine("El numero es mas grande");
                                writer.Flush();

                            }
                            else if (cNum == num)
                            {

                                partida.ganador = numId;
                                writer.WriteLine("Has acertado");
                                writer.Flush();
                                contador = reader.ReadLine();
                                Console.WriteLine("Has acertado, ZORIONAK!!");
                                Console.WriteLine("Numero de intentos realizados por ti: {0}", contador);
                                Console.WriteLine("Fin de la partida");
                                partida.enJuego = false;
                                break;

                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            socket.Close();
            writer.Close();
            network.Close();
            reader.Close();


        }


    }
}

