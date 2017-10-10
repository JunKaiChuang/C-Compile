using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace C_Compiler
{
    class CCompiler
    {
        const int overtimeSec = 5;

        const string tempFilePath = ".\\Temp\\";

        const string startup = "..\\..\\Resource\\Startup.bat";
        const string compileSuccess = "編譯成功";
        const string compileFail = "未編譯成功";
        const string overtime = "執行逾時";

        bool isOvertime;
        string compileInfo;
        
        public bool IsOvertime { get { return isOvertime; } }
        public string CompileInfo { get { return compileInfo; } }
        /// <summary>
        /// 執行程式碼
        /// </summary>
        /// <param name="userID">使用者ID</param>
        /// <param name="code">測試的C Code</param>
        /// <param name="testDatas">測試的資料，每個變數當作一行</param>
        /// <returns></returns>
        public string CompileC(string userID, string code, string[] testDatas)
        {
            if (!Directory.Exists(tempFilePath))
            {
                Directory.CreateDirectory(tempFilePath);
            }

            string results = "";
            isOvertime = false;
            string cFileName = string.Format("{0}.c", userID);
            string outputName = userID + "_output";
            string exeName = string.Format("{0}.exe", cFileName);
            string objName = string.Format("{0}.obj", cFileName);
            string info = string.Format("{0}_info.txt", userID);
            string batName = string.Format("{0}.bat", userID);

            try
            {

                DeleteTemp(cFileName);
                DeleteTemp(outputName);
                DeleteTemp(exeName);
                DeleteTemp(objName);

                GenCompileTarget(userID);

                WriteTextFile(cFileName, code);

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
                        sw.WriteLine(tempFilePath + batName);                        

                        sw.WriteLine(string.Format("{0}.exe  >> {1}.txt", userID, outputName));
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
                    {
                        //
                        Thread t = new Thread(new ThreadStart(delegate ()
                        {
                            results = sr.ReadToEnd();
                        }));

                        t.Start();

                        // 最多只能跑N秒!
                        if (!t.Join(overtimeSec * 1000))
                        {
                            t.Abort();
                            compileInfo =  overtime + overtimeSec + "秒";
                            isOvertime = true;
                            var proc = Process.GetProcessesByName(userID);
                            proc[0].Kill();
                        }
                    }
                        
                }
            }
            catch (Exception ex) { return ex.ToString(); }

            if (!isOvertime)
            {
                OutputCompileResult(userID);
            }
            return results;
        }

        /// <summary>
        /// 產生該User的專屬編譯bat
        /// </summary>
        /// <param name="userID"></param>
        void GenCompileTarget(string userID)
        {
            string batName = tempFilePath + string.Format("{0}.bat", userID);
            if (!File.Exists(batName))
            {
                File.Delete(batName);
            }
            using (FileStream fs = File.Create(batName))
            {
                byte[] codeText = new ASCIIEncoding().GetBytes(string.Format("cl {0}{1}.c", tempFilePath, userID));
                fs.Write(codeText, 0, codeText.Length);
            }
        }

        /// <summary>
        /// 刪除暫存檔
        /// </summary>
        /// <param name="fileName">暫存檔名稱</param>
        void DeleteTemp(string fileName)
        {
            fileName = tempFilePath + fileName;
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        /// <summary>
        /// 輸出文字檔
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        void WriteTextFile(string fileName, string text)
        {
            fileName = tempFilePath + fileName;

            using (FileStream fs = File.Create(fileName))
            {
                byte[] codeText = new UTF8Encoding(true).GetBytes(text);
                fs.Write(codeText, 0, codeText.Length);
            }
        }



        /// <summary>
        /// 輸出是否編譯成功
        /// </summary>
        void OutputCompileResult(string userID)
        {
            string exeName = string.Format("{0}.exe", userID);
            string info = string.Format("{0}_info.txt", userID);

            if (File.Exists(exeName))
            {
                compileInfo = compileSuccess;
            }
            else
            {
                compileInfo = compileFail;
            }
        }
    }
}
