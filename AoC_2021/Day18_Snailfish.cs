using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day18_Snailfish : AoCodeModule
   {
      public Day18_Snailfish()
      {

         inputFileName = @"InputFiles\" + this.GetType().Name + "SampleExplode.txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {


         ResetProcessTimer(true);
         List<SnailfishNumber> fishNumbers = new List<SnailfishNumber>();
         foreach (string processingLine in inputFile)
         {
            fishNumbers.Add(new SnailfishNumber(processingLine));
         }
         AddResult("nope. Work is sucking today. gotta come back for this. ");

      }
   }
   public class SnailfishNumber
   {
      string originalNumber = "";
      string reducedNumber = "";
      string workingNumber = "";
      public string Answer { get { return reducedNumber; } }
      public SnailfishNumber(string number)
      {
         originalNumber = number; // this is Json?
         //Json
         Reduce();
         Console.WriteLine(Answer);
      }
      public void Reduce()
      {
         workingNumber = originalNumber;
         bool finished = false;
         while (!finished)
            if (!Explode())
               if (!Split())
               {
                  finished = true;
                  reducedNumber = workingNumber;
               }

      }
      public bool Explode()
      {
         bool exploded = false;
         int nestingLevel = 0, currentNestStartIndex, previousNestStartIndex;
         for (int x = 0; x < workingNumber.Length; x++)
         {
            if (workingNumber[x] == '[')
            {
               currentNestStartIndex = x;
               if (nestingLevel == 4)// already 4 deep (this is the 5th)
               {
                  // time to go boom; We can safely modify the string we're iterating without fear since we are busting out of the iteration after manipulation.
                  int currentNestEndIndex = workingNumber.IndexOf(']', currentNestStartIndex);
                  int[] workingNumbers = workingNumber.Substring(currentNestStartIndex + 1, currentNestEndIndex - currentNestStartIndex - 1).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => Int32.Parse(n)).ToArray();

                  workingNumber = workingNumber.Remove(currentNestStartIndex, currentNestEndIndex - currentNestStartIndex + 1); // get rid of the pair we're exploding
                  int reverse = currentNestStartIndex, firstNumEndIndex=-1;
                  string newFirstNum = "";
                  while (reverse>=0)
                  {
                     if (int.TryParse(workingNumber[reverse].ToString(), out int parsedNum))
                     {
                        firstNumEndIndex = firstNumEndIndex > 0 ? firstNumEndIndex : reverse; // if this is the first parseable char we've hit in reverse, this is endpoint for replacement.
                        newFirstNum = parsedNum.ToString() + newFirstNum; // handles two digit numbers until split happens.
                     }
                     else
                     {
                        if (newFirstNum != "") // we found a parseable number BEFORE and now have gone past it.
                        {
                           reverse++; // reverse now points to the starting index where we will replace the number
                           break; // stop hunting. 
                        }
                     }
                     reverse--;
                  }

                  newFirstNum = (int.TryParse(newFirstNum, out int parsedNumber) ? workingNumbers[0] + parsedNumber : 0).ToString();

                  int forward = currentNestStartIndex, secondNumStartIndex=-1;
                  string newSecondNum = "";
                  while (forward < workingNumber.Length)
                  {
                     if (int.TryParse(workingNumber[forward].ToString(), out int parsedNum))
                     {
                        secondNumStartIndex = secondNumStartIndex > 0 ? secondNumStartIndex : forward;
                        newSecondNum = newSecondNum + parsedNum.ToString(); // handles two digit numbers until split happens.
                     }
                     else
                     {
                        if (newSecondNum != "") // we found a parseable number BEFORE and now have gone past it.
                        {
                           forward--;
                           break; // stop hunting. 
                        }
                     }
                     forward++;
                  }

                  newSecondNum = (int.TryParse(newSecondNum, out int parsedNumber2) ? workingNumbers[1] + parsedNumber2 : 0).ToString();
                  string result = workingNumber.Substring(0, reverse > 0 ? reverse : currentNestStartIndex)
                     + newFirstNum + ",";

                  workingNumber = workingNumber.Substring(0, reverse >0 ? reverse : currentNestStartIndex)
                     + newFirstNum + ","
                     + (forward == workingNumber.Length ? "0" + workingNumber.Substring(currentNestEndIndex) :
                     "0" + workingNumber.Substring(currentNestEndIndex - currentNestStartIndex + 1, secondNumStartIndex-currentNestEndIndex) + newSecondNum + workingNumber.Substring(forward));
                     

                  nestingLevel--;
                  exploded = true;
                  break;
               }
               else
               {
                  previousNestStartIndex = x;
                  nestingLevel++;
               }
            }
            else if (workingNumber[x] == ']')
            {
               nestingLevel--;
            }
         }
         return exploded;
      }
      public bool ExplodeFirstGo()
      {
         bool exploded = false;
         int nestingLevel = 0, currentNestStartIndex, previousNestStartIndex;
         for (int x=0; x<workingNumber.Length; x++)
         {
            if (workingNumber[x]=='[')
            {
               currentNestStartIndex = x;
               if (nestingLevel==4)// already 4 deep (this is the 5th)
               {
                  // time to go boom; We can safely modify the string we're iterating without fear since we are busting out of the iteration after manipulation.
                  int currentNestEndIndex = workingNumber.IndexOf(']',currentNestStartIndex);
                  int[] workingNumbers = workingNumber.Substring(currentNestStartIndex+1,currentNestEndIndex-currentNestStartIndex-1).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => Int32.Parse(n)).ToArray();
                  string firstNewNumber = "0", secondNewNumber = "0";
                  workingNumber = workingNumber.Remove(currentNestStartIndex, currentNestEndIndex - currentNestStartIndex + 1);
                  int potentialFirstLeftRegularNumberEndBoundary = workingNumber.Substring(0, currentNestStartIndex).LastIndexOf(',');
                  if (potentialFirstLeftRegularNumberEndBoundary != -1)
                  {
                     int potentialFirstLeftRegularNumberStartBoundary = workingNumber.Substring(0, currentNestStartIndex).LastIndexOf('[');
                     if (potentialFirstLeftRegularNumberStartBoundary != -1) // don't think this can happen....
                     {
                        string potentialFirstLeftRegularNumber = workingNumber.Substring(potentialFirstLeftRegularNumberStartBoundary + 1, potentialFirstLeftRegularNumberEndBoundary - potentialFirstLeftRegularNumberStartBoundary - 1);
                        if (int.TryParse(potentialFirstLeftRegularNumber, out int firstLeftRegularNumber))
                        {
                           firstLeftRegularNumber += workingNumbers[0];
                           firstNewNumber = firstLeftRegularNumber.ToString();
                           workingNumber = workingNumber.Substring(0, potentialFirstLeftRegularNumberStartBoundary+1) + firstNewNumber + workingNumber.Substring(potentialFirstLeftRegularNumberEndBoundary);
                        }
                     }
                  }
                  else
                  {
                     workingNumber = workingNumber.Substring(0, currentNestStartIndex) + firstNewNumber + workingNumber.Substring(currentNestStartIndex);
                  }
                  int potentialFirstRightRegularNumberStartBoundary = workingNumber.Substring(currentNestStartIndex).IndexOf(',');
                  if (potentialFirstRightRegularNumberStartBoundary != -1)
                  {
                     int potentialFirstRightRegularNumberEndBoundary = workingNumber.Substring(currentNestStartIndex).IndexOf(']',potentialFirstRightRegularNumberStartBoundary);
                     if (potentialFirstRightRegularNumberEndBoundary != -1) // don't think this can happen, either....
                     {
                        string potentialFirstRightRegularNumber = workingNumber.Substring(currentNestStartIndex).Substring(potentialFirstRightRegularNumberStartBoundary+1, potentialFirstRightRegularNumberEndBoundary - potentialFirstRightRegularNumberStartBoundary-1);
                        if (int.TryParse(potentialFirstRightRegularNumber, out int firstRightRegularNumber))
                        {
                           firstRightRegularNumber += workingNumbers[1];
                           secondNewNumber = firstRightRegularNumber.ToString();
                           workingNumber = workingNumber.Substring(0, potentialFirstRightRegularNumberStartBoundary+ currentNestStartIndex + 1) + secondNewNumber + workingNumber.Substring(potentialFirstRightRegularNumberEndBoundary + currentNestStartIndex);
                           workingNumber = workingNumber.Substring(0, currentNestStartIndex) + "0" + workingNumber.Substring(currentNestStartIndex);
                        }
                     }
                  }
                  else
                  {
                     workingNumber = workingNumber.Substring(0, currentNestStartIndex) + secondNewNumber + workingNumber.Substring(currentNestStartIndex);
                  }
                  nestingLevel--;
                  exploded = true;
                  break;
               }
               else
               {
                  previousNestStartIndex = x;
                  nestingLevel++;
               }
            }
            else if (workingNumber[x]==']')
            {
               nestingLevel--;
            }
         }
         return exploded;
      }
      public bool Split()
      {

         return false;
      }

   }
}
