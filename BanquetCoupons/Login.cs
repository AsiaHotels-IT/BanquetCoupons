﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BanquetCoupons
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private string connectionString;

        private void Login_Load(object sender, EventArgs e)
        {
            string iniPath = "config.ini";

            var config = IniReader.ReadIni(iniPath, "Database");

            string server = config.ContainsKey("Server") ? config["Server"] : "";
            string database = config.ContainsKey("Database") ? config["Database"] : "";
            string user = config.ContainsKey("User") ? config["User"] : "";
            string password = config.ContainsKey("Password") ? config["Password"] : "";

            // กำหนดให้ตัวแปรระดับคลาส
            connectionString = $"Server={server};Database={database};User Id={user};Password={password};";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (checkLogin(username, password, out string role))
            {
                MessageBox.Show($"ยินดีต้อนรับฝ่าย : {role}");
                //เปิดฟอร์มตาม role
                switch (role.ToLower())
                {
                    case "admin":
                        new AdminForm().Show();
                        break;
                    case "finance":
                        new FinanceForm(username).Show();
                        break;
                    case "banquet":
                        new BanquetDepartment(username).Show();
                        break;
                    case "service":
                        new service(username).Show();
                        break;
                    default:
                        MessageBox.Show("ข้อมูลไม่ถูกต้อง");
                        break;
                }
                this.Hide();
            }
            else
            {
                MessageBox.Show("ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง");
            }
        }

        private bool checkLogin(string Username, string Password, out string role)
        {
            role = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT Role FROM Users 
                           WHERE Username COLLATE Latin1_General_CS_AS = @user 
                           AND Password COLLATE Latin1_General_CS_AS = @pass";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@user", Username);
                    cmd.Parameters.AddWithValue("@pass", Password);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            role = reader["Role"].ToString();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("something wrong... \n" + ex.Message);
            }
            return false;
        }


    }
}
