using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace バックアップはできます
{
    public partial class ResultForm : Form
    {
        public ResultForm()
        {
            InitializeComponent();
        }

        private void ResultForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE", @"/select," + listBox1.SelectedItem.ToString());
        }
    }
}
