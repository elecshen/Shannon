using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shannon
{
    internal class Program
    {
        private static string s = "0 • ¬(x→y) x ¬(x←y) y ⊕ v ↓ ~ ¬y ← ¬x → / 1";
        private static string[] varStr = new string[] { "x", "y", "z"};

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            Console.WriteLine("Введите вектор значений(через пробел): ");
            string vec = Console.ReadLine();
            string[] arr = vec.Split(' ');
            int varCount = NumOfVarInVector(arr.Length);
            if (varCount < 2 || varCount > 3)
            {
                return;
            }
            int[] ints = new int[arr.Length];
            for(int i = 0; i < arr.Length; i++)
            {
                ints[i] = int.Parse(arr[i]);
            }

            Console.WriteLine($"Введите номер переменной разложения(от 0 по {varCount - 1})");
            int numVar = int.Parse(Console.ReadLine());
            int partLen = ints.Length / (int)Math.Pow(2, 1 + numVar);
            GetTableParts(partLen, ints, out List<int> part1, out List<int> part2);

            string part1Vec, part2Vec, fPart1, fPart2;
            fPart1 = GetFormulaPart(numVar, varCount, part1, out part1Vec);
            fPart2 = GetFormulaPart(numVar, varCount, part2, out part2Vec);

            Console.WriteLine("Вектор значений разложения при переменной равной 1:");
            Console.WriteLine(part1Vec);
            Console.WriteLine("\nВектор значений разложения при переменной равной 0:");
            Console.WriteLine(part2Vec);

            Console.WriteLine("\nФормула");
            Console.WriteLine(GetFormula(numVar, varCount, fPart1, fPart2));
        }

        private static int NumOfVarInVector(int length)
        {
            double degree = Math.Log2(length);
            if ( degree == Math.Truncate(degree) )
                return (int)degree;
            return 0;
        }

        private static void GetTableParts(int partLen, int[] ints, out List<int> part1, out List<int> part2)
        {
            part1 = new List<int>();
            part2 = new List<int>();
            for (int i = 0, a = 0; i < ints.Length; i++)
            {
                if (a < partLen)
                {
                    part1.Add(ints[i]);
                    a++;
                }
                else if (a < 2 * partLen)
                {
                    part2.Add(ints[i]);
                    a++;
                }
                else
                {
                    a = 0;
                    i--;
                }
            }
        }

        private static string GetFormulaPart(int numVar, int varCount, List<int> tablePart, out string outStr)
        {
            outStr = "";
            for (int i = 0; i < tablePart.Count; i++)
            {
                outStr += tablePart[i];
                if (i + 1 != tablePart.Count)
                    outStr += " ";
            }
            var workVars = varStr.Where(v => v != varStr[numVar]);
            if (varCount == 2)
                return Choose1VarFun(outStr, workVars.First());
            else if (varCount == 3)
                return Choose2VarsFun(outStr, workVars.First(), workVars.Last());
            else
                return "";
        }

        private static string GetFormula(int numVar, int varCount, string part1, string part2)
        {
            if (varCount == 2)
                return $"{varStr[numVar]}•{part1} v {GetNOT(varStr[numVar])}•{part2}";
            else if (varCount == 3)
                return $"{varStr[numVar]}•{GetBrackets(part1)} v {GetNOT(varStr[numVar])}•{GetBrackets(part2)}";
            else
                return "";
            
        }

        private static Dictionary<string, Func<string, string>> elem1VarFunc = new Dictionary<string, Func<string, string>>()
        {
            { "0 0", a => { return "0"; } },
            { "0 1", a => { return a; } },
            { "1 0", a => { return GetNOT(a); } },
            { "1 1", a => { return "1"; } }
        };

        private static string Choose1VarFun(string vector, string var1)
        {
            foreach (var elem in elem1VarFunc)
            {
                if (vector == elem.Key)
                {
                    return elem.Value(var1);
                }
            }
            return "";
        }

        private static Dictionary<string, Func<string, string, string>> elem2VarsFunc = new Dictionary<string, Func<string, string, string>>()
        {
            { "0 0 0 0", (a,b) => { return "0"; } },
            { "0 0 0 1", GetAND },
            { "0 0 1 0", GetNotIMPL },
            { "0 0 1 1", (a,b) => { return a; } },
            { "0 1 0 0", GetNotReIMPL },
            { "0 1 0 1", (a,b) => { return b; } },
            { "0 1 1 0", GetXOR },
            { "0 1 1 1", GetOR },
            { "1 0 0 0", GetPIERArr },
            { "1 0 0 1", GetEQUIV },
            { "1 0 1 0", (a,b) => { return GetNOT(b); } },
            { "1 0 1 1", GetReIMPL },
            { "1 1 0 0", (a,b) => { return GetNOT(a); } },
            { "1 1 0 1", GetIMPL },
            { "1 1 1 0", GetSCHAEFFER },
            { "1 1 1 1", (a,b) => { return "1"; } },
        };

        private static string Choose2VarsFun(string vector, string var1, string var2)
        {
            foreach(var elem in elem2VarsFunc)
            {
                if(vector == elem.Key)
                {
                    return elem.Value(var1,var2);
                }
            }
            return "";
        }

        private static string GetAND(string var1, string var2)
        {
            return $"{var1}•{var2}";
        }

        private static string GetNotIMPL(string var1, string var2)
        {
            return $"¬({var1}→{var2})";
        }

        private static string GetNotReIMPL(string var1, string var2)
        {
            return $"¬({var1}←{var2})";
        }

        private static string GetXOR(string var1, string var2)
        {
            return $"{var1}⊕{var2}";
        }

        private static string GetOR(string var1, string var2)
        {
            return $"{var1}v{var2}";
        }

        private static string GetPIERArr(string var1, string var2)
        {
            return $"{var1}↓{var2}";
        }

        private static string GetEQUIV(string var1, string var2)
        {
            return $"{var1}~{var2}";
        }

        private static string GetNOT(string var1)
        {
            return $"¬{var1}";
        }

        private static string GetReIMPL(string var1, string var2)
        {
            return $"{var1}←{var2}";
        }

        private static string GetIMPL(string var1, string var2)
        {
            return $"{var1}→{var2}";
        }

        private static string GetSCHAEFFER(string var1, string var2)
        {
            return $"{var1}/{var2}";
        }

        private static string GetBrackets(string var1)
        {
            return $"({var1})";
        }
    }
}
