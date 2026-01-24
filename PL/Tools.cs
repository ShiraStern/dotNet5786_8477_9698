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
    public static void IsValidPhone(string phone)
    {
        if (Regex.IsMatch(phone, @"^05([ -]?)\d{8}$"))
        MessageBox.Show("Phone number is not valid.");
    }

    public bool IsValidAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return false;

        return Regex.IsMatch(
            address.Trim(),
            @"^(?=.*[A-Za-zא-ת])(?=.*\d)[A-Za-zא-ת0-9\s.,'""/\-]{5,100}$"
        );
    }

}
