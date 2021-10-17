using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ad_Info_Theory
{
    class Program
    {
        // Information Theoretical Measurements of English.
             
        static void Main(string[] args)
        {
            string ReadText = Filtering.readRawText();
            string FilteredText = Filtering.filterText(ReadText);

            // Filtered data is written in "FilteredText" file.
            Filtering.WriteText(Filtering.FilteredTextPath, FilteredText);

            // Table 1: Monograms with Blanks
            Analyzer.MonogramTable(FilteredText);

            // Table 2: Diagrams with Blanks
            Analyzer.DigramTable(FilteredText);

            // Table 3: Conditionals with Blanks
            Analyzer.ConditionalTable(FilteredText);



        }
    }
}
