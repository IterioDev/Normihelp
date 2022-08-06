using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace Normihelp
{
    internal class Program
    {
        static string NorminetteCommand = "norminette";

        static void Main(string[] args)
        {
            SetupFileMonitor(args[0]);
            Console.ReadLine();
        }

        private static void SetupFileMonitor(string targetFilePath)
        {
            FileSystemWatcher fwatcher = new FileSystemWatcher();
            fwatcher.Path = Directory.GetParent(targetFilePath).FullName;
            fwatcher.Filter = Path.GetFileName(targetFilePath);
            fwatcher.NotifyFilter = NotifyFilters.LastWrite;
            fwatcher.Changed += (s, e) =>
            {
                fwatcher.EnableRaisingEvents = false;
                GetNorminetteOutput(targetFilePath);
                fwatcher.EnableRaisingEvents = true;
            };
            fwatcher.EnableRaisingEvents = true;
        }

        static void GetNorminetteOutput(string targetFilePath)
        {
            //Print Timestamp Info
            Console.Clear();
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Detect file modification. Rechecking for Errors..");

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = NorminetteCommand;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.Arguments = $"\"{targetFilePath}\"";

            Process p = Process.Start(psi);
            p.OutputDataReceived += P_OutputDataReceived;
            p.BeginOutputReadLine();

            p.WaitForExit();        
        }

        private static void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
                return;

            var data = e.Data.Replace("  ", " ");
            string errorInfo = Regex.Match(data, ":	(.*)").Groups[1].Value.Trim();
            string lineInfo = Regex.Match(data, "line: (.*),").Groups[1].Value.Trim();
            string columnInfo = Regex.Match(data, "col: (.*)\\)").Groups[1].Value.Trim();

            //Validity Check
            if (errorInfo == "" || lineInfo == "" || columnInfo == "")
                return;

            //Output
            Console.WriteLine($"Error: {errorInfo} @ Pos {columnInfo} on Line {lineInfo}");
        }
    }
}