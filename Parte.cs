using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

public class Parte
{
    public string Nombre { get; set; }
    public List<Cara> Caras { get; }
    [JsonIgnore]
    public Vector3 CentroDeMasa { get; private set; }

    public Parte(string nombre)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Caras = new List<Cara>();
        CentroDeMasa = Vector3.Zero;
    }

    public void AgregarCara(Cara cara)
    {
        if (cara == null) throw new ArgumentNullException(nameof(cara));
        Caras.Add(cara);
        RecalcularCentroDeMasa();
    }

    public Cara AgregarCara(Vector3 colorCara)
    {
        var cara = new Cara(colorCara);
        Caras.Add(cara);
        RecalcularCentroDeMasa();
        return cara;
    }

    public void RecalcularCentroDeMasa()
    {
        int count = 0;
        Vector3 acc = Vector3.Zero;

        foreach (var cara in Caras)
        {
            foreach (var punto in cara.Vertices)
            {
                acc += punto.Posicion;
                count++;
            }
        }

        CentroDeMasa = count > 0 ? acc / count : Vector3.Zero;
    }

    [JsonIgnore]
    public int TotalVertices
    {
        get
        {
            int total = 0;
            foreach (var cara in Caras)
                total += cara.Vertices.Count;
            return total;
        }
    }

    [JsonIgnore]
    public int TotalCaras => Caras.Count;

    /// <summary>
    /// Aplica una transformación (traslación, rotación, escala) a todos los puntos de esta parte
    /// </summary>
    public void Transformar(Matrix4 transformacion)
    {
        foreach (var cara in Caras)
        {
            foreach (var punto in cara.Vertices)
            {
                Vector4 pos4 = new Vector4(punto.Posicion, 1.0f);
                Vector4 nuevaPos = pos4 * transformacion;
                punto.Posicion = nuevaPos.Xyz;
            }
        }
        RecalcularCentroDeMasa();
    }

    /// <summary>
    /// Traslada la parte a una nueva posición
    /// </summary>
    public void Trasladar(Vector3 desplazamiento)
    {
        foreach (var cara in Caras)
        {
            foreach (var punto in cara.Vertices)
            {
                punto.Posicion += desplazamiento;
            }
        }
        RecalcularCentroDeMasa();
    }

    /// <summary>
    /// Rota la parte alrededor de su centro de masa
    /// </summary>
    public void Rotar(Vector3 eje, float anguloRadianes)
    {
        var rotacion = Matrix4.CreateFromAxisAngle(eje, anguloRadianes);
        var traslacionAlOrigen = Matrix4.CreateTranslation(-CentroDeMasa);
        var traslacionDeVuelta = Matrix4.CreateTranslation(CentroDeMasa);
        
        var transformacionCompleta = traslacionAlOrigen * rotacion * traslacionDeVuelta;
        Transformar(transformacionCompleta);
    }

    /// <summary>
    /// Escala la parte desde su centro de masa
    /// </summary>
    public void Escalar(Vector3 factorEscala)
    {
        var escala = Matrix4.CreateScale(factorEscala);
        var traslacionAlOrigen = Matrix4.CreateTranslation(-CentroDeMasa);
        var traslacionDeVuelta = Matrix4.CreateTranslation(CentroDeMasa);
        
        var transformacionCompleta = traslacionAlOrigen * escala * traslacionDeVuelta;
        Transformar(transformacionCompleta);
    }

    public override string ToString()
        => $"Parte '{Nombre}': {TotalCaras} caras, {TotalVertices} vértices, centro en {CentroDeMasa}";
}