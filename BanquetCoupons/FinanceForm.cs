using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BanquetCoupons
{
    public partial class FinanceForm : Form
    {
        public FinanceForm(string user)
        {
            InitializeComponent();
            this.user = user;
        }

        private string user;
        private FontManager fontManager;
        private void btnLogout_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        private void FinanceForm_Load(object sender, EventArgs e)
        {
            fontManager = new FontManager();  // สร้างครั้งเดียวตอนโหลดฟอร์ม
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            financeReport FR = new financeReport(user);
            FR.Dock = DockStyle.Fill;
            panelContent.Controls.Add(FR);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            financeHome FH = new financeHome(user);
            FH.Dock = DockStyle.Fill;
            panelContent.Controls.Add(FH);
        }

        private void panelContent_Paint_1(object sender, PaintEventArgs e)
        {
            financeHome FH = new financeHome(user);
            FH.Dock = DockStyle.Fill;
            panelContent.Controls.Add(FH);
        }
    }
}
