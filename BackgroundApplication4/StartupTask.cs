using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;


using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Nito.AsyncEx;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Net;
using System.IO;



// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BackgroundApplication4
{
    public sealed class StartupTask : IBackgroundTask
    {
        string rxBuffer;
        
       

        public async void Serial1()
        {
            string aqs = SerialDevice.GetDeviceSelector("UART0");                   /* Find the selector string for the serial device   */
            var dis = await DeviceInformation.FindAllAsync(aqs);                    /* Find the serial device with our selector string  */
            SerialDevice SerialPort = await SerialDevice.FromIdAsync(dis[0].Id);    /* Create an serial device with our selected device */

            /* Configure serial settings */
            SerialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
            SerialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            SerialPort.BaudRate = 9600;  // valor identico ao arduino
            SerialPort.Parity = SerialParity.None;
            SerialPort.StopBits = SerialStopBitCount.One;
            SerialPort.DataBits = 8;
        
            /* Read data in from the serial port */
            const uint maxReadLength = 1024;
            DataReader dataReader = new DataReader(SerialPort.InputStream);
            uint bytesToRead = await dataReader.LoadAsync(maxReadLength);
            rxBuffer = dataReader.ReadString(bytesToRead);
        }
        public void Run(IBackgroundTaskInstance taskInstance)
        {
              Serial1();
              EnviaDados();
         
        }

        public void EnviaDados()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://api.thingspeak.com/update?api_key=D3X4B8IT0PVBCQR3&field1="+ rxBuffer +""); // campo após "field1=" deve receber a variavel que irá receber os dados
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            var dataE = httpWebRequest.GetRequestStreamAsync();


            var httpResponse = httpWebRequest.GetResponseAsync();
        }


       
    }
}
