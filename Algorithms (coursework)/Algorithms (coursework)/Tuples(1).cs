using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Algorithms__coursework_
{
    public partial class TuplesForm : Form
    {
        private KeysConverter KFC = new KeysConverter();
        private BackgroundWorker BGW = new BackgroundWorker();
        String result = String.Empty;

        public TuplesForm()
        {
            InitializeComponent();
            BGW.WorkerSupportsCancellation = true;
        }

        #region Initialization
        
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private async void buttonOK_Click(object sender, EventArgs e)
        {
            var input = int.Parse(textBox1.Text);
            List<String> output = new List<string>();
            Dictionary<int, int> dictValues = new Dictionary<int, int>();

            BGW.RunWorkerAsync();

            await Task.Run(() =>
            {
                if (!checkBox1.Checked)
                {
                    if (input < 1)
                    {
                        textBox1.Text = "20";
                        textBoxLocked.Text = "input must be above 1.";
                    }
                    else
                    {
                        var currentRoot = NodeInit(input) as Node<int>;
                        output.Add(currentRoot.value.ToString());
                        WalkThroughTree(currentRoot, "", output);
                        result = string.Join(Environment.NewLine, output);
                        textBoxLocked.Text = result;
                    }
                }
                else
                {
                    List<int> userInput = textBox1.Text.ToArray().Select(x => Int32.Parse(x.ToString())).ToList();
                    Composing(userInput.Sum(), userInput, userInput.Count, dictValues, 0);
                    result = string.Join(Environment.NewLine, dictValues.Select(x => "q" + x.Key + " = " + x.Value));
                    textBoxLocked.Text = result;
                }
            });
            Task.WaitAll();
            BGW.CancelAsync();
        }
        
        #endregion Initialization

        #region Logic

        private void WalkThroughTree(Node<int> currentRoot, string path, List<string> output)
        {
            var keyValueList = currentRoot.Nodes.ToList();

            if (!String.IsNullOrEmpty(path))
            {
                output.Add(path + currentRoot.value);
            }

            ProcessSubNode(0, keyValueList.Count, path, keyValueList, output);
        }

        private void ProcessSubNode(int currentIndex, int maxIndex, string currentPath, List<KeyValuePair<int, Node<int>>> beams, List<string> result)
        {
            if (currentIndex < maxIndex)
            {
                var item = beams.ElementAt(currentIndex);
                var fullPath = currentPath + item.Key;

                if (item.Value.Nodes.Count > 0)
                {
                    WalkThroughTree(item.Value, fullPath, result);
                }
                else
                {
                    result.Add(fullPath + item.Value.value);
                }

                ProcessSubNode(currentIndex + 1, maxIndex, currentPath, beams, result);
            }
        }

        private object NodeInit(int input)
        {
            Node<int> root = new Node<int>(input);
            Dictionary<int, Node<int>> unique = new Dictionary<int, Node<int>>();
            NodeGen(input, input, root, unique);
            return root;
        }

        private void NodeGen(int Current, int Max, Node<int> root, Dictionary<int, Node<int>> unique)
        {
            if (Current > 0)
            {
                Generation(Current, Current - 1, Max, root, unique);
            }
        }

        private void Generation(int currentIndex, int v, int maxIndex, Node<int> root, Dictionary<int, Node<int>> unique)
        {
            var tempValue = currentIndex - v;

            if (tempValue > 0)
            {
                if (v != 0)
                {
                    if (unique.ContainsKey(tempValue))
                    {
                        root.Nodes.Add(v, unique[tempValue]);
                    }
                    else
                    {
                        var newNode = new Node<int>(tempValue);
                        NodeGen(tempValue, maxIndex, newNode, unique);
                        root.Nodes.Add(v, newNode);
                        unique.Add(tempValue, newNode);
                    }

                    Generation(currentIndex, v - 1, maxIndex, root, unique);
                }
            }
        }
       
        private static void Composing(int n, List<int> convertedInputs, int lastDigit, Dictionary<int, int> dValues, int digit)
        {
            dValues.Add(digit, n);

            if (digit < lastDigit)
            {
                n -= convertedInputs[digit];
                Composing(n, convertedInputs, lastDigit, dValues, digit + 1);
            }
        }

        #endregion Logic
    }

    public class Node<T>
    {
        public T value;
        public Dictionary<T, Node<T>> Nodes = new Dictionary<T, Node<T>>();

        public Node(T a)
        {
            value = a;
        }
    }
}
