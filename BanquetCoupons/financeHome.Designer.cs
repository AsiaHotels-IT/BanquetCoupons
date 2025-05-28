namespace BanquetCoupons
{
    partial class financeHome
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.userLogin = new System.Windows.Forms.Label();
            this.cbYear = new System.Windows.Forms.ComboBox();
            this.cbMonth = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(0)))), ((int)(((byte)(10)))));
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.userLogin);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 778);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1178, 43);
            this.panel1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(19, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "User";
            // 
            // userLogin
            // 
            this.userLogin.AutoSize = true;
            this.userLogin.ForeColor = System.Drawing.Color.White;
            this.userLogin.Location = new System.Drawing.Point(73, 15);
            this.userLogin.Name = "userLogin";
            this.userLogin.Size = new System.Drawing.Size(55, 13);
            this.userLogin.TabIndex = 2;
            this.userLogin.Text = "Username";
            // 
            // cbYear
            // 
            this.cbYear.FormattingEnabled = true;
            this.cbYear.Location = new System.Drawing.Point(344, 32);
            this.cbYear.Name = "cbYear";
            this.cbYear.Size = new System.Drawing.Size(121, 21);
            this.cbYear.TabIndex = 9;
            this.cbYear.Text = "Year";
            this.cbYear.SelectedIndexChanged += new System.EventHandler(this.cbYear_SelectedIndexChanged_1);
            this.cbYear.SelectionChangeCommitted += new System.EventHandler(this.cbYear_SelectionChangeCommitted);
            // 
            // cbMonth
            // 
            this.cbMonth.FormattingEnabled = true;
            this.cbMonth.Location = new System.Drawing.Point(172, 31);
            this.cbMonth.Name = "cbMonth";
            this.cbMonth.Size = new System.Drawing.Size(121, 21);
            this.cbMonth.TabIndex = 10;
            this.cbMonth.Text = "Month";
            this.cbMonth.SelectedIndexChanged += new System.EventHandler(this.cbMonth_SelectedIndexChanged_1);
            this.cbMonth.SelectionChangeCommitted += new System.EventHandler(this.cbMonth_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 20);
            this.label1.TabIndex = 11;
            this.label1.Text = "ข้อมูลคูปอง";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(32, 76);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(767, 591);
            this.flowLayoutPanel1.TabIndex = 12;
            // 
            // financeHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbMonth);
            this.Controls.Add(this.cbYear);
            this.Controls.Add(this.panel1);
            this.Name = "financeHome";
            this.Size = new System.Drawing.Size(1178, 821);
            this.Load += new System.EventHandler(this.financeHome_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label userLogin;
        private System.Windows.Forms.ComboBox cbYear;
        private System.Windows.Forms.ComboBox cbMonth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
