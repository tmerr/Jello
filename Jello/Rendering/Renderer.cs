using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace Jello.Rendering
{
    class Renderer
    {
        private int _vertexShaderHandle;
        private int _fragmentShaderHandle;
        private int _shaderProgramHandle;
        private int _projectionMatrixLocation;
        private int _modelviewMatrixLocation;

        private const string vertexShaderSource = @"
        #version 140
        uniform mat4 modelview_matrix;
        uniform mat4 projection_matrix;

        in vec3 vertex_position;
        in vec3 vertex_normal;
        out vec3 normal;

        void main(void)
        {
          normal = (modelview_matrix * vec4 ( vertex_normal, 0 ) ).xyz;
          gl_Position = projection_matrix * modelview_matrix * vec4 ( vertex_position, 1 );
        }";

        private const string fragmentShaderSource = @"
        #version 140
        precision highp float;

        const vec3 ambient = vec3 ( 0.1, 0.1, 0.1 );
        const vec3 lightVecNormalized = normalize( vec3( 0.5, 0.5, 2 ) );
        const vec3 lightColor = vec3( 1.0, 0.8, 0.2 );

        in vec3 normal;
        out vec4 out_frag_color;

        void main(void)
        {
          float diffuse = clamp( dot( lightVecNormalized, normalize( normal ) ), 0.0, 1.0 );
          out_frag_color = vec4( ambient + diffuse * lightColor, 1.0 );
        }";

        private void Initialize()
        {
            _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            _fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(_vertexShaderHandle, vertexShaderSource);
            GL.ShaderSource(_fragmentShaderHandle, fragmentShaderSource);
            GL.CompileShader(_vertexShaderHandle);
            GL.CompileShader(_fragmentShaderHandle);

            string vertexShaderInfoLog = GL.GetShaderInfoLog(_vertexShaderHandle);
            string fragmentShaderInfoLog = GL.GetShaderInfoLog(_fragmentShaderHandle);
            Console.WriteLine(vertexShaderInfoLog);
            Console.WriteLine(fragmentShaderInfoLog);

            _shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(_shaderProgramHandle, _vertexShaderHandle);
            GL.AttachShader(_shaderProgramHandle, _fragmentShaderHandle);
            GL.LinkProgram(_shaderProgramHandle);

            string programInfoLog = GL.GetProgramInfoLog(_shaderProgramHandle);
            Console.WriteLine(programInfoLog);

            _projectionMatrixLocation = GL.GetUniformLocation(_shaderProgramHandle, "projection_matrix");
            _modelviewMatrixLocation = GL.GetUniformLocation(_shaderProgramHandle, "modelview_matrix");
        }

        public Renderer()
        {
            Initialize();
        }

        public void Use()
        {
            GL.UseProgram(_shaderProgramHandle);
            GL.Enable(EnableCap.DepthTest);
        }

        public void Render(Camera camera, Color clearColor, IEnumerable<ModelData> buffergroups)
        {
            GL.ClearColor(clearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var projectionMatrix = camera.GetProjectionMatrix();
            var modelviewMatrix = Matrix4.Identity;
            GL.UniformMatrix4(_projectionMatrixLocation, false, ref projectionMatrix);
            GL.UniformMatrix4(_modelviewMatrixLocation, false, ref modelviewMatrix);

            foreach (var buffergroup in buffergroups)
            {
                var vertexBuffer = buffergroup.Vertices;;
                var normalBuffer = buffergroup.Normals;
                var indexBuffer = buffergroup.Indices;

                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer.Id);
                GL.EnableVertexAttribArray(0);
                GL.BindAttribLocation(_shaderProgramHandle, 0, "vertex_position");
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexBuffer.ElementSize, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer.Id);
                GL.EnableVertexAttribArray(1);
                GL.BindAttribLocation(_shaderProgramHandle, 1, "vertex_normal");
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, normalBuffer.ElementSize, 0);
                
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer.Id);

                GL.DrawElements(BeginMode.Triangles, indexBuffer.ElementCount, DrawElementsType.UnsignedInt, 0);
            }

	    GL.Flush();
        }
    }
}