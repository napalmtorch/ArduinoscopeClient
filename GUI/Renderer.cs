using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArduinoscopeClient
{
    public static class Renderer
    {
        public static void DrawGrid(int w, int h, int grid_sz, int thickness, Color color)
        {
            for (int i = 0; i < w; i += grid_sz)
            {
                Client.SpriteBatch.Draw(Client.Pixel, new Rectangle(i, 0, thickness, h), color);
            }

            for (int i = 0; i < h; i += grid_sz)
            {
                Client.SpriteBatch.Draw(Client.Pixel, new Rectangle(0, i, w, thickness), color);
            }
        }

        public static void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(point1, distance, angle, color, thickness);
        }

        public static void DrawLine(Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            Client.SpriteBatch.Draw(Client.Pixel, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawLines()
        {
            for (int p = 0; p < IOController.Pins.Count; p++)
            {
                int w = IOController.BufferSize;
                int k = IOController.Pins[p].ValueNormals.Length - w;

                int x0 = 0;
                int y0 = GetY(IOController.Pins[p].ValueNormals[k] * IOController.Pins[p].Scale);
                for (int i = 1; i < w; i++)
                {
                    k++;
                    int x1 = (int)(i * (IOController.BufferSize - 1) / (w - 1));
                    int y1 = GetY(IOController.Pins[p].ValueNormals[k] * IOController.Pins[p].Scale);
                    DrawLine(new Vector2(x0, y0), new Vector2(x1, y1), IOController.Pins[p].Color, 1);
                    x0 = x1;
                    y0 = y1;
                }
            }
        }

        public static int GetY(float val)
        {
            return (int)(val * (float)Client.Form.Size.Y);
        }
    }
}
