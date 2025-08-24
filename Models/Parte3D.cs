using OpenTK.Mathematics;
using System.Collections.Generic;

namespace OpenTKComputerSetup.Models
{
    public abstract class Parte3D
    {
        protected List<Poligono> caras;
        protected Vector3 posicion;
        protected Vector3 escala;

        public Parte3D(Vector3 posicion, Vector3 escala)
        {
            this.posicion = posicion;
            this.escala = escala;
            caras = new List<Poligono>();
            CrearGeometria();
        }

        protected abstract void CrearGeometria();

        public List<float> ObtenerVertices()
        {
            var vertices = new List<float>();
            
            foreach (var cara in caras)
            {
                if (cara.Vertices.Count >= 3)
                {
                    for (int i = 1; i < cara.Vertices.Count - 1; i++)
                    {
                        AgregarVertice(vertices, cara.Vertices[0]);
                        AgregarVertice(vertices, cara.Vertices[i]);
                        AgregarVertice(vertices, cara.Vertices[i + 1]);
                    }
                }
            }

            return vertices;
        }

        public List<uint> ObtenerIndicesLineas()
        {
            var indices = new List<uint>();
            uint indiceBase = 0;

            foreach (var cara in caras)
            {
                if (cara.Vertices.Count == 4)
                {
                    indices.AddRange(new uint[] { 
                        indiceBase, indiceBase + 1,
                        indiceBase + 1, indiceBase + 2,
                        indiceBase + 2, indiceBase + 3,
                        indiceBase + 3, indiceBase
                    });
                }
                else if (cara.Vertices.Count == 3)
                {
                    indices.AddRange(new uint[] { 
                        indiceBase, indiceBase + 1,
                        indiceBase + 1, indiceBase + 2,
                        indiceBase + 2, indiceBase
                    });
                }
                indiceBase += (uint)cara.Vertices.Count;
            }

            return indices;
        }

        private void AgregarVertice(List<float> vertices, Punto3D punto)
        {
            var pos = punto.Posicion * escala + posicion;
            vertices.AddRange(new float[] { pos.X, pos.Y, pos.Z, punto.Color.X, punto.Color.Y, punto.Color.Z });
        }
    }
}