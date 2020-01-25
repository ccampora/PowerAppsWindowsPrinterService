using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPrinterService
{
    interface ReadDriverInterface
    {
        string PopMessage();


        void init();

        // Number of the printer as suffix in App.config file
        void setPrinterNumber(int printerNumber);
        int getPrinterNumber(); 

        
    }
}
