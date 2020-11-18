using System.Windows.Forms;
using System.Drawing;
using System;
using System.Threading;
using System.Collections.Generic;

using System.IO.Ports;
using System.IO;
using System.Text;

using System.Xml;

namespace myform
{
    class Program
    {
        int writeSpeed = 1000;
        string comPortVal = "manual";
        Form form1;
        public Program()
        {
            getSettings();
            this.form1 = new Form();
        }

        [STAThread]
        static void Main(string[] args)
        {
            Program p = new Program();
            p.CreateMyForm();
        }

        public void CreateMyForm()
        {
            int topSpacer = 60;
            int butSpacer = 10;

            form1.BackColor = Color.Blue;

            Label settingsLabel1 = new Label();
            settingsLabel1.Text = "Write Speed: " + writeSpeed.ToString() + "\n";
            settingsLabel1.Text += "COM Port: " + comPortVal;
            settingsLabel1.Location = new Point(2, 2);
            settingsLabel1.ForeColor = Color.White;

            var buts = new List<Button>();
            Button writerBut = new Button();
            buts.Add(writerBut);
            Button uploadBut = new Button();
            buts.Add(uploadBut);
            Button basicifyBut = new Button();
            buts.Add(basicifyBut);
            Button cancelBut = new Button();
            buts.Add(cancelBut);

            writerBut.Text = "Write to HAL";
            uploadBut.Text = "Upload File to HAL";
            basicifyBut.Text = "Basicify File";
            cancelBut.Text = "Cancel";

            foreach (Button b in buts)
            {
                b.MinimumSize = new Size(75, 30);
                b.Width = b.Text.Length * 12;
                b.Font = new Font("Consolas", 12);
                b.BackColor = Color.Gray;
                b.ForeColor = Color.White;
                b.MouseEnter += new EventHandler(But_MouseHover);
                b.MouseLeave += new EventHandler(But_MouseLeave);
                //b.Enter += new EventHandler(But_MouseHover);
                //b.Leave += new EventHandler(But_MouseLeave);
            }

            //form1.Height / 2 - button1.Height / 2
            writerBut.Location = new Point(form1.Width / 2 - writerBut.Width / 2, topSpacer);
            uploadBut.Location = new Point(form1.Width / 2 - uploadBut.Width / 2, writerBut.Height + writerBut.Top + butSpacer);
            basicifyBut.Location = new Point(form1.Width / 2 - basicifyBut.Width / 2, uploadBut.Height + uploadBut.Top + butSpacer);
            cancelBut.Location = new Point(form1.Width / 2 - cancelBut.Width / 2, basicifyBut.Height + basicifyBut.Top + butSpacer);

            writerBut.Click += new EventHandler(WriteClick);
            uploadBut.Click += new EventHandler(UploadClick);
            basicifyBut.Click += new EventHandler(BasicifyClick);

            form1.Text = "HAL 1284 GUI";
            form1.FormBorderStyle = FormBorderStyle.FixedDialog;
            form1.MaximizeBox = false;

            form1.CancelButton = cancelBut;

            form1.StartPosition = FormStartPosition.CenterScreen;

            form1.Controls.Add(writerBut);
            form1.Controls.Add(uploadBut);
            form1.Controls.Add(basicifyBut);
            form1.Controls.Add(cancelBut);

            form1.Controls.Add(settingsLabel1);

            form1.ShowDialog();
        }

        private void WriteClick(object sender, EventArgs e)
        {
            writerMode();
        }

        private void UploadClick(object sender, EventArgs e)
        {
            fileMode();
        }

        private void BasicifyClick(object sender, EventArgs e)
        {
            basicify();
        }

        private void But_MouseHover(object sender, EventArgs e)
        {
            Button but = (Button)sender;
            but.BackColor = Color.White;
            but.ForeColor = Color.Gray;
        }

        public string openFile()
        {
            string returnPath = string.Empty;
            string ownPath = Directory.GetCurrentDirectory();
            // Show the dialog and get result.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK) // Test result.
            {
                openFileDialog1.InitialDirectory = ownPath;
                returnPath = openFileDialog1.FileName;
            }
            return returnPath;
        }

        private void But_MouseLeave(object sender, EventArgs e)
        {
            Button but = (Button)sender;
            but.BackColor = Color.Gray;
            but.ForeColor = Color.White;
        }

