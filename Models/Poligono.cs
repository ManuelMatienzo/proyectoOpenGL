using OpenTK.Mathematics;
using System.Collections.Generic;

namespace OpenTKComputerSetup.Models
{
    public class Poligono
    {
        public List<Punto3D> Vertices { get; set; }
        public Vector3 ColorPoligono { get; set; }

        public Poligono(Vector3 color)
        {
            Vertices = new List<Punto3D>();
            ColorPoligono = color;
        }

        public void AgregarVertice(Punto3D punto)
        {
            Vertices.Add(punto);
        }

        public void AgregarVertice(float x, float y, float z)
        {
            Vertices.Add(new Punto3D(x, y, z, ColorPoligono));
        }
    }
}