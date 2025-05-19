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
    public partial class BanquetDepartment : Form
    {
        public BanquetDepartment()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear(); // เคลียร์ของเก่า
            BQHome bQHome = new BQHome();
            bQHome.Dock = DockStyle.Fill;
            panelContent.Controls.Add(bQHome);
        }
    }
}
