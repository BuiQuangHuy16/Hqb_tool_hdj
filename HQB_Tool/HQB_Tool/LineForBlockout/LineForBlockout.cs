using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlphaBIM.LineForBlockout
{
    public partial class Form1 : Form
    {
        public bool AllBlockOut { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void rbAllBlockOut_CheckedChanged(object sender, EventArgs e)
        {
            AllBlockOut = rbAllBlockOut.Checked;
        }

        private void rbPickPoint_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
