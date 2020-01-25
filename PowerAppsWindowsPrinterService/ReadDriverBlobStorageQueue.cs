using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.Azure.Storage; // Namespace for CloudStorageAccount
using Microsoft.Azure.Storage.Queue; // Namespace for Queue storage types


namespace WindowsPrinterService
{
    public class ReadDriverBlobStorageQueue : ReadDriverInterface
    {

        CloudQueue _queue;
        private int printerNumber = 0; 

        public void init()
        {
            if(printerNumber != 0)
            {
                this.initQueue(CloudConfigurationManager.GetSetting("readQueueName" + (printerNumber)));
            }
        }

        public void initQueue(string _queueName)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            _queue = queueClient.GetQueueReference(_queueName);

            // Create the queue if it doesn't already exist
            _queue.CreateIfNotExists();

        }

        // Read and deletes the message. Assumes the printer queue handles retries upon possible hardware or OS failures.  
        string ReadDriverInterface.PopMessage()
        {
            if (_queue == null)
                return "queue not set";

            if(this.ifMessage())
            {
                // Get the next message
                CloudQueueMessage retrievedMessage = _queue.GetMessage();

                //Process the message in less than 30 seconds, and then delete the message
                _queue.DeleteMessage(retrievedMessage);

                return retrievedMessage.AsString;
            }
            else
                return "";

        }

        Boolean ifMessage()
        {

            // Peek at the next message
            CloudQueueMessage peekedMessage = _queue.PeekMessage();

            if (peekedMessage != null)
                return true;

            return false;
        }

        public void setPrinterNumber(int printerNumber)
        {
            this.printerNumber = printerNumber; 
        }

        public int getPrinterNumber()
        {
            return this.printerNumber;
        }
    }
}