        void basicify()
        {
            StringBuilder sb = new StringBuilder();
            string pathInput = openFile();
            Console.WriteLine("Input file: " + pathInput + "\n");
            string[] lines = System.IO.File.ReadAllLines(@pathInput);
            int index = 1;

            foreach (string line in lines)
            {
                if (line.Contains("//") && line.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                {
                    Console.WriteLine("//" + line.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    sb.AppendLine("//" + line.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                }
                else
                {
                    Console.WriteLine((index * 10).ToString() + " " + line);
                    sb.AppendLine((index * 10).ToString() + " " + line);
                }
                index++;
            }

            string filename = pathInput.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            System.IO.File.WriteAllText(@filename + ".basic", sb.ToString());
        }

        void writerMode()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;

            Boolean inWrite = true;
            SerialPort port = setupSerialPort();

            if (port == null)
            {
                Console.WriteLine("Can't find a COM port...");
                Thread.Sleep(2000);
                return;
            }
            else
            {
                port.Open();
            }
            Console.Clear();
            Console.WriteLine("Ready for input.");
            Console.WriteLine("Write 'exit' to go back to main menu");
            while (inWrite)
            {
                Console.Write("> ");

                string input = Console.ReadLine();

                if (input == "exit")
                {
                    inWrite = false;
                }
                else
                {
                    port.WriteLine(input);
                }
            }
            port.Close();
        }

        void fileMode()
        {
            SerialPort port = setupSerialPort();
            

            if (port == null)
            {
                Console.WriteLine("Can't find a COM port...");
                return;
            }

            port.Open();
			
			string pathInput = openFile();

            string[] lines = System.IO.File.ReadAllLines(@pathInput);

            foreach (string line in lines)
            {
                string[] newLines = line.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

                if (newLines.Length == 2)
                {
                    port.WriteLine(newLines[0]);
                    Console.WriteLine(newLines[0]);
                }
                else
                {
                    if (!line.Contains("//"))
                    {
                        port.WriteLine(line);
                        Console.WriteLine(line);
                    }
                }

                Thread.Sleep(this.writeSpeed);
            }
            port.Close();
        }

        SerialPort setupSerialPort()
        {
			string[] ports = SerialPort.GetPortNames();
			if(ports.Length == 0) return null;
			SerialPort sp = new SerialPort();
            if (this.comPortVal == "manual")
            {
                var formPopup = new Form();

                var radButs = new List<RadioButton>();
                

                for (int i = 0; i < ports.Length; i++)
                {
                    RadioButton rdb = new RadioButton();
                    rdb.Text = ports[i];
                    rdb.Size = new Size(60, 30);
                    rdb.Location = new Point(10, 30 * i);
                    formPopup.Controls.Add(rdb);
                    radButs.Add(rdb);
                }

                formPopup.Size = new Size(200, 30 * ports.Length + 120);

                Button okBut = new Button();
                okBut.Text = "OK";
                okBut.Location = new Point(formPopup.Width / 2 - okBut.Width / 2, 30 * ports.Length + 30);

                formPopup.AcceptButton = okBut;
                okBut.Click += delegate (object sender, EventArgs e)
                    {
                        serialOkBut(sender, e, formPopup, radButs, sp);
                    };

                formPopup.Controls.Add(okBut);

                formPopup.StartPosition = FormStartPosition.CenterScreen;

                formPopup.ShowDialog(form1);

                
                sp.BaudRate = 9600;
				
                return sp;
            }
            else
            {
                try
                {
                    sp.BaudRate = 9600;
                    sp.PortName = SerialPort.GetPortNames()[0];
                    return sp;
                }
                catch (Exception)
                {
                    Console.WriteLine("No port was found. Change your settings or try again.");
                    Thread.Sleep(3000);
                    return null;
                }
            }
        }

        private void serialOkBut(object sender, System.EventArgs e, Form formPopup, List<RadioButton> list, SerialPort sp)
        {
			
            foreach (RadioButton rdb in list)
            {
                if (rdb.Checked)
                {
					Console.WriteLine(rdb.Text);
					sp.PortName = rdb.Text;
                }
            }
            formPopup.Close();
        }

        void getSettings()
        {
            XmlDocument xmlSettings = new XmlDocument();
            xmlSettings.Load("settings.xml");

            XmlDocument settingsCOM = new XmlDocument();
            settingsCOM.LoadXml("<dummyRoot>" + xmlSettings.GetElementsByTagName("COMPort")[0].InnerXml + "</dummyRoot>");

            XmlDocument settingsWrite = new XmlDocument();
            settingsWrite.LoadXml("<dummyRoot>" + xmlSettings.GetElementsByTagName("writeSpeed")[0].InnerXml + "</dummyRoot>");

            this.comPortVal = settingsCOM.GetElementsByTagName("value")[0].InnerText;
            try
            {
                this.writeSpeed = Int32.Parse(settingsWrite.GetElementsByTagName("value")[0].InnerText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}