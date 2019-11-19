using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Algorithms__coursework_
{
    public partial class PermutationsForm : Form
    {
        BackgroundWorker BGW = new BackgroundWorker();

        public PermutationsForm()
        {
            InitializeComponent();
            BGW.WorkerSupportsCancellation = true;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (Tab2Checks())
            {
                var inputValues = textBox1.Text;
                var processedResult = "";

                textBox2.Text = "Calculating in progress..\r\n";

                await Task.Run(() =>
                {
                    try
                    {
                        processedResult = ProcessAlphametricEntryQueued(inputValues);

                        if (string.IsNullOrEmpty(processedResult))
                        {
                            processedResult = "Phew. Sorry. No result exist!";
                        }
                    }
                    catch (Exception ex)
                    {
                        processedResult = "Error occured: " + ex.Message;
                    }
                });
                Task.WaitAll();
                textBox2.Text += processedResult;
            }
        }

        bool Tab2Checks()
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Empty input values");
                return false;
            }

            var letters = new String(textBox1.Text.Except("+= ").ToArray()).ToList();
            if (letters.Count > 10)
            {
                MessageBox.Show("Too many letters. Only allowed no more than 10 distinct!");
                return false;
            }

            if (!letters.All(c => c >= 'A' && c <= 'Z'))
            {
                MessageBox.Show("Invalid letters inside input string");
                return false;
            }

            return true;
        }

        public static string ProcessAlphametricEntryQueued(string text)
        {
            var letters = text.Where(c => c >= 'A' && c <= 'Z').Distinct().OrderBy(c => c).ToList();
            var currentLetters = new String(letters.ToArray());
            
            var notZero = letters.Select((z, index) => new { letter = z, index = index }).Where(x => Regex.IsMatch(text, "[^A-Z]" + x.letter)).Select(x => x.index).ToList();

            var queue = new ConcurrentQueue<string>();
            bool finished = false;
            var finalAnswer = "";
            var replacedResult = "";
            var found = false;
            var sides = text.Split(new string[] { "==", "=" }, StringSplitOptions.None);

            var operationsLeft = new Queue<char>();
            var operationsRight = new Queue<char>();

            foreach (var anotherChar in sides[0])
            {
                if (anotherChar == '+')
                {
                    operationsLeft.Enqueue(anotherChar);
                }
            }

            foreach (var anotherChar in sides[1])
            {
                if (anotherChar == '+')
                {
                    operationsRight.Enqueue(anotherChar);
                }
            }

            var permutationGenerationTask = Task.Run(() =>
            {
                try
                {
                    GetPermutationsQueued("0123456789".ToArray(), queue);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    finished = true;
                }
            });

            while (true)
            {
                while (!queue.IsEmpty)
                {
                    string probableSolution;

                    if (!queue.TryDequeue(out probableSolution))
                    {
                        break;
                    }

                    var badCombination = false;
                    foreach (var nonZeroItem in notZero)
                    {
                        if (probableSolution[nonZeroItem] == '0')
                        {
                            badCombination = true;
                            break;
                        }
                    }

                    if (badCombination)
                    {
                        continue;
                    }

                    replacedResult = text;

                    for (int c = 0; c < letters.Count; c++)
                    {
                        replacedResult = replacedResult.Replace(currentLetters[c], probableSolution[c]);
                    }

                    sides = replacedResult.Split(new string[] { "==", "=" }, StringSplitOptions.None);

                    try
                    {
                        var solvedLeftTest = sides[0].Split('+').Select(a => double.Parse(a)).ToList();
                        var solvedRightTest = sides[1].Split('+').Select(a => double.Parse(a)).ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        continue;
                    }

                    var solvedLeft = sides[0].Split('+').Select(a => double.Parse(a)).ToList();
                    var solvedRight = sides[1].Split('+').Select(a => double.Parse(a)).ToList();

                    while (solvedLeft.Count > 1)
                    {
                        var operation = operationsLeft.Dequeue();
                        var leftOperand = solvedLeft[0];
                        var rightOperand = solvedLeft[1];

                        solvedLeft[0] = leftOperand + rightOperand;
                        solvedLeft.RemoveAt(1);
                        operationsLeft.Enqueue(operation);
                    }

                    while (solvedRight.Count > 1)
                    {
                        var operation = operationsRight.Dequeue();
                        var leftOperand = solvedRight[0];
                        var rightOperand = solvedRight[1];

                        solvedRight[0] = leftOperand + rightOperand;
                        solvedRight.RemoveAt(1);
                        operationsRight.Enqueue(operation);
                    }

                    found = (solvedLeft[0] == solvedRight[0]);

                    if (found)
                    {
                        finalAnswer += "{" + string.Join(", ", letters.ToList().Select((c, index) => "\"" + c + "\"=>" + probableSolution[index])) + "} \r\n" + replacedResult;
                    }
                }

                if (finished && queue.IsEmpty)
                {
                    break;
                }
            }

            return finalAnswer;
        }
        #region permutationsstuff
        public static void Swap(ref char a, ref char b)
        {
            if (a == b) return;

            a ^= b;
            b ^= a;
            a ^= b;
        }

        public static char[][] GetPermutations(char[] list, int length)
        {
            if (length == 1) return list.Select(t => new char[] { t }).ToArray();
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)).ToArray(),
                    (t1, t2) => t1.Concat(new char[] { t2 }).ToArray()).ToArray();
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
        #endregion
    }
}
