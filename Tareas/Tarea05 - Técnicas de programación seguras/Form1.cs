using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;


/*******************************
Link al video de youtube
https://youtu.be/XJZuoswSQPo
********************************/
namespace PSP05_TE_GestorPass
{
    public partial class Form1 : Form
    {
        private byte[] bytextoCifrado;
        private string publicKeyFile = string.Empty;
        private string privateKeyFile = string.Empty;

        string filename = string.Empty;
        string filePath = string.Empty;
        string nombre = string.Empty;
        string passFile = String.Empty;

        List<Registro> misRegistos = new List<Registro>();

        public Form1()
        {
            InitializeComponent(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //recoger y validar email
            string email = this.textBox5.Text;
            string nombre = this.textBox1.Text;
            if (new EmailAddressAttribute().IsValid(email))
            {
                if (this.radioButton1.Checked == true)
                {
                    filename = @"../../bbdd/" + nombre + ".txt";
                    File.Create(filename).Dispose();
                    publicKeyFile = @"../../publickeys/" + nombre + "_public.xml";
                    privateKeyFile = @"../../privatekeys/" + nombre + "_private.xml";
                    generarClaves(publicKeyFile, privateKeyFile);

                    //construir y enviar el email
                    string mensaje = "En este correa se adjunta tu clave privada. Guardala para poder desencriptar";
                    SendEmail(this.textBox5.Text, "Tu clave privada", mensaje, privateKeyFile);

                    MessageBox.Show("Usuario creado correctamente\nRevise su correo electronico para ver la clave privada.");

                    this.checkBox1.Enabled = true;
                    this.checkBox2.Enabled = true;
                    this.checkBox3.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Seleccione registrar el usuario", "error");
                }
            }
            else
            {
                MessageBox.Show("Revise la dirección de email", "email incorrecto");
            }
        }

    
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                groupBox2.Enabled = true;
            }
            else
            {
                groupBox2.Enabled = false;

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            nombre = this.textBox1.Text;
            

            Regex regex = new Regex(@"[a-z]"); 
            Match match = regex.Match(nombre);
            string filename = nombre + ".txt";

            DirectoryInfo di = new DirectoryInfo(@"../../bbdd");
            var exist = di.GetFiles(filename);

            if ((nombre.Length > 10) || (nombre.Length < 4) || (!match.Success))
            {
                MessageBox.Show("El nombre de usuario es incorrecto","nombre incorrecto");
            }
            else if (exist.Length == 1)
            {
                
                this.checkBox1.Enabled = true;
                this.checkBox2.Enabled = true;
                this.checkBox3.Enabled = true;
 
            }
            else
            {
                MessageBox.Show("El usuario no existe","error de registro");
                this.groupBox4.Enabled = true;
                this.radioButton1.Enabled = true;
                this.radioButton2.Enabled = true;
                this.textBox5.Enabled = true;
                this.button1.Enabled = true;
            }   
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string description = this.textBox3.Text;
            publicKeyFile = @"../../publickeys/" + nombre + "_public.xml";
            bool passValida = ValidarPassword(this.textBox4.Text);
            if (passValida == true)
            {
                bytextoCifrado = encriptar(publicKeyFile, Encoding.UTF8.GetBytes(this.textBox4.Text));

                Registro unRegistro = new Registro(description, bytextoCifrado);
                misRegistos.Add(unRegistro);
            }
            else
            {
                string message = "La contraseña es incorrecta.Recuerda\n" +
                                 " - Entre 8 y 10 caracteres\n" +
                                 " - Al menos 1 numero\n" +
                                 " - Al menos 1 mayuscula\n" +
                                 " - Al menos 1 minuscula\n" +
                                 " - Al menos 1 caracter especial (!@#&()–[{}:',?/*~$^+=<>)";
                MessageBox.Show(message);
            }
            
        }

        private void comboBox1_DroppedDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach(Registro registro in misRegistos)
            {
               comboBox1.Items.Add(registro.description);
            }

        }

        private void comboBox2_DroppedDown(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            foreach (Registro registro in misRegistos)
            {
                comboBox2.Items.Add(registro.description);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Recoger el fichero
                    filePath = openFileDialog.FileName;
                    this.label6.Text = filePath;
                }
            }

