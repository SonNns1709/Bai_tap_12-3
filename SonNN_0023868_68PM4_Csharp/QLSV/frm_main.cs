using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLSV
{
    public partial class frm_main : Form
    {
        private Form _activeChild = null;

        private void ShowChild(Form child)
        {
            if (_activeChild != null)
            {
                _activeChild.Close();
                _activeChild.Dispose();
                _activeChild = null;
            }

            _activeChild = child;
            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;
            this.panelMain.Controls.Clear();
            this.panelMain.Controls.Add(child);
            this.panelMain.Tag = child;
            child.BringToFront();
            child.Show();
        }
        public frm_main()
        {
            InitializeComponent();
        }

        private void quảnLýLớpHọcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_lophoc f = new frm_lophoc();
            ShowChild(f);
        }

        private void quảnLýSinhViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_sinhvien f = new frm_sinhvien();
            ShowChild(f);
        }

        void LoadMain()
        {
        
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadMain();
        }
    }
}
