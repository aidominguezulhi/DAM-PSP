using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSP05_TE_GestorPass
{ 
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class Registro
    {
        public string description { get; set; }
        public byte[] password { get; set; }

        public Registro(string description, byte[] password)
        {
            this.description = description;
            this.password = password;
        }

        public void updatePass(byte[] password)
        {
            this.password = password;
        }
    }
}
