using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager

namespace WindowsPrinterService
{

    class SharedNetworkPrinterPrintDriver : PrintDriverInterface
    {

        private int printerNumber;
        private string printerNetworkAddress; 

        public void init()
        {
            printerNetworkAddress = CloudConfigurationManager.GetSetting("printerNetworkAddress" + (printerNumber));
        }

        string getPrinterNetworkAddress()
        {
            return printerNetworkAddress;
        }

        public string PrintRaw(string rawtxt)
        {
            string tempfilepath = saveToTempFile(rawtxt);
            //string command = "ECHO "+rawtxt+" >C:\\01.txt";


            //string command = string.Format("echo {0}|more>lpt1", @rawtxt);
            //string command = string.Format("echo \"{0}\"|more>lpt1", @rawtxt);
            //string command = "echo " + rawtxt + "|more >lpt1";

            StringBuilder command = new StringBuilder();
            //command.AppendLine(@"net use LPT1: \\10.10.0.109\Por_Zebra_Nilus / persistent:yes \n");
            command.Append(@"print ");
            command.Append(@tempfilepath);
            command.Append(@" /D:"+getPrinterNetworkAddress());
            //net use LPT1: / delete

            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " +command.ToString());

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            string returnmsg = "";

            // wrap IDisposable into using (in order to release hProcess) 
            using (Process process = new Process())
            {
                process.StartInfo = procStartInfo;
                process.Start();

                // Add this: wait until process does its work
                process.WaitForExit();

                // and only then read the result
                returnmsg = process.StandardOutput.ReadToEnd();
                //Console.WriteLine(result);
            }
            return returnmsg;
        }

        public void setPrinterNumber(int printerNumber)
        {
            this.printerNumber = printerNumber;
        }

        private string saveToTempFile(string rawtext)
        {
            
            string path = @"c:\Windows\temp\" + Guid.NewGuid() +".zpl";

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                File.WriteAllText(path, rawtext);
            }
            return path;
        }

        public int getPrinterNumber()
        {
            return this.printerNumber; 
        }
    }
}
