﻿using BanquetCoupons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        string path = "config.ini";
        var config = IniReader.ReadIni(path, "Database");

        Console.WriteLine("ค่าจากไฟล์ INI : ");
        foreach(var item in config)
        {
            Console.WriteLine($"{item.Key} = {item.Value}");
        }
        //MessageBox.Show("Connect success");

        string connectionString = $"Server={config["Server"]};Database={config["Database"]};User Id={config["User"]};Password={config["Password"]};";
        Console.WriteLine("\n Connection String: ");
        Console.WriteLine(connectionString);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Login()); // เรียกฟอร์มหลัก
    }
}
