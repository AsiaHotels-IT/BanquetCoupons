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
    public partial class financeReport : UserControl
    {
        public financeReport(string user)
        {
            InitializeComponent();
            this.user = user;   
        }

        private string user;
        private void financeReport_Load(object sender, EventArgs e)
        {

        }
    }
}
