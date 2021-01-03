using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace Стрельба_треды
{
    public class DemonstratorEventArgs : EventArgs
    {

    }
    public class ShootingEventArgs : EventArgs
    {
        public List<KeyValuePair<int, int>> countOfHits;
        public List<KeyValuePair<int, int>> countOfMisses;
        public ShootingEventArgs(List<KeyValuePair<int, int>> acountOfHits, List<KeyValuePair<int, int>> acountOfMisses)
        {
            countOfHits = acountOfHits;
            countOfMisses = acountOfMisses;
        }
    }
    public class Demonstrator
    {
        List<KeyValuePair<int, int>> countOfHits;
        List<KeyValuePair<int, int>> countOfMisses;
        public delegate void ShootingEventHandler(object sender, ShootingEventArgs e);
        public event ShootingEventHandler ShootingEvent;
        int r;
        int maxCoord;
        int shotTime;
        int x;
        int y;
        bool started;
        Random rand;
        public Thread t;
        object lo;
        public delegate void DemonstratorEventHandler(object sender, DemonstratorEventArgs e);
        public event DemonstratorEventHandler DemonstratorEvent;
        public delegate void StopShootingEventHandler();
        public event StopShootingEventHandler StopShootingEvent;
        public Demonstrator(int ar, int amaxCoord, int ashotTime)
        {
            r = ar;
            maxCoord = amaxCoord;
            shotTime = ashotTime;
            rand = new Random();
            lo = new object();
            started = true;
            t = new Thread(PaintHits, 1);
            countOfHits = new List<KeyValuePair<int, int>>();
            countOfMisses = new List<KeyValuePair<int, int>>();

        }
        public void PaintHits(object sender)
        {
            while (true)
            {
                lock (lo)
                {
                    if (!started)
                        break;
                }
                GetRandomCoords();
                int side = (sender as PictureBox).Width;
                if (Math.Abs(x) > r || Math.Abs(y) > r)
                    countOfMisses.Add(new KeyValuePair<int, int>(x + side / 2, side / 2 - y));
                else
                {
                    if (x * y >= 0)//первая и третья четверть
                    {
                        if (Math.Abs(x) < r && Math.Abs(y) < r)
                            countOfHits.Add(new KeyValuePair<int, int>(x + side / 2, side / 2 - y));
                        else
                            countOfMisses.Add(new KeyValuePair<int, int>(x + side / 2, side / 2 - y));
                    }
                    else//вторая и четвертая
                    {
                        float len = (float)Math.Sqrt((r - Math.Abs(x)) * (r - Math.Abs(x)) + (r - Math.Abs(y)) * (r - Math.Abs(y)));
                        x += (sender as PictureBox).Width / 2;
                        y = (sender as PictureBox).Width / 2 - y;
                        if (len > r)
                            countOfHits.Add(new KeyValuePair<int, int>(x, y));
                        else
                            countOfMisses.Add(new KeyValuePair<int, int>(x, y));
                    }
                }
                ShootingEvent.Invoke(sender, new ShootingEventArgs(countOfHits, countOfMisses));
                Thread.Sleep(shotTime);
            }
        }
        public void OnStopShooting()
        {
            lock (lo)
            {
                started = false;
                t.Abort();
            }
        }
        void GetRandomCoords()
        {
            x = rand.Next(-maxCoord, maxCoord);
            y = rand.Next(-maxCoord, maxCoord);
        }
        public void WriteMessage(object sender, CalculatorEventArgs e)
        {
            string newText = "Между числами " + e.lowerBracket + " и " + e.upperBracket + " есть " + e.outNumber + " автоморфных(ые) чисел(ла)\r\n";
            if ((sender as TextBox).InvokeRequired)
                (sender as TextBox).Invoke(new Action<string>((s) => (sender as TextBox).Text += s), newText);
            else
                (sender as TextBox).Text = newText;
        }
        public void OnStopCalc()
        {
            DemonstratorEvent(this, new DemonstratorEventArgs());
        }
    }
}
