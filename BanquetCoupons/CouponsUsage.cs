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
    public partial class CouponsUsage : UserControl
    {
        public CouponsUsage(string user)
        {
            InitializeComponent();
            this.user = user;
        }

        private string user;
    }
}
