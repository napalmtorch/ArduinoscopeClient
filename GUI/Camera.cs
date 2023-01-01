using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArduinoscopeClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ArduinoscopeClient
{
    public class Camera
    {
        public Vector2 Position;
        public Matrix Transform;
        public float Zoom;
        public float Smoothness;
        public float Rotation;
        public float Angle;

        public Camera()
        {
            Position = Vector2.Zero;
            Transform = new Matrix();
            Zoom = 1.0f;
            Smoothness = 4.0f;
            Angle = 1.0f;
            Rotation = 0.0f;
        }

        public void Update(Vector2 follow)
        {

            float d = Smoothness * Client.Delta;
            Position.X += (follow.X - Position.X) * d;
            Position.Y += (follow.Y - Position.Y) * d;

            Transform = Matrix.CreateTranslation(new Vector3(-1 * Position.X, -Position.Y, 0)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(Zoom, Zoom, 1) *
                        Matrix.CreateTranslation(new Vector3(Client.Form.Size.X / 2, Client.Form.Size.Y / 2, 0));
        }
    }
}
