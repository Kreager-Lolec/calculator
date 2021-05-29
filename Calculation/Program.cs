using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Calculator
{
    class Program
    {
        class InvalidInputException : Exception
        {
            public InvalidInputException()
            {

            }

            public InvalidInputException(string name)
                : base(String.Format("Invalid input: {0}", name))
            {

            }

        }
        static void Main(string[] args)
        {
            //(1+2*(4+5*(2+3)*2)*5)+9==550
            try
            {
                Console.WriteLine("You must put two numbers, and operation,which you want to do with them \n" +
                   "You have six operations: " +
                   "Example of correct input: 1+1; 1 + 1; sqrt(5); 5^1/2; 1,2 + 1,3 \n" +
                   "Enter: ");
                string equation = Console.ReadLine();
                string dispEq = equation;
                bool containsPoint = false;
                if (equation.Contains('.'))
                {
                    containsPoint = true;
                    equation = equation.Replace('.', ',');
                    dispEq = dispEq.Replace(',', '.');
                }
                int leftBracket = 0;
                int rightBracket = 0;
                double result = 0;
                string tsq = "";
                List<string> subList;
                string tempSqrt = equation;
                while (tempSqrt.Contains("sqrt"))
                {
                    string sign = "";
                    if (tempSqrt[0] == '-' && !isnumber(equation[0]) && equation[0] != 's')
                    {
                        sign = tempSqrt[0].ToString();
                        tempSqrt = tempSqrt.Remove(0, 1);
                    }
                    double rootSqrt;
                    string extractedSqrt = ExtractSqrt(ref tempSqrt);
                    tempSqrt = tempSqrt.Replace("sqrt(" + extractedSqrt + ")", "");
                    validateequation(ref extractedSqrt, out subList);
                    Console.WriteLine("Values of extracted Sqrt: ");
                    foreach (var item in subList)
                    {
                        Console.WriteLine(item);
                    }
                    if (extractedSqrt[0] == '-')
                    {
                        throw new InvalidInputException(equation);
                    }
                    else if (subList.Count == 1)
                    {
                        rootSqrt = Math.Sqrt(double.Parse(extractedSqrt));
                    }
                    else
                    {
                        rootSqrt = Math.Sqrt(Prioritiescalculation(ref extractedSqrt, ref subList));
                    }
                    if (sign != "")
                    {
                        Console.WriteLine("Result of: " + sign + "sqrt(" + extractedSqrt + ")" + " is: " + sign + rootSqrt);
                    }
                    else
                    {
                        Console.WriteLine("Result of: " + "sqrt(" + extractedSqrt + ")" + " is: " + rootSqrt);
                    }
                    equation = equation.Replace("sqrt(" + extractedSqrt + ")", rootSqrt.ToString());
                }
                while (equation.Contains('(') && equation.Contains(')'))
                {
                    bracketindex(equation, ref leftBracket, ref rightBracket);
                    string subEquation = Substring(equation, leftBracket, rightBracket);
                    tsq = subEquation;
                    validateequation(ref tsq, out subList);
                    result = Prioritiescalculation(ref subEquation, ref subList);
                    equation = equation.Replace("(" + tsq + ")", result.ToString());
                    Console.WriteLine(tsq + " = " + result + "\nNow equation is " + equation + "\n----------------------------------");
                }
                while (equation.Contains("*") || equation.Contains("/") || equation.Contains("+") || equation.Contains("-") || equation.Contains("^"))
                {
                    validateequation(ref equation, out subList);
                    if (subList.Count >= 3)
                    {
                        tsq = equation;
                        result = Prioritiescalculation(ref equation, ref subList);
                        equation = equation.Replace(tsq, result.ToString());
                        Console.WriteLine(tsq + " = " + result + "\nNow equation is " + equation + "\n----------------------------------");
                    }
                    if (equation.Contains("-") && equation.Split().Length < 3) break;
                }
                if (containsPoint) equation = equation.Replace(',', '.');
                Console.WriteLine("Resulting value of " + dispEq + " is: " + equation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(new StackTrace().GetFrame(0).GetMethod().Name + ": " + ex.Message);
            }

        }

        static string ExtractSqrt(ref string equation)
        {
            int startIndex = -1;
            int endIndex = -1;
            string subEq = "";
            for (int i = 0; i < equation.Length; i++)
            {
                if (equation[i] == 's' && equation[i + 1] == 'q' && equation[i + 2] == 'r' && equation[i + 3] == 't' && equation[i + 4] == '(')
                {
                    startIndex = i + 5;
                    for (int j = startIndex; j < equation.Length; j++)
                    {
                        if (equation[j] == ')')
                        {
                            endIndex = j;
                            subEq = equation.Substring(startIndex, endIndex - startIndex);
                            Console.WriteLine("Sqrt subequation is such: " + equation.Substring(startIndex, endIndex - startIndex));
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return subEq;
        }

        //static double rootsqrt(string extractedSqrt)
        //{
        //    List<string> subList;
        //    validateequation(ref extractedSqrt, out subList);
        //    return Math.Sqrt(Prioritiescalculation(ref extractedSqrt, ref subList));
        //}
        /// <summary>
        /// this method check if element of string is bumber of dot('.')
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        static bool isnumber(char symbol)
        {
            if (char.IsDigit(symbol) || symbol == ',')
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// return index of border brackers
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="leftBracket"></param>
        /// <param name="rightBracket"></param>
        /// <returns></returns>
        static string Substring(string equation, int leftBracket, int rightBracket)
        {
            return equation.Substring(leftBracket, rightBracket - leftBracket).Trim('(', ')');
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requlteq"></param>
        /// <returns></returns>
        static List<string> ConvertToList(string[] requlteq)
        {
            List<string> resultarr = new List<string> { };
            foreach (var item in requlteq)
            {
                resultarr.Add(item);
            }
            return resultarr;
        }
        /// <summary>
        /// this method add spaces and trim current equation
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="resultarr"></param>
        static void validateequation(ref string equation, out List<string> resultarr)
        {
            equation = equation.Trim(' ');
            equation = equation.Replace(" ", "");
            string resultEq = equation[0].ToString();
            for (int i = 1; i < equation.Length; ++i)
            {
                if (isnumber(equation[i]) && isnumber(equation[i - 1]))// 
                {
                    resultEq += equation[i];
                }
                else if (isnumber(equation[i]) && !isnumber(equation[i - 1]))
                {
                    resultEq += " " + equation[i];
                }
                else if (!isnumber(equation[i]))
                {
                    resultEq += " " + equation[i] + " ";
                }
            }
            resultEq = resultEq.Replace("  ", " ");
            equation = equation.Trim(' ');
            resultarr = ConvertToList(resultEq.Split());
            for (int i = 0; i < resultarr.Count; i++)
            {
                if (resultarr[i] == "^" && resultarr[i + 1] == "-")
                {
                    resultarr[i + 2] = "-" + resultarr[i + 2];
                    resultarr.RemoveAt(i + 1);
                    foreach (var item in resultarr)
                    {
                        Console.WriteLine(item);
                    }
                }
                else if (resultarr[i] == "-")
                {
                    resultarr[i] = "+";
                    resultarr[i + 1] = "-" + resultarr[i + 1];
                }
            }
            Console.WriteLine("Local arythmetic will be such: " + resultEq);
        }
        /// <summary>
        /// this method do operations with two numbers
        /// </summary>
        /// <param name="leftNumber"></param>
        /// <param name="rightNumber"></param>
        /// <param name="operation"></param>
        /// <param name="result"></param>
        static void calculationtwo(string leftNumber, string rightNumber, string operation, out double result)
        {
            char sign = operation[0];
            Console.WriteLine("leftNumber: " + leftNumber);
            Console.WriteLine("rightNumber: " + rightNumber);
            double lNum = double.Parse(leftNumber);
            double rNum = double.Parse(rightNumber);
            result = 0;
            switch (sign)
            {
                case '+':
                    result = lNum + rNum;
                    break;
                case '-':
                    result = lNum - rNum;
                    break;
                case '*':
                    result = lNum * rNum;
                    break;
                case '/':
                    result = lNum / rNum;
                    break;
                case '^':
                    result = Math.Pow(lNum, rNum);
                    break;
                default:
                    Console.WriteLine("Invalid operation. Sigh: " + sign);
                    break;
            }
        }
        /// <summary>
        /// this method check what operation we shoud do foremost using priority of mathematical operands
        /// </summary>
        /// <param name="mathStr"></param>
        /// <param name="mathParts"></param>
        /// <returns></returns>
        static double Prioritiescalculation(ref string mathStr, ref List<string> mathParts)
        {
            double res = 0;
            string subEq = "";
            validateequation(ref mathStr, out mathParts);
            if (mathStr.Contains("^") || mathStr.Contains("pow"))
            {
                for (int i = 1; i < mathParts.Count; i++)
                {
                    if (mathParts[i] == "^")//If element is multiplier or diviser, we get operand and neighboring elements to make calculations
                    {
                        calculationtwo(mathParts[i - 1], mathParts[i + 1], mathParts[i], out res);//Taking parts chosen to get the value
                        subEq += mathParts[i - 1];
                        subEq += " " + mathParts[i];
                        subEq += " " + mathParts[i + 1];//Building subEquation string to replace
                        i--;/* Because of searching the sign we are now located on the middle index, and for better understanding of replacement we shift back once*/
                        mathParts.RemoveAt(i);
                        mathParts.RemoveAt(i);
                        mathParts.RemoveAt(i);//Removing elements relating to subEquation 
                        mathParts.Insert(i, res.ToString());//Replacing removed subEquation with value
                        mathStr = mathStr.Replace(subEq, res.ToString());
                        Console.WriteLine("Current equation: " + mathStr);
                    }
                }
            }
            if (mathStr.Contains("*") || mathStr.Contains("/"))
            {
                for (int i = 1; i < mathParts.Count; i++)
                {
                    if (mathParts[i] == "*" || mathParts[i] == "/")//If element is multiplier or diviser, we get operand and neighboring elements to make calculations
                    {
                        calculationtwo(mathParts[i - 1], mathParts[i + 1], mathParts[i], out res);//Taking parts chosen to get the value
                        subEq += mathParts[i - 1];
                        subEq += " " + mathParts[i];
                        subEq += " " + mathParts[i + 1];//Building subEquation string to replace
                        i--;/* Because of searching the sign we are now located on the middle index, and for better understanding of replacement we shift back once*/
                        mathParts.RemoveAt(i);
                        mathParts.RemoveAt(i);
                        mathParts.RemoveAt(i);//Removing elements relating to subEquation 
                        mathParts.Insert(i, res.ToString());//Replacing removed subEquation with value
                        mathStr = mathStr.Replace(subEq, res.ToString());
                        Console.WriteLine("Current equation: " + mathStr);
                    }
                }
            }
            if ((mathStr.Contains("+") || mathStr.Contains("-")))
            {
                for (int i = 1; i < mathParts.Count; i++)
                {
                    if (mathParts[i] == "+" || mathParts[i] == "-")
                    {
                        calculationtwo(mathParts[i - 1], mathParts[i + 1], mathParts[i], out res);//Taking parts chosen to get the value
                        subEq += mathParts[i - 1];
                        subEq += " " + mathParts[i];
                        subEq += " " + mathParts[i + 1];//Building subEquation string to replace
                        i--;
                        mathParts.RemoveAt(i);
                        mathParts.RemoveAt(i);
                        mathParts.RemoveAt(i);
                        mathParts.Insert(i, res.ToString());
                        Console.WriteLine("Current equation: " + mathStr);
                    }
                }
            }
            return res;
        }
        static string BuildmathString(List<string> mathParts)
        {
            string mathString = "";
            for (int i = 0; i < mathParts.Count; i++)
            {
                mathString += " " + mathParts[i];
            }
            return mathString.Trim(' ');
        }

        /// <summary>
        /// check indexes of brackets in current equation
        /// </summary>
        /// <param name="eq"></param>
        /// <param name="leftBracket"></param>
        /// <param name="rightBracket"></param>
        static void bracketindex(string eq, ref int leftBracket, ref int rightBracket)
        {
            for (int i = 0; i < eq.Length; i++)
            {
                if (eq[i] == '(')
                {
                    leftBracket = i;
                }
                if (eq[i] == ')')
                {
                    rightBracket = i;
                    break;
                }
            }
        }
    }
}
