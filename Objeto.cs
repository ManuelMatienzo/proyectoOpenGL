using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

public class Objeto
{
    public List<Cara> Caras { get; }
    public Vector3 CentroDeMasa { get; private set; }

    public Objeto()
    {
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
        return cara;
    }

    public void RecalcularCentroDeMasa()
    {
        int count = 0;
        Vector3 acc = Vector3.Zero;

        foreach (var cara in Caras)
        {
            foreach (var p in cara.Vertices)
            {
                acc += p.Posicion;
                count++;
            }
        }

        CentroDeMasa = count > 0 ? acc / count : Vector3.Zero;
    }

    public int TotalVertices
    {
        get
        {
            int n = 0;
            foreach (var cara in Caras) n += cara.Vertices.Count;
            return n;
        }
    }
}
