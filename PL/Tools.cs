using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PL;

internal class Tools
{
    public static bool IsValidPhone(string phone)
    {
        if (Regex.IsMatch(phone, @"^05([ -]?)\d{8}$"))
            return true;
        MessageBox.Show("Phone number is not valid.");
        return false;
    }

    public static bool IsValidAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return false;
        if (Regex.IsMatch(address.Trim(),
            @"^(?=.*[A-Za-zא-ת])(?=.*\d)[A-Za-zא-ת0-9\s.,'""/\-]{5,100}$"))
            return true;
        MessageBox.Show("Address is not valid.");
        return false;
    }



        // בדיקת שם מלא בעברית או באנגלית + הודעה פנימית
        public static bool ValidateFullName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Full name is required.");
            }

            // עברית או אנגלית, לפחות שני שמות
            string pattern = @"^[A-Za-zא-ת]+(\s[A-Za-zא-ת]+)+$";

            if (!Regex.IsMatch(name.Trim(), pattern))
            {
                MessageBox.Show("Invalid full name. Please enter first and last name in Hebrew or English.");
                return false;
        }
            return true;
    }
        // בדיקת תעודת זהות (מקבלת int וממירה ל-string בפנים)
        public static bool ValidateIdNumber(int id)
        {
            // המרה ל-string
            string idStr = id.ToString();

            // בדיקה: בדיוק 9 ספרות
            string pattern = @"^\d{9}$";

            if (!Regex.IsMatch(idStr, pattern))
            {
                MessageBox.Show("Invalid ID number.");
                return false;
        }
            return true;    
    }

    public static  bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))

            return false;

        if (Regex.IsMatch(email.Trim(), @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"))
            return true;
        MessageBox.Show(" email is not valid.");
        return false;

    }

}
