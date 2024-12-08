using System;
using System.Diagnostics;
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
            /*Thread gearSenderThread = new Thread(() =>
            {
                while (run)
                {
                    // Generate a random number between 1 and 9
                    int randomNumber = random.Next(1, 10); // 10 is exclusive

                    // Send it to the Arduino with a key "RandomNumber"
                    arduinoConnection.SendData("GEAR", randomNumber.ToString());

                    // Wait for 2 seconds before sending the next number
                    Thread.Sleep(2000);
                }
            });*/

            Thread lapTimeThread = new Thread(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                int frameDelay = 1000 / 12; // 30 FPS = ~33ms per frame

                while (run)
                {
                    // Start the stopwatch
                    stopwatch.Restart();

                    while (stopwatch.Elapsed < TimeSpan.FromMinutes(2)) // Run until 2 minutes
                    {
                        // Format the elapsed time as "m:ss.fff"
                        string elapsedTime = stopwatch.Elapsed.ToString(@"m\:ss\.fff");

                        // Send the formatted time to the Arduino
                        arduinoConnection.SendData("LAPTIME", elapsedTime);

                        // Wait for the next frame
                        Thread.Sleep(frameDelay);
                    }

                    // Reset the stopwatch after reaching 2:00.000
                    stopwatch.Reset();
                }
            });

            //gearSenderThread.Start();
            lapTimeThread.Start();
        }

        public void Stop()
        {
            run = false;
        }
    }
}
