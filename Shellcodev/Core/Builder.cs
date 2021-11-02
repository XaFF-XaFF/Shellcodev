using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Shellcodev.Core
{
    class Builder
    {
        private CompilerResults Generator(string stub, string payload, string path)
        {
            var references = new[] { "System.dll","System.Runtime.InteropServices.dll" };

            CompilerParameters parameters = new CompilerParameters(references, path);
            parameters.GenerateExecutable = true;
            parameters.CompilerOptions = "/optimize- /platform:x86 /unsafe /target:winexe";
            parameters.OutputAssembly = path;

            stub = stub.Replace("[PAYLOAD]", payload);

            using (var provider = new CSharpCodeProvider())
                return provider.CompileAssemblyFromSource(parameters, stub);
        }

        public void Build(byte[] shellcode)
        {
            string payload = Convert.ToBase64String(shellcode);
            string stub = Properties.Resources.stub;
            string path = null;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Executable files | *.exe";
                bool flag = saveFileDialog.ShowDialog() == DialogResult.OK;

                if (flag)
                    path = saveFileDialog.FileName;
                else
                    return;
            }

            CompilerResults results = Generator(stub, payload, path);

            if(results.Errors.Count == 0)
            {
                MessageBox.Show("File build succeeded", "Shellcodev", MessageBoxButtons.OK);
            }
            else
            {
                string currentTime = DateTime.Now.ToString("hh-mm-ss");
                string logPath = ErrorHandler(currentTime, results);

                var dialogResult = MessageBox.Show("File build failed. Would you like to check log file?", "Shellcodev", MessageBoxButtons.YesNo);
                switch(dialogResult)
                {
                    case DialogResult.Yes:
                        Process.Start("notepad.exe", logPath);
                        break;
                    default:
                        return;
                }
            }
        }

        private string ErrorHandler(string currentTime, CompilerResults results)
        {
            string logDir = "shld_logs";
            string logPath = logDir + "\\" + "build-" + currentTime + ".log";

            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            File.Create(logPath).Close();
            StreamWriter file = new StreamWriter(logPath);

            foreach(CompilerError error in results.Errors)
            {
                Console.WriteLine(error.ErrorText);
                file.WriteLine(error.ErrorText);
            }
            file.Flush();
            file.Close();

            return logPath;
        }
    }
}
