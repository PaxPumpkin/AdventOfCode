using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BunnyHeadquarters
{
    class BadTriangles
    {



        static string data = "";

        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("C:\\Sandbox\\2016AoC\\BunnyHeadquarters\\BunnyHeadquarters\\triangles.txt");
            int finalOutput = 0;
            int totalCount = 0;
            while (!sr.EndOfStream)
            {
                totalCount++;
                string row1 = sr.ReadLine().Trim();
                string row2 = sr.ReadLine().Trim();
                string row3 = sr.ReadLine().Trim();
                string[][] triangles= new string[3][];
                triangles[0] = row1.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                triangles[1] = row2.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                triangles[2] = row3.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                //{ row1.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries), row2.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries), row3.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries) };
                string[] sides = row1.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("Triangle: " + sides[0] + " - " + sides[1] + " - " + sides[2]);
                finalOutput += (new Triangle(triangles[0][0], triangles[1][0], triangles[2][0], totalCount)).isGoodTriangle ? 1 : 0;
                finalOutput += (new Triangle(triangles[0][1], triangles[1][1], triangles[2][1], totalCount)).isGoodTriangle ? 1 : 0;
                finalOutput += (new Triangle(triangles[0][2], triangles[1][2], triangles[2][2], totalCount)).isGoodTriangle ? 1 : 0;
                //finalOutput += (new Triangle(sides[0],sides[1],sides[2],totalCount)).isGoodTriangle?1:0;
            }
            Console.WriteLine("final combo: " + finalOutput.ToString());
        }
    }
    class Triangle
    {
        int x = 0;
        int y = 0;
        int z = 0;
        public bool isGoodTriangle = false;
        public Triangle(string a, string b, string c, int lineNumber)
        {
            x = Int32.Parse(a);
            y = Int32.Parse(b);
            z = Int32.Parse(c);
            int max = Math.Max(Math.Max(x, y),z);
            int testsum =(x==max?y+z:(y==max?x+z:x+y));
            if (y == z) { 
                Console.WriteLine("There is some equality here"); 
            }
            isGoodTriangle = testsum > max;
            //StreamWriter sw = new StreamWriter("C:\\Sandbox\\2016AoC\\BunnyHeadquarters\\BunnyHeadquarters\\triangles-OP.txt", true);
            //sw.WriteLine(lineNumber.ToString() + "-" + a + " " + b + " " + c + " - max: " + max.ToString() + " sum: " + testsum.ToString() + " goodTriangle: " + isGoodTriangle.ToString());
            //sw.Flush();
            //sw.Close();
        }
    }
}
