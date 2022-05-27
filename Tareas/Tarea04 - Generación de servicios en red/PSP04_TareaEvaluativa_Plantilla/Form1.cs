using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Mail;
using FluentFTP;



namespace ClienteFTP {
  public partial class Form1 : Form {
    public Form1() {
    
      InitializeComponent();
      
    }
        FtpListItem[] listItem = null;
        FtpClient client = null;
        string filePath = null;
        string baseDirectoryName = null;
        string newPath = string.Empty;

        //recoger fichero a enviar
        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
               
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Recoger el fichero
                    filePath = openFileDialog.FileName;
                    textBox5.AppendText(filePath);
                    button4.Enabled = true;
                }
            }

        }

        //FTP: listar ficheros
        private void button3_Click(object sender, EventArgs e) 
        {
            // recoger valores
            string username = textBox2.Text;
            string password = textBox3.Text;
            string server = textBox4.Text;
            
            // crear un objeto cliente FTP
            client = new FtpClient(server);

            // Se especifican las credenciales
            client.Credentials = new NetworkCredential(username, password);

            // Conectar a servidor
            try
            {
                client.Connect();
                client.ListingParser = FtpParser.Auto;
                textBox1.Clear();
                textBox1.AppendText("Servidor conectado correctamente\r\n");
                //listar nombre de los ficheros
                listItem = client.GetListing();
                
                for (int i = 0; i < listItem.Length; i++)
                {
                    FtpListItem item = listItem[i];
                    textBox1.AppendText(item.Name + "\r\n");

                }
                comboBox1.Enabled = true;
                button2.Enabled = true;
                button7.Enabled = true;
                button6.Enabled = true;
            }
            catch (FtpAuthenticationException ex) 
            {
                textBox1.AppendText("Error en el servidor remoto: (" + ex.CompletionCode + ")No ha iniciado sesión");
            }
            catch (SocketException ex)
            {
                textBox1.AppendText("URI no válido: no se puede analizar el nombre del host");
            }
        }

        //desconectar del servidor
        private void button1_Click(object sender, EventArgs e)
        {
            client.Disconnect();
            textBox1.Clear();
            textBox1.AppendText("Se ha cerrado la conexión con el cliente");
            button2.Enabled = false;
            button7.Enabled = false;
            button6.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;

            textBox5.Clear();
            textBox6.Visible = false;
            textBox6.Enabled = false;
            textBox8.Visible = false;
            textBox8.Enabled = false;
            textBox9.Visible = false;
            textBox9.Enabled = false;

            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;

            comboBox1.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            var fullPath = Path.GetDirectoryName(filePath);
            //Comprobar checkbox nombre

            bool nameChange = checkBox1.Checked;
            if (nameChange)
            {
                // cambiar nombre
                newPath = textBox6.Text;
            }
            else
            {
                // mantener nombre
                newPath = filePath;
            }

            //Comprobar checkbox email
            if (checkBox2.Checked)
            {
                string email = textBox9.Text;
                string mailMessage = "El usuario" + email + " ha subido el fichero " + newPath + " al servidor";
                SendMail(email, newPath, mailMessage);
            }

            // Enviar el fichero
            var directoryName = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(newPath);
            client.UploadFile(filePath, fileName);

            bool ok = client.FileExists(fileName);
            //sacar el pop up
            if (ok)
            {
                string message = "El fichero " + newPath + " se ha subido correctamente";
                string title = "Subido correctamente";
                MessageBox.Show(message, title);
            }

        }


        //listar contenido detallado
        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            listItem = client.GetListing();
            for (int i = 0; i < listItem.Length; i++)
            {
                FtpListItem item = listItem[i];
                textBox1.AppendText(item.Input + "\r\n");

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Rellenar textbox con directorio de descarga
        private void button7_Click(object sender, EventArgs e)
        {
            using (var ofd = new FolderBrowserDialog())
            {
                DialogResult result = ofd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    baseDirectoryName = ofd.SelectedPath;
                    if (baseDirectoryName != null)
                    {
                        textBox7.Clear();
                        textBox7.AppendText(baseDirectoryName);
                        button5.Enabled = true;
                    }
                }
            }
        }

        //descargar fichero
        private void button5_Click(object sender, EventArgs e)
        {
            // si no se selecciona un fichero, salta un error
            if(comboBox1.SelectedItem.ToString() == string.Empty)
            {
                string message = "El fichero seleccionado es incorrecto o no existe";
                string title = "Error";
                MessageBox.Show(message, title);
            }
            else
            {
                // descargar fichero
                string localPath = textBox7.Text + "\\" + comboBox1.SelectedItem.ToString();
                client.DownloadFile(localPath, comboBox1.SelectedItem.ToString());
                string message = "El fichero " + comboBox1.SelectedItem.ToString() + " se ha descargado correctamente";
                string title = "Descargado correctamente";
                MessageBox.Show(message, title);

                //Comprobar checkbox email
                if (checkBox3.Checked)
                {
                    string email = textBox9.Text;
                    string mailMessage = "El usuario" + email + " ha descargado el fichero " + comboBox1.SelectedItem.ToString() + " del servidor";
                    SendMail(email, comboBox1.SelectedItem.ToString(), mailMessage);
                }
            }
            
        }

        // enviar email para avisar de descarga o subida
        public void SendMail(string toEmail, string strFileName, string mensaje)
        {
            //Dirección de cuenta desde la cual se quiere enviar un correo electrónico
            MailAddress origen = new MailAddress("psppruebas@gmail.com", "Server");

            //Dirección de cuenta a la cual se quiere enviar un correo electrónico
            MailAddress destino = new MailAddress(toEmail, "Usuario");

            //Se especifica información del servidor, protocolo, credenciales, ...de la conexión que se va a realizar
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential(origen.Address, "Birtpsp2022"),
                EnableSsl = true

            };

            //Se escribe el mensage que vamos a enviar indicando cual será el receptor y el emisor
            using (MailMessage message = new MailMessage(origen, destino)
            {
                Subject = strFileName,
                Body = mensaje
            })
                //Se ejecuta el envío del mensaje.
                try
                {
                    smtp.Send(message);
                }
                catch (Exception ex)
                {
                    //En caso de error se muestra por la consola.
                    MessageBox.Show("El email no se ha podido enviar");
                    Console.WriteLine(ex.ToString());
                }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox6.Visible = true;
                textBox6.Enabled = true;
            }
            else
            {
                textBox6.Visible = false;
                textBox6.Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox8.Visible = true;
                textBox8.Enabled = true;
            }
            else
            {
                textBox8.Visible = false;
                textBox8.Enabled = false;
            }
        }
        
        private void checkBox3_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                textBox9.Visible = true;
                textBox9.Enabled = true;
            }
            else
            {
                textBox9.Visible = false;
                textBox9.Enabled = false;
            }
        }

        //rellenar combobox
        private void comboBox1_DroppedDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            listItem = client.GetListing(); // por si hubiera cambios en el directorio
            for (int i = 0; i < listItem.Length; i++)
            {
                FtpListItem item = listItem[i];
                comboBox1.Items.Add(item.Name);
            }
        }
    }
}
