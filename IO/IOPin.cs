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
    }
}
