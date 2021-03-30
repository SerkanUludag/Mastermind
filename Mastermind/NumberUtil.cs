using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mastermind
{
    public static class NumberUtil
    {
        public static bool IsNumberValid(int number)
        {
            if(IsDigitsUnique(number) && GetDigitCount(number) == 4)
            {
                return true;
            }
            return false;
        }

        private static bool IsDigitsUnique(int number)
        {
            char[] digits = GetDigits(number);

            if (digits.Length != digits.Distinct().Count())
            {
                return false;
            }
            return true;

        }

        public static char[] GetDigits(int number)
        {
            int numberOfDigits = GetDigitCount(number);
            char[] digits = new char[numberOfDigits];

            int i = numberOfDigits - 1;
            while (number > 0)
            {               
                digits[i] = (char)((number % 10) + 48);
                number = number / 10;
                i--;
            }
            return digits;
        }
        
        private static int GetDigitCount(int number)
        {
            return (int)Math.Floor(Math.Log10(number) + 1);
        }
    }
}
