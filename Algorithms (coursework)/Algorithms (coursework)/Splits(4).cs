using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Algorithms__coursework_
{
    public partial class SplitsForm : Form
    {
        BackgroundWorker BGW = new BackgroundWorker();

        public SplitsForm()
        {
            InitializeComponent();
            BGW.WorkerSupportsCancellation = true;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value < numericUpDown2.Value) numericUpDown2.Value = numericUpDown1.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value < numericUpDown2.Value) numericUpDown2.Value = numericUpDown1.Value;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            String processedResult = String.Empty;
            var queue = new ConcurrentQueue<string>();
            var n = (int)numericUpDown1.Value;
            var m = (int)numericUpDown2.Value;
            var selectedConst = (int)numericUpDown3.Value;

            textBox1.Text = "Calculating.\r\nn = " + n + "; m = " + m + ";\r\nConstant is " + selectedConst + ";\r\n\r\n";

            BGW.RunWorkerAsync();

            await Task.Run(() =>
            {
                try
                {
                    AlgorithmH(n, m, queue, selectedConst);
                }
                catch
                {
                }
            });

            Task.WaitAll();
            while (!queue.IsEmpty)
            {
                string probableSolution;

                if (!queue.TryDequeue(out probableSolution))
                {
                    continue;
                }

                processedResult += probableSolution + "\t";
            }
            textBox1.Text += processedResult;
            BGW.CancelAsync();
        }

        private static void AlgorithmH(int n, int m, ConcurrentQueue<string> queue, int selectedConst)
        {
            bool check = false;
            var a = new int[m + 1];
            var x = 0;
            
            if (selectedConst != 0)
            {
                a[0] = n - m * selectedConst + selectedConst;
                for (int j1 = 1; j1 < m; j1++)
                {
                    a[j1] = selectedConst;
                }
                a[m] = selectedConst - 2;
            }
            else
            {
                a[0] = n - m + 1;
                for (int j1 = 1; j1 < m; j1++)
                {
                    a[j1] = 1;
                }
                a[m] = -1;
            }
            // плохо while true
            while (true)
            {
                var currentResult = string.Join("", a.Take(a.Length - 1));
                queue.Enqueue(currentResult);
                var a2 = a[1];
                var a1 = a[0];
                if (a2 >= a1 - 1)
                {
                    check = true;
                }
                if (!check)
                {
                    a[0] = a[0] - 1;
                    a[1] = a[1] + 1;
                    continue;
                }
                var j = 2;
                var s = a[0] + a[1] - 1;

                if (a[j] >= a[0] - 1)
                {
                    do
                    {
                        s = s + a[j];
                        j = j + 1;
                    }
                    while (a[j] >= a[0] - 1);
                }

                if ((j + 1) > m)
                {
                    return;
                }
                else
                {
                    x = a[j] + 1;
                    a[j] = x;
                    j = j - 1;
                }
                while (j > 0)
                {
                    a[j] = x;
                    s = s - x;
                    j = j - 1;
                }

                a[0] = s;
            }
        }
    }
}
