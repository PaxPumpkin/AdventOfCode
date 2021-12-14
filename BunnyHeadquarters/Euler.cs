using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class Euler
    {
        static void Main(string[] args)
        {
            //problem 1
            //int summation = 0;
            //for (int x = 1; x < 1000; x++)
            //{
            //    if (x % 3 == 0 || x % 5 == 0) { 
            //        summation += x; 
            //    }
            //}


            //problem 2
            //Int64 summation = 0;
            //int number1 = 1;
            //int number2 = 2;
            //summation += 2; // start it off, 2 is the first one that should have been in here :)
            //while (number1 + number2 < 4000000)
            //{
            //    int fibonacci = number1 + number2;
            //    if (fibonacci % 2 == 0) { summation += fibonacci; } else { Console.WriteLine("not even: " + fibonacci.ToString()); }
            //    number1 = number2;
            //    number2 = fibonacci;
            //}

            //problem 3
            //long n = 600851475143;
            //List<long> factors = new List<long>();
            //while (n % 2 == 0)
            //{
            //    if (!factors.Contains(2)) factors.Add(2);
            //    n = n / 2;
            //}

            //// n must be odd at this point
            //for (int i = 3; i <= Math.Sqrt(n); i += 2)
            //{
            //    // While i divides n, print i and divide n
            //    while (n % i == 0)
            //    {
            //        if (!factors.Contains(i)) factors.Add(i);
            //        n = n / i;
            //    }
            //}

            //// n, after all that, is now itself a prime number.
            //if (n > 2)
            //    if (!factors.Contains(n)) factors.Add(n);

            //long summation = factors.OrderBy(number => number).Last();

            //problem 4
            // the largest palidrome number made from multiplying two three digit numbers
            // the largest number that can be made from multiplying two three digit numbers is 998001, so let's start there and go down.
            // the smallest three digit number is 100, so that's 10000.
            //int summation = 0; // this will hold our result.
            //for (int x = 998001; x > 10000; x--)
            //{
            //    // ok, is X a palindrome?
            //    // for ODD-digit counts, the center digit won't matter. For even, the last half, reversed, should equal the front half.
            //    string theNumber = x.ToString();
            //    string frontHalf = "";
            //    string backHalf = "";
            //    char[] comparitor;
            //    if (theNumber == "997799")
            //    {
            //        Console.WriteLine("got a test case");
            //    }
            //    if (theNumber.Length % 2 == 0)
            //    {//even
            //        frontHalf = theNumber.Substring(0,((int)(theNumber.Length / 2)));
            //        backHalf = theNumber.Substring(((int)(theNumber.Length/2))); //zero-index based.
            //    }
            //    else
            //    {//odd - ignore the center digit.
            //        frontHalf = theNumber.Substring(0, ((int)((theNumber.Length-1) / 2)));
            //        backHalf = theNumber.Substring(((int)((theNumber.Length-1) / 2))+1); 
            //    }
            //    comparitor=backHalf.ToArray();
            //    StringBuilder sb = new StringBuilder();
            //    for (int q=comparitor.GetUpperBound(0); q>=0; q--)
            //    {
            //        sb.Append(comparitor[q]);
            //    }
            //    backHalf = sb.ToString(); // now reversed.
            //    if (frontHalf.Equals(backHalf))
            //    {//PALINDROME!!!
            //        Console.WriteLine("PALINDROME: " + theNumber);
            //        // I could do some FACTORING here, but that's going to require me to pull out some old stuff that I barely remember how to do
            //        // SO.... instead, I'm going to brute-force it. I mean, I've got a bazillion cycles a second here!
            //        // The number has to be evenly divisible by a three digit number ( 100-999 )%==0 and the result ALSO has to be a three digit number. SO...
            //        bool gotit = false;
            //        for (int q=100; q<=999; q++){
            //            if (x%q==0 && (x/q).ToString().Length==3){
            //                Console.WriteLine(x.ToString() + " is evenly divisible by " + q.ToString() + " and results in " + (x / q).ToString());
            //                summation = x;
            //                gotit = true;
            //                break;
            //            }
            //        }
            //        if (gotit){break;}
            //    }
            //}
            
            
            
            //public 6 - sum square difference
            long summation = 0;

            //long sumofsquares = 0;
            //long squareofsum = 0;
            //for (int x = 1; x <= 100; x++)
            //{
            //    squareofsum += x;
            //    sumofsquares += Int64.Parse(Math.Pow(x, 2).ToString());
            //}
            //squareofsum = Int64.Parse(Math.Pow(squareofsum, 2).ToString());
            //summation = squareofsum - sumofsquares;


            //problem 7 - 10001st prime
            //int counter = 1;
            //long thePrime = 2;
            //long candidate = 2;
            //while (counter < 10001)
            //{
            //    candidate++;
            //    if (isPrime(candidate))
            //    {
            //        counter++;
            //        thePrime = candidate;
            //        Console.WriteLine("#: " + counter.ToString() + " : " + thePrime.ToString()  );
            //    }
            //}

            //problem 8 - string of 13 that makes the biggest product.
            //string inputNonsense = "7316717653133062491922511967442657474235534919493496983520312774506326239578318016984801869478851843858615607891129494954595017379583319528532088055111254069874715852386305071569329096329522744304355766896648950445244523161731856403098711121722383113622298934233803081353362766142828064444866452387493035890729629049156044077239071381051585930796086670172427121883998797908792274921901699720888093776657273330010533678812202354218097512545405947522435258490771167055601360483958644670632441572215539753697817977846174064955149290862569321978468622482839722413756570560574902614079729686524145351004748216637048440319989000889524345065854122758866688116427171479924442928230863465674813919123162824586178664583591245665294765456828489128831426076900422421902267105562632111110937054421750694165896040807198403850962455444362981230987879927244284909188845801561660979191338754992005240636899125607176060588611646710940507754100225698315520005593572972571636269561882670428252483600823257530420752963450";
            //int scanningLength = 13; // will be 13. 4 is proof of concept, at 5832 (9*9*8*9 in a row somewhere)
            //long maxProduct = 0;
            //for (int x = 0; x <= inputNonsense.Length - scanningLength; x++)
            //{
            //    long testProduct = 1; // safe to always start at 1
            //    for (int y = 0; y < scanningLength; y++)
            //    {
            //        testProduct *= Int64.Parse(inputNonsense[x + y].ToString());
            //    }
            //    if (testProduct > maxProduct) maxProduct = testProduct;
            //}
            //summation = maxProduct;

            //problem 9 -pythagorean triplets




            Console.WriteLine("result: " + summation.ToString());
            Console.ReadLine();
        }
        private static bool isPrime(long test)
        {
            if (test % 2 == 0) return false; // even number and we filtered out 2 with a short-circuit in the call to this function.
            for (long x = 3; x * x <= test; x += 2) // stole this from a website. Tests odd number(2 is the only even prime), and we only need to test up to the square root of the value to be checked. After that, we're kinda going through duplicate efforts
            {
                if (test % x == 0) return false; // it was divisible by something in our iteration.
            }
            //if we got here, nothing evenly divides into it. 
            return true;
        }
    }
}
