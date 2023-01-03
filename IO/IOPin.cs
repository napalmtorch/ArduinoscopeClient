using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ArduinoscopeClient
{
    public enum IOPinType : byte { Analog, Digital }

    public class IOPin
    {
        public string    Name = string.Empty;
        public byte      Pin;
        public IOPinType Type;
        public Color     Color;
        public bool      Pullup;
        public int       Maximum;
        public float     Scale;
        public int[]     Values = new int[1];
        public float[]   ValueNormals = new float[1];

        public IOPin() { }

        public IOPin(string name, byte pin, IOPinType type, int max, float scale, Color color, int count, bool pullup = false)
        {
            this.Pullup  = pullup;
            this.Name    = name;
            this.Pin     = pin;
            this.Type    = type;
            this.Maximum = max;
            this.Scale   = scale;
            this.Color   = color;
            Resize(count);
        }

        public void Resize(int count)
        {
            if (count == 0) { count = 1; }
            Values       = new int[count];
            ValueNormals = new float[count];
        }

        public float GetAverage()
        {
            float avg = 0;
            foreach (float vn in ValueNormals) { avg += vn; }
            avg /= ValueNormals.Length;
            return avg;
        }

        public string Serialize()
        {
            string serialized = "PIN," + Name + "," + Pin + "," + ((int)Type).ToString() + "," + (Pullup ? "1," : "0,") + Maximum + "," + Scale.ToString() + "," + Color.PackedValue.ToString();
            return serialized;
        }

        public void Deserialize(string serialized)
        {
            string[] parts = serialized.Split(',');
            if (parts.Length != 8) { Debug.Error("Invalid argument count while deserializing IO pin"); return; }
            if (parts[0] != "PIN") { Debug.Error("String is not a serialized pin"); return; }

            Name = parts[1];
            
            if (!byte.TryParse(parts[2], out Pin)) { Debug.Error("Error deserializing pin number value"); return; }

            int type = 0;
            if (!int.TryParse(parts[3], out type)) { Debug.Error("Error deserializing pin type value"); return; }
            Type = (IOPinType)type;

            if (parts[4] == "1") { Pullup = true; }
            else if (parts[4] == "0") { Pullup = false; }
            else { Debug.Error("Error deserializing pin pullup flag"); return; }

            if (!int.TryParse(parts[5], out Maximum)) { Debug.Error("Error deserializing pin maximum value"); return; }

            if (!float.TryParse(parts[6], out Scale)) { Debug.Error("Error deserializing pin scale value"); return; }

            uint color = 0;
            if (!uint.TryParse(parts[7], out color)) { Debug.Error("Error deserializing pin color value"); return; }
            Color = new Color(color);
        }
    }
}
