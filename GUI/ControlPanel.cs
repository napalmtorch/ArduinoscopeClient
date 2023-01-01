using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ImGuiNET;
using SVEC4 = System.Numerics.Vector4;

namespace ArduinoscopeClient
{
    public static class ControlPanel
    {
        public static Point Size { get { return new Point(400, 440); } }
        public static bool  IsHovering { get; private set; }
        public static bool  IsActive { get; private set; }
        public static float Speed { get { return _speed; } }

        private static string _selpin   = "";
        private static string _name     = "";
        private static int    _pin      = 0;
        private static int    _max      = 0;
        private static float  _scale    = 1.0f;
        private static float  _speed    = 1.0f;
        private static bool   _digital  = false;
        private static bool   _pullup   = false;

        private static SVEC4  _color   = new SVEC4(1.0f, 1.0f, 1.0f, 1.0f);

        public static void Initialize()
        {
            IOController.PortName = (SerialPort.GetPortNames().Length > 0 ? SerialPort.GetPortNames()[0] : "");
            IOController.BaudRate = 9600;
        }

        public static void Update(GameTime gt)
        {

        }

        public static void Draw(GameTime gt)
        {
            Client.UIRenderer.BeginLayout(gt);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Size.X, Size.Y));
            ImGui.Begin("Control Panel", ImGuiWindowFlags.NoResize);

            StateButtons();
            BeginComboPortName();
            BeginComboBaudRate();

            if (ImGui.SliderFloat("Multiplier", ref _speed, 0.001f, 10.0f)) { }

            BeginPinManager();

            IsHovering = ImGui.IsAnyItemHovered() | ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow);
            IsActive = ImGui.IsAnyItemActive();

            ImGui.End();

            Client.UIRenderer.EndLayout();
        }

        private static void StateButtons()
        {
            string connect_str = (IOController.IsConnected ? "Disconnect" : "Connect");
            if (ImGui.Button(connect_str))
            {
                if (IOController.IsConnected) { IOController.End(); }
                else { IOController.Begin(); }
            }
            ImGui.SameLine();

            string pause_str = (IOController.IsPaused ? "Continue" : "Pause");
            if (ImGui.Button(pause_str))
            {
                if (!IOController.IsPaused) { IOController.Pause(); }
                else { IOController.Continue(); }
            }
        }

        private static void BeginComboPortName()
        {
            if (ImGui.BeginCombo("Port", IOController.PortName))
            {
                string[] ports = SerialPort.GetPortNames();
                for (int i = 0; i < ports.Length; i++) { if (ImGui.Selectable(ports[i])) { IOController.PortName = ports[i]; } }
                ImGui.EndCombo();
            }
        }

        private static void BeginComboBaudRate()
        {
            if (ImGui.BeginCombo("Baud", IOController.BaudRate.ToString()))
            {
                int[] bauds = IOController.BaudRates;
                for (int i = 0; i < bauds.Length; i++) { if (ImGui.Selectable(bauds[i].ToString())) { IOController.BaudRate = bauds[i]; } }
                ImGui.EndCombo();
            }
        }

        private static void BeginPinManager()
        {
            ImGui.LabelText("", "Selected Pin: " + (_selpin.Length > 0 ? _selpin : "None"));
            if (ImGui.BeginListBox("I/O Pins", new System.Numerics.Vector2(208, 80)))
            {
                for (int i = 0; i < IOController.Pins.Count; i++)
                {
                    if (ImGui.Selectable(IOController.Pins[i].Name)) 
                    { 
                        _selpin = IOController.Pins[i].Name;
                        SelectedPinChanged();
                    }
                }
                ImGui.EndListBox();
            }

            if (ImGui.InputText("Name", ref _name, 64))
            {

            }

            if (ImGui.InputInt("Pin Number", ref _pin, 1))
            {

            }

            if (ImGui.InputInt("Maximum", ref _max, 1))
            {

            }

            if (ImGui.InputFloat("Scale", ref _scale, 0.25f))
            {

            }

            if (ImGui.ColorEdit4("Color", ref _color))
            {

            }

            if (ImGui.RadioButton("Analog", !_digital)) { _digital = false; _pullup = false; }

            ImGui.SameLine();
            if (ImGui.RadioButton("Digital", _digital)) { _digital = true; }

            if (_digital)
            {
                ImGui.SameLine();
                if (ImGui.Checkbox("Pullup Mode", ref _pullup)) { }
            }

            if (ImGui.Button("Add"))
            {
                bool valid = true;
                if (!IOController.ValidateName(_name)) { Debug.Warning("Invalid pin name"); valid = false; }
                if (IOController.Exists(_name)) { Debug.Warning("Pin with name already exists"); valid = false; }

                if (valid)
                {
                    IOPin pin = new IOPin(_name, (byte)_pin, (_digital ? IOPinType.Digital : IOPinType.Analog), _max, _scale, new Color(new Vector4(_color.X, _color.Y, _color.Z, _color.W)), IOController.BufferSize, _pullup);
                    IOController.Pins.Add(pin);
                    IOController.SendPinConfiguration();
                    Debug.Info("Saved pin to index " + (IOController.Pins.Count - 1).ToString());
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Modify"))
            {
                if (_selpin.Length > 0)
                {
                    IOPin pin = IOController.GetPinByName(_selpin);
                    if (pin.Name.Length == 0) { Debug.Warning("Cannot modify pin that has not yet been added"); }
                    else if (!IOController.ValidateName(_name)) { Debug.Warning("Invalid pin name"); }
                    else
                    {
                        int i = IOController.GetPinIndex(pin.Name);
                        if (i < 0 || i >= IOController.Pins.Count) { Debug.Error("Failed to modify pin - invalid index " + i); }
                        IOController.Pins[i] = new IOPin(_name, (byte)_pin, (_digital ? IOPinType.Digital : IOPinType.Analog), _max, _scale, new Color(new Vector4(_color.X, _color.Y, _color.Z, _color.W)), IOController.BufferSize, _pullup);
                        IOController.SendPinConfiguration();
                        Debug.Info("Modified pin at index " + i);
                    }
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Remove"))
            {
                IOPin pin = IOController.GetPinByName(_selpin);
                if (pin.Name.Length == 0) { Debug.Warning("No pin selected"); }
                else
                {
                    int i = IOController.GetPinIndex(pin.Name);
                    if (i < 0 || i >= IOController.Pins.Count) { Debug.Error("Failed to remove pin - invalid index " + i); }
                    IOController.Pins.RemoveAt(i);
                    IOController.SendPinConfiguration();
                }
            }
        }

        private static void SelectedPinChanged()
        {
            IOPin pin = IOController.GetPinByName(_selpin);
            if (pin.Name.Length == 0) { return; }

            _name    = pin.Name;
            _pin     = pin.Pin;
            _max     = pin.Maximum;
            _scale   = pin.Scale;
            _digital = pin.Type == IOPinType.Digital;
            _pullup  = pin.Pullup;
            _color   = new SVEC4(pin.Color.R / 255.0f, pin.Color.G / 255.0f, pin.Color.B / 255.0f, pin.Color.A / 255.0f);
        }
    }
}
