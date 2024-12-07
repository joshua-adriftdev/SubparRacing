using IRSDKSharper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubparRacing
{
    public class TelemetryService
    {
        public event Action OnTelemetry;
        public event Action OnStart;

        public IRacingSdk irsdk { get; }

        public TelemetryService()
        {
            irsdk = new IRacingSdk();

            irsdk.OnException += OnException;
            irsdk.OnConnected += OnConnected;
            irsdk.OnDisconnected += OnDisconnected;
            irsdk.OnStopped += OnStopped;
            irsdk.OnTelemetryData += OnTelemetryData;
            irsdk.OnDebugLog += OnDebugLog;

            // this means fire the OnTelemetryData event every 30 data frames (2 times a second)
            irsdk.UpdateInterval = 30;
        }

        private void OnTelemetryData()
        {
            var rpm = irsdk.Data.GetValue("RPM");
            if (App.arduinoIsConnected) 
            {
                Debug.Write("Sending to Arduino.");
                var arduinoPort = App.arduinoConnection.ArduinoPort;

                if (arduinoPort != null)
                {
                    arduinoPort.Write($"A{rpm}");
                }
            }

            OnTelemetry.Invoke();
        }

        public void Start()
        {
            Debug.WriteLine("Starting...");
            irsdk.Start();   
        }

        public void Stop() {
            irsdk.Stop();
        }

        private void OnDebugLog(string message) { Debug.WriteLine(message); }

        private void OnStopped()
        {
            Debug.WriteLine("Stopped.");
        }

        private void OnDisconnected()
        {
            Debug.WriteLine("Disconnected.");
        }

        private void OnConnected()
        {
            Debug.WriteLine("Connected.");
            OnStart.Invoke();
        }

        private void OnException(Exception exception)
        {
            Debug.Write("Exception: " + exception);
        }

    }
}
