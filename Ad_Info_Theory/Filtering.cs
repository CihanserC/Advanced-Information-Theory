using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ad_Info_Theory
{
    class Filtering
    {
        // Datapaths
        public static string InputTextPath    = @"C:\Users\User\Desktop\Adv_Inf_Th._HW1\Input.txt";
        public static string FilteredTextPath = @"C:\Users\User\Desktop\Adv_Inf_Th._HW1\FilteredText.txt";
        public static string MonogramPath = @"C:\Users\User\Desktop\Adv_Inf_Th._HW1\Monogram.txt";
        public static string DiagramPath = @"C:\Users\User\Desktop\Adv_Inf_Th._HW1\Diagram.txt";
        public static string ConditionalPath = @"C:\Users\User\Desktop\Adv_Inf_Th._HW1\Conditional.txt";

        public static string readRawText()
        {
            string text = "";
            //Read the input file
            try
            {
                text = File.ReadAllText(InputTextPath);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.ToString());
            }

            return text;
        }

        public static string eliminateConsecutiveBlanks(string text)
        {
            string resultString = text;
            var strResult = new StringBuilder();
            strResult.Append(resultString);

            while (resultString.Contains("  "))
            {
                strResult = strResult.Replace("  ", " ");
                resultString = strResult.ToString();
            }

            return resultString;
        }

        public static string TrimCharacters(string filteredText)
        {
            string trimStr = filteredText;
            int length = 100000;
            trimStr = trimStr.Substring(0, length);
            return trimStr;
        }

        public static void WriteText(string Datapath, string Text)
        {
            try
            {
                File.WriteAllText(Datapath, Text);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static string prepareText(string FilteredText)
        {
            var strResult = new StringBuilder();
            string upperCaseLetters = FilteredText.ToUpper();
            strResult.Append(upperCaseLetters);
            // Uppercase İ do not exist in English, we must replace with I.
            strResult = strResult.Replace("İ", "I");
            // Can not represent blank character so '*' symbol is used for representation.
            strResult = strResult.Replace(" ", "-");
            upperCaseLetters = strResult.ToString();

            return upperCaseLetters;
        }

        public static string filterText(string RawText)
        {
            // 1- Filter out punctuation characters and numbers.
            // 2- The must not be 2 space characters consecutively.
            // Note: It assume there are no special names in the string.

            string FilteredText = RawText;

            // Trim non-letter characters except blank.
            foreach (char c in FilteredText)
            {
                if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                {
                    FilteredText = FilteredText.Replace(Convert.ToString(c), "");
                }
            }

            // Eliminate consecutively duplicating blank characters.
            FilteredText = eliminateConsecutiveBlanks(FilteredText);
            // Trim after the 100.000 characters
            FilteredText = TrimCharacters(FilteredText);
            // Make letters uppercase and use "-" instead of blank char.
            FilteredText = prepareText(FilteredText);

            return FilteredText;
        }

    }
}
