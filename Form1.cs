using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Стрельба_треды
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int minH = 395;
        Calculator calc;
        Demonstrator dem;
        int r;
        int sleepTime;
        int shotTime;
        int maxCoords;
        List<KeyValuePair<int, int>> countOfHits;
        List<KeyValuePair<int, int>> countOfMisses;
        private void OnPaintShot(object sender, ShootingEventArgs e)
        {
            countOfHits = e.countOfHits;
            countOfMisses = e.countOfMisses;
            Invoke(new Action<string>((s) => { _lb5.Text = "Количество попаданий:" + s; }), countOfHits.Count.ToString());
            Invoke(new Action<string>((s) => { _lb6.Text = "Количество выстрелов:" + s; }), (countOfHits.Count + countOfMisses.Count).ToString());
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (dem != null && dem.t.IsAlive)
            {
                PaintTarget(e.Graphics);
                PaintShot(e.Graphics);
            }
        }
        private void PaintTarget(Graphics gr)
        {
            int side = _picBox1.Width;
            Graphics g = gr;
            g.FillRectangle(Brushes.White, new RectangleF(_picBox1.Location.X, _picBox1.Location.Y, side, side));
            g.FillRectangle(Brushes.Gray, new RectangleF(_picBox1.Location.X + side / 2 - r, _picBox1.Location.Y + side / 2 - r, 2 * r, 2 * r));
            g.FillEllipse(Brushes.White, new RectangleF(_picBox1.Location.X + side / 2 - 2 * r, _picBox1.Location.Y + side / 2 - 2 * r, 2 * r, 2 * r));
            g.FillEllipse(Brushes.White, new RectangleF(_picBox1.Location.X + side / 2, _picBox1.Location.Y + side / 2, 2 * r, 2 * r));
            g.DrawLine(Pens.Blue, _picBox1.Location.X + side / 2 - 1, _picBox1.Location.Y, _picBox1.Location.X + side / 2, _picBox1.Location.Y + side);
            g.DrawLine(Pens.Blue, _picBox1.Location.X, _picBox1.Location.Y + side / 2 - 1, _picBox1.Location.X + side, _picBox1.Location.Y + side / 2);
        }
        private void PaintShot(Graphics gr)
        {
            Graphics g = gr;
            if (countOfHits != null)
                for (int i = 0; i < countOfHits.Count; i++)
                    g.FillEllipse(Brushes.Lime, new RectangleF(_picBox1.Location.X + countOfHits[i].Key - 2, _picBox1.Location.Y + countOfHits[i].Value - 2, 4, 4));
            if (countOfMisses != null)
                for (int i = 0; i < countOfMisses.Count; i++)
                    g.FillEllipse(Brushes.Red, new RectangleF(_picBox1.Location.X + countOfMisses[i].Key - 2, _picBox1.Location.Y + countOfMisses[i].Value - 2, 4, 4));
        }
        private void _bt5_Click(object sender, EventArgs e)
        {
            if (calc.t.IsAlive)
                _bt2_Click(sender, e);
            dem.OnStopShooting();
            _tb2.Enabled = true;
            _tb3.Enabled = true;
            _tb4.Enabled = true;
            _bt5.Enabled = false;
        }

        private void _bt2_Click(object sender, EventArgs e)
        {
            dem.OnStopCalc();
            _tb1.Enabled = true;
            _bt2.Enabled = false;
            _bt1.Enabled = true;
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (calc != null)
                calc.t.Abort();
            if (dem != null)
                dem.t.Abort();
        }

        private void _bt3_Click(object sender, EventArgs e)
        {
            if (calc != null)
                calc.t.Abort();
            if (dem != null)
                dem.t.Abort();
            Application.Exit();
        }

        private void _bt1_Click(object sender, EventArgs e)
        {

            _tb1.Enabled = false;
            _tb2.Enabled = false;
            _tb3.Enabled = false;
            _tb4.Enabled = false;
            if (dem != null)
                dem.t.Abort();
            _picBox1.Size = new Size(2 * maxCoords, 2 * maxCoords);
            if (minH < _picBox1.Height)
                Form1.ActiveForm.MinimumSize = new Size(900 + _picBox1.Width, _picBox1.Height + 50);
            else
                Form1.ActiveForm.MinimumSize = new Size(900 + _picBox1.Width, minH);
            calc = new Calculator(sleepTime);
            dem = new Demonstrator(r, maxCoords, shotTime);
            dem.ShootingEvent += OnPaintShot;
            calc.CalculatorEvent += dem.WriteMessage;
            dem.DemonstratorEvent += calc.Stop;
            _bt2.Enabled = true;
            _bt5.Enabled = true;
            _bt1.Enabled = false;
            calc.t.Start(_tb5);
            dem.t.Start(_picBox1);
        }

        private void _tb1_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                sleepTime = Int32.Parse(_tb1.Text);
                if (sleepTime <= 0)
                    throw new Exception();
                _ep1.Clear();
            }
            catch
            {
                _ep1.SetError(_tb1, "Введите положительное целое число.");
                e.Cancel = true;
            }
        }

        private void _tb2_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                shotTime = Int32.Parse(_tb2.Text);
                if (shotTime <= 0)
                    throw new Exception();
                _ep2.Clear();
            }
            catch
            {
                _ep2.SetError(_tb2, "Введите положительное целое число.");
                e.Cancel = true;
            }
        }

        private void _tb3_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                r = Int32.Parse(_tb3.Text);
                if (r <= 0 || r > 400)
                    throw new Exception();
                _ep3.Clear();
            }
            catch
            {
                _ep3.SetError(_tb3, "Введите положительное целое число (не большее 400).");
                e.Cancel = true;
            }
        }

        private void _tb4_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                maxCoords = Int32.Parse(_tb4.Text);
                if (maxCoords <= 0 || maxCoords > 400)
                    throw new Exception();
                _ep4.Clear();
            }
            catch
            {
                _ep4.SetError(_tb4, "Введите положительное целое число (не большее 400).");
                e.Cancel = true;
            }
        }
        bool IsReady()
        {
            if (r > 0 && sleepTime > 0 && shotTime > 0 && maxCoords > 0)
            {
                _bt1.Enabled = true;
                return true;
            }
            else
                return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void _tb1_Validated(object sender, EventArgs e)
        {
            IsReady();
        }

        private void _tb2_Validated(object sender, EventArgs e)
        {
            IsReady();
        }

        private void _tb3_Validated(object sender, EventArgs e)
        {
            IsReady();
        }

        private void _tb4_Validated(object sender, EventArgs e)
        {
            IsReady();
        }

        private void _picBox1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
