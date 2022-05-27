using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Mail;


namespace ClienteFTP {
  public partial class Form1 : Form {
    public Form1() 
    {
    
      InitializeComponent();
      
    }

    FtpWebResponse response = null;
    StreamReader reader = null;
    FtpWebRequest request = null;

    //FTP: listar ficheros
    private void button1_Click(object sender, EventArgs e) 
    {
        try
        {
            this.server = textBox4.Text;
            this.name = textBox2.Text;
            this.password = textBox3.Text;
            request = (FtpWebRequest)WebRequest.Create("ftp://" + this.server + "/");
            request.Method = WebRequestMethods.Ftp. ListDirectory;
            request.Credentials = new NetworkCredential(this.name, this.password);

            response = (FtpWebResponse)request.GetResponse();

            while (response!=null)
            {
                Stream responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);
                string line = string.Empty;
                string texto = string.Empty;

                textBox1.Clear();
                do
                {
                    line = reader.ReadLine();
                    comboBox1.Items.Add(line);
                    textBox1.AppendText(line + "\r\n");

                } while (!reader.EndOfStream);

                    
                comboBox1.SelectedIndex = 1;
                button6.Enabled = true;
                button2.Enabled = true;
                button7.Enabled = true;
                button3.Enabled = false;
                response = null;

            }

                
        }
        catch(Exception ex)
        {
                
            this.textBox1.Text = ex.Message;
              
        }
        finally
        {

            if (!(reader == null))
            {
                reader.Close();
            };
            if (!(response == null))
                {
                response.Close();
            };

        };
          

    }


