using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

public sealed class Game : GameWindow
{
    int _vao, _vbo, _ebo, _program;
    int _indexCount;

    const string VertexSrc = @"#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
out vec3 vColor;
void main(){ vColor=aColor; gl_Position=vec4(aPos,1.0); }";

    const string FragmentSrc = @"#version 330 core
in vec3 vColor; out vec4 FragColor;
void main(){ FragColor=vec4(vColor,1.0); }";

    // Paleta line-art
    static readonly Vector3 Stroke = new(0f, 0f, 0f);
    static readonly Vector3 Fill   = new(0.98f, 0.98f, 0.98f);

    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        // Fondo blanco y sin depth-test para dibujar por orden (painter's algorithm).
        GL.ClearColor(1f, 1f, 1f, 1f);
        GL.Disable(EnableCap.DepthTest);

        var escena = CrearEscenaLineArt(); // monitor, teclado, cpu (todo en un Objeto)
        _program = CompilarYLinkear(VertexSrc, FragmentSrc);
        CrearBuffersDesdeObjeto(escena, out _vao, out _vbo, out _ebo, out _indexCount);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(_program);
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
        Context.SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        if (_ebo != 0) GL.DeleteBuffer(_ebo);
        if (_vbo != 0) GL.DeleteBuffer(_vbo);
        if (_vao != 0) GL.DeleteVertexArray(_vao);
        if (_program != 0) GL.DeleteProgram(_program);
    }

    // ===================== ESCENA LINE-ART =====================
    private static Objeto CrearEscenaLineArt()
    {
        var obj = new Objeto();

        // --- MONITOR ---
        {
            // Marco exterior
            AgregarRectConBorde(obj, centro: new(0f, 0.12f, 0f), w: 1.10f, h: 0.70f, grosor: 0.03f);

            // Pantalla interior (marco fino)
            AgregarRectConBorde(obj, centro: new(0f, 0.12f, 0.002f), w: 0.92f, h: 0.54f, grosor: 0.015f);

            // Cuello y base
            AgregarRectConBorde(obj, centro: new(0f, -0.25f, 0.001f), w: 0.12f, h: 0.12f, grosor: 0.015f);
            AgregarRectConBorde(obj, centro: new(0f, -0.34f, 0.001f), w: 0.42f, h: 0.12f, grosor: 0.02f);
        }

        // --- CPU (a la derecha) ---
        {
            AgregarRectConBorde(obj, centro: new(0.68f, 0.00f, 0f), w: 0.42f, h: 0.74f, grosor: 0.02f);
            // Ranura
            AgregarRectConBorde(obj, centro: new(0.68f, 0.26f, 0.002f), w: 0.28f, h: 0.07f, grosor: 0.015f);
            // Botones (dos círculos con aro)
            AgregarCirculoConBorde(obj, centro: new(0.78f, -0.08f, 0.0025f), radio: 0.03f, grosor: 0.012f);
            AgregarCirculoConBorde(obj, centro: new(0.78f, -0.18f, 0.0025f), radio: 0.025f, grosor: 0.010f);
        }

        // --- TECLADO (trapezoidal) ---
        {
            // Trapezoide base (ligera perspectiva)
            var topLeft  =  new Vector3(-0.60f, -0.72f, 0f);
            var topRight =  new Vector3(+0.60f, -0.72f, 0f);
            var botRight =  new Vector3(+0.72f, -0.90f, 0f);
            var botLeft  =  new Vector3(-0.72f, -0.90f, 0f);

            AgregarTrapezoideConBorde(obj, topLeft, topRight, botRight, botLeft, grosor: 0.02f);

            // Rejilla de teclas (líneas dentro del trapezoide)
            // 12 columnas y 4 filas, centradas
            AgregarGridEnTrapezoide(obj, topLeft, topRight, botRight, botLeft, cols: 12, rows: 4, grosor: 0.012f);

            // Barra espaciadora (recto dentro del trapezoide, centrado horizontal)
            var sY = 0.82f; // fracción de altura desde arriba (entre 0..1)
            var leftEdge  = Lerp(topLeft,  botLeft,  sY);
            var rightEdge = Lerp(topRight, botRight, sY);
            var center    = (leftEdge + rightEdge) * 0.5f;
            AgregarLinea(obj, center + (rightEdge - leftEdge) * (-0.20f), center + (rightEdge - leftEdge) * (0.20f), grosor: 0.02f);
        }

        obj.RecalcularCentroDeMasa(); // (no afecta al dibujo)
        return obj;
    }

    // ===================== HELPERS DE DIBUJO (line-art) =====================

    // Rectángulo relleno + borde negro
    private static void AgregarRectConBorde(Objeto obj, Vector3 centro, float w, float h, float grosor)
    {
        float hw = w * 0.5f, hh = h * 0.5f;

        // Relleno
        AgregarQuad(obj,
            new(centro.X - hw, centro.Y - hh, centro.Z),
            new(centro.X + hw, centro.Y - hh, centro.Z),
            new(centro.X + hw, centro.Y + hh, centro.Z),
            new(centro.X - hw, centro.Y + hh, centro.Z),
            Fill);

        // Bordes como 4 tiras
        float t = grosor;
        // Arriba
        AgregarQuad(obj,
            new(centro.X - hw, centro.Y + hh, centro.Z + 0.0005f),
            new(centro.X + hw, centro.Y + hh, centro.Z + 0.0005f),
            new(centro.X + hw, centro.Y + hh - t, centro.Z + 0.0005f),
            new(centro.X - hw, centro.Y + hh - t, centro.Z + 0.0005f),
            Stroke);
        // Abajo
        AgregarQuad(obj,
            new(centro.X - hw, centro.Y - hh + t, centro.Z + 0.0005f),
            new(centro.X + hw, centro.Y - hh + t, centro.Z + 0.0005f),
            new(centro.X + hw, centro.Y - hh, centro.Z + 0.0005f),
            new(centro.X - hw, centro.Y - hh, centro.Z + 0.0005f),
            Stroke);
        // Izquierda
        AgregarQuad(obj,
            new(centro.X - hw, centro.Y - hh, centro.Z + 0.0005f),
            new(centro.X - hw + t, centro.Y - hh, centro.Z + 0.0005f),
            new(centro.X - hw + t, centro.Y + hh, centro.Z + 0.0005f),
            new(centro.X - hw, centro.Y + hh, centro.Z + 0.0005f),
            Stroke);
        // Derecha
        AgregarQuad(obj,
            new(centro.X + hw - t, centro.Y - hh, centro.Z + 0.0005f),
            new(centro.X + hw, centro.Y - hh, centro.Z + 0.0005f),
            new(centro.X + hw, centro.Y + hh, centro.Z + 0.0005f),
            new(centro.X + hw - t, centro.Y + hh, centro.Z + 0.0005f),
            Stroke);
    }

    // Trapezoide relleno + borde negro
    private static void AgregarTrapezoideConBorde(Objeto obj, Vector3 tl, Vector3 tr, Vector3 br, Vector3 bl, float grosor)
    {
        // Relleno (dos triángulos)
        AgregarQuad(obj, tl, tr, br, bl, Fill);

        // Bordes (4 líneas como quads finitos)
        AgregarLinea(obj, tl, tr, grosor);
        AgregarLinea(obj, tr, br, grosor);
        AgregarLinea(obj, br, bl, grosor);
        AgregarLinea(obj, bl, tl, grosor);
    }

    // Línea gruesa como un quad orientado entre A y B
    private static void AgregarLinea(Objeto obj, Vector3 a, Vector3 b, float grosor)
    {
        var dir = (b - a);
        float len = dir.Length;
        if (len <= 1e-6f) return;
        dir /= len;

        // Perpendicular 2D (x,y) y grosor/2
        var perp = new Vector3(-dir.Y, dir.X, 0f) * (grosor * 0.5f);

        var p1 = a - perp;
        var p2 = a + perp;
        var p3 = b + perp;
        var p4 = b - perp;

        AgregarQuad(obj, p1, p2, p3, p4, Stroke);
    }

    // Grid (vertical y horizontal) dentro de un trapezoide
    private static void AgregarGridEnTrapezoide(Objeto obj, Vector3 tl, Vector3 tr, Vector3 br, Vector3 bl, int cols, int rows, float grosor)
    {
        // Verticales
        for (int c = 1; c < cols; c++)
        {
            float t = c / (float)cols;
            var top    = Lerp(tl, tr, t);
            var bottom = Lerp(bl, br, t);
            AgregarLinea(obj, top, bottom, grosor);
        }
        // Horizontales
        for (int r = 1; r < rows; r++)
        {
            float s = r / (float)rows;
            var left  = Lerp(tl, bl, s);
            var right = Lerp(tr, br, s);
            AgregarLinea(obj, left, right, grosor);
        }
    }

    // Círculo con borde: dibuja disco blanco y aro negro (dos discos concéntricos)
    private static void AgregarCirculoConBorde(Objeto obj, Vector3 centro, float radio, float grosor)
    {
        int seg = 48;
        // Disco exterior negro (borde)
        AgregarDisco(obj, centro, radio, Stroke, seg);
        // Disco interior blanco (relleno)
        AgregarDisco(obj, centro, radio - grosor, Fill, seg);
    }

    private static void AgregarDisco(Objeto obj, Vector3 centro, float radio, Vector3 color, int seg)
    {
        var cara = obj.AgregarCara(color);
        // Centro
        cara.AgregarVertice(centro);
        // Borde
        for (int i = 0; i <= seg; i++)
        {
            float a = (float)(i * Math.PI * 2.0 / seg);
            float x = centro.X + radio * MathF.Cos(a);
            float y = centro.Y + radio * MathF.Sin(a);
            cara.AgregarVertice(new Vector3(x, y, centro.Z));
        }
    }

    // Quad genérico (en la misma Z)
    private static void AgregarQuad(Objeto obj, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 color)
    {
        var cara = obj.AgregarCara(color);
        cara.AgregarVertice(a); cara.AgregarVertice(b); cara.AgregarVertice(c); cara.AgregarVertice(d);
    }

    private static Vector3 Lerp(Vector3 a, Vector3 b, float t) => a + (b - a) * t;

    // ===================== BUFFERS =====================
    private static void CrearBuffersDesdeObjeto(Objeto objeto, out int vao, out int vbo, out int ebo, out int indexCount)
    {
        var vertices = new List<float>();
        var indices  = new List<int>();
        int baseVertex = 0;

        foreach (var cara in objeto.Caras)
        {
            foreach (var p in cara.Vertices)
                vertices.AddRange(p.ToVertexData());

            for (int i = 1; i <= cara.Vertices.Count - 2; i++)
            {
                indices.Add(baseVertex + 0);
                indices.Add(baseVertex + i);
                indices.Add(baseVertex + i + 1);
            }
            baseVertex += cara.Vertices.Count;
        }

        vao = GL.GenVertexArray();
        vbo = GL.GenBuffer();
        ebo = GL.GenBuffer();

        GL.BindVertexArray(vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Punto.StrideBytes, 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Punto.StrideBytes, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
        indexCount = indices.Count;
    }

    // ===================== SHADERS =====================
    private static int CompilarYLinkear(string vsSrc, string fsSrc)
    {
        int vs = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vs, vsSrc);
        GL.CompileShader(vs);
        GL.GetShader(vs, ShaderParameter.CompileStatus, out int s1); if (s1 == 0) throw new Exception(GL.GetShaderInfoLog(vs));

        int fs = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fs, fsSrc);
        GL.CompileShader(fs);
        GL.GetShader(fs, ShaderParameter.CompileStatus, out int s2); if (s2 == 0) throw new Exception(GL.GetShaderInfoLog(fs));

        int prog = GL.CreateProgram();
        GL.AttachShader(prog, vs);
        GL.AttachShader(prog, fs);
        GL.LinkProgram(prog);
        GL.GetProgram(prog, GetProgramParameterName.LinkStatus, out int ok); if (ok == 0) throw new Exception(GL.GetProgramInfoLog(prog));

        GL.DeleteShader(vs); GL.DeleteShader(fs);
        return prog;
    }
}
