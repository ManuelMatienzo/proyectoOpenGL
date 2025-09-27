using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

public class Objeto
{
    public List<Parte> Partes { get; } = new();
    [JsonIgnore]
    public Vector3 CentroDeMasa { get; private set; }

    public Objeto() { CentroDeMasa = Vector3.Zero; }

    public void AgregarParte(Parte parte)
    {
        if (parte == null) throw new ArgumentNullException(nameof(parte));
        Partes.Add(parte);
        RecalcularCentroDeMasa();
    }

    public Parte AgregarParte(string nombre)
    {
        var parte = new Parte(nombre);
        Partes.Add(parte);
        RecalcularCentroDeMasa();
        return parte;
    }

    public void RecalcularCentroDeMasa()
    {
        int count = 0;
        Vector3 acc = Vector3.Zero;

        foreach (var parte in Partes)
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

        CentroDeMasa = count > 0 ? acc / count : Vector3.Zero;
    }

    [JsonIgnore]
    public int TotalVertices
    {
        get
        {
            int total = 0;
            foreach (var parte in Partes)
                total += parte.TotalVertices;
            return total;
        }
    }

    [JsonIgnore]
    public int TotalCaras
    {
        get
        {
            int total = 0;
            foreach (var parte in Partes)
                total += parte.TotalCaras;
            return total;
        }
    }

    [JsonIgnore]
    public int TotalPartes => Partes.Count;

    /// <summary>
    /// Obtiene todas las caras de todas las partes (para compatibilidad con código existente)
    /// </summary>
    [JsonIgnore]
    public IEnumerable<Cara> Caras
    {
        get
        {
            foreach (var parte in Partes)
            {
                foreach (var cara in parte.Caras)
                {
                    yield return cara;
                }
            }
        }
    }

    /// <summary>
    /// Métodos de compatibilidad con el código existente
    /// Estos métodos agregan caras a una parte por defecto llamada "Principal"
    /// </summary>
    public void AgregarCara(Cara cara)
    {
        var parteDefecto = ObtenerOCrearParteDefecto();
        parteDefecto.AgregarCara(cara);
        RecalcularCentroDeMasa();
    }

    public Cara AgregarCara(Vector3 colorCara)
    {
        var parteDefecto = ObtenerOCrearParteDefecto();
        var cara = parteDefecto.AgregarCara(colorCara);
        RecalcularCentroDeMasa();
        return cara;
    }

    private Parte ObtenerOCrearParteDefecto()
    {
        var parteDefecto = Partes.FirstOrDefault(p => p.Nombre == "Principal");
        if (parteDefecto == null)
        {
            parteDefecto = new Parte("Principal");
            Partes.Add(parteDefecto);
        }
        return parteDefecto;
    }

    /// <summary>
    /// Aplica una transformación a todo el objeto
    /// </summary>
    public void Transformar(Matrix4 transformacion)
    {
        foreach (var parte in Partes)
        {
            parte.Transformar(transformacion);
        }
        RecalcularCentroDeMasa();
    }

    /// <summary>
    /// Traslada el objeto completo (todas sus partes) en el espacio.
    /// </summary>
    public void Trasladar(Vector3 desplazamiento)
    {
        var m = Matrix4.CreateTranslation(desplazamiento);
        Transformar(m);
    }

    /// <summary>
    /// Rota el objeto alrededor de su centro de masa actual.
    /// </summary>
    public void Rotar(Vector3 eje, float anguloRadianes)
    {
        var toOrigin = Matrix4.CreateTranslation(-CentroDeMasa);
        var rot = Matrix4.CreateFromAxisAngle(eje, anguloRadianes);
        var back = Matrix4.CreateTranslation(CentroDeMasa);
        Transformar(toOrigin * rot * back);
    }

    /// <summary>
    /// Escala el objeto alrededor de su centro de masa actual.
    /// </summary>
    public void Escalar(Vector3 factorEscala)
    {
        var toOrigin = Matrix4.CreateTranslation(-CentroDeMasa);
        var scale = Matrix4.CreateScale(factorEscala);
        var back = Matrix4.CreateTranslation(CentroDeMasa);
        Transformar(toOrigin * scale * back);
    }

    /// <summary>
    /// Busca una parte por nombre
    /// </summary>
    public Parte? BuscarParte(string nombre)
    {
        return Partes.FirstOrDefault(p => string.Equals(p.Nombre, nombre, StringComparison.OrdinalIgnoreCase));
    }

    public override string ToString()
        => $"Objeto: {TotalPartes} partes, {TotalCaras} caras, {TotalVertices} vértices";
}