        //FTP: Selecciona el fichero que para subir y lo deja en textBox5
        private void button2_Click(object sender, EventArgs e) 
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Seleccionar el fichero";
            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            { textBox5.Text = openFileDialog1.FileName;
                button4.Enabled = true;
            }
            else
            { textBox5.Text = "No has llegado a seleccionar el fichero!"; }
        }
   

        private void button3_Click(object sender, EventArgs e) 
        {
            try
            {

                string serverFileName = string.Empty;
                if (checkBox1.Checked == true)
                {
                    serverFileName = textBox6.Text;
                }
                else
                {
                    serverFileName = textBox5.Text;

                }
                button4.Enabled = true;
                
                request = (FtpWebRequest)WebRequest.Create("ftp://" + textBox4.Text + "/" + Path.GetFileName(serverFileName));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                request.Credentials = new NetworkCredential(textBox2.Text, textBox3.Text);

                //response = (FtpWebResponse)request.GetResponse();

                byte[] fileContents;

                using (reader = new StreamReader(textBox5.Text))
                {
                    fileContents = Encoding.UTF8.GetBytes(reader.ReadToEnd());

                }

                request.ContentLength = fileContents.Length;

                using (Stream requesStream = request.GetRequestStream())
                {
                    requesStream.Write(fileContents, 0, fileContents.Length);
                }
                using (response = (FtpWebResponse)request.GetResponse())
                {

                    string caption = "Resultado de subida de Fichero FTP";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    

                    DialogResult result = MessageBox.Show("Fichero subido con código:" + response.StatusDescription,caption, buttons);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        // Closes the parent form.
                        this.Close();
                    }
                    Debug.WriteLine("Fichero subido con código" + response.StatusDescription);
                    if (checkBox2.Checked == true)
                    {
                        SendMail(textBox8.Text, serverFileName, " es el nombre del fichero subido al servidor FTP.");
                    }
                }
                checkBox2.CheckState = CheckState.Unchecked;
                checkBox1.CheckState = CheckState.Unchecked;
                textBox8.Enabled = false;
                textBox6.Enabled = false;
                button4.Enabled = false;



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                this.textBox1.Text = ex.Message;

            }
            finally
            {

                if (!(reader == null))
                {
                    reader.Close();
                };
                if (!(response == null))
                {
                    response.Close();
                };

            };
        }
       


        private void button6_Click(object sender, EventArgs e)
        {

      

            try
            {
                request = (FtpWebRequest)WebRequest.Create("ftp://" + textBox4.Text + "/");
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                //request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential(textBox2.Text, textBox3.Text);

                response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);
                String s = reader.ReadToEnd();

                this.textBox1.Text = s;
                Debug.WriteLine(s);
            }
            catch (Exception ex)
            {

                this.textBox1.Text = ex.Message;

            }
            finally
            {

                if (!(reader == null))
                {
                    reader.Close();
                };
                if (!(response == null))
                {
                    response.Close();
                };

            };


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            response.Close();
            reader.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {


         
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            request.Credentials = new NetworkCredential(textBox2.Text, textBox3.Text);

            response = (FtpWebResponse)request.GetResponse();
            

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Seleccionar el fichero";
            openFileDialog1.InitialDirectory = @"ftp://" + textBox4.Text + "/";
            
            openFileDialog1.Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            { textBox7.Text = openFileDialog1.FileName; }
            else
            { textBox7.Text = "No has llegado a seleccionar el directorio local!"; }
        }



        private void button7_Click(object sender, EventArgs e)
        {
            

            using (var fd = new FolderBrowserDialog())
            {
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fd.SelectedPath))
                {
                    textBox7.Text = fd.SelectedPath;
                }
            }
            button5.Enabled = true;
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
       

            try
            {
                string strFileNameLocal = textBox7.Text + "\\" + comboBox1.SelectedItem.ToString();

                Download(textBox4.Text, textBox2.Text, textBox3.Text, comboBox1.SelectedItem.ToString(), strFileNameLocal);

                if (checkBox3.Checked == true)
                {
                    SendMail(textBox9.Text, comboBox1.SelectedItem.ToString(), " es el nombre del fichero descargado desde el servidor FTP.");
                }

                checkBox3.CheckState = CheckState.Unchecked;
                textBox9.Enabled = false;
                button5.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                this.textBox1.Text = ex.Message;

            }
            finally
            {

                if (!(reader == null))
                {
                    reader.Close();
                };
                if (!(response == null))
                {
                    response.Close();
                };

                //Debug.WriteLine("Listado del directorio raíz: " + response.StatusDescription);


            };
        }

        public void Download(string strServer, string strUser, string strPassword,
                 string strFileNameFTP, string strFileNameLocal)
        {
            //FtpWebRequest ftpRequest;

            // Crea el objeto de conexión del servidor FTP
            request = (FtpWebRequest)WebRequest.Create(string.Format("ftp://{0}/{1}", strServer,
                                                                         strFileNameFTP));
            // Asigna las credenciales
            request.Credentials = new NetworkCredential(strUser, strPassword);
            // Asigna las propiedades
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            // Descarga el archivo y lo graba
            using (FileStream stmFile = File.OpenWrite(strFileNameLocal))
            {

                // Obtiene el stream sobre la comunicación FTP
                using (Stream responseStream = ((FtpWebResponse)request.GetResponse()).GetResponseStream())
                {
                    byte[] arrBytBuffer = new byte[4096];
                    int intRead;

                    // Lee los datos del stream y los graba en el archivo
                    while ((intRead = responseStream.Read(arrBytBuffer, 0, 4096)) != 0)
                        stmFile.Write(arrBytBuffer, 0, intRead);

                    
                    // Cierra el stream FTP	
                    responseStream.Close();
                }

                MessageBoxButtons buttons = MessageBoxButtons.OK;
                string caption = "Resultado de descarga de Fichero FTP";
                
                
                DialogResult result = MessageBox.Show(strFileNameLocal+"Fichero descargado con éxito:", caption, buttons);
                // Cierra el archivo de salida
                stmFile.Flush();
                stmFile.Close();
            }
        }

        public void SendMail(string toEmail, string strFileName, string mensaje)
        {
            //Dirección de cuenta desde la cual se quiere enviar un correo electrónico
            MailAddress origen = new MailAddress("birtpsp@gmail.com", "From BirtPSP");

            //Dirección de cuenta a la cual se quiere enviar un correo electrónico
            MailAddress destino = new MailAddress(toEmail, toEmail);

            //Se especifica información del servidor, protocolo, credenciales, ...de la conexión que se va a realizar
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential(origen.Address, "Birt2022"),
                EnableSsl = true

            };

            //Se escribe el mensage que vamos a enviar indicando cual será el receptor y el emisor
            using (MailMessage mezua = new MailMessage(origen, destino)
            {
                Subject = strFileName + " desde Birt",
                Body = strFileName + mensaje
            })

                //Se ejecuta el envío del mensaje.
                try
                {
                    smtp.Send(mezua);

                }
                catch (Exception ex)
                {
                    //En caso de error se muestra por la consola.
                    Console.WriteLine(ex.ToString());
                }
                finally 
                { 
                    smtp.Dispose();
                }
     
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
            if (checkBox1.Checked == true)
            {
                textBox6.Visible = true;
                textBox6.Enabled = true;
            }
            else
            {
                textBox6.Visible = false;
                
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                textBox8.Visible = true;
                textBox8.Enabled = true;
            }
            else
            {
                textBox8.Visible = false;
            }
        }
        
        private void checkBox3_CheckedChanged_1(object sender, EventArgs e)
        {


            if (checkBox3.Checked == true)
            {
                textBox9.Visible = true;
                textBox9.Enabled = true;
            }
            else
            {
                textBox9.Visible = false;
            }
            

        }

        //DroppedDown refrescar la información que contiene.
        private void comboBox1_DroppedDown(object sender, EventArgs e)
        {
           
            
            try
            {
                this.server = textBox4.Text;
                this.name = textBox2.Text;
                this.password = textBox3.Text;
                request = (FtpWebRequest)WebRequest.Create("ftp://" + this.server + "/");
                //request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential(this.name, this.password);

                response = (FtpWebResponse)request.GetResponse();

                while (response != null)
                {
                    Stream responseStream = response.GetResponseStream();
                    reader = new StreamReader(responseStream);
                    string line = string.Empty;
                    string texto = string.Empty;
                    comboBox1.Items.Clear();

                    do
                    {
                        line = reader.ReadLine();
                        comboBox1.Items.Add(line);
                        

                    } while (!reader.EndOfStream);


                
                    response = null;

                }


            }
            catch (Exception ex)
            {

                this.textBox1.Text = ex.Message;

            }
            finally
            {

                if (!(reader == null))
                {
                    reader.Close();
                };
                if (!(response == null))
                {
                    response.Close();
                };

            };
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
