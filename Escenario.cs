using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

/// <summary>
/// Escenario de la escena: contiene múltiples objetos lógicos (por ejemplo: monitor, teclado, CPU, etc.).
/// Provee utilidades para obtener estadísticas y aplicar transformaciones a nivel global.
/// </summary>
public class Escenario
{
    /// <summary>Lista de objetos contenidos en el escenario.</summary>
    public List<Objeto> Objetos { get; } = new();

    /// <summary>Centro de masa global (promedio simple de todos los puntos de todos los objetos).</summary>
    [JsonIgnore]
    public Vector3 CentroDeMasa { get; private set; } = Vector3.Zero;

    /// <summary>Agrega un objeto existente al escenario.</summary>
    public void AgregarObjeto(Objeto objeto)
    {
        if (objeto == null) throw new ArgumentNullException(nameof(objeto));
        Objetos.Add(objeto);
        RecalcularCentroDeMasa();
    }

    /// <summary>Crea un objeto vacío, lo agrega y lo retorna para encadenar construcción.</summary>
    public Objeto CrearObjeto()
    {
        var obj = new Objeto();
        Objetos.Add(obj);
        return obj;
    }

    /// <summary>Recalcula el centro de masa global del escenario.</summary>
    public void RecalcularCentroDeMasa()
    {
        int count = 0;
        Vector3 acc = Vector3.Zero;

        foreach (var obj in Objetos)
        {
            foreach (var parte in obj.Partes)
            {
                foreach (var cara in parte.Caras)
                {
                    foreach (var p in cara.Vertices)
                    {
                        acc += p.Posicion;
                        count++;
                    }
                }
            }
        }

        CentroDeMasa = count > 0 ? acc / count : Vector3.Zero;
    }

    [JsonIgnore] public int TotalObjetos => Objetos.Count;
    [JsonIgnore] public int TotalPartes => Objetos.Sum(o => o.TotalPartes);
    [JsonIgnore] public int TotalCaras => Objetos.Sum(o => o.TotalCaras);
    [JsonIgnore] public int TotalVertices => Objetos.Sum(o => o.TotalVertices);

    /// <summary>Enumeración de todas las partes del escenario.</summary>
    [JsonIgnore]
    public IEnumerable<Parte> Partes => Objetos.SelectMany(o => o.Partes);

    /// <summary>Enumeración de todas las caras del escenario.</summary>
    [JsonIgnore]
    public IEnumerable<Cara> Caras => Partes.SelectMany(p => p.Caras);

    /// <summary>Enumeración de todos los puntos del escenario.</summary>
    [JsonIgnore]
    public IEnumerable<Punto> Puntos => Caras.SelectMany(c => c.Vertices);

    /// <summary>
    /// Aplica una transformación general (matriz 4x4) a todos los puntos del escenario.
    /// </summary>
    public void Transformar(Matrix4 transformacion)
    {
        foreach (var obj in Objetos)
            obj.Transformar(transformacion);
        RecalcularCentroDeMasa();
    }

    /// <summary>Traslada todo el escenario en el espacio.</summary>
    public void Trasladar(Vector3 desplazamiento)
    {
        var matriz = Matrix4.CreateTranslation(desplazamiento);
        Transformar(matriz);
    }

    /// <summary>Rota todo el escenario alrededor de su centro de masa.</summary>
    public void Rotar(Vector3 eje, float anguloRadianes)
    {
        var rot = Matrix4.CreateFromAxisAngle(eje, anguloRadianes);
        var toOrigin = Matrix4.CreateTranslation(-CentroDeMasa);
        var back = Matrix4.CreateTranslation(CentroDeMasa);
        Transformar(toOrigin * rot * back);
    }

    /// <summary>Escala el escenario alrededor de su centro de masa.</summary>
    public void Escalar(Vector3 factorEscala)
    {
        var scale = Matrix4.CreateScale(factorEscala);
        var toOrigin = Matrix4.CreateTranslation(-CentroDeMasa);
        var back = Matrix4.CreateTranslation(CentroDeMasa);
        Transformar(toOrigin * scale * back);
    }

    public override string ToString()
        => $"Escenario: {TotalObjetos} objetos, {TotalPartes} partes, {TotalCaras} caras, {TotalVertices} vértices";
}
