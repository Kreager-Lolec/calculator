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
                equation = equation.ToLower();
                string dispEq = equation;
                bool containsPoint = false;
                if (equation.Contains('.'))
                {
                    containsPoint = true;
                    equation = equation.Replace('.', ',');
                    dispEq = dispEq.Replace(',', '.');
                }
                int leftBracket = -1;
                int rightBracket = -1;
                double result = 0;
                string tsq = "";
                List<string> subList = new List<string>() { };
                while (equation.Contains("sqrt") || equation.Contains("pow") || equation.Contains("sin") || equation.Contains("cos")
                    || equation.Contains("tan") || equation.Contains("cot") || equation.Contains("log") || equation.Contains("ln"))
                {
                    string subEquation = ExtractSubEquation(ref equation, ref leftBracket, ref rightBracket, ref tsq, ref subList);
                    if (equation.Contains("sqrt"))
                    {
                        CalculateSqrt(ref equation, leftBracket, ref subEquation, ref result, ref subList, ref tsq);
                    }
                    else if (equation.Contains("pow"))
                    {
                        CalculatePow(ref equation, leftBracket, ref subEquation, ref result, ref subList, ref tsq);
                    }
                    else if (equation.Contains("asin") || equation.Contains("acos") || equation.Contains("atan") || equation.Contains("acot"))
                    {
                        CalculateArcTrigonometric(ref equation, leftBracket, ref subEquation, ref result, ref subList, ref tsq);
                    }
                    else if (equation.Contains("sin") || equation.Contains("cos") || equation.Contains("tan") || equation.Contains("cot"))
                    {
                        CalculateTrigonometric(ref equation, leftBracket, ref subEquation, ref result, ref subList, ref tsq);
                    }
                    else if (equation.Contains("log") || equation.Contains("ln"))
                    {
                        CalculateLogarithm(ref equation, leftBracket, ref subEquation, ref result, ref subList, ref tsq);
                    }
                    else
                    {
                        throw new InvalidInputException(equation);
                    }
                    Console.WriteLine(tsq + " = " + result + "\nNow equation is " + equation + "\n----------------------------------");
                }
                while (equation.Contains('(') && equation.Contains(')'))
                {
                    string subEquation = ExtractSubEquation(ref equation, ref leftBracket, ref rightBracket, ref tsq, ref subList);
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
        /// <summary>
        /// this method extract subequation from bracket's equation
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="leftBracket"></param>
        /// <param name="rightBracket"></param>
        /// <param name="tsq"></param>
        /// <param name="subList"></param>
        /// <returns></returns>
        static string ExtractSubEquation(ref string equation, ref int leftBracket, ref int rightBracket, ref string tsq, ref List<string> subList)
        {
            bracketindex(equation, ref leftBracket, ref rightBracket);
            string subEquation = Substring(equation, leftBracket, rightBracket);
            tsq = subEquation;
            validateequation(ref tsq, out subList);
            return subEquation;
        }
        /// <summary>
        /// this method calculate logarithms
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="leftBracket"></param>
        /// <param name="subEquation"></param>
        /// <param name="result"></param>
        /// <param name="subList"></param>
        /// <param name="tsq"></param>
        static void CalculateLogarithm(ref string equation, int leftBracket, ref string subEquation, ref double result, ref List<string> subList, ref string tsq)
        {
            if (subEquation.Contains('-'))
            {
                throw new InvalidInputException(equation);
            }
            if (equation[leftBracket - 1] == 'n')
            {
                result = Math.Log(Prioritiescalculation(ref subEquation, ref subList));
                equation = equation.Replace("ln(" + tsq + ")", result.ToString());
            }
            else if (equation[leftBracket - 1] == 'g')
            {
                result = Math.Log2(Prioritiescalculation(ref subEquation, ref subList));
                equation = equation.Replace("log(" + tsq + ")", result.ToString());
            }
            else
            {
                result = Prioritiescalculation(ref subEquation, ref subList);
                equation = equation.Replace("(" + tsq + ")", result.ToString());
            }
        }
        /// <summary>
        /// this method calculate trigonometric arc functions
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="leftBracket"></param>
        /// <param name="subEquation"></param>
        /// <param name="result"></param>
        /// <param name="subList"></param>
        /// <param name="tsq"></param>
        static void CalculateArcTrigonometric(ref string equation, int leftBracket, ref string subEquation, ref double result, ref List<string> subList, ref string tsq)
        {
            if (double.Parse(subEquation) > 1 || double.Parse(subEquation) < 0)
            {
                throw new InvalidInputException(equation);
            }
            if (equation[leftBracket - 1] == 'n')
            {
                if (equation[leftBracket - 2] == 'i')
                {
                    result = Math.Asin(Prioritiescalculation(ref subEquation, ref subList));
                    equation = equation.Replace("asin(" + tsq + ")", result.ToString());
                }
                else if (equation[leftBracket - 2] == 'a')
                {
                    result = Math.Atan(Prioritiescalculation(ref subEquation, ref subList));
                    equation = equation.Replace("atan(" + tsq + ")", result.ToString());
                }
            }
            else if (equation[leftBracket - 1] == 's')
            {
                result = Math.Acos(Prioritiescalculation(ref subEquation, ref subList));
                equation = equation.Replace("acos(" + tsq + ")", result.ToString());
            }
            else if (equation[leftBracket - 1] == 't')
            {
                result = Math.Atan(Prioritiescalculation(ref subEquation, ref subList)) / 1;
                equation = equation.Replace("acot(" + tsq + ")", result.ToString());
            }
            else
            {
                result = Prioritiescalculation(ref subEquation, ref subList);
                equation = equation.Replace("(" + tsq + ")", result.ToString());
            }
        }
        /// <summary>
        /// this method calculate trigonometric standtard functions
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="leftBracket"></param>
        /// <param name="subEquation"></param>
        /// <param name="result"></param>
        /// <param name="subList"></param>
        /// <param name="tsq"></param>
        static void CalculateTrigonometric(ref string equation, int leftBracket, ref string subEquation, ref double result, ref List<string> subList, ref string tsq)
        {
            if (equation[leftBracket - 1] == 'n')
            {
                if (equation[leftBracket - 2] == 'i')
                {
                    result = Math.Sin(Prioritiescalculation(ref subEquation, ref subList));
                    equation = equation.Replace("sin(" + tsq + ")", result.ToString());
                }
                else if (equation[leftBracket - 2] == 'a')
                {
                    result = Math.Tan(Prioritiescalculation(ref subEquation, ref subList));
                    equation = equation.Replace("tan(" + tsq + ")", result.ToString());
                }
            }
            else if (equation[leftBracket - 1] == 's')
            {
                result = Math.Cos(Prioritiescalculation(ref subEquation, ref subList));
                equation = equation.Replace("cos(" + tsq + ")", result.ToString());
            }
            else if (equation[leftBracket - 1] == 't')
            {
                result = Math.Tan(Prioritiescalculation(ref subEquation, ref subList)) / 1;
                equation = equation.Replace("cot(" + tsq + ")", result.ToString());
            }
            else
            {
                result = Prioritiescalculation(ref subEquation, ref subList);
                equation = equation.Replace("(" + tsq + ")", result.ToString());
            }
        }

        /// <summary>
        /// this method calculate equation under pow operand
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="leftBracket"></param>
        /// <param name="subEquation"></param>
        /// <param name="result"></param>
        /// <param name="subList"></param>
        /// <param name="tsq"></param>
        static void CalculatePow(ref string equation, int leftBracket, ref string subEquation, ref double result, ref List<string> subList, ref string tsq)
        {
            if (equation[leftBracket - 1] == 'w')//If previous element is "w", that means that's operand pow, and we will raise to power value
            {
                if (!subEquation.Contains(','))
                {
                    result = Prioritiescalculation(ref subEquation, ref subList);
                    equation = equation.Replace("pow(" + tsq + ")", result.ToString());
                }
                else
                {
                    equation = equation.Replace("pow(" + tsq + ")", ExtractPow(subEquation));
                }
            }
            else
            {
                result = Prioritiescalculation(ref subEquation, ref subList);
                equation = equation.Replace("(" + tsq + ")", result.ToString());
            }
        }
        /// <summary>
        /// this method calculate equation under sqrt operand
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="leftBracket"></param>
        /// <param name="subEquation"></param>
        /// <param name="result"></param>
        /// <param name="subList"></param>
        /// <param name="tsq"></param>

        static void CalculateSqrt(ref string equation, int leftBracket, ref string subEquation, ref double result, ref List<string> subList, ref string tsq)
        {
            if (equation[leftBracket - 1] == 't')//If previous element is "t", that means that's operand sqrt, and we will extract the root
            {
                result = Math.Sqrt(Prioritiescalculation(ref subEquation, ref subList));
                equation = equation.Replace("sqrt(" + tsq + ")", result.ToString());
            }
            else
            {
                result = Prioritiescalculation(ref subEquation, ref subList);
                equation = equation.Replace("(" + tsq + ")", result.ToString());
            }
        }
        /// <summary>
        /// this method extract underpow equation or value, and add ^ operand
        /// </summary>
        /// <param name="equation"></param>
        /// <returns></returns>
        static string ExtractPow(string equation)
        {
            int countComa = 0;//Value to calculate count of comas
            int firstComa = -1;//Value to get index of the first coma in the equation
            int comaBetweenValues = -1;//Value to get index of the coma, which will be replaced by "^"
            for (int i = 0; i < equation.Length; i++)
            {
                if (equation[i] == ',')//pow(2,2) If we have such equation, count of comas will be one
                {
                    countComa += 1;
                    if (countComa == 1)
                    {
                        firstComa = i;
                    }
                    else if (countComa == 2)//pow(2,3,1,2) If we have such equation, count of comas will be two, and index of coma, which will be replaced, is the current index
                    {
                        comaBetweenValues = i;
                    }
                    else if (countComa == 3 && equation.Contains('+') || equation.Contains('*') || equation.Contains('/') || equation.Contains('-'))//pow(2,3+3,2,3)
                    {
                        comaBetweenValues = i;
                    }
                }
            }
            if (countComa == 1)//If we have one coma, index of coma, which will be replaced, will be the first coma, which has been found in the equation
            {
                comaBetweenValues = firstComa;
            }
            Console.WriteLine(comaBetweenValues);
            equation = equation.Insert(comaBetweenValues, "^");//Replacing coma by "^"
            equation = equation.Remove(comaBetweenValues + 1, 1);//Deleting ","
            return equation;
        }

        /// <summary>
        /// this method check if element of string is number of dot('.')
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
            bool Isonenumber = true;
            validateequation(ref mathStr, out mathParts);
            if (mathStr.Contains("^") || mathStr.Contains("pow"))
            {
                Isonenumber = false;
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
                Isonenumber = false;
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
                Isonenumber = false;
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
            if (Isonenumber)
            {
                res = double.Parse(mathStr);
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
