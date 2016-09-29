using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace Fragments
{
    class MediaPlayer : IDisposable
    {
        #region instance variables
        private VlcInstance instance;
        private VlcMediaPlayer player;
        private VlcMedia media;
        private IntPtr window;
        private Hashtable actions;        
        static Random rnd = new Random();
        private Thread behaviourLoop;
        private SerialController serialController;
        public string nextAction;
        #endregion
        public MediaPlayer(IntPtr window)
        {
            // Configuration //
            string[] args = new string[] { "-I", "dummy", "--ignore-config" };

            actions = new Hashtable(); // action or serial character, and the name of the directory containing videos
            actions.Add("0", "Idle");   // default 
            actions.Add("1", "Active");
        
            serialController = new SerialController("COM3", 9600, Parity.None, StopBits.One, 8, Handshake.None, false, runNextActionFromSerial);

            // Initialization
            this.window = window;
            instance = new VlcInstance(args);
            
            SerialPort mySerialPort = new SerialPort("COM1");

            // Main Program Behaviour Loop 
            behaviourLoop = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
               // start default action
               doAction("0");

                while (true)
                {
                    if (!player.IsPlaying())
                    {
                        doAction("0");
                    }
                    else
                    {
                        if (nextAction != null)
                        {
                            doAction(nextAction);
                            nextAction = null;
                        }
                    }
                }
            });
            behaviourLoop.Start();
        }

        public void doAction(string action)
        {
            switch (action)
            {
                default:
                    // Play random video from the associated actions folder
                    try {
                        string[] files = Directory.GetFiles(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\" + actions[action] + @"\", "*.mpg");
                        play(files[rnd.Next(files.Length)]);
                    }catch (Exception E)
                    {
                        Console.WriteLine(E.ToString());
                    }
                    break;
            }            
        }

        public void play(string filename)
        {
            #if DEBUG
            Console.WriteLine("Playing: " + filename);
            #endif
            try
            {
                if (player == null)
                {
                    media = new VlcMedia(instance, filename);
                    player = new VlcMediaPlayer(media);
                    player.Drawable = window;
                }
                else
                {
                    media.Dispose();                    
                    media = new VlcMedia(instance, filename);
                    player.Media = media;
                }
                player.Play();
            }
                catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }

            // Wait until we know video is playing (or timeout) so the behaviour loop doesnt start the next video before this one plays
            for (int i = 0; i < 20 || player.IsPlaying() != true; i++)
                Thread.Sleep(100);
        }

        private void runNextActionFromSerial(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            char c = (char)sp.ReadChar();
#if DEBUG
            Console.Write ("S:'" + c.ToString() + "' ");
#endif
            if (Char.IsLetterOrDigit(c) && c != '0') 
                nextAction = c.ToString();
        }
        public void Dispose()
        {
            behaviourLoop.Abort();
            if (serialController != null)
                serialController.Dispose();
            if (player != null) player.Dispose();
            instance.Dispose();
        }
    }
}
