using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace SubparRacing
{
    public class ArduinoConnection
    {
        #region EventArgs

        public class ConnectionEventArgs : EventArgs
        {
            public readonly SerialPort? ArduinoPort;
            public readonly DateTime ConnectionTime;

            public ConnectionEventArgs(SerialPort? arduinoPort = null)
            {
                ArduinoPort = arduinoPort;
                ConnectionTime = DateTime.Now;
            }
        }

        #endregion

        #region Events and Delegates

        public delegate void ArduinoConnectedHandler(object connection, ConnectionEventArgs connectionInformation);
        public delegate void ArduinoSearchingHandler(object connection, ConnectionEventArgs connectionInformation);
        public delegate void ArduinoDisconnectedHandler(object connection, ConnectionEventArgs connectionInformation);

        public event ArduinoConnectedHandler? ArduinoConnected = null;
        public event ArduinoSearchingHandler? ArduinoSearching = null;
        public event ArduinoDisconnectedHandler? ArduinoDisconnected = null;

        #endregion

        #region Fields

        private readonly byte[] handshake;
        private SerialPort? arduinoPort;
        private bool run;
        private Thread runThread;

        #endregion

        #region Properties

        /// <summary>
        /// Getter for Arduino SerialPort
        /// </summary>
        public SerialPort? ArduinoPort { get => arduinoPort; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for ArduinoConnection
        /// </summary>
        /// <param name="handshake">Handshake byte[] for synchronization with Arduino Board</param>
        public ArduinoConnection(byte[] handshake)
        {
            this.handshake = handshake;

            runThread = new Thread(RunConnection);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Main execution method to work with ArduinoConnection
        /// </summary>
        private void RunConnection()
        {
            Debug.WriteLine("Attempting to Connect Port");
            arduinoPort = ConnectPort();
            arduinoPort.BaudRate = 9600;

            Debug.WriteLine("Connected Port");
            OnArduinoConnected(this, new ConnectionEventArgs(arduinoPort));

            do
            {
                if (!arduinoPort.IsOpen)
                {
                    OnArduinoDisconnected(this, new ConnectionEventArgs());

                    arduinoPort = ConnectPort();

                    OnArduinoConnected(this, new ConnectionEventArgs(arduinoPort));
                }

                Thread.Sleep(300);
            }
            while (run);

            if (arduinoPort != null && arduinoPort.IsOpen)
                arduinoPort.Close();
        }

        public void Start()
        {
            runThread.Start();
            run = true;
            Debug.WriteLine("Arduino is Running");
        }

        public void Stop()
        {
            run = false;
        }

        /// <summary>
        /// Checks for available Serial Ports and returns the port with the desired Arduino Board
        /// </summary>
        /// <returns>Port used by desired Arduino Board</returns>
        private SerialPort ConnectPort()
        {
            Debug.WriteLine("COM Ports: " + String.Join(", ", SerialPort.GetPortNames()));

            SerialPort temporalPort, finalPort = new SerialPort();
            bool arduinoConnected = false;
            byte[] readHandshake = new byte[6];

            Debug.WriteLine("A");

            //Launch searching event
            OnArduinoSearch(this, new ConnectionEventArgs());

            Debug.WriteLine("B");

            //While there is not any board connected
            while (!arduinoConnected)
            {
                Thread.Sleep(1000);

                if (!run)
                    break;

                //string[] portNames = ["COM6"];

                //Checks all serial ports available on the PC
                foreach (string portName in SerialPort.GetPortNames()) // SerialPort.GetPortNames()
                {
                    //string portName = "COM5";
                    Debug.WriteLine("Testing Port Name - " + portName);
                    if (!run)
                        break;

                    temporalPort = new SerialPort("COM6"); // Temp

                    //Checks if the current port is used
                    Debug.WriteLine("D - " + temporalPort.IsOpen);
                    if (!temporalPort.IsOpen)
                    {
                        finalPort.PortName = "COM6";

                        try
                        {
                            finalPort.BaudRate = 9600;
                            finalPort.WriteTimeout = 500;
                            finalPort.ReadTimeout = 500;
                            finalPort.RtsEnable = true;
                            finalPort.DtrEnable = true;

                            Debug.WriteLine("E1");
                            finalPort.Open(); //Open port

                            Debug.WriteLine("E2 - " + String.Join(", ", handshake) + " - " + handshake.Length);

                            finalPort.WriteLine("subpar"); //Send private handshake to Arduino

                            Debug.WriteLine("F");

                            //Wait until all bytes from Arduino handshake are ready
                            int count = 0;
                            Debug.WriteLine("G - " + readHandshake.Length);

                            string myReadHandshake = "";
                            while (myReadHandshake == "")
                            {
                                Debug.WriteLine("Reading");
                                try
                                {
                                    myReadHandshake = finalPort.ReadLine().ReplaceLineEndings("");
                                    temporalPort.Close();
                                }
                                catch (TimeoutException e)
                                {
                                    Debug.WriteLine("Timeout");
                                    goto Final;
                                }
                            }

                            //If handshakes are the same, desired Arduino Board is found
                            Debug.WriteLine("Read Handshake: " + myReadHandshake);
                            if (myReadHandshake.Equals("subpar"))
                            {
                                arduinoConnected = true;
                                break;
                            }
                            else
                            {
                                Debug.WriteLine("Apparently '" + myReadHandshake + "' doesn't fucking equal subpar");
                                finalPort.Close();
                                temporalPort.Close();
                                arduinoConnected = false;
                                break;
                            }
                        }
                        catch (Exception ex) {
                            Debug.Write("Error - " + ex.ToString());
                            

                            if (finalPort.IsOpen)
                                finalPort.Close();
                                temporalPort.Close();
                            arduinoConnected = false;
                            continue;
                        }
                    }

                Final:
                    finalPort.Close();
                    temporalPort.Close();
                    readHandshake = new Byte[6];
                }
            }

            return finalPort;
        }

        #region Event methods

        /// <summary>
        /// Method to launch the Connection event
        /// </summary>
        /// <param name="connection">Who sends the event</param>
        /// <param name="connectionInformation">Information about the connection</param>
        protected void OnArduinoConnected(object connection, ConnectionEventArgs connectionInformation)
        {
            ArduinoConnected?.Invoke(connection, connectionInformation);
        }

        /// <summary>
        /// Method to launch the Searching event
        /// </summary>
        /// <param name="connection">Who sends the event</param>
        /// <param name="connectionInformation">Information about the connection</param>
        protected void OnArduinoSearch(object connection, ConnectionEventArgs connectionInformation)
        {
            ArduinoSearching?.Invoke(connection, connectionInformation);
        }

        /// <summary>
        /// Method to launch the Disconnection event
        /// </summary>
        /// <param name="connection">Who sends the event</param>
        /// <param name="connectionInformation">Information about the disconnection</param>
        protected void OnArduinoDisconnected(object connection, ConnectionEventArgs connectionInformation)
        {
            ArduinoDisconnected?.Invoke(connection, connectionInformation);
        }

        #endregion

        /// <summary>
        /// Checks public Arduino handshake with private handshake
        /// </summary>
        /// <param name="response">Readed handshake from Arduino</param>
        /// <returns>True if they are the same, false otherwise</returns>
        private bool CheckHandshake(byte[] response)
        {
            Debug.WriteLine("Response: " + response.ToArray());
            if (response.Length != handshake.Length)
                return false;

            for (int i = 0; i < response.Length; i++)
                if (response[i] != handshake[i])
                    return false;

            return true;
        }

        #endregion

        /// <summary>
        /// Sends a key-value pair to the Arduino in the format "Key:Value"
        /// </summary>
        /// <param name="key">The key to send</param>
        /// <param name="value">The value to send</param>
        public void SendData(string key, string value)
        {
            if (arduinoPort == null || !arduinoPort.IsOpen)
            {
                Debug.WriteLine("Arduino is not connected.");
                return;
            }

            try
            {
                //string dataToSend = $"{key}•{value}";
                string dataToSend = $"DEBUG:{value}";
                arduinoPort.WriteLine(dataToSend);
                Debug.WriteLine($"Sent to Arduino: {dataToSend}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending data: {ex.Message}");
            }
        }
    }


}
