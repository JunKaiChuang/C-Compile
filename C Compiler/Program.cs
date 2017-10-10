using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            CCompiler cCompiler = new CCompiler();
            string code = @"";
            string[] testDatas = { "200", "30" };

            //讀取Resource資料夾底下的"code.txt"作為code來源
            using (StreamReader sr = new StreamReader("..\\..\\Resource\\code.txt"))
            {
                code = sr.ReadToEnd();
            }

            var s = cCompiler.CompileC("102590007", code, testDatas);
            Console.Clear();
            Console.WriteLine(cCompiler.CompileInfo);
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            return ;
        }
    }
}
