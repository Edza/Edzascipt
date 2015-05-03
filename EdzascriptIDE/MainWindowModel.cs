using Edzascript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdzascriptIDE
{
    class MainWindowModel
    {
        private readonly Parser _parser = new Parser();

        public string Parse(string input)
        {
            return this._parser.Parse(input);
        }

        async public void BuildAndRun(string input)
        {
            await Task.Delay(1);

            string path = System.IO.Directory.GetCurrentDirectory();
            string asmFile = path + "\\output.asm";
            string exeFile = "output.exe";
            string objFile = path + "\\output.obj";

            System.IO.File.Delete(asmFile);
            System.IO.File.Delete(exeFile);
            System.IO.File.Delete(objFile);

            System.IO.File.WriteAllText(asmFile, input);

            string strCmdText;
            strCmdText = "/c /coff /Cp /nologo /I\"C:\\Masm32\\Include\" \"" + asmFile + "\"";
            Process mlExe = Process.Start("C:\\masm32\\bin\\ML.EXE", strCmdText);

            mlExe.WaitForExit();


            strCmdText = "/SUBSYSTEM:CONSOLE /RELEASE /VERSION:4.0 /LIBPATH:\"C:\\Masm32\\Lib\" /OUT:\"" + exeFile + "\" \"" + objFile + "\"";
            Process linkExe = Process.Start("C:\\masm32\\bin\\LINK.EXE", strCmdText);

            linkExe.WaitForExit();

            try
            {
                System.Diagnostics.Process.Start(exeFile, "");
            }
            catch (Exception)
            {

                MessageBox.Show("Compilation error!");
            }
        }
    }
}
