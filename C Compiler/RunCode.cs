using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Compiler
{
    class CCompiler
    {
        const string cFile = "sample.c";
        const string output = "output.txt";
        const string startup = "..\\..\\Resource\\Startup.bat";
        const string compile = "..\\..\\Resource\\Compile.bat";

        /// <summary>
        /// 執行程式碼
        /// </summary>
        /// <param name="code">測試的C Code</param>
        /// <param name="testDatas">測試的資料，每個變數當作一行</param>
        /// <returns></returns>
        public string CompileC(string code, string[] testDatas)
        {
            string results = "";            

            try
            {

                DeleteTemp(cFile);
                DeleteTemp(output);

                using (FileStream fs = File.Create(cFile))
                {
                    byte[] codeText = new UTF8Encoding(true).GetBytes(code);
                    fs.Write(codeText, 0, codeText.Length);
                }

                Process process = new Process();
                process.StartInfo.FileName = @"C:\Windows\system32\cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                using (StreamWriter sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        //This batch starts up the Visual Studio Command Prompt.
                        sw.WriteLine(startup);
                        //This batch does the compilation, once the Command Prompt
                        //is running, using the 'cl' command.
                        sw.WriteLine(compile);

                        //依序輸入測資
                        foreach (var str in testDatas)
                        {
                            sw.WriteLine(str);
                        }
                    }
                }

                using (StreamReader sr = process.StandardOutput)
                {
                    if (sr.BaseStream.CanRead)
                        results = sr.ReadToEnd();
                }
            }
            catch (Exception ex) { return ex.ToString(); }

            return results;
        }

        /// <summary>
        /// 刪除暫存檔
        /// </summary>
        /// <param name="fileName">暫存檔名稱</param>
        void DeleteTemp(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
