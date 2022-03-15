using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace InfoTheory
{
    class Program
    {
        // Information Theoretical Measurements of English.
             
        static void Main()
        {
            // Read Raw Unfiltered text file and filter it for ther frequency, probability and entropy analyses
            string ReadText = Filtering.ReadText(Filtering.InputTextPath);
            string FilteredText = Filtering.FilterText(ReadText);

            // Filtered data is written in "Filtered" file.
            Filtering.WriteText(Filtering.FilteredTextPath, FilteredText);

            // Generates Monogram, Digram and Conditional Tables
            Analyzer.PrintAllTables(FilteredText);

            // Apply Huffman Code, Compress and Decompress the filtered text and fill the look-up table.
            ProcessMethods pMethods = new ProcessMethods();
            pMethods.ApplyHuffman();
            
            Filtering.WriteText(Filtering.LookupPath, ProcessMethods.LookUpString);

            // Encode Text and Write to File
            Filtering.WriteText(Filtering.CompressedPath, pMethods.EncodeText());

            // Decode Text and Write to File
            Filtering.WriteText(Filtering.DecompressedPath, pMethods.DecodeText());
            
        }
    }
}
