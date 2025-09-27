using System;
using System.IO;
using System.Text.Json;

public static class Deserializador
{
    public static Escenario? Cargar(string path)
    {
        if (!File.Exists(path)) { Console.WriteLine($"[Deserializador] Archivo no existe: {path}"); return null; }
        try
        {
            var json = File.ReadAllText(path);
            var esc = JsonSerializer.Deserialize<Escenario>(json, Serializador.Options);
            if (esc == null) { Console.WriteLine("[Deserializador] Escenario null tras deserialización"); return null; }
            // Recalcular centros de masa (no serializados)
            foreach (var obj in esc.Objetos)
            {
                foreach (var parte in obj.Partes)
                {
                    parte.RecalcularCentroDeMasa();
                }
                obj.RecalcularCentroDeMasa();
            }
            esc.RecalcularCentroDeMasa();
            Console.WriteLine($"[Deserializador] Escenario cargado desde '{path}' (formato directo)");
            return esc;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Deserializador] Error cargando '{path}': {ex.Message}");
            return null;
        }
    }
}
