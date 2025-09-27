using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

class Program
{
    const string ArchivoEscenario = "escenario.json";

    static void Main()
    {
        var gws = GameWindowSettings.Default;
        var nws = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Escenario Interactive",
            APIVersion = new Version(3, 3),
        };

        // Intentar cargar, si no existe: crear escenario vacío (la lógica de seed genérico está en Game)
        Escenario esc = System.IO.File.Exists(ArchivoEscenario)
            ? (Deserializador.Cargar(ArchivoEscenario) ?? new Escenario())
            : new Escenario();

        using var game = new Game(gws, nws, esc);
        game.Run();
    }
}