            string description = comboBox1.SelectedItem.ToString();
            foreach(Registro registro in misRegistos)
            {
                if (registro.description == description)
                {
                    byte[] desencriptado = Desencriptar(filePath, registro.password);
                    this.textBox2.Text = Encoding.UTF8.GetString(desencriptado);
                }
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            int posicion = 0;
            foreach(Registro registro in misRegistos)
            {
                if (registro.description == comboBox2.SelectedItem)
                {
                    posicion = misRegistos.IndexOf(registro);
                }
            }
            misRegistos.RemoveAt(posicion);


        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            passFile = @"../../bbdd/" + nombre + ".txt";
            using (StreamWriter w = File.AppendText(passFile))
            {
                foreach (Registro registro in misRegistos)
                {
                    w.WriteLine(registro.description);
                    w.WriteLine(BitConverter.ToString(registro.password));
                } 
                
                MessageBox.Show("Contraseña guardada correctamente", "Contraseña guardada");
                Application.Exit();
            }

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                groupBox3.Enabled = true;
            }
            else
            {
                groupBox3.Enabled = false;

            }
        }

        
     
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                groupBox1.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = false;

            }
        }

               
              

        //Método que genera tanto clave pública como clave privada para hacer uso de algoritmo asimétrico.
        //@string publicKF: Nombre del fichero donde se guarda la clave pública
        //@string privateKF: Nombre del fichero donde se guarda la clave privada
        private static void generarClaves(string publicKF, string privateKF)
        {
            //Creamos un objeto de tipo RSACryptoServiceProvider para hacer uso de la clave pública y clave privada para su poserior uso.
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                //Obtiene o establece un valor que indica si la clave debe conservarse en el proveedor de servicios criptográficos (CSP).
                //Le indicamos el valor a false porque no queremos que esté en ningún proveedor de servicios.
                rsa.PersistKeyInCsp = false;

                //Borramos cualquier fichero que contenga los mismos nombres
                if (File.Exists(publicKF))
                    File.Delete(publicKF);
                if (File.Exists(privateKF))
                    File.Delete(privateKF);


                //ToXmlString(false): Crea un string con la clave pública. Para que sea pública hay que pasarle como parámetro (false).
                string publicKey = rsa.ToXmlString(false);

                //Crea un fichero y guarda el texto de la clave en el fichero.
                File.WriteAllText(publicKF, publicKey);


                //Mismo proceso anterior para la clave privada
                string privateKey = rsa.ToXmlString(true);
                File.WriteAllText(privateKF, privateKey);

            }
        }

        public static byte[] Desencriptar(string privateKF, byte[] textoEncriptado)
        {

            byte[] desencriptado;
            //Se crea un objeto de tipo RSACryptoServiceProvider para poder hacer uso de sus métodos.
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                //Le indicamos el valor a false porque no queremos que esté en ningún proveedor de servicios.
                rsa.PersistKeyInCsp = false;


                //Lee el contenido del fichero y lo guarda en un string
                string privateKey = File.ReadAllText(privateKF);

                //FromXmlString(false): Inicializa un objeto RSA de la información de clave de una cadena XML.
                //En este caso clave privada ya que la utilizaremos para descifrar
                rsa.FromXmlString(privateKey);

                //Descifra los datos que se cifraron anteriormente.
                //@textoEncriptado: Datos que se van a descifrar.
                //@Booleano: true para realizar el cifrado RSA directo mediante el relleno de OAEP (solo disponible en equipos con Windows XP o versiones posteriores como en nuestro caso); de lo contrario, false 
                desencriptado = rsa.Decrypt(textoEncriptado, true);

            }
            return (desencriptado);

        }

        // enviar email para avisar de descarga o subida

        
        public void SendEmail(string sendTo, string subject, string body, string filename)
        {
            string sendFrom = "psppruebas@gmail.com";
            string password = "Birtpsp2022";
            try
            {
                MailMessage correo = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                correo.From = new MailAddress(sendFrom, "pruebas", System.Text.Encoding.UTF8);
                correo.To.Add(sendTo);

                correo.Subject = subject;
                correo.SubjectEncoding = System.Text.Encoding.UTF8;

                correo.Body = body;
                correo.BodyEncoding = System.Text.Encoding.UTF8;

                correo.IsBodyHtml = false;

                correo.Priority = MailPriority.High;

                correo.Attachments.Add(new Attachment(filename));

                smtp.Credentials = new System.Net.NetworkCredential(sendFrom, password);
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
               
                smtp.Send(correo);
                
            }
            
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Envío de Correo", MessageBoxButtons.OK);
            }
        }

        public static byte[] encriptar(string publicKF, byte[] textoPlano)
        {
            byte[] encriptado;

            //Se crea un objeto de tipo RSACryptoServiceProvider para poder hacer uso de sus métodos de encriptación
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                //Le indicamos el valor a false porque no queremos que esté en ningún proveedor de servicios.
                rsa.PersistKeyInCsp = false;

                //Lee el contenido del fichero y lo guarda en un string
                string publicKey = File.ReadAllText(publicKF);

                //FromXmlString(publicKey): Inicializa un objeto RSA de la información de clave de una cadena XML.
                rsa.FromXmlString(publicKey);

                //Cifra los datos con el algoritmo RSA.
                //@textoPlano: datos que se van a cifrar
                //@Booleano: true para realizar el cifrado RSA directo mediante el relleno de OAEP (solo disponible en equipos con Windows XP o versiones posteriores como en nuestro caso); de lo contrario, false para usar el relleno PKCS#1 v1.5.
                encriptado = rsa.Encrypt(textoPlano, true);

            }

            //Valor que se devuelve
            return encriptado;

        }

        public bool ValidarPassword(string password)
        {
            Match matchNumeros = Regex.Match(password, @"\d");
            Match matchMayusculas = Regex.Match(password, @"[A-Z]");
            Match matchMinusculas = Regex.Match(password, @"[a-z]");
            Match matchEspeciales = Regex.Match(password, @"[!@#&()–[{}:',?/*~$^+=<>]");
            if((password.Length > 7) && (password.Length < 11) && (matchNumeros.Success) && (matchMayusculas.Success) && (matchMinusculas.Success) && (matchEspeciales.Success)){
                return true;
            }

            return false;
        }

        
    }
}


     