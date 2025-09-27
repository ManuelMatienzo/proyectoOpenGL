using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public sealed class Game : GameWindow
{
    int _vao, _vbo, _ebo, _program;
    int _indexCount;
    int _vertexCount;
    long _frameCount;

    private readonly Escenario _escenario;
    private int _objSeleccionado = 0;
    private bool _modoParte = false;
    private int _parteSeleccionada = 0;
    private bool _dirty = true;
    private double _time;

    private Matrix4 _view;
    private Matrix4 _projection;
    private int _uMvpLocation;

    const string VertexSrc = "#version 330\nlayout (location = 0) in vec3 aPos;\nlayout (location = 1) in vec3 aColor;\nuniform mat4 uMVP;\nout vec3 vColor;\nvoid main(){ vColor = aColor; gl_Position = uMVP * vec4(aPos,1.0); }";

    const string FragmentSrc = "#version 330\nin vec3 vColor; out vec4 FragColor;\nvoid main(){ FragColor = vec4(vColor,1.0); }";

    // (Paleta removida: no usada en la versión limpia)

    // Constructor original (por compatibilidad) crea un escenario simple
    public Game(GameWindowSettings gws, NativeWindowSettings nws)
        : this(gws, nws, new Escenario()) { }

    // Nuevo constructor recibiendo un escenario externo
    public Game(GameWindowSettings gws, NativeWindowSettings nws, Escenario escenario) : base(gws, nws)
    {
        _escenario = escenario ?? throw new ArgumentNullException(nameof(escenario));
        // Generar seed si el escenario está vacío (aunque exista un JSON vacío)
        if (_escenario.Objetos.Count == 0)
        {
            Console.WriteLine("[Seed] Escenario vacío: generando seed (monitor/cpu/teclado)...");
            GenerarSeedGenerica();
            Console.WriteLine($"[Seed] Generado: objetos={_escenario.Objetos.Count}");
            try { Serializador.Guardar(ArchivoEscenario, _escenario); } catch (Exception ex) { Console.WriteLine($"[Seed] Error guardando seed: {ex.Message}"); }
        }
        else
        {
            Console.WriteLine($"[Seed] Escenario cargado con {_escenario.Objetos.Count} objetos. No se regenera seed.");
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.30f, 0.32f, 0.35f, 1f);
        GL.Enable(EnableCap.DepthTest);
        _program = CompilarYLinkear(VertexSrc, FragmentSrc);
        _uMvpLocation = GL.GetUniformLocation(_program, "uMVP");
        _view = Matrix4.LookAt(new Vector3(2.2f, 1.2f, 3.0f), Vector3.Zero, Vector3.UnitY);
        ActualizarProyeccion();
        ReconstruirBuffers();
        ImprimirInstrucciones();
    }
    // Depuración avanzada eliminada para versión de producción

    private void ImprimirInstrucciones()
    {
        Console.WriteLine("=== Controles ===");
        Console.WriteLine("ARCHIVO  : Ctrl+S Guardar  Ctrl+L Cargar  Ctrl+N Nuevo  ESC Salir");
        Console.WriteLine("ESCENARIO: R RotarY  T MoverX-  Y MoverX+  O Escalar+  P Escalar-");
        Console.WriteLine("OBJETO   : F1/F2 Obj prev/next  H Izq  L Der  J Abajo  K Arriba  G RotarZ  U Esc+  I Esc-");
        Console.WriteLine("PARTE    : Tab Modo Obj/Parte  PageUp/PageDown Cambiar parte (en modo parte)");
        Console.WriteLine();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        _time += args.Time;
        var kb = KeyboardState;
        if (kb.IsKeyDown(Keys.Escape)) Close();

        if (kb.IsKeyPressed(Keys.F4)) { /* (opcional) puntos, actualmente ignorado */ }

        bool transformacionCPU = false;

        // Cámara fija
        _view = Matrix4.LookAt(new Vector3(2.2f, 1.2f, 3.0f), Vector3.Zero, Vector3.UnitY);

        // Shortcuts CRUD (con Control)
        if (kb.IsKeyDown(Keys.LeftControl) || kb.IsKeyDown(Keys.RightControl))
        {
            if (kb.IsKeyPressed(Keys.N)) NuevoEscenario();
            if (kb.IsKeyPressed(Keys.S)) GuardarEscenario();
            if (kb.IsKeyPressed(Keys.L)) CargarEscenario();
            // Creación deshabilitada en runtime (Ctrl+O / Ctrl+P / Ctrl+C)
        }

        // Selección objeto
        if (kb.IsKeyPressed(Keys.F1)) { _objSeleccionado = (_objSeleccionado - 1 + _escenario.Objetos.Count) % _escenario.Objetos.Count; Console.WriteLine($"Objeto seleccionado: {_objSeleccionado}"); _dirty = true; AjustarParteSiFueraDeRango(); }
        if (kb.IsKeyPressed(Keys.F2)) { _objSeleccionado = (_objSeleccionado + 1) % _escenario.Objetos.Count; Console.WriteLine($"Objeto seleccionado: {_objSeleccionado}"); _dirty = true; AjustarParteSiFueraDeRango(); }

        // Toggle modo parte
        if (kb.IsKeyPressed(Keys.Tab))
        {
            _modoParte = !_modoParte;
            if (_modoParte)
            {
                if (_escenario.Objetos.Count == 0 || _escenario.Objetos[_objSeleccionado].Partes.Count == 0)
                {
                    Console.WriteLine("[ModoParte] Sin partes en el objeto actual; permaneciendo en modo Objeto");
                    _modoParte = false;
                }
                else
                {
                    _parteSeleccionada = Math.Clamp(_parteSeleccionada, 0, _escenario.Objetos[_objSeleccionado].Partes.Count - 1);
                    Console.WriteLine($"[ModoParte] ACTIVADO. Parte {_parteSeleccionada} / {_escenario.Objetos[_objSeleccionado].Partes.Count} ({_escenario.Objetos[_objSeleccionado].Partes[_parteSeleccionada].Nombre})");
                }
            }
            else
            {
                Console.WriteLine("[ModoParte] Desactivado (modo Objeto)");
            }
            _dirty = true;
        }

        // Navegación de partes
        if (_modoParte && _escenario.Objetos.Count > 0)
        {
            var partes = _escenario.Objetos[_objSeleccionado].Partes;
            if (partes.Count > 0)
            {
                if (kb.IsKeyPressed(Keys.PageUp)) { _parteSeleccionada = (_parteSeleccionada - 1 + partes.Count) % partes.Count; Console.WriteLine($"[Parte] Seleccionada {_parteSeleccionada} ({partes[_parteSeleccionada].Nombre})"); _dirty = true; }
                if (kb.IsKeyPressed(Keys.PageDown)) { _parteSeleccionada = (_parteSeleccionada + 1) % partes.Count; Console.WriteLine($"[Parte] Seleccionada {_parteSeleccionada} ({partes[_parteSeleccionada].Nombre})"); _dirty = true; }
            }
        }

        // Transformaciones CPU (objeto o parte)
        if (_escenario.Objetos.Count > 0)
        {
            var obj = _escenario.Objetos[_objSeleccionado];
            Parte? parteActual = null;
            if (_modoParte && obj.Partes.Count > 0)
            {
                _parteSeleccionada = Math.Clamp(_parteSeleccionada, 0, obj.Partes.Count - 1);
                parteActual = obj.Partes[_parteSeleccionada];
            }
            Vector3 delta = Vector3.Zero;
            float mov = 0.25f * (float)args.Time;
            if (kb.IsKeyDown(Keys.H)) delta.X -= mov;
            if (kb.IsKeyDown(Keys.L)) delta.X += mov;
            if (kb.IsKeyDown(Keys.J)) delta.Y -= mov;
            if (kb.IsKeyDown(Keys.K)) delta.Y += mov;
            if (delta != Vector3.Zero)
            {
                if (parteActual != null) parteActual.Trasladar(delta); else obj.Trasladar(delta);
                transformacionCPU = true;
            }
            if (kb.IsKeyDown(Keys.G))
            {
                if (parteActual != null) parteActual.Rotar(Vector3.UnitZ, 1.0f * (float)args.Time); else obj.Rotar(Vector3.UnitZ, 1.0f * (float)args.Time);
                transformacionCPU = true;
            }
            if (kb.IsKeyDown(Keys.U))
            {
                var esc = new Vector3(1f + 0.5f * (float)args.Time);
                if (parteActual != null) parteActual.Escalar(esc); else obj.Escalar(esc);
                transformacionCPU = true;
            }
            if (kb.IsKeyDown(Keys.I))
            {
                var esc = new Vector3(1f - 0.5f * (float)args.Time);
                if (parteActual != null) parteActual.Escalar(esc); else obj.Escalar(esc);
                transformacionCPU = true;
            }
        }

        // Transformaciones CPU globales
        if (kb.IsKeyDown(Keys.R)) { _escenario.Rotar(Vector3.UnitY, 0.5f * (float)args.Time); transformacionCPU = true; }
        Vector3 deltaEsc = Vector3.Zero;
        float escMov = 0.25f * (float)args.Time;
        if (kb.IsKeyDown(Keys.T)) deltaEsc.X -= escMov;
        if (kb.IsKeyDown(Keys.Y)) deltaEsc.X += escMov;
        if (deltaEsc != Vector3.Zero) { _escenario.Trasladar(deltaEsc); transformacionCPU = true; }
        if (kb.IsKeyDown(Keys.O)) { _escenario.Escalar(new Vector3(1f + 0.5f * (float)args.Time)); transformacionCPU = true; }
        if (kb.IsKeyDown(Keys.P)) { _escenario.Escalar(new Vector3(1f - 0.5f * (float)args.Time)); transformacionCPU = true; }

        if (transformacionCPU) { _dirty = true; if (_time > 0.25) { ReconstruirSiDirty(); _time = 0.0; } }
        else { ReconstruirSiDirty(); }
    }

    private void ActualizarProyeccion()
    {
        float aspect = ClientSize.X / (float)Math.Max(1, ClientSize.Y);
        _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), aspect, 0.1f, 100f);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _frameCount++;
        GL.UseProgram(_program);
    // Probar orden view * projection (modelo identidad). Si aún no se ve nada, revertiremos a model*view*projection.
    Matrix4 mvp = _view * _projection;
        GL.UniformMatrix4(_uMvpLocation, false, ref mvp);

        GL.BindVertexArray(_vao);
    GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);

        var err = GL.GetError();
        if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            Console.WriteLine($"[GL ERROR] {err}");

        GL.BindVertexArray(0);
        GL.UseProgram(0);
        Context.SwapBuffers();
    }

    private void ReconstruirBuffers()
    {
    LiberarBuffersEscena();
        HashSet<Cara>? highlight = null;
        if (_modoParte && _escenario.Objetos.Count > 0)
        {
            var obj = _escenario.Objetos[_objSeleccionado];
            if (obj.Partes.Count > 0)
            {
                _parteSeleccionada = Math.Clamp(_parteSeleccionada, 0, obj.Partes.Count - 1);
                highlight = new HashSet<Cara>(obj.Partes[_parteSeleccionada].Caras);
            }
        }
        CrearBuffersDesdeEscenario(_escenario, out _vao, out _vbo, out _ebo, out _indexCount, out _vertexCount, highlight);
    }

    private void ReconstruirSiDirty()
    {
        if (_dirty)
        {
            ReconstruirBuffers();
            _dirty = false;
        }
    }

    private void LiberarBuffersEscena()
    {
        if (_ebo != 0) { GL.DeleteBuffer(_ebo); _ebo = 0; }
        if (_vbo != 0) { GL.DeleteBuffer(_vbo); _vbo = 0; }
        if (_vao != 0) { GL.DeleteVertexArray(_vao); _vao = 0; }
    }

    private void LiberarBuffers()
    {
        LiberarBuffersEscena();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        LiberarBuffers();
        if (_program != 0) GL.DeleteProgram(_program);
    }

    private static void CrearBuffersDesdeEscenario(Escenario escenario, out int vao, out int vbo, out int ebo, out int indexCount, out int vertexCount, HashSet<Cara>? highlightCaras = null)
    {
        var vertices = new List<float>();
        var indices  = new List<int>();
        int baseVertex = 0;
        foreach (var cara in escenario.Caras)
        {
            int added = cara.Vertices.Count;
            for (int i = 0; i < added; i++)
            {
                var p = cara.Vertices[i];
                Vector3 color = p.Color;
                if (highlightCaras != null && highlightCaras.Contains(cara))
                {
                    color = Vector3.Clamp(color * 0.3f + new Vector3(1f,1f,0f) * 0.7f, Vector3.Zero, Vector3.One);
                }
                vertices.Add(p.Posicion.X);
                vertices.Add(p.Posicion.Y);
                vertices.Add(p.Posicion.Z);
                vertices.Add(color.X);
                vertices.Add(color.Y);
                vertices.Add(color.Z);
            }
            if (added >= 3)
            {
                for (int t = 1; t <= added - 2; t++)
                {
                    indices.Add(baseVertex + 0);
                    indices.Add(baseVertex + t);
                    indices.Add(baseVertex + t + 1);
                }
            }
            baseVertex += added;
        }
        vao = GL.GenVertexArray();
        vbo = GL.GenBuffer();
        ebo = GL.GenBuffer();
        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.DynamicDraw);
        GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,Punto.StrideBytes,0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1,3,VertexAttribPointerType.Float,false,Punto.StrideBytes,3*sizeof(float));
        GL.EnableVertexAttribArray(1);
        GL.BindVertexArray(0);
        indexCount = indices.Count;
        vertexCount = baseVertex;
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

    private void AjustarParteSiFueraDeRango()
    {
        if (_escenario.Objetos.Count == 0) return;
        var partes = _escenario.Objetos[_objSeleccionado].Partes;
        if (partes.Count == 0)
        {
            _parteSeleccionada = 0;
            if (_modoParte)
            {
                _modoParte = false;
                Console.WriteLine("[ModoParte] Desactivado: el objeto no tiene partes");
            }
        }
        else
        {
            _parteSeleccionada = Math.Clamp(_parteSeleccionada, 0, partes.Count - 1);
        }
    }

    // ================= CRUD ESCENARIO =================
    const string ArchivoEscenario = "escenario.json";
    private void GenerarSeedGenerica()
    {
        // Seed mínima SOLO primera ejecución: monitor, cpu, teclado.
        Console.WriteLine("[Seed] Creando objetos seed...");
        // Monitor
        var monitor = new Objeto(); _escenario.AgregarObjeto(monitor);
        var parteMonitor = new Parte("monitor"); monitor.AgregarParte(parteMonitor);
        // Marco externo
        CrearRect(parteMonitor, new Vector3(-0.6f, 0.4f, 0f), 0.8f, 0.5f, new Vector3(0.1f,0.1f,0.1f));
        // Pantalla interna
        CrearRect(parteMonitor, new Vector3(-0.6f, 0.4f, 0.01f), 0.7f, 0.4f, new Vector3(0.05f,0.15f,0.4f));
        // Base
        CrearRect(parteMonitor, new Vector3(-0.6f, 0.07f, 0f), 0.25f, 0.06f, new Vector3(0.12f,0.12f,0.12f));

        // CPU (torre)
        var cpu = new Objeto(); _escenario.AgregarObjeto(cpu);
        var parteCpu = new Parte("cpu"); cpu.AgregarParte(parteCpu);
        CrearRect(parteCpu, new Vector3(0.2f, 0.25f, 0f), 0.25f, 0.55f, new Vector3(0.2f,0.2f,0.22f));
        // Franja frontal
        CrearRect(parteCpu, new Vector3(0.2f, 0.45f, 0.01f), 0.22f, 0.08f, new Vector3(0.25f,0.25f,0.28f));
        // Botón
        CrearRect(parteCpu, new Vector3(0.28f, 0.37f, 0.02f), 0.03f, 0.03f, new Vector3(0.8f,0.1f,0.1f));

        // Teclado (plano)
        var teclado = new Objeto(); _escenario.AgregarObjeto(teclado);
        var parteTeclado = new Parte("teclado"); teclado.AgregarParte(parteTeclado);
        CrearRect(parteTeclado, new Vector3(-0.15f, -0.15f, 0f), 0.55f, 0.18f, new Vector3(0.18f,0.18f,0.18f));
        CrearRect(parteTeclado, new Vector3(-0.15f, -0.15f, 0.01f), 0.5f, 0.14f, new Vector3(0.3f,0.3f,0.3f));

        _escenario.RecalcularCentroDeMasa(); 
        Console.WriteLine($"[Seed] Centros recalculados. Total objetos: {_escenario.Objetos.Count}");
    }

    private static void CrearRect(Parte parte, Vector3 centro, float ancho, float alto, Vector3 color)
    {
        var cara = parte.AgregarCara(color);
        float hw = ancho * 0.5f; float hh = alto * 0.5f;
        cara.AgregarVertice(centro + new Vector3(-hw,-hh,centro.Z));
        cara.AgregarVertice(centro + new Vector3( hw,-hh,centro.Z));
        cara.AgregarVertice(centro + new Vector3( hw, hh,centro.Z));
        cara.AgregarVertice(centro + new Vector3(-hw, hh,centro.Z));
    }
    private void NuevoEscenario()
    {
        _escenario.Objetos.Clear();
        _objSeleccionado = 0; _parteSeleccionada = 0; _modoParte = false;
        _dirty = true; ReconstruirSiDirty();
        Console.WriteLine("[CRUD] Escenario vaciado");
    }

    private void GuardarEscenario()
    {
        try { Serializador.Guardar(ArchivoEscenario, _escenario); }
        catch (Exception ex) { Console.WriteLine($"[CRUD] Error guardando: {ex.Message}"); }
    }

    private void CargarEscenario()
    {
        var esc = Deserializador.Cargar(ArchivoEscenario);
        if (esc != null)
        {
            _escenario.Objetos.Clear();
            foreach (var o in esc.Objetos) _escenario.AgregarObjeto(o);
            _objSeleccionado = 0; _parteSeleccionada = 0; _modoParte = false;
            _dirty = true; ReconstruirSiDirty();
        }
    }

    // Métodos de creación interactiva eliminados para impedir modificaciones en ejecución
}
