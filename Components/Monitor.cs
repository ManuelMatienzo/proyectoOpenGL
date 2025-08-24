using OpenTK.Mathematics;
using OpenTKComputerSetup.Models;

namespace OpenTKComputerSetup.Components
{
    public class Monitor : Parte3D
    {
        public Monitor(Vector3 posicion, Vector3 escala) : base(posicion, escala) { }

        protected override void CrearGeometria()
        {
            Vector3 colorMarco = new Vector3(0.2f, 0.2f, 0.2f);
            Vector3 colorPantalla = new Vector3(0.1f, 0.1f, 0.1f);
            Vector3 colorBase = new Vector3(0.3f, 0.3f, 0.3f);
            Vector3 colorSoporte = new Vector3(0.25f, 0.25f, 0.25f);

            // PANTALLA FRONTAL
            var pantalla = new Poligono(colorPantalla);
            pantalla.AgregarVertice(-0.6f, 0.15f, 0.02f);
            pantalla.AgregarVertice(0.6f, 0.15f, 0.02f);
            pantalla.AgregarVertice(0.6f, 0.85f, 0.02f);
            pantalla.AgregarVertice(-0.6f, 0.85f, 0.02f);
            caras.Add(pantalla);

            // MARCO FRONTAL
            var marcoFrontal = new Poligono(colorMarco);
            marcoFrontal.AgregarVertice(-0.65f, 0.1f, 0.03f);
            marcoFrontal.AgregarVertice(0.65f, 0.1f, 0.03f);
            marcoFrontal.AgregarVertice(0.65f, 0.9f, 0.03f);
            marcoFrontal.AgregarVertice(-0.65f, 0.9f, 0.03f);
            caras.Add(marcoFrontal);

            // PARTE TRASERA
            var trasera = new Poligono(colorMarco);
            trasera.AgregarVertice(-0.65f, 0.1f, -0.05f);
            trasera.AgregarVertice(0.65f, 0.1f, -0.05f);
            trasera.AgregarVertice(0.65f, 0.9f, -0.05f);
            trasera.AgregarVertice(-0.65f, 0.9f, -0.05f);
            caras.Add(trasera);

            // BORDES LATERALES DEL MONITOR
            var bordeSuperior = new Poligono(colorMarco);
            bordeSuperior.AgregarVertice(-0.65f, 0.9f, -0.05f);
            bordeSuperior.AgregarVertice(0.65f, 0.9f, -0.05f);
            bordeSuperior.AgregarVertice(0.65f, 0.9f, 0.03f);
            bordeSuperior.AgregarVertice(-0.65f, 0.9f, 0.03f);
            caras.Add(bordeSuperior);

            var bordeInferior = new Poligono(colorMarco);
            bordeInferior.AgregarVertice(-0.65f, 0.1f, -0.05f);
            bordeInferior.AgregarVertice(0.65f, 0.1f, -0.05f);
            bordeInferior.AgregarVertice(0.65f, 0.1f, 0.03f);
            bordeInferior.AgregarVertice(-0.65f, 0.1f, 0.03f);
            caras.Add(bordeInferior);

            var bordeIzquierdo = new Poligono(colorMarco);
            bordeIzquierdo.AgregarVertice(-0.65f, 0.1f, -0.05f);
            bordeIzquierdo.AgregarVertice(-0.65f, 0.9f, -0.05f);
            bordeIzquierdo.AgregarVertice(-0.65f, 0.9f, 0.03f);
            bordeIzquierdo.AgregarVertice(-0.65f, 0.1f, 0.03f);
            caras.Add(bordeIzquierdo);

            var bordeDerecho = new Poligono(colorMarco);
            bordeDerecho.AgregarVertice(0.65f, 0.1f, -0.05f);
            bordeDerecho.AgregarVertice(0.65f, 0.9f, -0.05f);
            bordeDerecho.AgregarVertice(0.65f, 0.9f, 0.03f);
            bordeDerecho.AgregarVertice(0.65f, 0.1f, 0.03f);
            caras.Add(bordeDerecho);

            // BASE DEL MONITOR
            var baseMonitor = new Poligono(colorBase);
            baseMonitor.AgregarVertice(-0.3f, 0.0f, -0.2f);
            baseMonitor.AgregarVertice(0.3f, 0.0f, -0.2f);
            baseMonitor.AgregarVertice(0.3f, 0.03f, 0.05f);
            baseMonitor.AgregarVertice(-0.3f, 0.03f, 0.05f);
            caras.Add(baseMonitor);

            var baseInferior = new Poligono(colorBase);
            baseInferior.AgregarVertice(-0.3f, 0.0f, -0.2f);
            baseInferior.AgregarVertice(0.3f, 0.0f, -0.2f);
            baseInferior.AgregarVertice(0.3f, 0.0f, 0.05f);
            baseInferior.AgregarVertice(-0.3f, 0.0f, 0.05f);
            caras.Add(baseInferior);

            // Bordes de la base
            var baseFrontal = new Poligono(colorBase);
            baseFrontal.AgregarVertice(-0.3f, 0.0f, 0.05f);
            baseFrontal.AgregarVertice(0.3f, 0.0f, 0.05f);
            baseFrontal.AgregarVertice(0.3f, 0.03f, 0.05f);
            baseFrontal.AgregarVertice(-0.3f, 0.03f, 0.05f);
            caras.Add(baseFrontal);

            var baseTrasera = new Poligono(colorBase);
            baseTrasera.AgregarVertice(-0.3f, 0.0f, -0.2f);
            baseTrasera.AgregarVertice(0.3f, 0.0f, -0.2f);
            baseTrasera.AgregarVertice(0.3f, 0.03f, -0.2f);
            baseTrasera.AgregarVertice(-0.3f, 0.03f, -0.2f);
            caras.Add(baseTrasera);

            var baseIzquierda = new Poligono(colorBase);
            baseIzquierda.AgregarVertice(-0.3f, 0.0f, -0.2f);
            baseIzquierda.AgregarVertice(-0.3f, 0.0f, 0.05f);
            baseIzquierda.AgregarVertice(-0.3f, 0.03f, 0.05f);
            baseIzquierda.AgregarVertice(-0.3f, 0.03f, -0.2f);
            caras.Add(baseIzquierda);

            var baseDerecha = new Poligono(colorBase);
            baseDerecha.AgregarVertice(0.3f, 0.0f, -0.2f);
            baseDerecha.AgregarVertice(0.3f, 0.0f, 0.05f);
            baseDerecha.AgregarVertice(0.3f, 0.03f, 0.05f);
            baseDerecha.AgregarVertice(0.3f, 0.03f, -0.2f);
            caras.Add(baseDerecha);

            // SOPORTE VERTICAL DEL MONITOR
            var soporteVertical = new Poligono(colorSoporte);
            soporteVertical.AgregarVertice(-0.05f, 0.03f, -0.02f);
            soporteVertical.AgregarVertice(0.05f, 0.03f, -0.02f);
            soporteVertical.AgregarVertice(0.05f, 0.1f, -0.02f);
            soporteVertical.AgregarVertice(-0.05f, 0.1f, -0.02f);
            caras.Add(soporteVertical);

            var soporteVerticalTrasero = new Poligono(colorSoporte);
            soporteVerticalTrasero.AgregarVertice(-0.05f, 0.03f, -0.08f);
            soporteVerticalTrasero.AgregarVertice(0.05f, 0.03f, -0.08f);
            soporteVerticalTrasero.AgregarVertice(0.05f, 0.1f, -0.08f);
            soporteVerticalTrasero.AgregarVertice(-0.05f, 0.1f, -0.08f);
            caras.Add(soporteVerticalTrasero);

            var soporteIzquierdo = new Poligono(colorSoporte);
            soporteIzquierdo.AgregarVertice(-0.05f, 0.03f, -0.08f);
            soporteIzquierdo.AgregarVertice(-0.05f, 0.03f, -0.02f);
            soporteIzquierdo.AgregarVertice(-0.05f, 0.1f, -0.02f);
            soporteIzquierdo.AgregarVertice(-0.05f, 0.1f, -0.08f);
            caras.Add(soporteIzquierdo);

            var soporteDerecho = new Poligono(colorSoporte);
            soporteDerecho.AgregarVertice(0.05f, 0.03f, -0.08f);
            soporteDerecho.AgregarVertice(0.05f, 0.03f, -0.02f);
            soporteDerecho.AgregarVertice(0.05f, 0.1f, -0.02f);
            soporteDerecho.AgregarVertice(0.05f, 0.1f, -0.08f);
            caras.Add(soporteDerecho);

            var soporteSuperior = new Poligono(colorSoporte);
            soporteSuperior.AgregarVertice(-0.05f, 0.1f, -0.08f);
            soporteSuperior.AgregarVertice(0.05f, 0.1f, -0.08f);
            soporteSuperior.AgregarVertice(0.05f, 0.1f, -0.02f);
            soporteSuperior.AgregarVertice(-0.05f, 0.1f, -0.02f);
            caras.Add(soporteSuperior);
        }
    }
}