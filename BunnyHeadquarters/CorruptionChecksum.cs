using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BunnyHeadquarters
{
    class CorruptionChecksum
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("C:\\Sandbox\\2016AoC\\BunnyHeadquarters\\BunnyHeadquarters\\corruptionchecksumpaxl.txt");
            List<string> spreadsheetrows = new List<string>();
            while (!sr.EndOfStream)
            {
                spreadsheetrows.Add(sr.ReadLine());
            }
            sr.Close();
            int checksumminmax = 0;
            // MinMaxDifference and Sum
            foreach (string row in spreadsheetrows)
            {
                int rowMin = Int32.MaxValue;
                int rowMax = Int32.MinValue;
                string[] rowcols = row.Split(new char[] { '\t' });
                foreach (string col in rowcols)
                {
                    int testval = Int32.Parse(col);
                    rowMin = Math.Min(rowMin, testval);
                    rowMax = Math.Max(rowMax, testval);
                }
                checksumminmax += (rowMax - rowMin);
            }
            int checksumevendivide = 0;
            // Evenly divisible values and Sum
            foreach (string row in spreadsheetrows)
            {
                decimal thisVal = 0,nextVal=0;
                int x=0,y=0,result=0;
                string[] rowcols = row.Split(new char[] { '\t' });
                for (x = 0, thisVal = decimal.Parse(rowcols[x]); x < rowcols.Length && result == 0; x++,thisVal=decimal.Parse(rowcols[x]))
                {
                    for (y = 0, nextVal = decimal.Parse(rowcols[y]); y < rowcols.Length; y++,nextVal=(y<rowcols.Length)?-1:decimal.Parse(rowcols[y]))
                    {
                        if ((y != x) && nextVal!=0 && ((thisVal / nextVal) == Math.Floor(thisVal / nextVal)))
                        {
                                result = Int32.Parse(Math.Floor(thisVal / nextVal).ToString());
                                break;
                        }
                    }
                }

                checksumevendivide += result;
            }
            Console.WriteLine("result:" + checksumminmax.ToString() + ", " + checksumevendivide.ToString());
        }
    }
}
