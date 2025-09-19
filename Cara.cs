using System.Collections.Generic;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

public class Cara
{
    [JsonInclude]
    public List<Punto> Vertices { get; private set; }
    
    [JsonInclude]
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

    // Traslación: Mueve la cara en una dirección específica
    public void Trasladar(float dx, float dy, float dz)
    {
        var translacion = Matrix4.CreateTranslation(dx, dy, dz);
        AplicarTransformacion(translacion);
    }

    // Rotación: Rota la cara alrededor de un eje
    public void Rotar(float anguloGrados, Vector3 eje)
    {
        var rotacion = Matrix4.CreateFromAxisAngle(eje, MathHelper.DegreesToRadians(anguloGrados));
        AplicarTransformacion(rotacion);
    }

    // Rotaciones específicas alrededor de los ejes principales
    public void RotarX(float anguloGrados)
    {
        var rotacion = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(anguloGrados));
        AplicarTransformacion(rotacion);
    }

    public void RotarY(float anguloGrados)
    {
        var rotacion = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(anguloGrados));
        AplicarTransformacion(rotacion);
    }

    public void RotarZ(float anguloGrados)
    {
        var rotacion = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(anguloGrados));
        AplicarTransformacion(rotacion);
    }

    // Escalación: Cambia el tamaño de la cara
    public void Escalar(float sx, float sy, float sz)
    {
        var escalacion = Matrix4.CreateScale(sx, sy, sz);
        AplicarTransformacion(escalacion);
    }

    public void EscalarUniforme(float factor)
    {
        Escalar(factor, factor, factor);
    }

    // Reflexión: Refleja la cara sobre un plano (X, Y, o Z)
    public void ReflejarX()
    {
        var reflexion = Matrix4.CreateScale(-1, 1, 1);
        AplicarTransformacion(reflexion);
    }

    public void ReflejarY()
    {
        var reflexion = Matrix4.CreateScale(1, -1, 1);
        AplicarTransformacion(reflexion);
    }

    public void ReflejarZ()
    {
        var reflexion = Matrix4.CreateScale(1, 1, -1);
        AplicarTransformacion(reflexion);
    }

    // Método privado para aplicar cualquier transformación a todos los vértices
    private void AplicarTransformacion(Matrix4 matriz)
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            var punto = Vertices[i];
            var posicion = new Vector4(punto.Posicion.X, punto.Posicion.Y, punto.Posicion.Z, 1.0f);
            var nuevaPosicion = matriz * posicion;
            punto.Posicion = new Vector3(nuevaPosicion.X, nuevaPosicion.Y, nuevaPosicion.Z);
        }
    }
}
