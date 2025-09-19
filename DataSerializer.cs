using System;
using System.Text.Json;
using System.IO;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

public static class DataSerializer
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters = { new Vector3Converter() }
    };

    public static void GuardarObjeto(string filePath, Objeto objeto)
    {
        var jsonString = JsonSerializer.Serialize(objeto, _options);
        File.WriteAllText(filePath, jsonString);
    }

    public static Objeto CargarObjeto(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("El archivo no existe.", filePath);

        var jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Objeto>(jsonString, _options);
    }
}

// Convertidor personalizado para Vector3 ya que es un tipo de OpenTK
public class Vector3Converter : JsonConverter<Vector3>
{
    public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        float x = 0, y = 0, z = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return new Vector3(x, y, z);

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string propertyName = reader.GetString();
            reader.Read();

            switch (propertyName.ToLower())
            {
                case "x":
                    x = reader.GetSingle();
                    break;
                case "y":
                    y = reader.GetSingle();
                    break;
                case "z":
                    z = reader.GetSingle();
                    break;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteNumber("Z", value.Z);
        writer.WriteEndObject();
    }
}
