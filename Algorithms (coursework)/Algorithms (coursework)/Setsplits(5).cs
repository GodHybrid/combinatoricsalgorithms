using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Algorithms__coursework_
{
    public partial class SetSplitsForm : Form
    {
        BackgroundWorker BGW = new BackgroundWorker();
        public SetSplitsForm()
        {
            InitializeComponent();
            BGW.WorkerSupportsCancellation = true;
        }

        async private void button1_Click(object sender, EventArgs e)
        {
            var inputValues = textBox1.Text;
            var processedResult = "";

            var splitedValues = textBox1.Text.Split(new string[] { ",", "-", ";" }, StringSplitOptions.RemoveEmptyEntries);

            textBox2.Text = "Calculating in progress..\r\n";

            BGW.RunWorkerAsync();

            await Task.Run(() =>
            {
                try
                {
                    processedResult = GenerateSubsetsEntryQueued(splitedValues);
                }
                catch (Exception ex)
                {
                    processedResult = "Error occured: " + ex.Message;
                }
            });

            textBox2.Text += processedResult + "\n";
            BGW.CancelAsync();
        }

        public static string GenerateSubsetsEntryQueued(string[] values)
        {
            var queue = new ConcurrentQueue<string>();
            string Result = string.Empty;

            var subsetGenerationTask = Task.Run(() =>
            {
                try
                {
                    for (var i = 1; i <= values.Length; i++)
                    {
                        foreach (IEnumerable<string> anotherCombination in Combinations(values, i))
                        {
                            GetPermutationsQueued(string.Join("", anotherCombination).ToArray(), queue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            Task.WaitAll(new Task[] {subsetGenerationTask});
            //MessageBox.Show("waited all");
            while (!queue.IsEmpty)
            {
                string probableSolution;
                if (!queue.TryDequeue(out probableSolution))
                {
                    continue;
                }

                Result += probableSolution + "\t";
            }

            return Result;
        }

        public static void Swap(ref char a, ref char b)
        {
            if (a == b) return;

            a ^= b;
            b ^= a;
            a ^= b;
        }

        public static void GetPermutationsQueued(char[] list, ConcurrentQueue<string> queue)
        {
            int x = list.Length - 1;
            GetPermutationsQueued(list, 0, x, queue);
        }

        public static void GetPermutationsQueued(char[] list, int k, int m, ConcurrentQueue<string> queue)
        {
            if (k == m)
            {
                queue.Enqueue(new string(list));
            }
            else
            {
                for (int i = k; i <= m; i++)
                {
                    Swap(ref list[k], ref list[i]);
                    GetPermutationsQueued(list, k + 1, m, queue);
                    Swap(ref list[k], ref list[i]);
                }
            }
        }

        public static bool NextCombination(IList<int> num, int n, int k)
        {
            if (k <= 0) return false;

            bool finished;
            var changed = finished = false;

            for (var i = k - 1; !finished && !changed; i--)
            {
                if (num[i] < n - 1 - (k - 1) + i)
                {
                    num[i]++;
                    if (i < k - 1)
                    {
                        for (var j = i + 1; j < k; j++)
                        {
                            num[j] = num[j - 1] + 1;
                        }
                    }
                    changed = true;
                }
                finished = i == 0;
            }

            return changed;
        }

        public static IEnumerable Combinations<T>(IEnumerable<T> elements, int k)
        {
            var elem = elements.ToArray();
            var size = elem.Length;

            if (k > size) yield break;

            var numbers = new int[k];

            for (var i = 0; i < k; i++)
            {
                numbers[i] = i;
            }

            do
            {
                yield return numbers.Select(n => elem[n]);
            } while (NextCombination(numbers, size, k));
        }
    }
}
