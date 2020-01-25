# PowerAppsWindowsPrinterService
Is a windows service that reads from an Azure Storage Queue and prints the content to an specific network printer. The initial purpose of this service was to print .zpl files from PowerApps to zebra printers, but it can be extended to support others. 

The service flows as follow: 

1. The services check every 5 seconds for a message in an Azure Storage Queue
2. Once a message is received, it takes up to 10 message from the Queue and removes them.
3. Each message is written to a file in a system tmp folder
4. The command "print file.zpl /D:networkaddress" is called

The printer has to be available in the network and shared. 

Note: The project is shipped with a PowerApps - canvas app to test it. 
