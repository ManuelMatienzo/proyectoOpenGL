using OpenTK.Mathematics;
using OpenTKComputerSetup.Models;

namespace OpenTKComputerSetup.Components
{
    public class CPU : Parte3D
    {
        public CPU(Vector3 posicion, Vector3 escala) : base(posicion, escala) { }

        protected override void CrearGeometria()
        {
            Vector3 colorGabinete = new Vector3(0.3f, 0.3f, 0.3f);
            Vector3 colorPanel = new Vector3(0.4f, 0.4f, 0.4f);

            // Cuerpo principal del CPU
            var frente = new Poligono(colorPanel);
            frente.AgregarVertice(-0.2f, 0.0f, 0.6f);
            frente.AgregarVertice(0.2f, 0.0f, 0.6f);
            frente.AgregarVertice(0.2f, 0.75f, 0.6f);
            frente.AgregarVertice(-0.2f, 0.75f, 0.6f);
            caras.Add(frente);

            var atras = new Poligono(colorGabinete);
            atras.AgregarVertice(-0.2f, 0.0f, -0.2f);
            atras.AgregarVertice(0.2f, 0.0f, -0.2f);
            atras.AgregarVertice(0.2f, 0.75f, -0.2f);
            atras.AgregarVertice(-0.2f, 0.75f, -0.2f);
            caras.Add(atras);

            var lado1 = new Poligono(colorGabinete);
            lado1.AgregarVertice(-0.2f, 0.0f, -0.2f);
            lado1.AgregarVertice(-0.2f, 0.0f, 0.6f);
            lado1.AgregarVertice(-0.2f, 0.75f, 0.6f);
            lado1.AgregarVertice(-0.2f, 0.75f, -0.2f);
            caras.Add(lado1);

            var lado2 = new Poligono(colorGabinete);
            lado2.AgregarVertice(0.2f, 0.0f, -0.2f);
            lado2.AgregarVertice(0.2f, 0.0f, 0.6f);
            lado2.AgregarVertice(0.2f, 0.75f, 0.6f);
            lado2.AgregarVertice(0.2f, 0.75f, -0.2f);
            caras.Add(lado2);

            var superior = new Poligono(colorGabinete);
            superior.AgregarVertice(-0.2f, 0.75f, -0.2f);
            superior.AgregarVertice(0.2f, 0.75f, -0.2f);
            superior.AgregarVertice(0.2f, 0.75f, 0.6f);
            superior.AgregarVertice(-0.2f, 0.75f, 0.6f);
            caras.Add(superior);

            var inferior = new Poligono(colorGabinete);
            inferior.AgregarVertice(-0.2f, 0.0f, -0.2f);
            inferior.AgregarVertice(0.2f, 0.0f, -0.2f);
            inferior.AgregarVertice(0.2f, 0.0f, 0.6f);
            inferior.AgregarVertice(-0.2f, 0.0f, 0.6f);
            caras.Add(inferior);

            // Botón de encendido
            var boton = new Poligono(new Vector3(0.8f, 0.8f, 0.8f));
            boton.AgregarVertice(-0.05f, 0.7f, 0.61f);
            boton.AgregarVertice(0.05f, 0.7f, 0.61f);
            boton.AgregarVertice(0.05f, 0.65f, 0.61f);
            boton.AgregarVertice(-0.05f, 0.65f, 0.61f);
            caras.Add(boton);

            // Rejillas de ventilación
            for (int i = 0; i < 5; i++)
            {
                var rejilla = new Poligono(new Vector3(0.2f, 0.2f, 0.2f));
                float y = 0.3f + i * 0.08f;
                rejilla.AgregarVertice(-0.15f, y, 0.61f);
                rejilla.AgregarVertice(0.15f, y, 0.61f);
                rejilla.AgregarVertice(0.15f, y + 0.03f, 0.61f);
                rejilla.AgregarVertice(-0.15f, y + 0.03f, 0.61f);
                caras.Add(rejilla);
            }
        }
    }
}