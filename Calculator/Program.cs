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
                while (equation.Contains('(') && equation.Contains(')'))
                {
                    loopindex(equation, ref leftbracket, ref rightbracket);
                    string subequation = equation.Substring(leftbracket, rightbracket - leftbracket).Trim('(', ')');
                    string tsq = subequation;
                    string[] subeqarr;
                    makeequation(ref tsq, out subeqarr);
                    double result;
                    calculationtwo(subeqarr[0],subeqarr[1],subeqarr[2], out result);
                    Console.WriteLine(tsq + " = " + result + "\nNow equation is " + equation + "\n----------------------------------");
                    equation = equation.Replace("(" + tsq + ")", result.ToString());
                }
                //while equation.contain(дужки)
                Console.WriteLine("Now equation is: " + equation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(new StackTrace().GetFrame(0).GetMethod().Name + ": " + ex.Message);
            }

        }

        static bool isnumber(char symbol)
        {
            if (char.IsDigit(symbol) || symbol == '.')
            {
                return true;
            }
            return false;
        }
        static void makeequation(ref string equation, out string[] resultarr)
        {
            equation = equation.Trim(' ');
            equation = equation.Replace(" ", "");
            string resulteq = equation[0].ToString();
            for (int i = 1; i < equation.Length; ++i)
            {
                if (resulteq.Split().Length == 4) break;
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
            resultarr = resulteq.Split(' ');
            Console.WriteLine("Local arythmetic will be such: " + resulteq);
        }
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
        //void calculate(string mathstr, out double res)
        //{
        //    res = 0;
        //    string[] mathparts = null;
        //    makeequation(ref mathstr, out mathparts);
        //    if (!mathstr.Contains("*") || !mathstr.Contains("|"))
        //        for (int i = 0; i < mathparts.Length; i++)
        //        {
        //            if (mathparts[i] == "+" || mathparts[i] == "-" || mathparts[i] == "*" || mathparts[i] == "/")
        //            {
        //                calculationtwo(mathparts[i - 1], mathparts[i + 1], mathparts[i], out res);
        //            }
        //        }
        //    Console.WriteLine("Result of the calculate is " + res);
        //}
        static void loopindex(string eq, ref int leftbracket, ref int rightbracket)
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
