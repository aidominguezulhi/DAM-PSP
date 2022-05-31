using System;
using System.Net;
namespace ComunicacionPrimerosPaso
{
    class Program
    {
        static void Main(string[] args)
        {
            //Recoge la dirección ip de google
            IPHostEntry infoHost = Dns.GetHostEntry("www.google.es");
            IPAddress ipAddress = infoHost.AddressList[0];
            Console.WriteLine("La direcion de google es: {0}", ipAddress.ToString());

            //Recoge la dirección ip de la máquina local
            infoHost = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = infoHost.AddressList[1];
            Console.WriteLine("La direcion de esta maquina es: {0}", ipAddress);

        }
    }
}