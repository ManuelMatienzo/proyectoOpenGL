using OpenTK.Mathematics;
using OpenTKComputerSetup.Models;

namespace OpenTKComputerSetup.Components
{
    public class Teclado : Parte3D
    {
        public Teclado(Vector3 posicion, Vector3 escala) : base(posicion, escala) { }

        protected override void CrearGeometria()
        {
            Vector3 colorBase = new Vector3(0.2f, 0.2f, 0.2f);
            Vector3 colorTeclas = new Vector3(0.9f, 0.9f, 0.9f);

            // Base del teclado
            var basePoligono = new Poligono(colorBase);
            basePoligono.AgregarVertice(-0.5f, 0.0f, 0.0f);
            basePoligono.AgregarVertice(0.5f, 0.0f, 0.0f);
            basePoligono.AgregarVertice(0.5f, 0.03f, 0.3f);
            basePoligono.AgregarVertice(-0.5f, 0.03f, 0.3f);
            caras.Add(basePoligono);

            var inferior = new Poligono(colorBase);
            inferior.AgregarVertice(-0.5f, 0.0f, 0.0f);
            inferior.AgregarVertice(0.5f, 0.0f, 0.0f);
            inferior.AgregarVertice(0.5f, 0.0f, 0.3f);
            inferior.AgregarVertice(-0.5f, 0.0f, 0.3f);
            caras.Add(inferior);

            // Bordes del teclado
            var bordeFrontal = new Poligono(colorBase);
            bordeFrontal.AgregarVertice(-0.5f, 0.0f, 0.3f);
            bordeFrontal.AgregarVertice(0.5f, 0.0f, 0.3f);
            bordeFrontal.AgregarVertice(0.5f, 0.03f, 0.3f);
            bordeFrontal.AgregarVertice(-0.5f, 0.03f, 0.3f);
            caras.Add(bordeFrontal);

            var bordeTrasero = new Poligono(colorBase);
            bordeTrasero.AgregarVertice(-0.5f, 0.0f, 0.0f);
            bordeTrasero.AgregarVertice(0.5f, 0.0f, 0.0f);
            bordeTrasero.AgregarVertice(0.5f, 0.03f, 0.0f);
            bordeTrasero.AgregarVertice(-0.5f, 0.03f, 0.0f);
            caras.Add(bordeTrasero);

            var bordeIzquierdo = new Poligono(colorBase);
            bordeIzquierdo.AgregarVertice(-0.5f, 0.0f, 0.0f);
            bordeIzquierdo.AgregarVertice(-0.5f, 0.0f, 0.3f);
            bordeIzquierdo.AgregarVertice(-0.5f, 0.03f, 0.3f);
            bordeIzquierdo.AgregarVertice(-0.5f, 0.03f, 0.0f);
            caras.Add(bordeIzquierdo);

            var bordeDerecho = new Poligono(colorBase);
            bordeDerecho.AgregarVertice(0.5f, 0.0f, 0.0f);
            bordeDerecho.AgregarVertice(0.5f, 0.0f, 0.3f);
            bordeDerecho.AgregarVertice(0.5f, 0.03f, 0.3f);
            bordeDerecho.AgregarVertice(0.5f, 0.03f, 0.0f);
            caras.Add(bordeDerecho);

            // Teclas representativas con volumen
            for (int fila = 0; fila < 3; fila++)
            {
                for (int col = 0; col < 8; col++)
                {
                    float x = -0.35f + col * 0.1f;
                    float z = 0.05f + fila * 0.08f;
                    CrearTecla(x, z, 0.07f, 0.06f, colorTeclas);
                }
            }
        }

        private void CrearTecla(float x, float z, float ancho, float profundidad, Vector3 color)
        {
            var teclaSuperior = new Poligono(color);
            teclaSuperior.AgregarVertice(x - ancho/2, 0.05f, z);
            teclaSuperior.AgregarVertice(x + ancho/2, 0.05f, z);
            teclaSuperior.AgregarVertice(x + ancho/2, 0.05f, z + profundidad);
            teclaSuperior.AgregarVertice(x - ancho/2, 0.05f, z + profundidad);
            caras.Add(teclaSuperior);

            var colorBorde = new Vector3(color.X * 0.8f, color.Y * 0.8f, color.Z * 0.8f);
            
            var bordeFrontal = new Poligono(colorBorde);
            bordeFrontal.AgregarVertice(x - ancho/2, 0.03f, z + profundidad);
            bordeFrontal.AgregarVertice(x + ancho/2, 0.03f, z + profundidad);
            bordeFrontal.AgregarVertice(x + ancho/2, 0.05f, z + profundidad);
            bordeFrontal.AgregarVertice(x - ancho/2, 0.05f, z + profundidad);
            caras.Add(bordeFrontal);

            var bordeTrasero = new Poligono(colorBorde);
            bordeTrasero.AgregarVertice(x - ancho/2, 0.03f, z);
            bordeTrasero.AgregarVertice(x + ancho/2, 0.03f, z);
            bordeTrasero.AgregarVertice(x + ancho/2, 0.05f, z);
            bordeTrasero.AgregarVertice(x - ancho/2, 0.05f, z);
            caras.Add(bordeTrasero);

            var bordeIzquierdo = new Poligono(colorBorde);
            bordeIzquierdo.AgregarVertice(x - ancho/2, 0.03f, z);
            bordeIzquierdo.AgregarVertice(x - ancho/2, 0.03f, z + profundidad);
            bordeIzquierdo.AgregarVertice(x - ancho/2, 0.05f, z + profundidad);
            bordeIzquierdo.AgregarVertice(x - ancho/2, 0.05f, z);
            caras.Add(bordeIzquierdo);

            var bordeDerecho = new Poligono(colorBorde);
            bordeDerecho.AgregarVertice(x + ancho/2, 0.03f, z);
            bordeDerecho.AgregarVertice(x + ancho/2, 0.03f, z + profundidad);
            bordeDerecho.AgregarVertice(x + ancho/2, 0.05f, z + profundidad);
            bordeDerecho.AgregarVertice(x + ancho/2, 0.05f, z);
            caras.Add(bordeDerecho);
        }
    }
}