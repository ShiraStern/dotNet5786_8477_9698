using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PL
{
    internal class Tools
    {


        // בדיקת שם מלא בעברית או באנגלית + הודעה פנימית
        public static void ValidateFullName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Full name is required.");
                throw new Exception();
            }

            // עברית או אנגלית, לפחות שני שמות
            string pattern = @"^[A-Za-zא-ת]+(\s[A-Za-zא-ת]+)+$";

            if (!Regex.IsMatch(name.Trim(), pattern))
            {
                MessageBox.Show("Invalid full name. Please enter first and last name in Hebrew or English.");
                throw new Exception();
            }
        }
        // בדיקת תעודת זהות (מקבלת int וממירה ל-string בפנים)
        public static void ValidateIdNumber(int id)
        {
            // המרה ל-string
            string idStr = id.ToString();

            // בדיקה: בדיוק 9 ספרות
            string pattern = @"^\d{9}$";

            if (!Regex.IsMatch(idStr, pattern))
            {
                MessageBox.Show("Invalid ID number.");
                throw new Exception();
            }
        }















    }
}
