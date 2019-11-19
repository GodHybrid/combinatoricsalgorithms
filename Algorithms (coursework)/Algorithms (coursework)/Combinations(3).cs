using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Algorithms__coursework_
{
    public partial class CombinationsForm : Form
    {
        public CombinationsForm()
        {
            InitializeComponent();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value > numericUpDown1.Value) numericUpDown2.Value = numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value >= numericUpDown1.Value) numericUpDown2.Value = numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String Result = String.Empty;
            int N = (int)numericUpDown1.Value;
            int T = (int)numericUpDown2.Value;
            
            Result = GenerateGraySequenceEntryQueued(N, T);
            textBox1.Text = "Generated combinations: " + Result + "\n";
        }

        public static string GenerateGraySequenceEntryQueued(int n, int t)
        {
            List<String> arr = new List<String>(new string[] {"0", "1"});
            String Result = String.Empty;
            
            int i, j;
            for (i = 2; i < (1 << n); i = i << 1)
            {
                for (j = i - 1; j >= 0; j--)
                    arr.Add(arr[j]);
                
                for (j = 0; j < i; j++)
                    arr[j] = "0" + arr[j];
                
                for (j = i; j < 2 * i; j++)
                    arr[j] = "1" + arr[j];
            }

            for (i = 0; i < arr.Count(); i++)
                if(arr[i].ToString().Count(f => f == '1') <= t)
                    Result += arr[i] + " ";

            return Result + Environment.NewLine;
        }
    }
}
