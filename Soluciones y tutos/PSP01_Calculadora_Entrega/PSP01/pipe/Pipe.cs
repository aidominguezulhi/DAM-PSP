using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace pipe
{
    class Pipe
    {
      
        static void Main(string[] args)
        {
            Process p;
            StartServer(out p);
            Task.Delay(1000).Wait();
            string operacion;

            //Preparar conexion del cliente
            var client = new NamedPipeClientStream("PSP01_UD01_Pipes");
            client.Connect();
            StreamReader reader = new StreamReader(client);
            StreamWriter writer = new StreamWriter(client);

            while (true)
            {
                operacion = "";
                PintarMenu(ref operacion);
                //Console.WriteLine(" operaccion: " + operacion);

                if (operacion != null)
                {
                    if ("E".Equals(operacion))
                    {
                        // Salir del while (true)
                        break;
                    }
                    writer.WriteLine(operacion);
                    writer.Flush();
                    Console.WriteLine("Resultado: {0}", reader.ReadLine());
                }
            }
            //Parar pipes
            if (p != null && !p.HasExited)
            {
                p.Kill();
                p = null;
            }
        }
        
        static Process StartServer(out Process p1)
        {
            // iniciar un proceso con el servidor y devolver
            ProcessStartInfo info = new ProcessStartInfo(@"..\..\..\..\pipeServidor\bin\Release\netcoreapp3.1\win-x64\pipeServidor.exe");
            
            // su valor por defecto el false, si se establece a true no se "crea" ventana
            info.CreateNoWindow = false;
            info.WindowStyle = ProcessWindowStyle.Normal;
            // indica si se utiliza el cmd para lanzar el proceso
            info.UseShellExecute = true;
            p1 = Process.Start(info);
            return p1;
        }


        private static void PintarMenu(ref string operacion)
        {
            Console.WriteLine("Operación a realizar:");
            Console.WriteLine(" 1 - Suma");
            Console.WriteLine(" 2 - Resta");
            Console.WriteLine(" 3 - Multiplicar");
            Console.WriteLine(" 4 - Dividir");
            Console.WriteLine(" 5 - Potencia");
            Console.WriteLine(" (-1) Salir");
            Console.Write("Introduzca la operación: ");
            string input = Console.ReadLine();
            //int op;
            bool ret = ValidaOperacion(input, ref operacion);//, out op);
        }

        private static bool ValidaOperacion(string cadenaIn, ref string operacion)
        {
            int opcion;
            if (!Int32.TryParse(cadenaIn, out int op))
            {
                Console.WriteLine("Opción {0} no válida.", op);
                //opcion = op;
                operacion = null;
                return false;
            } else
            {
                opcion = op;
            }
            switch (opcion)
            {
                case 1: // Suma
                    operacion = operacion + "+ ";
                    IntroducirOperandos(ref operacion);
                    break;
                
                case 2: // Resta
                    operacion = operacion + "- ";
                    IntroducirOperandos(ref operacion);
                    break;

                case 3: // Multiplicación
                    operacion = operacion + "* ";
                    IntroducirOperandos(ref operacion);
                    break;
                case 4: // División
                    operacion = operacion + "/ ";
                    IntroducirOperandos(ref operacion);
                    break;
                case 5: // Potencia
                    operacion = operacion + "^ ";
                    IntroducirOperandos(ref operacion);
                    break;
                case -1: // Salir
                    operacion = operacion + "E";
                    Console.WriteLine("Exit");
                break;

                default: // Error de opcion
                    Console.WriteLine("Opción {0} no válida.", op);
                    operacion = null;
                break;

            }
            return false;
        }

        private static bool IntroducirOperandos(ref string operacion)
        {
            Console.Write("Introduce el primer operando :");
            string input = Console.ReadLine();
            if (!Single.TryParse(input, out float op))
            {
                Console.WriteLine("El primer operando NO es un número");
                operacion = null;
                return false;
            }
            Console.Write("Introduce el segundo operando :");
            string input2 = Console.ReadLine();
            if (!Single.TryParse(input2, out float op2))
            {
                Console.WriteLine("El segundo operando NO es un número");
                operacion = null;
                return false;
            }
            operacion = operacion + input + " " + input2;
            return true;
        }
    }
}
