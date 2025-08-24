using OpenTK.Mathematics;

namespace OpenTKComputerSetup.Models
{
    public class Punto3D
    {
        public Vector3 Posicion { get; set; }
        public Vector3 Color { get; set; }

        public Punto3D(float x, float y, float z, Vector3 color)
        {
            Posicion = new Vector3(x, y, z);
            Color = color;
        }

        public Punto3D(Vector3 posicion, Vector3 color)
        {
            Posicion = posicion;
            Color = color;
        }
    }
}