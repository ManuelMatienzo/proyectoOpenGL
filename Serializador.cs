using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class Serializador
{
    static readonly JsonSerializerOptions _opts = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static void Guardar(string path, Escenario escenario)
    {
        if (escenario == null) throw new ArgumentNullException(nameof(escenario));
        var json = JsonSerializer.Serialize(escenario, _opts);
        File.WriteAllText(path, json);
        Console.WriteLine($"[Serializador] Escenario guardado en '{path}' (formato directo)");
    }

    internal static JsonSerializerOptions Options => _opts;
}
