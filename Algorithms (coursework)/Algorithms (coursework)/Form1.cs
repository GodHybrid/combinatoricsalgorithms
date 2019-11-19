using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Algorithms__coursework_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = null;
            comboBox1.SelectedText = "[ Choose a task ]";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    new TuplesForm().ShowDialog();
                    break;
                case 1:
                    new PermutationsForm().ShowDialog();
                    break;
                case 2:
                    new CombinationsForm().ShowDialog();
                    break;
                case 3:
                    new SplitsForm().ShowDialog();
                    break;
                case 4:
                    new SetSplitsForm().ShowDialog();
                    break;
                default:
                    break;
            }
        }
    }
}
