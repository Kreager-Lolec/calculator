using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            //(1+2*(4+5*(2+3)*2)*5)+9
            try
            {
                Console.WriteLine("You must put two numbers, and operation,which you want to do with them \n" +
                   "You have six operations: " +
                   "1) Addition \n" +
                   "2) Subtraction \n" +
                   "3) Multiplication \n" +
                   "4) Division \n" +
                   "5) The raising of to \n" +
                   "6) root extraction \n" +
                   "Example of correct input: 1+1; 1 + 1; sqrt(5); 5^1/2 \n" +
                   "Enter: ");
                string equation = Console.ReadLine();
                int leftbracket = 0;
                int rightbracket = 0;
                loopindex(equation, ref leftbracket, ref rightbracket);
                string subequation = equation.Substring(leftbracket, rightbracket - leftbracket).Trim('(', ')');
                string tsq = subequation;
                string[] subeqarr;
                makeequation(ref tsq, out subeqarr);
                double result;
                calculationtwo(subeqarr[0], subeqarr[2], subeqarr[1], out result);
                Console.WriteLine(tsq + " = " + result);
                equation = equation.Replace("(" + tsq + ")", result.ToString());
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
            Console.WriteLine(resulteq);
            resultarr = resulteq.Split(' ');
            foreach (var item in resultarr)
            {
                Console.WriteLine(item);
            }
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
