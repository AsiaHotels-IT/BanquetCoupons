namespace BanquetCoupons
{
    partial class BQReport
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
            this.btnExport = new System.Windows.Forms.Button();
            this.dataGridReport = new System.Windows.Forms.DataGridView();
            this.btnReport = new System.Windows.Forms.Button();
            this.pnlAddData = new System.Windows.Forms.Panel();
            this.lblCus = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCanteen = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.qty = new System.Windows.Forms.Label();
            this.bqTopic = new System.Windows.Forms.Label();
            this.cateringDate = new System.Windows.Forms.DateTimePicker();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.userLogin = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPage = new System.Windows.Forms.Label();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridReport)).BeginInit();
            this.pnlAddData.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExport
            // 
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnExport.Location = new System.Drawing.Point(701, 322);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(147, 45);
            this.btnExport.TabIndex = 19;
            this.btnExport.Text = "พิมพ์รายงาน";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Visible = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click_1);
            // 
            // dataGridReport
            // 
            this.dataGridReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridReport.Location = new System.Drawing.Point(634, 375);
            this.dataGridReport.Name = "dataGridReport";
            this.dataGridReport.Size = new System.Drawing.Size(461, 345);
            this.dataGridReport.TabIndex = 18;
            // 
            // btnReport
            // 
            this.btnReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnReport.Location = new System.Drawing.Point(869, 322);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(147, 45);
            this.btnReport.TabIndex = 17;
            this.btnReport.Text = "รายงาน";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click_1);
            // 
            // pnlAddData
            // 
            this.pnlAddData.Controls.Add(this.lblCus);
            this.pnlAddData.Controls.Add(this.label4);
            this.pnlAddData.Controls.Add(this.lblCanteen);
            this.pnlAddData.Controls.Add(this.label3);
            this.pnlAddData.Controls.Add(this.qty);
            this.pnlAddData.Controls.Add(this.bqTopic);
            this.pnlAddData.Location = new System.Drawing.Point(634, 154);
            this.pnlAddData.Name = "pnlAddData";
            this.pnlAddData.Size = new System.Drawing.Size(461, 164);
            this.pnlAddData.TabIndex = 16;
            // 
            // lblCus
            // 
            this.lblCus.AutoSize = true;
            this.lblCus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCus.Location = new System.Drawing.Point(215, 25);
            this.lblCus.Name = "lblCus";
            this.lblCus.Size = new System.Drawing.Size(103, 25);
            this.lblCus.TabIndex = 11;
            this.lblCus.Text = "ห้องจัดเลี้ยง";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(25, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 25);
            this.label4.TabIndex = 10;
            this.label4.Text = "ห้องจัดเลี้ยง";
            // 
            // lblCanteen
            // 
            this.lblCanteen.AutoSize = true;
            this.lblCanteen.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCanteen.Location = new System.Drawing.Point(215, 68);
            this.lblCanteen.Name = "lblCanteen";
            this.lblCanteen.Size = new System.Drawing.Size(103, 25);
            this.lblCanteen.TabIndex = 9;
            this.lblCanteen.Text = "ห้องจัดเลี้ยง";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(215, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 25);
            this.label3.TabIndex = 6;
            this.label3.Text = "จำนวน";
            // 
            // qty
            // 
            this.qty.AutoSize = true;
            this.qty.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qty.Location = new System.Drawing.Point(25, 111);
            this.qty.Name = "qty";
            this.qty.Size = new System.Drawing.Size(162, 25);
            this.qty.TabIndex = 5;
            this.qty.Text = "จำนวนคูปองที่สร้าง";
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
            // cateringDate
            // 
            this.cateringDate.Location = new System.Drawing.Point(22, 99);
            this.cateringDate.Name = "cateringDate";
            this.cateringDate.Size = new System.Drawing.Size(308, 20);
            this.cateringDate.TabIndex = 15;
            this.cateringDate.ValueChanged += new System.EventHandler(this.cateringDate_ValueChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(22, 152);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(592, 600);
            this.flowLayoutPanel1.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(0)))), ((int)(((byte)(10)))));
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.ForeColor = System.Drawing.Color.White;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1178, 66);
            this.panel2.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label2.Location = new System.Drawing.Point(35, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "รายงานการสร้างคูปอง";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(19, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "User";
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
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.userLogin);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 778);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1178, 43);
            this.panel1.TabIndex = 20;
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lblPage.Location = new System.Drawing.Point(838, 738);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(14, 20);
            this.lblPage.TabIndex = 30;
            this.lblPage.Text = "-";
            this.lblPage.Click += new System.EventHandler(this.lblPage_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(764, 738);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(62, 23);
            this.btnPrev.TabIndex = 29;
            this.btnPrev.Text = "ก่อนหน้า";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(917, 738);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(62, 23);
            this.btnNext.TabIndex = 28;
            this.btnNext.Text = "ถัดไป";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // BQReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.Controls.Add(this.lblPage);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.dataGridReport);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.pnlAddData);
            this.Controls.Add(this.cateringDate);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.Name = "BQReport";
            this.Size = new System.Drawing.Size(1178, 821);
            this.Load += new System.EventHandler(this.BQReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridReport)).EndInit();
            this.pnlAddData.ResumeLayout(false);
            this.pnlAddData.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridView dataGridReport;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.Panel pnlAddData;
        private System.Windows.Forms.Label lblCus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCanteen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label qty;
        private System.Windows.Forms.Label bqTopic;
        private System.Windows.Forms.DateTimePicker cateringDate;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label userLogin;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
    }
}
