using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPrinterService
{
    interface PrintDriverInterface
    {
        string PrintRaw(string rawtxt);
        void setPrinterNumber(int printerNumber);
        int getPrinterNumber(); 

        void init(); 
    }
}
