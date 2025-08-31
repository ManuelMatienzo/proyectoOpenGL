using System.Collections.Generic;
using OpenTK.Mathematics;

public class Cara
{
    public List<Punto> Vertices { get; }
    public Vector3 ColorCara { get; set; }

    public Cara(Vector3 color)
    {
        Vertices = new List<Punto>();
        ColorCara = color;
    }

    public void AgregarVertice(Punto punto)
    {
        Vertices.Add(punto);
    }

    public void AgregarVertice(float x, float y, float z)
    {
        Vertices.Add(new Punto(new Vector3(x, y, z), ColorCara));
    }

    public void AgregarVertice(Vector3 posicion)
        => Vertices.Add(new Punto(posicion, ColorCara));

    public int VertexCount => Vertices.Count;
}
