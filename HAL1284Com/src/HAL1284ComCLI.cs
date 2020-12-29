using System;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace HAL1284Comm
{
    class Program
    {
        int writeSpeed = 1000;
        string comPortVal = "manual";
        public Program()
        {
            getSettings();
        }

        static void Main(string[] args)
        {
            Boolean inMain = true;

            while (inMain)
            {
                Program p = new Program();
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("Welcome to the official HAL 1284 CLI");
                string[] possibleModes = { "1", "2", "3", "0" };
                Console.WriteLine("Select one of the following modes by their number:" + "\n");
                Console.WriteLine(" 1. Write directly to HAL 1284");
                Console.WriteLine(" 2. Upload file to HAL 1284");
                Console.WriteLine(" 3. Basic-ify a program");
                Console.WriteLine(" 0. Exit Program");
                Console.WriteLine("");
                string mode = p.getMode(3, possibleModes);
                Console.WriteLine("Chosen mode is: " + mode);
                switch (mode)
                {
                    case "1":

                        p.writerMode();
                        Console.ResetColor();
                        Console.Clear();
                        break;
                    case "2":
                        p.fileMode(3);
                        Console.ResetColor();
                        Console.Clear();
                        break;
                    case "3":
                        p.basicify(3);
                        Console.ResetColor();
                        Console.Clear();
                        break;
                    case "0":
                        inMain = false;
                        Console.ResetColor();
                        Console.Clear();
                        break;
                    default:
                        //Default
                        break;
                }
            }
        }

        void basicify(int maxTries)
        {
            StringBuilder sb = new StringBuilder();
            string pathInput = getPath(maxTries);
            Console.WriteLine("Input file: " + pathInput + "\n");
            string[] lines = System.IO.File.ReadAllLines(@pathInput);
            int index = 1;

            foreach (string line in lines)
            {
                Console.WriteLine((index * 10).ToString() + " " + line);
                sb.AppendLine((index * 10).ToString() + " " + line);

                index++;
            }

            string filename = pathInput.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            System.IO.File.WriteAllText(@filename + ".basic", sb.ToString());
            Thread.Sleep(2000);
        }

        void writerMode()
        {
            Boolean inWrite = true;
            SerialPort port = setupSerialPort(3);

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

        void fileMode(int maxTries)
        {
            Boolean pathNotFound = true;
            string pathInput = "";
            int tries = 0;
            SerialPort port = setupSerialPort(3);

            if (port == null)
            {
                Console.WriteLine("Can't find a COM port...");
                Thread.Sleep(2000);
                return;
            }

            Console.Clear();
            Console.WriteLine("Please input path to file");
            Console.WriteLine("Write 'exit' to go back to main menu");
            while (pathNotFound)
            {
                Console.Write("> ");
                pathInput = Console.ReadLine();
                if (pathInput == "exit")
                {
                    return;
                }
                if (File.Exists(pathInput))
                {
                    pathNotFound = false;
                }
                tries++;
                if (tries > maxTries)
                {
                    pathNotFound = false;
                }
            }
            port.Open();

            string[] lines = System.IO.File.ReadAllLines(@pathInput);

            foreach (string line in lines)
            {
                port.WriteLine(line);
                Console.WriteLine(line);

                Thread.Sleep(this.writeSpeed);
            }
            Thread.Sleep(2000);
            port.Close();
        }

        SerialPort setupSerialPort(int maxTries)
        {
            int tries = 0;
            Boolean portNotFound = true;
            string portInput = "";

            if (this.comPortVal == "manual")
            {
                while (portNotFound)
                {
                    string[] ports = SerialPort.GetPortNames();
                    Console.WriteLine("Pick one of the ports:");
                    if (ports.Length == 0)
                    {
                        return null;
                    }
                    foreach (string port in ports)
                    {
                        Console.WriteLine(" - " + port);
                    }

                    Console.Write("> ");
                    portInput = Console.ReadLine();
                    Boolean doesExist = false;

                    foreach (string port in ports)
                    {
                        if (port == portInput)
                        {
                            doesExist = true;
                        }
                    }
                    if (!doesExist)
                    {
                        Console.WriteLine(portInput + " is not a valid port. Try again");
                    }
                    else
                    {
                        portNotFound = false;
                    }
                    tries++;
                    if (tries > maxTries)
                    {
                        return null;
                    }
                }
            }
            else
            {
                try
                {
                    portInput = SerialPort.GetPortNames()[0];
                }
                catch (Exception)
                {
                    Console.WriteLine("No port was found. Change your settings or try again.");
                    Thread.Sleep(3000);
                    return null;
                }
            }

            SerialPort sp = new SerialPort();
            sp.BaudRate = 9600;
            sp.PortName = portInput;
            return sp;
        }

        string getPath(int maxTries)
        {
            Boolean pathNotFound = true;
            string pathInput = "";
            int tries = 0;

            Console.Clear();
            Console.WriteLine("Please input path to file");
            Console.WriteLine("Write 'exit' to go back to main menu");
            while (pathNotFound)
            {
                Console.Write("> ");
                pathInput = Console.ReadLine();
                if (pathInput == "exit")
                {
                    return "";
                }
                if (File.Exists(pathInput))
                {
                    pathNotFound = false;
                }
                tries++;
                if (tries > maxTries)
                {
                    pathNotFound = false;
                }
            }
            return pathInput;
        }

        string getMode(int maxTries, string[] modes)
        {
            int tries = 0;
            Boolean hasModeBeenSet = true;
            string returnMode = "0";
            Console.WriteLine("Please choose a mode:");
            while (hasModeBeenSet)
            {
                Console.Write("> ");
                string modeInput = Console.ReadLine();

                foreach (string possibleModes in modes)
                {
                    if (modeInput == possibleModes)
                    {
                        hasModeBeenSet = false;
                        return modeInput;
                    }
                }
                Console.WriteLine("Chosen mode doesn't exist. Please try again:");
                if (tries > maxTries)
                {
                    hasModeBeenSet = false;
                }
                tries++;
            }
            return returnMode;
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