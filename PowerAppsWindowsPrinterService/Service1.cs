using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Azure; // Namespace for CloudConfigurationManager

namespace WindowsPrinterService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;) 
        ReadDriverInterface[] reader_ = new ReadDriverInterface[100];
        PrintDriverInterface[] printer_ = new PrintDriverInterface[100];
        string messagetoprint = "";

        public Service1()
        {
            for (int iprinter = 0; iprinter < 100; iprinter++)
            {

                string printerDriverClassName = CloudConfigurationManager.GetSetting("printerDriverClassName" + (iprinter + 1));
                string readDriverClassName = CloudConfigurationManager.GetSetting("readDriverClassName" + (iprinter + 1));

                if(printerDriverClassName != null && readDriverClassName != null)
                {
                    reader_[iprinter] = Activator.CreateInstance(Type.GetType(readDriverClassName)) as ReadDriverInterface;
                    printer_[iprinter] = Activator.CreateInstance(Type.GetType(printerDriverClassName)) as PrintDriverInterface;
                    reader_[iprinter].setPrinterNumber(iprinter + 1);
                    printer_[iprinter].setPrinterNumber(iprinter + 1); 
                    


                    reader_[iprinter].init();
                    printer_[iprinter].init(); 
                }
            }
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;


        }
        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //string printreturn = "";


            for (int iprinter = 0; reader_[iprinter] != null; iprinter++)
            {
                messagetoprint = reader_[iprinter].PopMessage();

                for (int i = 0; messagetoprint.Length > 0 && i < 10; i++) { 
                    printer_[iprinter].PrintRaw(messagetoprint);
                    messagetoprint = reader_[iprinter].PopMessage();
                }
            }


            //WriteToFile(messagetoprint + "Service is recall at " + DateTime.Now);
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {

                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }


        }

    }
}
