using System;
using System.Threading.Tasks;

public class Example
{
    //async es necesario para definir que el metodo se va a utilizar de forma asincrona
    //gracias a la programacion asincrona no se bloquea o si la ejecucion.
    public static async Task Main()
    {
        // las tareas se ejecutan de forma asincrona, para esperar a que se termine la tarea se utiliza await
        // await es donde se "detiene" la ejecución hasta que termine
        // la tarea se define con una expresion lambda. Una expresion lambda es una forma de definir una función que
        // se "convierte" en un objeto lamda("interfaz")--clase que implementa la interfaz--new 
        // el primer proceso no es necesario para el segundo proceso ni para el final
        await Task.Run(() => {
            // Just loop.
            int ctr = 0;
            for (ctr = 0; ctr <= 10000000; ctr++)
            { }
            Console.WriteLine("Finalizado {0} bucle de iteraciones",
                              ctr);
        });
        // el segundo proceso es necesario para el producto final
        await Task.Run(() => {
            // Just loop.
            int ctr = 0;
            for (ctr = 0; ctr <= 10; ctr++)
            { }
            Console.WriteLine("Finalizado {0} bucle de iteraciones",
                              ctr);
        });
        Console.Write("Producto final");
    }
}
