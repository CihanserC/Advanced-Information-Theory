using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ad_Info_Theory
{

    public class Analyzer
    {
        public static char[] Alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '-' };

        public static List<Letters> SingleLetters = new List<Letters>();
        public static List<Letters> JointLetters1 = new List<Letters>();
        public static List<Letters> CondLetters   = new List<Letters>();

        // Sum of Monograms
        public static int    TotalCount = 0;
        public static double TotalProbability = 0;
        public static double TotalEntropy = 0;
                             
        // Sum of Diagrams   
        public static int    TotalJointCount = 0;
        public static double TotalJointProbability = 0;
        public static double TotalJointEntropy = 0;

        // Sum of Conditionals
        public static double totalCondProbability = 0;
        public static double totalCondEntropy = 0;


        public static void SetFrequency(string FilteredText)
        {
            for (int i=0; i< Alphabet.Length ; i++)
            {
                int charCount = FilteredText.Where(x => (x == Alphabet[i])).Count();
                SingleLetters.Add(new Letters { Name = Alphabet[i].ToString(), Frequency = charCount });
                TotalCount += charCount;
            }
        }

        public static void JointFrequency(string FilteredText)
        {
            var strResult = new StringBuilder();

            for (int i = 0; i < Alphabet.Length; i++)
            {
                for (int j = 0; j < Alphabet.Length; j++)
                {
                    strResult.Append(Alphabet[i]);
                    strResult.Append(Alphabet[j]);

                    string jointLetters = strResult.ToString();

                    int jointCharCount = Regex.Matches(FilteredText, jointLetters).Count;
                    strResult.Clear();

                    JointLetters1.Add(new Letters {  Name=  jointLetters, Frequency = jointCharCount }) ;
                    TotalJointCount += jointCharCount;

                }
            }
        }

        public static void SetProbability(List<Letters> singleLetterList)
        {
            for (int i = 0; i < singleLetterList.Count; i++)
            {
                singleLetterList[i].Probability = (double)singleLetterList[i].Frequency / TotalCount;
                TotalProbability += singleLetterList[i].Probability;
            }
            TotalProbability = Math.Round(TotalProbability, 5);
        }

        public static void SetJointProbability(List<Letters> singleLetterList)
        {
            foreach (Letters item in JointLetters1)
            {
                char firstLetter  = item.Name[0];
                char secondLetter = item.Name[1];

                int index1 = Array.IndexOf(Alphabet, firstLetter);
                int index2 = Array.IndexOf(Alphabet, secondLetter);

                item.Probability = Math.Round(singleLetterList[index1].Probability * singleLetterList[index2].Probability, 9);

                TotalJointProbability += item.Probability;
            }
        }

        public static void SetCondProbability()
        {
            // p(A,B) / p(A, A to Z)

            double condProb = 0;

            foreach (var item in JointLetters1)
            {
                foreach (var j in JointLetters1)
                {
                    if(item.Name[0] == j.Name[0])
                    {
                        condProb += j.Probability;
                    }
                }
                condProb = Math.Round(condProb, 5);
                item.ConditionalProbability = Math.Round(item.Probability / condProb, 5);
            }

        }

        public static void SetEntropyForAll(List<Letters> Letters, int CalculationType)
        {
            double total = 0;

            if (CalculationType == 1 || CalculationType == 2)
            {
                foreach (var item in Letters)
                {
                    item.Entropy = Math.Round(-1 * (item.Probability * Math.Log2(item.Probability)), 5);
                    total += item.Entropy;
                }
            }
            else if (CalculationType == 3)
            {
                foreach (var item in Letters)
                {
                    item.Entropy = Math.Round(-1 * (item.ConditionalProbability * Math.Log2(item.ConditionalProbability)), 5);
                    total += item.Entropy;
                }
            }

            if (CalculationType == 1)
            {
                TotalEntropy = total;
            }
            else if (CalculationType == 2)
            {
                TotalJointEntropy = total;
            }
            else if (CalculationType == 3)
            {
                totalCondEntropy = total;
            }

        }

        public static void WriteMonogramTable()
        {
            string[] columns = { "Monogram", "Frequency", "Probability", "Entropy" };
            string[] lines   = { "========", "========= ", "===========", "=======" };
            string content = "";

            // Add columns and lines
            for (int i = 0; i < columns.Length; i++)
            {
                content += Convert.ToString(columns[i]).PadRight(15);
            }
            content += "\n";

            for (int i = 0; i < lines.Length; i++)
            {
                content += Convert.ToString(lines[i]).PadRight(15);
            }
            content += "\n";

            for (int j = 0; j < Alphabet.Length; j++)
            {
                content += Convert.ToString(SingleLetters[j].Name).PadRight(15) +
                           Convert.ToString(SingleLetters[j].Frequency).PadRight(15) +
                           Convert.ToString(SingleLetters[j].Probability).PadRight(15) +
                           Convert.ToString(SingleLetters[j].Entropy).PadRight(15);
                content += "\n";
            }

            // Writing last row
            content += "Total:".PadRight(15) + Convert.ToString(TotalCount).PadRight(15) +
           Convert.ToString(TotalProbability).PadRight(15) +
           Convert.ToString(TotalEntropy).PadRight(15);

           Filtering.WriteText(Filtering.MonogramPath, content);
        }

        public static void WriteDigramTable()
        {
            string[] columns = { "Diagram", "Frequency", "Probability", "Entropy" };
            string[] lines   = { "=======", "========= ", "===========", "=======" };
            string content = "";

            // Add columns and lines
            for (int i = 0; i < columns.Length; i++)
            {
                content += Convert.ToString(columns[i]).PadRight(15);
            }
            content += "\n";
            for (int i = 0; i < lines.Length; i++)
            {
                content += Convert.ToString(lines[i]).PadRight(15);
            }
            content += "\n";

            foreach (var item in JointLetters1)
            {
                content += Convert.ToString(item.Name).PadRight(15) +
                           Convert.ToString(item.Frequency).PadRight(15) +
                           Convert.ToString(item.Probability).PadRight(15) +
                           Convert.ToString(item.Entropy).PadRight(15);
                content += "\n";
            }

            // Writing last row
            content += "Total:".PadRight(15) + Convert.ToString(TotalCount).PadRight(15) +
            Convert.ToString(TotalProbability).PadRight(15) +
            Convert.ToString(TotalEntropy).PadRight(15);

            Filtering.WriteText(Filtering.DiagramPath, content);
        }

        public static void WriteConditionalTable()
        {
            string[] columns = { "Conditional", "Probability", "Entropy" };
            string[] lines   = { "===========", "===========", "=======" };
            string content = "";

            // Add columns and lines
            for (int i = 0; i < columns.Length; i++)
            {
                content += Convert.ToString(columns[i]).PadRight(18);
            }
            content += "\n";
            for (int i = 0; i < lines.Length; i++)
            {
                content += Convert.ToString(lines[i]).PadRight(18);
            }
            content += "\n";

            foreach (var item in JointLetters1)
            {
                content += Convert.ToString(item.Name).PadRight(18) +
                           Convert.ToString(item.ConditionalProbability).PadRight(18) +
                           Convert.ToString(item.Entropy).PadRight(18);
                content += "\n";

            }

            content += "Total:".PadRight(18) + Convert.ToString(TotalProbability).PadRight(18) +
           Convert.ToString(totalCondEntropy).PadRight(18);

            Filtering.WriteText(Filtering.ConditionalPath, content);

        }


        public static void MonogramTable(string FilteredText)
        {
            SetFrequency(FilteredText);
            SetProbability(SingleLetters);
            SetEntropyForAll(SingleLetters, 1);
            //SetEntropy(SingleLetters);
            WriteMonogramTable();
        }

        public static void DigramTable(string FilteredText)
        {
            JointFrequency(FilteredText);
            SetJointProbability(SingleLetters);
            SetEntropyForAll(JointLetters1, 2);
            //SetJointEntropy();
            WriteDigramTable();
        }

        public static void ConditionalTable(string FilteredText)
        {
            // There are no need to calculate frequency in conditional table.
            SetCondProbability();
            SetEntropyForAll(CondLetters, 3);
            //SetCondEntropy();
            WriteConditionalTable();
        }

    }

    public class Letters
    {
        public string Name                   { get; set; }
        public int    Frequency              { get; set; }
        public double Probability            { get; set; }
        public double Entropy                { get; set; }
        public double ConditionalProbability { get; set; }
    }


}
