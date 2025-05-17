using System;
using System.Collections.Generic;
using System.IO;

class IniReader
{
    public static Dictionary<string, string> ReadIni(string filepath, string section)
    {
        var config = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(filepath);
        bool inSection = false;

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("#")) //ข้ามบรรทัดว่าง
            continue; 

            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                inSection = trimmed.Substring(1, trimmed.Length - 2).Equals(section, StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (inSection && trimmed.Contains("="))
            {
                var parts = trimmed.Split('=', (char)2);
                if (parts.Length == 2)
                    config[parts[0].Trim()] = parts[1].Trim();
            }
        }
        return config;
    }
}