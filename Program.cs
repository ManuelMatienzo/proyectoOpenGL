using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--test-serialization")
        {
            EjemploSerializacion.EjecutarEjemplo();
            return;
        }
        var gws = GameWindowSettings.Default;
        var nws = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Setup",
            APIVersion = new Version(3, 3),
        };
        using var game = new Game(gws, nws);
        game.Run();
    }
}
