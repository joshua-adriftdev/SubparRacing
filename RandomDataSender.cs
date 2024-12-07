using System;
using System.Threading;

namespace SubparRacing
{
    public class RandomDataSender
    {
        private readonly ArduinoConnection arduinoConnection;
        private readonly Random random;
        private bool run;

        public RandomDataSender(ArduinoConnection connection)
        {
            arduinoConnection = connection;
            random = new Random();
        }

        public void Start()
        {
            run = true;

            // Run in a separate thread to avoid blocking the main application
            Thread senderThread = new Thread(() =>
            {
                while (run)
                {
                    // Generate a random number between 1 and 9
                    int randomNumber = random.Next(1, 10); // 10 is exclusive

                    // Send it to the Arduino with a key "RandomNumber"
                    arduinoConnection.SendData("DEBUG", randomNumber.ToString());

                    // Wait for 2 seconds before sending the next number
                    Thread.Sleep(2000);
                }
            });

            senderThread.Start();
        }

        public void Stop()
        {
            run = false;
        }
    }
}
