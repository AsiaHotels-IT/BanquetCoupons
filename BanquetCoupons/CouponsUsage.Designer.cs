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
            this.dateToday = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlAddData = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblCanteen = new System.Windows.Forms.Label();
            this.qty = new System.Windows.Forms.Label();
            this.note = new System.Windows.Forms.Label();
            this.btAdd = new System.Windows.Forms.Button();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.bqTopic = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSumqty = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.pnlAddData.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.pnlAddData.Controls.Add(this.lblSumqty);
            this.pnlAddData.Controls.Add(this.label3);
            this.pnlAddData.Controls.Add(this.btnClear);
            this.pnlAddData.Controls.Add(this.lblCanteen);
            this.pnlAddData.Controls.Add(this.qty);
            this.pnlAddData.Controls.Add(this.note);
            this.pnlAddData.Controls.Add(this.btAdd);
            this.pnlAddData.Controls.Add(this.txtQty);
            this.pnlAddData.Controls.Add(this.bqTopic);
            this.pnlAddData.Location = new System.Drawing.Point(674, 163);
            this.pnlAddData.Name = "pnlAddData";
            this.pnlAddData.Size = new System.Drawing.Size(461, 282);
            this.pnlAddData.TabIndex = 7;
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(300, 175);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(93, 46);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "ยกเลิก";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblCanteen
            // 
            this.lblCanteen.AutoSize = true;
            this.lblCanteen.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCanteen.Location = new System.Drawing.Point(164, 25);
            this.lblCanteen.Name = "lblCanteen";
            this.lblCanteen.Size = new System.Drawing.Size(103, 25);
            this.lblCanteen.TabIndex = 9;
            this.lblCanteen.Text = "ห้องจัดเลี้ยง";
            // 
            // qty
            // 
            this.qty.AutoSize = true;
            this.qty.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qty.Location = new System.Drawing.Point(19, 86);
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
            this.note.Location = new System.Drawing.Point(166, 128);
            this.note.Name = "note";
            this.note.Size = new System.Drawing.Size(143, 16);
            this.note.TabIndex = 4;
            this.note.Text = "***กรุณารับคูปองจากลูกค้า***";
            // 
            // btAdd
            // 
            this.btAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAdd.Location = new System.Drawing.Point(169, 175);
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Maroon;
            this.label5.Location = new System.Drawing.Point(514, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "***เลือกงานจัดเลี้ยง***";
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(275, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 25);
            this.label3.TabIndex = 11;
            this.label3.Text = "จาก";
            // 
            // lblSumqty
            // 
            this.lblSumqty.AutoSize = true;
            this.lblSumqty.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSumqty.Location = new System.Drawing.Point(323, 86);
            this.lblSumqty.Name = "lblSumqty";
            this.lblSumqty.Size = new System.Drawing.Size(24, 25);
            this.lblSumqty.TabIndex = 12;
            this.lblSumqty.Text = "0";
            // 
            // CouponsUsage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label5);
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
            this.pnlAddData.ResumeLayout(false);
            this.pnlAddData.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label userLogin;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label dateToday;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel pnlAddData;
        private System.Windows.Forms.Label bqTopic;
        private System.Windows.Forms.Label note;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Label qty;
        private System.Windows.Forms.Label lblCanteen;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblSumqty;
        private System.Windows.Forms.Label label3;
    }
}
