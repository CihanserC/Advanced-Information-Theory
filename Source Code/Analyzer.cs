using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace InfoTheory

{

    public class Analyzer
    {
        public static char[] Alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                                          'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '-' };
        //'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'ı', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',  };

        public static List<Letters> SingleLetters = new List<Letters>();
        public static List<Letters> JointLetters1 = new List<Letters>();
        public static List<Letters> CondLetters   = new List<Letters>();

        // Sum of Monograms
        public static int    TotalCount = 0;
        public static double TotalProbability = 0;
        public static double TotalEntropy = 0;

        // Sum of Digrams   
        public static int    TotalJointCount = 0;
        public static double TotalJointProbability = 0;
        public static double TotalJointEntropy = 0;
        //public static double DiRedundancy = 0;

        // Sum of Conditionals
        public static double totalCondProbability = 0;
        public static double totalCondEntropy = 0;
        //public static double CondRedundancy = 0;

        public static double Redundancy = 0;
        public static double AverageLength = 0;


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
                    CondLetters.Add(new Letters { Name = jointLetters, Frequency = jointCharCount });
                    TotalJointCount += jointCharCount;

                }
            }
        }

        public static void SetProbability(List<Letters> singleLetterList)
        {
            for (int i = 0; i < singleLetterList.Count; i++)
            {
                singleLetterList[i].Probability = Math.Round((double)singleLetterList[i].Frequency / TotalCount, 5); 
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

                item.Probability = Math.Round(singleLetterList[index1].Probability * singleLetterList[index2].Probability, 5);

                TotalJointProbability += item.Probability;
            }
            TotalJointProbability = Math.Round(TotalJointProbability, 4);
        }

        public static void SetCondProbability()
        {
            // p(A,B) / p(A, A to Z)

            int i = 0;
            int j = 0;
            int k = 0;
            double[] condProbFirstLetter = new double[Alphabet.Length];

            while (i < JointLetters1.Count)
            {
                double condProb = 0;

                while ((JointLetters1[i].Name[0] == JointLetters1[j].Name[0]) && j < JointLetters1.Count -1)
                {
                    condProb += JointLetters1[j].Probability;
                    j++;
                }
                condProbFirstLetter[k] = Math.Round(condProb,6);
                k++;
                i += Alphabet.Length;
            }

            int m = 0;
            while (m < CondLetters.Count)
            {
                CondLetters[m].Probability = Math.Round(JointLetters1[m].Probability / condProbFirstLetter[m/Alphabet.Length],5);
                m++;
            }

            foreach (var item2 in CondLetters)
            {
                totalCondProbability += item2.Probability;
            }
            totalCondProbability = Math.Round(totalCondProbability, 4);

        }

        public static void SetEntropyForAll(List<Letters> Letters, int CalculationType)
        {
            double total = 0.0;

            foreach (var item in Letters)
            {
                item.Entropy = Math.Round(-1 * (item.Probability * Math.Log2(item.Probability)), 5);
                total +=  Double.IsNaN(item.Entropy) ? 0: item.Entropy; 

            }           

            if (CalculationType == 1)
            {
                TotalEntropy = Math.Round(total,4);
            }
            else if (CalculationType == 2)
            {
                TotalJointEntropy = Math.Round(total, 4);
            }
            else if (CalculationType == 3)
            {
                totalCondEntropy = Math.Round(total, 4);
            }
        }

        // Calculates redundancy for all 3 tables.
        public static void CalculateRedundancy(double entropy)
        {
            // 27 is the total character quantity in the alphabet.
            Redundancy = Math.Round(1 - (entropy / Alphabet.Length), 4);
        }

        public static void CheckNaNDoubles(List<Letters> LetterList)
        {
            foreach (var item in LetterList)
            {
                if(item.Frequency == 0 || item.Probability == 0)
                {
                    item.Entropy = 0;
                }
            }
        }

        public static void WriteMonogramTable()
        {
            string[] columns = { "Monogram", "Frequency", "Probability", "Entropy" };
            string[] lines   = { "========", "=========", "===========", "=======" };
            string content = "";
            CheckNaNDoubles(SingleLetters);
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
            content += "Total:   ".PadRight(15) + Convert.ToString(TotalCount).PadRight(15) +
                        Convert.ToString(TotalProbability).PadRight(15) +
                        Convert.ToString(TotalEntropy).PadRight(15);


            content += "\nRedundancy:".PadRight(16) + Convert.ToString(Redundancy).PadRight(15);

            Filtering.WriteText(Filtering.MonogramPath, content);
        }

        public static void WriteDigramTable()
        {
            string[] columns = { "Diagram", "Frequency", "Probability", "Entropy" };
            string[] lines   = { "=======", "=========", "===========", "=======" };
            string content = "";
            CheckNaNDoubles(JointLetters1);
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
            content += "Total:".PadRight(15) + Convert.ToString(TotalJointCount).PadRight(15) +
                        Convert.ToString(TotalJointProbability).PadRight(15) +
                        Convert.ToString(TotalJointEntropy).PadRight(15);

            content += "\nRedundancy: ".PadRight(16) + Convert.ToString(Redundancy).PadRight(15);

            Filtering.WriteText(Filtering.DigramPath, content);
        }

        public static void WriteConditionalTable()
        {
            string[] columns = { "Conditional", "Probability", "Entropy" };
            string[] lines   = { "===========", "===========", "=======" };
            string content = "";
            CheckNaNDoubles(CondLetters);
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
                           Convert.ToString(item.Probability).PadRight(18) +
                           Convert.ToString(item.Entropy).PadRight(18);
                content += "\n";

            }

            content += "Total:".PadRight(18) + Convert.ToString(TotalJointProbability).PadRight(18) +
                        Convert.ToString(totalCondEntropy).PadRight(18);

            content += "\nRedundancy: ".PadRight(19) + Convert.ToString(Redundancy).PadRight(15);

            Filtering.WriteText(Filtering.ConditionalPath, content);
        }

        public static void MonogramTable(string FilteredText)
        {
            SetFrequency(FilteredText);
            SetProbability(SingleLetters);
            SetEntropyForAll(SingleLetters, 1);
            CalculateRedundancy(TotalEntropy);
            WriteMonogramTable();
        }

        public static void DigramTable(string FilteredText)
        {
            JointFrequency(FilteredText);
            SetJointProbability(SingleLetters);
            SetEntropyForAll(JointLetters1, 2);
            CalculateRedundancy(TotalJointEntropy);
            WriteDigramTable();
        }

        public static void ConditionalTable()
        {
            // There are no need to calculate frequency in conditional table.
            SetCondProbability();
            SetEntropyForAll(CondLetters, 3);
            CalculateRedundancy(totalCondEntropy);
            WriteConditionalTable();
        }

        // Generates Monogram, Diagram and Conditional Tables
        public static void PrintAllTables(string FilteredText)
        {
            // Table 1: Monograms with Blanks
            Analyzer.MonogramTable(FilteredText);

            // Table 2: Diagrams with Blanks
            Analyzer.DigramTable(FilteredText);

            // Table 3: Conditionals with Blanks
            Analyzer.ConditionalTable();
        }

    }

    public class Letters
    {
        public string Name                   { get; set; }
        public int    Frequency              { get; set; }
        public double Probability            { get; set; }
        public double Entropy                { get; set; }
    }


}
