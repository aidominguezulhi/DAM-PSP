using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace pipeServidor
{
    class PipeServidor
    {
        static void Main(string[] args)
        {
            try
            {
                var server = new NamedPipeServerStream("PSP01_UD01_Pipes");
                server.WaitForConnection();
                Console.WriteLine("Conexión a servidor establecida.");
                Console.WriteLine("Pipe Servidor esperando datos.");
                StreamReader reader = new StreamReader(server);
                StreamWriter writer = new StreamWriter(server);
                while (true)
                {
                    var line = reader.ReadLine();
                    Console.WriteLine("Pipe Servidor procesando datos: '{0}'",line);
                    float resultado = ProcesaOperdador(line);
                    writer.WriteLine(resultado.ToString());
                    Console.WriteLine("Pipe Servidor datos enviados: '{0}'", resultado);
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}  Apangado servidor por error", e.Message);
            }
        }

        private static float ProcesaOperdador(string operador)
        {
            float resultado = 0;
            float op1;
            float op2;
            string[] datOperador = operador.Split(' ');
            if (!Single.TryParse(datOperador[1], out op1))
            {
                Console.WriteLine("No se puede parsear a numero el operando 1 '{0}'.", op1);
            }
            if (!Single.TryParse(datOperador[2], out op2))
            {
                Console.WriteLine("No se puede parsear a numero el operando 2 '{0}'.", op2);
            }

            Console.WriteLine("Pipe Servidor operación: '{0} {1} {2}'", op1, datOperador[0], op2);
            if ("+".Equals(datOperador[0]))
            {
                resultado = op1 + op2;

            }
            else if ("-".Equals(datOperador[0]))
            {
                resultado = op1 - op2;

            }
            else if ("*".Equals(datOperador[0]))
            {
                resultado = op1 * op2;
            }
            else if ("/".Equals(datOperador[0]))
            {
                resultado = op1 / op2;
            }
            else if ("^".Equals(datOperador[0]))
            {
                resultado = 1;
                for (int i= 1; i <= op2;i++)
                    resultado = resultado * op1;
            }
            //Console.WriteLine("Ret: {0}", resultado);
            return resultado;
        }
    }
}
