using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Стрельба_треды
{
    public class CalculatorEventArgs : EventArgs
    {
        public int outNumber;
        public int lowerBracket;
        public int upperBracket;
        public CalculatorEventArgs(int num, int lower, int upper)
        {
            lowerBracket = lower;
            upperBracket = upper;
            outNumber = num;
        }
    }
    public class Calculator
    {
        Random rand;
        int lowerBracket;
        int upperBracket;
        int sleepTime;
        bool started;
        public Thread t;
        object lo;
        public delegate void CalculatorEventHandler(object sender, CalculatorEventArgs e);
        public event CalculatorEventHandler CalculatorEvent;
        List<int> nums;
        public Calculator(int seconds)
        {
            rand = new Random();
            sleepTime = seconds;
            lo = new object();
            t = new Thread(this.Start, 1);
            nums = new List<int>();
            FindNums();
        }
        public void Start(object sender)
        {
            started = true;
            while (true)
            {
                lock (lo)
                {
                    if (!started)
                        break;
                }
                int result = 0;
                GetRandomBrackets();
                int lindex = nums.FindIndex((a) => (a >= lowerBracket));
                int uindex = nums.FindLastIndex((a) => (a <= upperBracket));
                if (lindex == -1)
                    result = 0;
                else
                    result = uindex - lindex + 1;
                CalculatorEvent(sender, new CalculatorEventArgs(result, lowerBracket, upperBracket));
                Thread.Sleep(sleepTime);
            }
        }
        public void Stop(object sender, DemonstratorEventArgs e)
        {
            lock (lo)
            {
                started = false;
                t.Abort();
            }
        }
        void GetRandomBrackets()
        {
            do
            {
                lowerBracket = rand.Next(2, 1000000);
                upperBracket = rand.Next(2, 1000000);
            }
            while (lowerBracket == upperBracket);
            if (lowerBracket > upperBracket)
            {
                int t = lowerBracket;
                lowerBracket = upperBracket;
                upperBracket = t;
            }
        }
        void FindNums()
        {
            for (int i = 1; i <= 1000000;)
            {
                int len = i.ToString().Length;
                ulong sqr = ((ulong)i) * ((ulong)i);
                ulong p = (ulong)Math.Pow(10, len);
                int a = (int)(sqr % p);
                if (a == i)
                    nums.Add(i);
                if (i % 10 == 1)
                    i += 4;
                else if (i % 10 == 5)
                    i++;
                else
                    i += 5;
            }
            nums.Sort();
        }
    }

}
