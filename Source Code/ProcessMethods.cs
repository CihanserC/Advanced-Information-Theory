using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace InfoTheory

{
    public class ProcessMethods
    {
        public static string LookUpString = "";
        public List<LookupTable> lookupTable = new List<LookupTable>();
        public static string HuffmanCodeText;
        public static List<EncodeHuffman> nodeList;

        public List<EncodeHuffman> GetListFromFile()
        {
            List<EncodeHuffman> nodeList = new List<EncodeHuffman>();

            try
            {
                FileStream stream = new FileStream(Filtering.FilteredTextPath, FileMode.Open, FileAccess.Read);

                for (int i = 0; i < stream.Length; i++)
                {
                    string read = Convert.ToChar(stream.ReadByte()).ToString();
                    if (nodeList.Exists(x => x.symbol == read))
                    {
                        // Increase the frequency if character exists.
                        nodeList[nodeList.FindIndex(y => y.symbol == read)].FrequencyIncrease(); 
                    }
                    else
                    {
                        // Add the character if not exist in the Nodelist
                        nodeList.Add(new EncodeHuffman(read));   
                    }
                }
                // Sort nodes by frequency value.
                nodeList.Sort();
                stream.Close();
                return nodeList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //  Creates a Tree according to Nodes(frequency, symbol)
        public void GetTreeFromList(List<EncodeHuffman> nodeList)
        {
            while (nodeList.Count > 1)                          
            {
                EncodeHuffman node1 = nodeList[0];                
                nodeList.RemoveAt(0);                           
                EncodeHuffman node2 = nodeList[0];                
                nodeList.RemoveAt(0);
                // Sending the constructor to make a new Node from this nodes.
                nodeList.Add(new EncodeHuffman(node1, node2));
                // Sort it again according to frequency.
                nodeList.Sort();                                
            }
        }


        // Setting the codes of the nodes of tree. Recursive method.
        public void SetCodeToTheTree(string code, EncodeHuffman Nodes)
        {
            if (Nodes == null)
                return;
            if (Nodes.leftTree == null && Nodes.rightTree == null)
            {
                Nodes.code = code;
                return;
            }
            SetCodeToTheTree(code + "0", Nodes.leftTree);
            SetCodeToTheTree(code + "1", Nodes.rightTree);
        }

        //Printing the Look-Up Table
        public void PrintLookUpTable(EncodeHuffman nodeList)
        {
            if (nodeList == null) { return; }
            if (nodeList.leftTree == null && nodeList.rightTree == null)
            {
                LookUpString += "Symbol:"+nodeList.symbol.PadRight(10)+ "Code:"+nodeList.code+"\n";
                lookupTable.Add(new LookupTable { Symbol = nodeList.symbol, Code = nodeList.code });
                return;
            }
            PrintLookUpTable(nodeList.leftTree);
            PrintLookUpTable(nodeList.rightTree);
        }

        public string EncodeText()
        {
            string str = "";            
            List<EncodeHuffman> nodeList = new List<EncodeHuffman>();
            try
            {
                FileStream stream = new FileStream(Filtering.FilteredTextPath, FileMode.Open, FileAccess.Read);
                for (int i = 0; i < stream.Length; i++)
                {            
                    string read = Convert.ToChar(stream.ReadByte()).ToString();
                    int indx = lookupTable.FindIndex(x => x.Symbol.CompareTo(read) == 0);
                    if (indx != -1 ) 
                    {
                        str = String.Concat(str, lookupTable[indx].Code);
                    } 
                    else
                    {
                        break;
                    }

                }

                HuffmanCodeText = str;
                List<string> asciiby8 = new List<string>();

                // Add zero to the bits to create ascii bytes. Leftmost bit is about data concurrency.
                for (int i = 0; i < str.Length-7; i+=7)
                {
                    asciiby8.Add(str.Substring(i, 7).PadLeft(8,'0'));
                }

                string encodedText = "";
                foreach (var item in asciiby8)
                {
                    encodedText += item;
                }


                stream.Close();
                return encodedText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex);
                return null;
            }

        }

        public string DecodeText()
        {
            string str = "";
            string outputString = "";
            List<EncodeHuffman> nodeList = new List<EncodeHuffman>();

            try
            {
                FileStream stream = new FileStream(Filtering.CompressedPath, FileMode.Open, FileAccess.Read);
                for (int i = 0; i < stream.Length; i++)
                {
                    string read = Convert.ToChar(stream.ReadByte()).ToString();

                     if(i % 8 != 0) {
                        str = String.Concat(str, read);
                     }
                    int indx = lookupTable.FindIndex(x => x.Code.CompareTo(str) == 0);

                    if (indx != -1)
                    {
                        outputString = String.Concat(outputString, lookupTable[indx].Symbol);
                        str = "";
                    }
                }
                stream.Close();
                return outputString;
            }
            catch (Exception)
            {
                return null;
            }

        }


        public string DecodeTextFromHuffman()
        {
            string str = "";
            int interval = 1;
            for (int i = 0; i + interval < HuffmanCodeText.Length; i = i+ interval)
            {
                interval = 1;
                int nodeIndex = nodeList.FindIndex(y => y.code == HuffmanCodeText.Substring(i, interval));

                while (!FindSymbolFromCode(HuffmanCodeText.Substring(i, interval), nodeList[0]))
                {
                    interval++;
                    if (FindSymbolFromCode(HuffmanCodeText.Substring(i, interval), nodeList[0])) { break; } 
                }
            }
            str = HuffmanConvertedString;
            return str;
        }

        public static string HuffmanConvertedString ="";
        public bool FindSymbolFromCode(string code, EncodeHuffman Nodes)
        {
            bool symbolFound = false;

            if (Nodes == null) { return false; }
            if ((Nodes.leftTree == null && Nodes.rightTree == null) && Nodes.code == code )
            {
                HuffmanConvertedString += Nodes.symbol;
                return true;
            }
            FindSymbolFromCode(code, Nodes.leftTree);
            FindSymbolFromCode(code, Nodes.rightTree);
            
            return symbolFound;
        }

        public void ApplyHuffman()
        {
            nodeList = GetListFromFile();
            GetTreeFromList(nodeList);
            SetCodeToTheTree("", nodeList[0]);
            PrintLookUpTable(nodeList[0]);
        }

    }



    public class LookupTable
    {
        public string Symbol { get; set; }
        public string Code { get; set; }
    }
}
