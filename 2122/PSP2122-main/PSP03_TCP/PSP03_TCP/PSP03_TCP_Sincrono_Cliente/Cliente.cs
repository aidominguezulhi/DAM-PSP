using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClienteSincrono
{

  
    public class TCPCliente
    {
        //Socket Cliente
        TcpClient client = null;

        // Un stream es un flujo de datos entre un origen y un destino que se almacenan temporalmente en un buffer (como si fuera un fichero)
        //Permite intercalar operaciones de escritura(por parte del punto origen) y lectura (en el punto origen)

        NetworkStream str = null;

        //Aunque NetworkStream disponga de métodos de escritura y lectura. Existen dos clases que facilitan dicha labor. StreamReader y StreamWriter
        StreamReader sr = null;
        StreamWriter sw = null;

        public static int Main(String[] args)
        {
            TCPCliente appcliente = new TCPCliente();
            string servidor = "127.0.0.1";
            Int32 port = 13000;
            string msg = "Quiero que me conviertas a mayúscula este texto";
            appcliente.Conectar(servidor, port);
            appcliente.EnvioDatos();
            appcliente.MostrarDatos();
            appcliente.Cerrar();

            Console.WriteLine("\n Pulsa intro para continuar");
            Console.Read();
            return 0;
         
        }
        
        public TCPCliente()
        {

        }
        private void Conectar(String server, Int32 port)
        {
            try
            {

                this.client = new TcpClient(server, port);
                this.str = this.client.GetStream();
                this.sr = new StreamReader(this.str);
                this.sw = new StreamWriter(this.str);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Excepción creación de socket o buffer: {0}", e);
            }
        }
        private void EnvioDatos()
        {
            try
            {
                Console.WriteLine("Indica el texto que quieres convertir a mayúscula:");
                string data = Console.ReadLine();
                this.sw.WriteLine(data+ "<EOF>");
                this.sw.Flush();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error envio de datos: {0}", e);
            }
        }
        private void MostrarDatos()
        {
            try
            {
                string data = string.Empty;

                while (true)
                {
                    data += sr.ReadLine();
                    if (data.Contains("<EOF>"))
                    {
                        break;
                    }
                }
                Console.WriteLine(data);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error envio de recepción de datos: {0}", e);
            }
        }

        private void Cerrar()
        {
            try
            {
                this.sr.Close();
                this.sw.Close();
                this.str.Close();
                this.client.Close();
                Console.WriteLine("Todas las conexiones cerradas");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en el cierre de conexión: {0}", e);
            }
        }
    }
}