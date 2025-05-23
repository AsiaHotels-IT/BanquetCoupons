using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace BanquetCoupons
{
    public partial class BanquetDepartment : Form
    {
        public BanquetDepartment(string user)
        {
            InitializeComponent();
            this.user = user;
        }

        private string user;
        private void btnLogout_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear(); // เคลียร์ของเก่า
            BQHome bQHome = new BQHome(user);
            bQHome.Dock = DockStyle.Fill;
            panelContent.Controls.Add(bQHome);
            //MessageBox.Show($"{user}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            Coupons coupons = new Coupons(user);
            coupons.Dock = DockStyle.Fill;
            panelContent.Controls.Add(coupons);
        }

        private void panelContent_Paint(object sender, PaintEventArgs e)
        {
            BQHome bQHome = new BQHome(user);
            bQHome.Dock = DockStyle.Fill;
            panelContent.Controls.Add(bQHome);
        }

        private void BanquetDepartment_Load(object sender, EventArgs e)
        {

        }
    }
}
