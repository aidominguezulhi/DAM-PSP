using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class MyTcpListener
{

    //ATRIBUTOS

    //Socket Listener
    TcpListener server = null;
    //Socket de comunicación con el cliente
    TcpClient client = null;

    // Un stream es un flujo de datos entre un origen y un destino que se almacenan temporalmente en un buffer (como si fuera un fichero)
    //Permite intercalar operaciones de escritura(por parte del punto origen) y lectura (en el punto origen)

    NetworkStream str = null;

    //Aunque NetworkStream disponga de métodos de escritura y lectura. Existen dos clases que facilitan dicha labor. StreamReader y StreamWriter
    StreamReader sr = null;
    StreamWriter sw = null;

    
    //CONSTRUCTOR
    public MyTcpListener()
    {
    }

    private void Conectar(IPAddress server, Int32 port)
    {
        try
        {

            this.server = new TcpListener(server, port);
                
            // Start listening for client requests.
            this.server.Start();
            this.client = this.server.AcceptTcpClient(); //linea bloqueante (espera hasta que se conecta algún cliente).

            this.str = this.client.GetStream();
            this.sr = new StreamReader(this.str);
            this.sw = new StreamWriter(this.str);

        }
        catch (Exception e)
        {
            Console.WriteLine("Excepción creación de socket o buffer: {0}", e);
        }
    }

    private void EnvioRecepcionDatos()
    {
        string data=string.Empty;
        try
        {
            while(true)
            {
                data += sr.ReadLine();
                if (data.Contains("<EOF>"))
                {
                    break;
                }
            }
            this.sw.WriteLine(data.ToUpper());
            this.sw.Flush();
            Console.WriteLine(data.ToUpper());

        }
        catch (Exception e)
        {
            Console.WriteLine("Error envio de datos: {0}", e);
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

    public static int Main(String[] args)
    {

        // Set the TcpListener on port 13000.
        MyTcpListener appservidor = new MyTcpListener();

        Int32 port = 13000;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        appservidor.Conectar(localAddr, port);
        appservidor.EnvioRecepcionDatos();
        appservidor.Cerrar();


        Console.WriteLine("\nPulsa intro para continuar...");
        Console.Read();
        return 0;
    }
}
