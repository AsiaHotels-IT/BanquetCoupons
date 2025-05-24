namespace BanquetCoupons
{
    partial class CouponsUsage
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.userLogin = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dateToday = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlAddData = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.qty = new System.Windows.Forms.Label();
            this.note = new System.Windows.Forms.Label();
            this.btAdd = new System.Windows.Forms.Button();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.couTopic = new System.Windows.Forms.Label();
            this.bqTopic = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.lblCanteen = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlAddData.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.Location = new System.Drawing.Point(68, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "รับคูปอง";
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
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(0)))), ((int)(((byte)(10)))));
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.userLogin);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 778);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1178, 43);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Controls.Add(this.label1);
            this.panel2.ForeColor = System.Drawing.Color.White;
            this.panel2.Location = new System.Drawing.Point(22, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 66);
            this.panel2.TabIndex = 4;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // dateToday
            // 
            this.dateToday.AutoSize = true;
            this.dateToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.dateToday.Location = new System.Drawing.Point(42, 129);
            this.dateToday.Name = "dateToday";
            this.dateToday.Size = new System.Drawing.Size(51, 20);
            this.dateToday.TabIndex = 5;
            this.dateToday.Text = "label3";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(23, 163);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(634, 609);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // pnlAddData
            // 
            this.pnlAddData.Controls.Add(this.btnClear);
            this.pnlAddData.Controls.Add(this.lblCanteen);
            this.pnlAddData.Controls.Add(this.txtSerial);
            this.pnlAddData.Controls.Add(this.label4);
            this.pnlAddData.Controls.Add(this.label3);
            this.pnlAddData.Controls.Add(this.qty);
            this.pnlAddData.Controls.Add(this.note);
            this.pnlAddData.Controls.Add(this.btAdd);
            this.pnlAddData.Controls.Add(this.txtQty);
            this.pnlAddData.Controls.Add(this.couTopic);
            this.pnlAddData.Controls.Add(this.bqTopic);
            this.pnlAddData.Location = new System.Drawing.Point(674, 163);
            this.pnlAddData.Name = "pnlAddData";
            this.pnlAddData.Size = new System.Drawing.Size(461, 282);
            this.pnlAddData.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(189, 233);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 25);
            this.label3.TabIndex = 6;
            this.label3.Text = "จำนวน";
            // 
            // qty
            // 
            this.qty.AutoSize = true;
            this.qty.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qty.Location = new System.Drawing.Point(25, 234);
            this.qty.Name = "qty";
            this.qty.Size = new System.Drawing.Size(144, 25);
            this.qty.TabIndex = 5;
            this.qty.Text = "จำนวนคูปองที่ใช้";
            // 
            // note
            // 
            this.note.AutoSize = true;
            this.note.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.note.ForeColor = System.Drawing.Color.Maroon;
            this.note.Location = new System.Drawing.Point(170, 119);
            this.note.Name = "note";
            this.note.Size = new System.Drawing.Size(143, 16);
            this.note.TabIndex = 4;
            this.note.Text = "***กรุณารับคูปองจากลูกค้า***";
            // 
            // btAdd
            // 
            this.btAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAdd.Location = new System.Drawing.Point(169, 157);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(93, 46);
            this.btAdd.TabIndex = 3;
            this.btAdd.Text = "เพิ่มข้อมูล";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // txtQty
            // 
            this.txtQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQty.Location = new System.Drawing.Point(169, 85);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(100, 26);
            this.txtQty.TabIndex = 2;
            // 
            // couTopic
            // 
            this.couTopic.AutoSize = true;
            this.couTopic.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.couTopic.Location = new System.Drawing.Point(25, 84);
            this.couTopic.Name = "couTopic";
            this.couTopic.Size = new System.Drawing.Size(129, 25);
            this.couTopic.TabIndex = 1;
            this.couTopic.Text = "หมายเลขคูปอง";
            // 
            // bqTopic
            // 
            this.bqTopic.AutoSize = true;
            this.bqTopic.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bqTopic.Location = new System.Drawing.Point(25, 25);
            this.bqTopic.Name = "bqTopic";
            this.bqTopic.Size = new System.Drawing.Size(62, 25);
            this.bqTopic.TabIndex = 0;
            this.bqTopic.Text = "BQID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(275, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 25);
            this.label4.TabIndex = 7;
            this.label4.Text = "-";
            // 
            // txtSerial
            // 
            this.txtSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerial.Location = new System.Drawing.Point(300, 83);
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.Size = new System.Drawing.Size(132, 26);
            this.txtSerial.TabIndex = 8;
            // 
            // lblCanteen
            // 
            this.lblCanteen.AutoSize = true;
            this.lblCanteen.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCanteen.Location = new System.Drawing.Point(164, 25);
            this.lblCanteen.Name = "lblCanteen";
            this.lblCanteen.Size = new System.Drawing.Size(62, 25);
            this.lblCanteen.TabIndex = 9;
            this.lblCanteen.Text = "BQID";
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(300, 157);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(93, 46);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "ยกเลิก";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // CouponsUsage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.Controls.Add(this.pnlAddData);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.dateToday);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "CouponsUsage";
            this.Size = new System.Drawing.Size(1178, 821);
            this.Load += new System.EventHandler(this.CouponsUsage_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pnlAddData.ResumeLayout(false);
            this.pnlAddData.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label userLogin;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label dateToday;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel pnlAddData;
        private System.Windows.Forms.Label bqTopic;
        private System.Windows.Forms.Label note;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Label couTopic;
        private System.Windows.Forms.Label qty;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSerial;
        private System.Windows.Forms.Label lblCanteen;
        private System.Windows.Forms.Button btnClear;
    }
}
