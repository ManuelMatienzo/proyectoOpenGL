using OpenTK.Mathematics;
using OpenTKComputerSetup.Models;

namespace OpenTKComputerSetup.Components
{
    public class Mesa : Parte3D
    {
        public Mesa(Vector3 posicion, Vector3 escala) : base(posicion, escala) { }

        protected override void CrearGeometria()
        {
            Vector3 colorMesa = new Vector3(0.6f, 0.4f, 0.2f);
            
            // Superficie superior de la mesa
            var superficie = new Poligono(colorMesa);
            superficie.AgregarVertice(-2.0f, 0.0f, -1.2f);
            superficie.AgregarVertice(2.0f, 0.0f, -1.2f);
            superficie.AgregarVertice(2.0f, 0.0f, 1.2f);
            superficie.AgregarVertice(-2.0f, 0.0f, 1.2f);
            caras.Add(superficie);

            // Superficie inferior
            var inferior = new Poligono(colorMesa);
            inferior.AgregarVertice(-2.0f, -0.1f, -1.2f);
            inferior.AgregarVertice(2.0f, -0.1f, -1.2f);
            inferior.AgregarVertice(2.0f, -0.1f, 1.2f);
            inferior.AgregarVertice(-2.0f, -0.1f, 1.2f);
            caras.Add(inferior);

            // Bordes laterales para dar profundidad
            var bordeFrontal = new Poligono(colorMesa);
            bordeFrontal.AgregarVertice(-2.0f, -0.1f, 1.2f);
            bordeFrontal.AgregarVertice(2.0f, -0.1f, 1.2f);
            bordeFrontal.AgregarVertice(2.0f, 0.0f, 1.2f);
            bordeFrontal.AgregarVertice(-2.0f, 0.0f, 1.2f);
            caras.Add(bordeFrontal);

            var bordeTrasero = new Poligono(colorMesa);
            bordeTrasero.AgregarVertice(-2.0f, -0.1f, -1.2f);
            bordeTrasero.AgregarVertice(2.0f, -0.1f, -1.2f);
            bordeTrasero.AgregarVertice(2.0f, 0.0f, -1.2f);
            bordeTrasero.AgregarVertice(-2.0f, 0.0f, -1.2f);
            caras.Add(bordeTrasero);

            var bordeIzquierdo = new Poligono(colorMesa);
            bordeIzquierdo.AgregarVertice(-2.0f, -0.1f, -1.2f);
            bordeIzquierdo.AgregarVertice(-2.0f, -0.1f, 1.2f);
            bordeIzquierdo.AgregarVertice(-2.0f, 0.0f, 1.2f);
            bordeIzquierdo.AgregarVertice(-2.0f, 0.0f, -1.2f);
            caras.Add(bordeIzquierdo);

            var bordeDerecho = new Poligono(colorMesa);
            bordeDerecho.AgregarVertice(2.0f, -0.1f, -1.2f);
            bordeDerecho.AgregarVertice(2.0f, -0.1f, 1.2f);
            bordeDerecho.AgregarVertice(2.0f, 0.0f, 1.2f);
            bordeDerecho.AgregarVertice(2.0f, 0.0f, -1.2f);
            caras.Add(bordeDerecho);
        }
    }
}