using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Text;

namespace Fragments
{
    class SerialController
    {
        private SerialPort serialPort;
        public SerialController(string port, int baudRate, Parity parity, StopBits stopBits, int dataBits, Handshake handShake, bool rtsEnable, SerialDataReceivedEventHandler DataReceivedHandler)
        {
            serialPort = new SerialPort(port);
            serialPort.BaudRate = baudRate;
            serialPort.Parity = parity;
            serialPort.StopBits = stopBits;
            serialPort.DataBits = dataBits;
            serialPort.Handshake = handShake;
            serialPort.RtsEnable = rtsEnable;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            serialPort.Open();
        }
        
        public void Dispose()
        {
            serialPort.Close();
        }
    }
}