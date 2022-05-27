
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace CenaFilosofos
{
    class Program
    {
        private const int CANTIDAD_FILOSOFOS = 5;

        static void Main(string[] args)
        {         
            //inicializar filosofos
            var filos = InicializarFilosofos();
            
            Console.WriteLine("Comienza la comida");

            //crear los hilos para cada filosofo
            var hiloFilos = new List<Thread>();

            foreach (var filo in filos)
            {
                var hiloFilo = new Thread(new ThreadStart(filo.CicloComer));
                hiloFilos.Add(hiloFilo);
                hiloFilo.Start();
            }

            foreach (var hilo in hiloFilos)
            {
                hilo.Join();
            }

            Console.WriteLine("La comida ha finalizado!");
        }

        private static List<Filosofo> InicializarFilosofos()
        {
            
            var filos = new List<Filosofo>(CANTIDAD_FILOSOFOS);
            for (int i = 0; i < CANTIDAD_FILOSOFOS; i++)
            {
                filos.Add(new Filosofo(filos, i));
            }

            foreach (var filo in filos)
            {
                filo.palilloDerecho = filo.filoDerecha.palilloIzquierdo ?? new Palillos();
                filo.palilloIzquierdo = filo.filoIzquierda.palilloDerecho ?? new Palillos();
            }

            return filos;
        }
        
    }

    public enum Estado
    {
        Pensando = 0,
        Comiendo = 1,
        Intentando = 2,
        Hambriento = 3,
    }

    [DebuggerDisplay("Nombre = {Nombre}")]
    public class Filosofo
    {
        private const int CANTIDAD_COMIDAS = 5;
        
        private readonly List<Filosofo> listaFilos;
        private int numComidas = 0;
        private readonly int indice;

        public Filosofo(List<Filosofo> listadoFilos, int i)
        {
            listaFilos = listadoFilos;
            indice = i;
            this.Nombre = string.Format("Filosofo {0}", indice);
            this.Estado = Estado.Pensando;
        }

        public string Nombre { get; private set; }
        public Estado Estado { get; private set; }
        public Palillos palilloIzquierdo { get; set; }
        public Palillos palilloDerecho { get; set; }

        public Filosofo filoIzquierda
        {
            get
            {
                if (indice == 0)
                    return listaFilos[listaFilos.Count - 1];
                else
                    return listaFilos[indice - 1];
            }
        }

        public Filosofo filoDerecha
        {
            get
            {
                if (indice == listaFilos.Count - 1)
                    return listaFilos[0];
                else
                    return listaFilos[indice + 1];
            }
        }

        public void CicloComer()
        {
            while (numComidas < CANTIDAD_COMIDAS)
            {

                this.CogerPalillos();
                
            }
        }

        private void CogerPalillos()
        {
            bool lockTaken = false;
            bool lockTaken1 = false;

            try
            {
                Intentando();
                Thread.Sleep(1000);
                Monitor.TryEnter(this.palilloIzquierdo, 1000, ref lockTaken);
                if (lockTaken)
                {
                    // The critical section.
                    Console.WriteLine("{0} coge el palillo de la izquierda", this.Nombre);
                    Hambriento();
                    Thread.Sleep(1000);
                    try
                    {
                        Monitor.TryEnter(this.palilloDerecho, 1000, ref lockTaken1);
                        if (lockTaken)
                        {
                            Console.WriteLine("{0} coge el palillo de la derecha.", this.Nombre);
                            Thread.Sleep(1000);
                            this.Comer();
                            if (numComidas.Equals(CANTIDAD_COMIDAS))
                            {
                                this.Pensar();
                                lockTaken = false;
                                lockTaken1 = false;
                            }
                        }
                        else
                        {
                            sueltaUnPalillo("izquierdo");
                            Intentando();
                        }
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        if (lockTaken1)
                        {
                            sueltaUnPalillo("derecho");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("{0} esta hambriento", this.Nombre);
                }
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    sueltaUnPalillo("izquierdo");
                }
            }
        }

        private void Comer()
        {
            this.Estado = Estado.Comiendo;
            numComidas++;
            Console.WriteLine("{0} comiendo por {1}a vez", this.Nombre, numComidas);
            Thread.Sleep(1000);
            if (Monitor.IsEntered(this.palilloDerecho))
            {
                Monitor.Exit(this.palilloDerecho);
            }
            if (Monitor.IsEntered(this.palilloIzquierdo))
            {
                Monitor.Exit(this.palilloIzquierdo);
            }
            Console.WriteLine("Filosofo {0} libera los palillos", this.Nombre);
            
        }

        private void Pensar()
        {
            this.Estado = Estado.Pensando;
            Console.WriteLine("{0} pensando", this.Nombre);

            /*if (Monitor.IsEntered(this.palilloDerecho))
            {
                Monitor.Exit(this.palilloDerecho);
            }
            if (Monitor.IsEntered(this.palilloIzquierdo))
            {
                Monitor.Exit(this.palilloIzquierdo);
            }*/
        }
        private void Hambriento()
        {
            this.Estado = Estado.Hambriento;
            Console.WriteLine("{0} esta hambriento", this.Nombre);
        }

        private void Intentando()
        {
            
            Console.WriteLine("{0} esta intentando comer", this.Nombre);
        }
        
        private void sueltaUnPalillo(string palillo)
        {
            //sueltaUnPalillo("izquierda");
            Console.WriteLine("{0} deja el palillo de la {1}", this.Nombre, palillo);
            if (palillo.Equals("izquierdo") && Monitor.IsEntered(palilloIzquierdo))
            {
                Monitor.Exit(palilloIzquierdo);
            }
            if (palillo.Equals("derecho") && Monitor.IsEntered(palilloDerecho))
            {
                Monitor.Exit(palilloDerecho);
            }
            
        }
    }

    [DebuggerDisplay("Nombre = {Nombre}")]
    public class Palillos
    {
        private static int contador = 1;
        public string Nombre { get; private set; }

        public Palillos()
        {
            this.Nombre = "Palillo " + contador++;
        }
    }
}
