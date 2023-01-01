using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework;

namespace ArduinoscopeClient
{
    public static class IOController
    {
        public static int[] BaudRates { get; private set; } = new int[10] { 110, 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };

        public static string      PortName = "";
        public static int         BaudRate = 0;
        public static string      LastInput   { get; private set; } = "";
        public static bool        IsConnected { get { return _running; } }
        public static bool        IsPaused    { get { return _paused; } }
        public static Thread      Thread      { get; private set; } = null;
        public static SerialPort  Serial      { get; private set; } = null;
        public static List<IOPin> Pins        { get; private set; } = new List<IOPin>()
        {
            new IOPin("A0", 54, IOPinType.Analog, 1024, 1.0f, Color.Red, 1024, false),
        };

        public static int BufferSize = 1024;

        private static bool   _running = false;
        public static bool    _paused  = false;
        public static bool    _reading = false;
        private static string _input   = "";

        public static void Begin(string portname, int baudrate)
        {
            PortName  = portname;
            BaudRate  = baudrate;

            Begin();
        }

        public static void Begin()
        {
            _running  = true;
            Thread    = new Thread(Run);
            Thread.Start();
        }

        public static void End()
        {
            while (_reading);
            _running = false;
        }

        public static void Pause()
        {
            _paused = true;
        }

        public static void Continue()
        {
            _paused = false;
        }

        public static void Run()
        {
            try
            {
                Serial = new SerialPort(PortName, BaudRate);
                Serial.DataReceived += OnDataReceived;
                Serial.Open();
                Debug.OK("Established serial connection to " + PortName + " at " + BaudRate + " baud");
            }
            catch (Exception ex) { Debug.Error("Failed to connect to serial " + PortName + " - " + ex.Message); return; }

            SendPinConfiguration();

            while (_running)
            {

            }

            if (Serial.IsOpen) { Serial.Close(); }
            Debug.Info("Disconnected serial " + PortName);
        }

        private static void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!_running || _paused || Serial == null || !Serial.IsOpen) { return; }
            _reading = true;
            string str = Serial.ReadLine();
            LastInput = str;
            if (str.StartsWith(">") && str.Contains(":"))
            {
                int pin = 0;
                string pin_str = str.Substring(1, str.IndexOf(':') - 1);
                int.TryParse(pin_str, out pin);
                if (GetPinIndex(pin) < 0 || GetPinIndex(pin) >= Pins.Count) { _reading = false; return; }

                int val = 0;
                string val_str = str.Substring(str.IndexOf(':') + 1);
                int.TryParse(val_str, out val);

                float v = (float)val / Pins[GetPinIndex(pin)].Maximum;
                PushValue(GetPinIndex(pin), val, v);
            }
            _reading = false;
        }

        public static void SendPinConfiguration()
        {
            if (Serial == null || !Serial.IsOpen) { return; }
            Serial.Write("-C\n");
            Thread.Sleep(100);

            for (int i = 0; i < Pins.Count; i++)
            {
                if (Pins[i].Pullup) { Serial.Write("+d"); }
                else { Serial.Write(Pins[i].Type == IOPinType.Analog ? "+A" : "+d"); }
                Serial.Write(Pins[i].Pin + "\n");
            }
        }

        public static void PushValue(int pin_index, int raw, float value)
        {
            if (pin_index < 0 || pin_index >= Pins.Count) { return; }
            for (int i = 0; i < BufferSize - 1; i++)
            {
                Pins[pin_index].Values[i] = Pins[pin_index].Values[i + 1];
                Pins[pin_index].ValueNormals[i] = Pins[pin_index].ValueNormals[i + 1];
            }
            Pins[pin_index].Values[BufferSize - 1] = raw;
            Pins[pin_index].ValueNormals[BufferSize - 1] = value;
        }

        public static bool ValidateName(string name)
        {
            if (name == null || name.Length == 0) { return false; }
            if (name.Length > 63) { return false; }
            return true;
        }

        public static bool Exists(string name)
        {
            for (int i = 0; i < Pins.Count; i++)
            {
                if (Pins[i].Name == name) { return true; }
            }
            return false;
        }

        public static int GetPinIndex(string name)
        {
            for (int i = 0; i < Pins.Count; i++)
            {
                if (Pins[i].Name == name) { return i; }
            }
            return -1;
        }

        public static int GetPinIndex(int pin)
        {
            for (int i = 0; i < Pins.Count; i++)
            {
                if (Pins[i].Pin == pin) { return i; }
            }
            return -1;
        }

        public static IOPin GetPinByName(string name)
        {
            for (int i = 0; i < Pins.Count; i++)
            {
                if (Pins[i].Name == name) { return Pins[i]; }
            }
            return new IOPin("", 0, IOPinType.Analog, 0, 0, Color.Black, 0);
        }
    }
}
