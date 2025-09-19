using System;
using OpenTK.Mathematics;

public static class EjemploSerializacion
{
    public static void EjecutarEjemplo()
    {
        // 1. Crear un objeto de ejemplo (un cubo simple)
        var cubo = CrearCuboEjemplo();

        // 2. Guardar el cubo en un archivo JSON
        string rutaArchivo = "cubo.json";
        Console.WriteLine($"Guardando cubo en {rutaArchivo}...");
        DataSerializer.GuardarObjeto(rutaArchivo, cubo);
        Console.WriteLine("Cubo guardado exitosamente!");

        // 3. Cargar el cubo desde el archivo JSON
        Console.WriteLine($"\nCargando cubo desde {rutaArchivo}...");
        var cuboCargado = DataSerializer.CargarObjeto(rutaArchivo);
        Console.WriteLine($"Cubo cargado exitosamente!");
        Console.WriteLine($"Número de caras: {cuboCargado.Caras.Count}");
        Console.WriteLine($"Total de vértices: {cuboCargado.TotalVertices}");
        Console.WriteLine($"Centro de masa: {cuboCargado.CentroDeMasa}");
    }

    private static Objeto CrearCuboEjemplo()
    {
        var cubo = new Objeto();
        
        // Colores para cada cara
        var colorFrontal = new Vector3(1.0f, 0.0f, 0.0f);    // Rojo
        var colorTrasera = new Vector3(0.0f, 1.0f, 0.0f);    // Verde
        var colorSuperior = new Vector3(0.0f, 0.0f, 1.0f);   // Azul
        var colorInferior = new Vector3(1.0f, 1.0f, 0.0f);   // Amarillo
        var colorDerecha = new Vector3(1.0f, 0.0f, 1.0f);    // Magenta
        var colorIzquierda = new Vector3(0.0f, 1.0f, 1.0f);  // Cian

        // Crear las caras del cubo
        // Cara frontal
        var caraFrontal = new Cara(colorFrontal);
        caraFrontal.AgregarVertice(-0.5f, -0.5f, 0.5f);
        caraFrontal.AgregarVertice(0.5f, -0.5f, 0.5f);
        caraFrontal.AgregarVertice(0.5f, 0.5f, 0.5f);
        caraFrontal.AgregarVertice(-0.5f, 0.5f, 0.5f);
        cubo.AgregarCara(caraFrontal);

        // Cara trasera
        var caraTrasera = new Cara(colorTrasera);
        caraTrasera.AgregarVertice(0.5f, -0.5f, -0.5f);
        caraTrasera.AgregarVertice(-0.5f, -0.5f, -0.5f);
        caraTrasera.AgregarVertice(-0.5f, 0.5f, -0.5f);
        caraTrasera.AgregarVertice(0.5f, 0.5f, -0.5f);
        cubo.AgregarCara(caraTrasera);

        // Cara superior
        var caraSuperior = new Cara(colorSuperior);
        caraSuperior.AgregarVertice(-0.5f, 0.5f, -0.5f);
        caraSuperior.AgregarVertice(-0.5f, 0.5f, 0.5f);
        caraSuperior.AgregarVertice(0.5f, 0.5f, 0.5f);
        caraSuperior.AgregarVertice(0.5f, 0.5f, -0.5f);
        cubo.AgregarCara(caraSuperior);

        // Cara inferior
        var caraInferior = new Cara(colorInferior);
        caraInferior.AgregarVertice(-0.5f, -0.5f, -0.5f);
        caraInferior.AgregarVertice(0.5f, -0.5f, -0.5f);
        caraInferior.AgregarVertice(0.5f, -0.5f, 0.5f);
        caraInferior.AgregarVertice(-0.5f, -0.5f, 0.5f);
        cubo.AgregarCara(caraInferior);

        // Cara derecha
        var caraDerecha = new Cara(colorDerecha);
        caraDerecha.AgregarVertice(0.5f, -0.5f, 0.5f);
        caraDerecha.AgregarVertice(0.5f, -0.5f, -0.5f);
        caraDerecha.AgregarVertice(0.5f, 0.5f, -0.5f);
        caraDerecha.AgregarVertice(0.5f, 0.5f, 0.5f);
        cubo.AgregarCara(caraDerecha);

        // Cara izquierda
        var caraIzquierda = new Cara(colorIzquierda);
        caraIzquierda.AgregarVertice(-0.5f, -0.5f, -0.5f);
        caraIzquierda.AgregarVertice(-0.5f, -0.5f, 0.5f);
        caraIzquierda.AgregarVertice(-0.5f, 0.5f, 0.5f);
        caraIzquierda.AgregarVertice(-0.5f, 0.5f, -0.5f);
        cubo.AgregarCara(caraIzquierda);

        return cubo;
    }
}
