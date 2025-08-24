using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;
using System;
using OpenTKComputerSetup.Models;
using OpenTKComputerSetup.Components;

namespace OpenTKComputerSetup
{
    class Program : GameWindow
    {
        private int _vertexBufferObject;
        private int _elementBufferObject;
        private int _vertexArrayObject;
        private int _shaderProgram;
        private Matrix4 _view;
        private Matrix4 _projection;

        
        private bool _isDragging = false;
        private Vector2 _lastMousePosition = Vector2.Zero;
        private float _mouseRotationX = 0.0f;
        private float _mouseRotationY = 0.0f;

        private List<Parte3D> _componentes;
        private List<float> _todosVertices;
        private List<uint> _todosIndicesTriangulos;

        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.95f, 0.95f, 0.95f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // Crear componentes del setup
            _componentes = new List<Parte3D>
            {
                new Mesa(new Vector3(0, -0.6f, 0), new Vector3(1, 1, 1)),
                new Components.Monitor(new Vector3(0, -0.6f, -0.5f), new Vector3(1, 1, 1)),
                new CPU(new Vector3(1.2f, -0.6f, 0.2f), new Vector3(1, 1, 1)),
                new Teclado(new Vector3(0, -0.6f, 0.4f), new Vector3(1, 1, 1))
            };

            CombinarGeometrias();
            ConfigurarBuffers();
            CrearShaders();

            GL.Viewport(0, 0, Size.X, Size.Y);
            _view = Matrix4.LookAt(new Vector3(0, 1, 4), Vector3.Zero, Vector3.UnitY);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Size.X / (float)Size.Y, 0.1f, 100f);
        }

        private void CombinarGeometrias()
        {
            _todosVertices = new List<float>();
            _todosIndicesTriangulos = new List<uint>();
            uint offsetIndice = 0;

            foreach (var componente in _componentes)
            {
                var vertices = componente.ObtenerVertices();
                _todosVertices.AddRange(vertices);
                
                uint numVertices = (uint)(vertices.Count / 6);
                for (uint i = 0; i < numVertices; i++)
                {
                    _todosIndicesTriangulos.Add(offsetIndice + i);
                }
                
                offsetIndice += numVertices;
            }
        }

        private void ConfigurarBuffers()
        {
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _todosVertices.Count * sizeof(float), _todosVertices.ToArray(), BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _todosIndicesTriangulos.Count * sizeof(uint), _todosIndicesTriangulos.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        private void CrearShaders()
        {
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec3 aPosition;
                layout(location = 1) in vec3 aColor;
                out vec3 fragColor;
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                void main()
                {
                    gl_Position = projection * view * model * vec4(aPosition, 1.0);
                    fragColor = aColor;
                }
            ";

            string fragmentShaderSource = @"
                #version 330 core
                in vec3 fragColor;
                out vec4 color;
                void main()
                {
                    color = vec4(fragColor, 1.0);
                }
            ";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);
            GL.LinkProgram(_shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Size.X / (float)Size.Y, 0.1f, 100f);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                _isDragging = true;
                _lastMousePosition = new Vector2(MouseState.X, MouseState.Y);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left) _isDragging = false;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Vector2 currentPos = new Vector2(e.X, e.Y);
            if (_isDragging)
            {
                Vector2 delta = currentPos - _lastMousePosition;
                _mouseRotationY += delta.X * 0.01f;
                _mouseRotationX += delta.Y * 0.01f;
            }
            _lastMousePosition = currentPos;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
            if (KeyboardState.IsKeyDown(Keys.R))
            {
                _mouseRotationX = 0.0f;
                _mouseRotationY = 0.0f;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(_shaderProgram);

            Matrix4 model = Matrix4.CreateRotationY(_mouseRotationY) * Matrix4.CreateRotationX(_mouseRotationX);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "view"), false, ref _view);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref _projection);

            GL.BindVertexArray(_vertexArrayObject);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _todosVertices.Count / 6);

            SwapBuffers();
        }

        static void Main(string[] args)
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1200, 800),
                Title = "Setup de Computadora 3D",
            };

            using var window = new Program(GameWindowSettings.Default, nativeWindowSettings);
            window.Run();
        }
    }
}