using OpenTK.Mathematics;

public class Punto
{
    public Vector3 Posicion { get; set; }

    public Vector3 Color { get; set; }

    public Vector3 CentroDeMasa => Posicion;

    public Punto(float x, float y, float z, Vector3 color)
    {
        Posicion = new Vector3(x, y, z);
        Color = Clamp01(color);
    }

    public Punto(Vector3 posicion, Vector3 color)
    {
        Posicion = posicion;
        Color = Clamp01(color);
    }

    public Punto(float x, float y, float z) : this(new Vector3(x, y, z), new Vector3(1f, 1f, 1f)) { }
    public Punto(Vector3 posicion)          : this(posicion, new Vector3(1f, 1f, 1f)) { }

    public const int VertexFloatCount = 6;                  
    public const int StrideBytes      = VertexFloatCount * sizeof(float);

    public float[] ToVertexData() => new[]
    {
        Posicion.X, Posicion.Y, Posicion.Z,
        Color.X,    Color.Y,    Color.Z
    };

    public static float[] Empaquetar(IEnumerable<Punto> puntos)
    {
        var list = new List<float>();
        foreach (var p in puntos)
            list.AddRange(p.ToVertexData());
        return list.ToArray();
    }

    private static Vector3 Clamp01(Vector3 c) => new(
        MathHelper.Clamp(c.X, 0f, 1f),
        MathHelper.Clamp(c.Y, 0f, 1f),
        MathHelper.Clamp(c.Z, 0f, 1f)
    );

    public override string ToString()
        => $"Punto({Posicion.X:0.###},{Posicion.Y:0.###},{Posicion.Z:0.###}; color={Color})";
}
