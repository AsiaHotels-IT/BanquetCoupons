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
    public partial class service : Form
    {
        public service(string user)
        {
            InitializeComponent();
            this.user = user;
        }

        private string user;

        private void service_Load(object sender, EventArgs e)
        {
           
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        private void panelContent_Paint(object sender, PaintEventArgs e)
        {
            CouponsUsage CU = new CouponsUsage(user);
            CU.Dock= DockStyle.Fill;
            panelContent.Controls.Add(CU);
        }


        private void btnHome_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            CouponsUsage CU = new CouponsUsage(user);
            CU.Dock= DockStyle.Fill;
            panelContent.Controls.Add(CU);
        }
    }


}
