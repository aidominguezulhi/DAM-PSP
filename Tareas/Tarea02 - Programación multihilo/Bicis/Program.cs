using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;


namespace consumerProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Creación de nueva colección en este caso de números enteros.
            // Se inicializa la Colección a 100 elementos. Es decir, no se podrán generar más de 100 ni consumir más de 100 en el mismo instante.
            BlockingCollection<int> dataItems = new BlockingCollection<int>(100);
            BlockingCollection<int> dataItems2 = new BlockingCollection<int>(100);


            // Tarea que realiza función de CONSUMIDOR. Consume los elementos de la colección BlockingCollection
            Task t1 = Task.Run(() =>
            {
                while (!dataItems.IsCompleted) //Analiza si la colección se ha marcado como completada
                {

                    int data = -1;
                    // Se bloquea sí dataItems.Count == 0. Es decir si no hay elemento para consumir se queda a la espera

                    try
                    {
                        //Thread.Sleep(2000); //Consumidor duerme hilo 2 segundos.
                        //Cogemos un elemento de la colección de dataItems.
                        //Si no hay nada para coger, se queda la tarea esperando hasta que haya algún elemento para coger.
                        data = dataItems.Take();


                    }
                    //Recogemos una excepción que no vamos a tratar en este ejercicio. Entraría dentro del catch si hay una operación no válida.
                    catch (InvalidOperationException) {  }

                    if (data != -1)
                    {
                        Console.WriteLine("Coger la bici numero {0} desde Gros", data);
                        if (data % 3 == 0)
                        {
                            //Console.WriteLine("isadding Complete {0}", dataItems.IsAddingCompleted);
                            if (!dataItems.IsAddingCompleted)
                            {
                                dataItems.Add(data);
                                Console.WriteLine("Devolver la biciclea {0} al almacen principal", data);
                                //Console.WriteLine("cantidad almacen principal {0}", dataItems2.Count);
                            }
                            else
                            {
                                dataItems2.Add(data);
                                Console.WriteLine("Devolver la biciclea {0} al almacen secundario", data);
                                //Console.WriteLine("cantidad almacen secundario {0}", dataItems.Count);
                            }
                        }

                    }

                }

                //Si no hay nada por consumir, muestra este mensaje por pantalla.
                //Console.WriteLine("\r\nNo hay más bicicletas por consumir en el almacen principal");
            });

            // Tarea que realiza función de PRODUCTOR. Produce los elementos de la colección BlockingCollection.
            // Añade elementos a la colección.
            Task t2 = Task.Run(() =>
            {
                int data = 0;
                bool AnadirElemento = true;

                //Mientras haya un elemento por añadir entra dentro del bucle
                while (AnadirElemento)
                {

                    // Añadimos el dato a la colección.
                    // Si la capacidad ha llegado a 100 que es el tope de la colección se queda esperando a que se vacíe la colección para añadirle un número.
                    // No hay que hacer ninguna comprobación lo hae automático.
                    // No hay que esperar a nada para crearlo.

                    dataItems.Add(data);
                    Console.WriteLine("Anadir la bicicleta {0} al almacen principal", data);
                    //Si he leido algo, muestro por consola.

                    //Aumentamos el número de datos
                    data++;

                    //Si el número data ha llegado al máximo finalizamos el programa.
                    //ESte número no tiene nada que ver con el 100 con el que hemos creado la colección.
                    //Los 1000 números serán los números a generar para que se guarden dentro de la colección, aquellos que se producen y se consumen.
                    if (data == 200)
                    {
                        //Cuando se llegue a la cantidad de 1000 números ya no se guardará nada más dentro de la colección.
                        AnadirElemento = false;
                    }

                }
                // La colección no va a aceptar más elementos
                Console.WriteLine("\r\nAlmacen principal cerrado, no se pueden devolver bicicletas\r\n");
                dataItems.CompleteAdding();

            });

            Task t3 = Task.Run(() =>
            {
                while (!dataItems.IsCompleted) //Analiza si la colección se ha marcado como completada
                {

                    int data = -1;
                    // Se bloquea sí dataItems.Count == 0. Es decir si no hay elemento para consumir se queda a la espera

                    try
                    {
                        //Thread.Sleep(2000); //Consumidor duerme hilo 2 segundos.
                        //Cogemos un elemento de la colección de dataItems.
                        //Si no hay nada para coger, se queda la tarea esperando hasta que haya algún elemento para coger.
                        data = dataItems.Take();


                    }
                    //Recogemos una excepción que no vamos a tratar en este ejercicio. Entraría dentro del catch si hay una operación no válida.
                    catch (InvalidOperationException) { }

                    if (data != -1)
                    {
                        Console.WriteLine("Coger la bici numero {0} desde Amara", data);
                        if (data % 5 == 0)
                        {
                            //Console.WriteLine("isadding Complete {0}", dataItems.IsAddingCompleted);
                            if (!dataItems.IsAddingCompleted)
                            {
                                dataItems.Add(data);
                                Console.WriteLine("Devolver la biciclea {0} al almacen principal", data);
                                //Console.WriteLine("cantidad almacen principal {0}", dataItems2.Count);
                            }
                            else
                            {
                                dataItems2.Add(data);
                                Console.WriteLine("Devolver la biciclea {0} al almacen secundario", data);
                                //Console.WriteLine("cantidad almacen secundario {0}", dataItems.Count);
                            }
                        }

                    }

                }

                //Si no hay nada por consumir, muestra este mensaje por pantalla.
                //Console.WriteLine("\r\nNo quedan bicicletas en el almacen principal");
            });

            //Se espera a que finalicen las tareas
            //Si ejecutamos de una forma síncrona los objetos necesitamos  un wait. Para que primero se ejecute el t1 y luego el t2.
            t2.Wait();
            t1.Wait();
            t3.Wait();
            Console.WriteLine("\r\nNo hay más bicicletas por consumir en el almacen principal");
            Console.WriteLine("Bicicletas en el almacen secundario {0}", dataItems2.Count);
        }
    }
}