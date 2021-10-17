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

        public static List<SingleLetter> singleLetters = new List<SingleLetter>();
        public static List<JointLetters> jointLetters1 = new List<JointLetters>();
        public static List<JointLetters> condLetters   = new List<JointLetters>();

        // Sum of Monograms
        public static int    totalCount = 0;
        public static double totalProbability = 0;
        public static double totalEntropy = 0;

        // Sum of Diagrams
        public static int    totalJointCount = 0;
        public static double totalJointProbability = 0;
        public static double totalJointEntropy = 0;

        // Sum of Conditionals
        public static double totalCondProbability = 0;
        public static double totalCondEntropy = 0;


        public static void setFrequency(string FilteredText)
        {
            for (int i=0; i< Alphabet.Length ; i++)
            {
                int charCount = FilteredText.Where(x => (x == Alphabet[i])).Count();
                singleLetters.Add(new SingleLetter { name = Alphabet[i].ToString(), frequency = charCount });
                totalCount += charCount;
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

                    jointLetters1.Add(new JointLetters {  jointName=  jointLetters, frequency = jointCharCount }) ;
                    totalJointCount += jointCharCount;

                }
            }
        }

        public static void setProbability(List<SingleLetter> singleLetterList)
        {
            for (int i = 0; i < singleLetterList.Count; i++)
            {
                singleLetterList[i].probability = (double)singleLetterList[i].frequency / totalCount;
                totalProbability += singleLetterList[i].probability;
            }
            totalProbability = Math.Round(totalProbability, 5);
        }

        public static void setJointProbability(List<SingleLetter> singleLetterList)
        {
            foreach (JointLetters item in jointLetters1)
            {
                char firstLetter  = item.jointName[0];
                char secondLetter = item.jointName[1];

                int index1 = Array.IndexOf(Alphabet, firstLetter);
                int index2 = Array.IndexOf(Alphabet, secondLetter);

                item.probability = Math.Round(singleLetterList[index1].probability * singleLetterList[index2].probability, 9);

                totalJointProbability += item.probability;
            }
        }

        public static void setCondProbability()
        {
            // p(A,B) / p(A, A to Z)

            double condProb = 0;

            foreach (var item in jointLetters1)
            {
                foreach (var j in jointLetters1)
                {
                    if(item.jointName[0] == j.jointName[0])
                    {
                        condProb += j.probability;
                    }
                }
                condProb = Math.Round(condProb, 5);
                item.conditionalProbability = Math.Round(item.probability / condProb, 5);
            }

        }

        public static void setEntropy(List<SingleLetter> singleLetterList)
        {
            for (int i = 0; i < singleLetterList.Count; i++)
            {
                singleLetterList[i].entropy = Math.Round(-1 * (singleLetterList[i].probability * Math.Log2(singleLetterList[i].probability)), 5);
                totalEntropy += singleLetterList[i].entropy;
            }
        }

        public static void setJointEntropy()
        {
            foreach (var item in jointLetters1)
            {
                item.entropy = Math.Round(-1 * (item.probability * Math.Log2(item.probability)), 5);
                totalJointEntropy += item.entropy;
            }
        }

        public static void setCondEntropy()
        {
            foreach (var item in jointLetters1)
            {
                item.entropy = Math.Round(-1 * (item.conditionalProbability * Math.Log2(item.conditionalProbability)), 5);
                totalCondEntropy += item.entropy;
            }
        }

        public static void writeMonogramTable()
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
                content += Convert.ToString(singleLetters[j].name).PadRight(15) +
                           Convert.ToString(singleLetters[j].frequency).PadRight(15) +
                           Convert.ToString(singleLetters[j].probability).PadRight(15) +
                           Convert.ToString(singleLetters[j].entropy).PadRight(15) + "\n";
            }

            // Writing last row
           content += "Total:".PadRight(15) + Convert.ToString(totalCount).PadRight(15) +
           Convert.ToString(totalProbability).PadRight(15) +
           Convert.ToString(totalEntropy).PadRight(15);

           Filtering.WriteText(Filtering.MonogramPath, content);
        }

        public static void writeDigramTable()
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

            foreach (var item in jointLetters1)
            {
                content += Convert.ToString(item.jointName).PadRight(15) +
                           Convert.ToString(item.frequency).PadRight(15) +
                           Convert.ToString(item.probability).PadRight(15) +
                           Convert.ToString(item.entropy).PadRight(15) + "\n";
            }

            // Writing last row
            content += "Total:".PadRight(15) + Convert.ToString(totalCount).PadRight(15) +
            Convert.ToString(totalProbability).PadRight(15) +
            Convert.ToString(totalEntropy).PadRight(15);

            Filtering.WriteText(Filtering.DiagramPath, content);
        }

        public static void writeConditionalTable()
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

            foreach (var item in jointLetters1)
            {
                content += Convert.ToString(item.jointName).PadRight(18) +
                           Convert.ToString(item.conditionalProbability).PadRight(18) +
                           Convert.ToString(item.entropy).PadRight(18) + "\n";
            }

           content += "Total:".PadRight(15) + Convert.ToString(totalProbability).PadRight(15) +
           Convert.ToString(totalCondEntropy).PadRight(15);

            Filtering.WriteText(Filtering.ConditionalPath, content);

        }


        public static void MonogramTable(string FilteredText)
        {
            setFrequency(FilteredText);
            setProbability(singleLetters);
            setEntropy(singleLetters);
            writeMonogramTable();
        }

        public static void DigramTable(string FilteredText)
        {
            JointFrequency(FilteredText);
            setJointProbability(singleLetters);
            setJointEntropy();
            writeDigramTable();
        }

        public static void ConditionalTable(string FilteredText)
        {
            // There are no need to calculate frequency in conditional table.
            setCondProbability();
            setCondEntropy();
            writeConditionalTable();
        }

    }

    public class SingleLetter
    {
        public string name          { get; set; }
        public int    frequency     { get; set; }
        public double probability   { get; set; }
        public double entropy       { get; set; }
    }
    public class JointLetters
    {
        public string jointName              { get; set; }
        public int    frequency              { get; set; }
        public double probability            { get; set; }
        public double entropy                { get; set; }
        public double conditionalProbability { get; set; }
    }


}
