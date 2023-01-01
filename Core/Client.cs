using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.ImGui;
using System;

namespace ArduinoscopeClient;

public static class Client
{
    public static MainForm              Form;
    public static GraphicsDeviceManager GraphicsMgr;
    public static SpriteBatch           SpriteBatch;
    public static SpriteFont            Font;
    public static Texture2D             Pixel;
    public static ImGUIRenderer         UIRenderer;

    public static int FPS { get { return _fps; } }
    public static float Delta { get { return _delta; } }
    public static float DeltaTimed { get { return _delta_timed; } }

    private static float _delta, _delta_timed, _timer;
    private static int   _fps, _frames;

    public static void Main(string[] args)
    {
        Form = new MainForm();
        using (Form) { Form.Run(); }
    }

    public static void MeasurePerformance(GameTime gt)
    {
        _delta = (float)gt.ElapsedGameTime.TotalSeconds;

        _timer += _delta;
        if (_timer >= 1.0f)
        {
            _timer       = 0.0f;
            _fps         = _frames;
            _frames      = 0;
            _delta_timed = _delta;
        }
        _frames++;
    }
}
