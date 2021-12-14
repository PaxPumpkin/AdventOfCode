using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day18_OperationOrder : AoCodeModule
    {
        public Day18_OperationOrder()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            //inputFileName = @"InputFiles\" + this.GetType().Name + "Sample.txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            //** > Result for Day18_OperationOrder part 1:Sum of all evaluated expressions: 45283905029161(Process: 3.6891 ms)
            //** > Result for Day18_OperationOrder part 2:Sum of all evaluated expressions: 216975281211165(Process: 4.6516 ms)
            ResetProcessTimer(true);
            long result = 0;
            long individual = 0;
            foreach (string processingLine in inputFile)
            {
                individual =  Evaluate(processingLine);
                //Print(processingLine + " = " + individual.ToString());
                result += individual;
            }
            AddResult("Sum of all evaluated expressions: " + result.ToString()); // includes elapsed time from last ResetProcessTimer
            //Print("----------------------------------------------------------------------------");
            ResetProcessTimer(true);
            result = 0;
            foreach (string processingLine in inputFile)
            {
                individual = EvaluateShittyPrecedence(processingLine);
                //Print(processingLine + " = " + individual.ToString());
                result += individual;
            }
            AddResult("Sum of all evaluated expressions: " + result.ToString()); // includes elapsed time from last ResetProcessTimer

        }
        public long Evaluate(string funkyMath)
        {
            string originalInput = funkyMath;
            int closeParenthesis = 0;
            long result=0;
            bool startedwithOpenParen = funkyMath.StartsWith("(");
            List<string> subStatements = funkyMath.Split(new char[] { '(' }).ToList();
            if (subStatements.Count > 1)
            {
                List<string> cleaned = new List<string>();
                foreach (string piece in subStatements)
                {
                    cleaned.Add(piece.Trim());
                }
                subStatements = cleaned.ToList();
                cleaned.Clear();
                int index = 0;
                foreach (string statement in subStatements)
                {
                    if (statement != "")
                    {
                        string fuctionalStatement = statement;
                        if (index > 0 || startedwithOpenParen)
                        {
                            fuctionalStatement = "(" + fuctionalStatement;
                        }
                        closeParenthesis = fuctionalStatement.IndexOf(')');
                        if (closeParenthesis > -1)
                        {
                            string piece = fuctionalStatement.Substring((index > 0 || startedwithOpenParen) ? 1 : 0, closeParenthesis - ((index > 0 || startedwithOpenParen) ? 1 : 0));
                            long value = Evaluate(piece);
                            piece = value.ToString() + (closeParenthesis == fuctionalStatement.Length ? "" : fuctionalStatement.Substring(closeParenthesis + 1));
                            cleaned.Add(piece);
                        }
                        else
                        {
                            cleaned.Add(fuctionalStatement);
                        }
                    }
                    else
                    {
                        if (index>0)
                            cleaned.Add("(");
                    }
                    index++;
                }
                subStatements = cleaned.ToList();
                cleaned.Clear();
                funkyMath = "";
                foreach (string statement in subStatements)
                {
                    funkyMath += statement + ((statement=="(") ?"":" "); 
                }
                funkyMath = funkyMath.Substring(0, funkyMath.Length - 1).Trim();
                result = Evaluate(funkyMath);
            }
            else
            {
                // no close parens.
                if (funkyMath.IndexOf('*')<0 && funkyMath.IndexOf('+') < 0)
                {
                    result = long.Parse(funkyMath.Trim());
                }
                else
                {
                    int index = 0;
                    long currentValue = -1;
                    if (funkyMath.IndexOf('*') >= 0 || funkyMath.IndexOf('+') >= 0)
                    {
                        string segment = "";
                        char operation = ' ';
                        foreach (char marker in funkyMath)
                        {
                            if (marker == '*' || marker == '+')
                            {
                                if (currentValue < 0)
                                {
                                    currentValue = long.Parse(segment);
                                    segment = "";
                                    operation = marker;
                                }
                                else
                                {
                                    currentValue = (operation == '*') ? currentValue * long.Parse(segment) : currentValue + long.Parse(segment);
                                    segment = "";
                                    operation = marker;
                                }
                                //break;
                            }
                            else
                            {
                                segment += marker.ToString();
                            }
                        }
                        result = (operation == '*') ? currentValue * long.Parse(segment) : currentValue + long.Parse(segment);
                    }
                }
            }

            return result;
        }

        public long EvaluateShittyPrecedence(string funkyMath)
        {
            string originalInput = funkyMath;
            int closeParenthesis = 0;
            long result = 0;
            bool startedwithOpenParen = funkyMath.StartsWith("(");
            List<string> subStatements = funkyMath.Split(new char[] { '(' }).ToList();
            if (subStatements.Count > 1)
            {
                List<string> cleaned = new List<string>();
                foreach (string piece in subStatements)
                {
                    cleaned.Add(piece.Trim());
                }
                subStatements = cleaned.ToList();
                cleaned.Clear();
                int index = 0;
                foreach (string statement in subStatements)
                {
                    if (statement != "")
                    {
                        string fuctionalStatement = statement;
                        if (index > 0 || startedwithOpenParen)
                        {
                            fuctionalStatement = "(" + fuctionalStatement;
                        }
                        closeParenthesis = fuctionalStatement.IndexOf(')');
                        if (closeParenthesis > -1)
                        {
                            string piece = fuctionalStatement.Substring((index > 0 || startedwithOpenParen) ? 1 : 0, closeParenthesis - ((index > 0 || startedwithOpenParen) ? 1 : 0));
                            long value = EvaluateShittyPrecedence(piece);
                            piece = value.ToString() + (closeParenthesis == fuctionalStatement.Length ? "" : fuctionalStatement.Substring(closeParenthesis + 1));
                            cleaned.Add(piece);
                        }
                        else
                        {
                            cleaned.Add(fuctionalStatement);
                        }
                    }
                    else
                    {
                        if (index > 0)
                            cleaned.Add("(");
                    }
                    index++;
                }
                subStatements = cleaned.ToList();
                cleaned.Clear();
                funkyMath = "";
                foreach (string statement in subStatements)
                {
                    funkyMath += statement + ((statement == "(") ? "" : " ");
                }
                funkyMath = funkyMath.Substring(0, funkyMath.Length - 1).Trim();
                result = EvaluateShittyPrecedence(funkyMath);
            }
            else
            {
                // no close parens.
                if (funkyMath.IndexOf('*') < 0 && funkyMath.IndexOf('+') < 0)
                {
                    result = long.Parse(funkyMath.Trim());
                }
                else
                {
                    int index = 0;
                    long currentValue = 0;
                    List<string> badPrecedence = funkyMath.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    index = badPrecedence.IndexOf("+");
                    while (index > 0) // has to be >0 or this doesn't make sense....
                    {
                        int firstOperand = index - 1, secondOperand = index + 1;
                        currentValue = long.Parse(badPrecedence[firstOperand]) + long.Parse(badPrecedence[secondOperand]);
                        badPrecedence.RemoveRange(firstOperand, 3);
                        badPrecedence.Insert(firstOperand, currentValue.ToString());
                        index = badPrecedence.IndexOf("+");
                    }
                    index = badPrecedence.IndexOf("*");
                    while (index > 0) // has to be >0 or this doesn't make sense....
                    {
                        int firstOperand = index - 1, secondOperand = index + 1;
                        if (currentValue == 0) currentValue = 1;
                        currentValue = long.Parse(badPrecedence[firstOperand]) * long.Parse(badPrecedence[secondOperand]);
                        badPrecedence.RemoveRange(firstOperand, 3);
                        badPrecedence.Insert(firstOperand, currentValue.ToString());
                        index = badPrecedence.IndexOf("*");
                    }
                    result = long.Parse(badPrecedence[0]);
                }
            }
            return result;
        }
    }
}
