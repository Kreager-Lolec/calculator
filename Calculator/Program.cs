using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            //(1+2*(4+5*(2+3)*2)*5)+9==550
            try
            {
                Console.WriteLine("You must put two numbers, and operation,which you want to do with them \n" +
                   "You have six operations: " +
                   "Example of correct input: 1+1; 1 + 1; sqrt(5); 5^1/2 \n" +
                   "Enter: ");
                string equation = Console.ReadLine();
                int leftbracket = 0;
                int rightbracket = 0;
                double result = 0;
                string tsq = "";
                List<string> sublist;
                while (equation.Contains("*") || equation.Contains("/") || equation.Contains("+") || equation.Contains("-"))
                {
                    while (equation.Contains('(') && equation.Contains(')'))
                    {
                        bracketindex(equation, ref leftbracket, ref rightbracket);
                        string subequation = Substring(equation, leftbracket, rightbracket);
                        tsq = subequation;
                        validateequation(ref tsq, out sublist);
                        result = Prioritiescalculation(ref subequation, ref sublist);
                        equation = equation.Replace("(" + tsq + ")", result.ToString());
                        Console.WriteLine(tsq + " = " + result + "\nNow equation is " + equation + "\n----------------------------------");
                    }
                    tsq = equation;
                    validateequation(ref equation, out sublist);
                    result = Prioritiescalculation(ref equation, ref sublist);
                    equation = equation.Replace(tsq, result.ToString());
                    Console.WriteLine(tsq + " = " + result + "\nNow equation is " + equation + "\n----------------------------------");
                }
                Console.WriteLine("Now equation is: " + equation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(new StackTrace().GetFrame(0).GetMethod().Name + ": " + ex.Message);
            }

        }
        /// <summary>
        /// this method check if element of string is bumber of dot('.')
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        static bool isnumber(char symbol)
        {
            if (char.IsDigit(symbol) || symbol == '.')
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// return index of border brackers
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="leftbracket"></param>
        /// <param name="rightbracket"></param>
        /// <returns></returns>
        static string Substring(string equation, int leftbracket, int rightbracket)
        {
            return equation.Substring(leftbracket, rightbracket - leftbracket).Trim('(', ')');
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
            string resulteq = equation[0].ToString();
            for (int i = 1; i < equation.Length; ++i)
            {
                if (isnumber(equation[i]) && isnumber(equation[i - 1]))// 
                {
                    resulteq += equation[i];
                }
                else if (isnumber(equation[i]) && !isnumber(equation[i - 1]))
                {
                    resulteq += " " + equation[i];
                }
                else if (!isnumber(equation[i]))
                {
                    resulteq += " " + equation[i] + " ";
                }
            }
            resulteq = resulteq.Replace("  ", " ");
            equation = equation.Trim(' ');
            resultarr = ConvertToList(resulteq.Split());
            Console.WriteLine("Local arythmetic will be such: " + resulteq);
        }
        /// <summary>
        /// this method do operations with two numbers
        /// </summary>
        /// <param name="leftnumber"></param>
        /// <param name="rightnumber"></param>
        /// <param name="operation"></param>
        /// <param name="result"></param>
        static void calculationtwo(string leftnumber, string rightnumber, string operation, out double result)
        {
            char sign = operation[0];
            Console.WriteLine("Leftnumber: " + leftnumber);
            Console.WriteLine("Rightnumber: " + rightnumber);
            double lnum = double.Parse(leftnumber);
            double rnum = double.Parse(rightnumber);
            result = 0;
            switch (sign)
            {
                case '+':
                    result = lnum + rnum;
                    break;
                case '-':
                    result = lnum - rnum;
                    break;
                case '*':
                    result = lnum * rnum;
                    break;
                case '/':
                    result = lnum / rnum;
                    break;
                default:
                    Console.WriteLine("Invalid operation. Sigh: " + sign);
                    break;
            }
        }
        /// <summary>
        /// this method check what operation we shoud do foremost using priority of mathematical operands
        /// </summary>
        /// <param name="mathstr"></param>
        /// <param name="mathparts"></param>
        /// <returns></returns>
        static double Prioritiescalculation(ref string mathstr, ref List<string> mathparts)
        {
            double res = 0;
            string subeq = "";
            validateequation(ref mathstr,out mathparts);
            if (mathstr.Contains("*") || mathstr.Contains("/"))
            {
                for (int i = 0; i < mathparts.Count; i++)
                {
                    if (mathparts[i] == "*" || mathparts[i] == "/")//If element is multiplier or diviser, we get operand and neighboring elements to make calculations
                    {
                        calculationtwo(mathparts[i - 1], mathparts[i + 1], mathparts[i], out res);//Taking parts chosen to get the value
                        subeq += mathparts[i - 1];
                        subeq += " " + mathparts[i];
                        subeq += " " + mathparts[i + 1];//Building subequation string to replace
                        mathparts.RemoveAt(i - 1);
                        //Console.WriteLine("Length of list: " + mathparts.Count);
                        mathparts.RemoveAt(i - 1);
                        //Console.WriteLine("Length of list: " + mathparts.Count);
                        mathparts.RemoveAt(i - 1);
                        //Console.WriteLine("Length of list: " + mathparts.Count);
                        mathparts.Insert(i - 1, res.ToString());
                        i--;
                        //Console.WriteLine("List of math parts : ");
                        //foreach (var item in mathparts)
                        //{
                        //    Console.WriteLine(item);
                        //}
                        mathstr = mathstr.Replace(subeq, res.ToString());
                        Console.WriteLine("Current equation: " + mathstr);
                    }
                }
            }
            if ((mathstr.Contains("+") || mathstr.Contains("-")))
            {
                for (int i = 0; i < mathparts.Count; i++)
                {
                    if (mathparts[i] == "+" || mathparts[i] == "-")
                    {
                        calculationtwo(mathparts[i - 1], mathparts[i + 1], mathparts[i], out res);//Taking parts chosen to get the value
                        subeq += mathparts[i - 1];
                        subeq += " " + mathparts[i];
                        subeq += " " + mathparts[i + 1];//Building subequation string to replace
                        mathparts.RemoveAt(i - 1);
                        //Console.WriteLine("Length of list: " + mathparts.Count);
                        mathparts.RemoveAt(i - 1);
                        //Console.WriteLine("Length of list: " + mathparts.Count);
                        mathparts.RemoveAt(i - 1);
                        //Console.WriteLine("Length of list: " + mathparts.Count);
                        mathparts.Insert(i - 1, res.ToString());
                        //Console.WriteLine("List of math parts : ");
                        i--;
                        //foreach (var item in mathparts)
                        //{
                        //    Console.WriteLine(item);
                        //}
                        //mathstr = mathstr.Replace(subeq, res.ToString());
                        Console.WriteLine("Current equation: " + mathstr);
                    }
                }
            }
            return res;
        }
        static string Buildmathstring(List<string> mathparts)
        {
            string mathstring = "";
            for (int i = 0; i < mathparts.Count; i++)
            {
                mathstring += " " + mathparts[i];
            }
            return mathstring.Trim(' ');
        }
       
        /// <summary>
        /// check indexes of brackets in current equation
        /// </summary>
        /// <param name="eq"></param>
        /// <param name="leftbracket"></param>
        /// <param name="rightbracket"></param>
        static void bracketindex(string eq, ref int leftbracket, ref int rightbracket)
        {
            for (int i = 0; i < eq.Length; i++)
            {
                if (eq[i] == '(')
                {
                    leftbracket = i;
                }
                if (eq[i] == ')')
                {
                    rightbracket = i;
                    break;
                }
            }
        }
    }
}