using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace GK_3DRendering.Engine
{
    class Shader : IDisposable
    {
        public int Handle;
        private bool _disposed = false;
        private static int _inUse;

        public Shader(params (string shaderPath, ShaderType shaderType)[] shaders)
        {
            Handle = GL.CreateProgram();
            
            List<int> shaderHandles = new List<int>();
            foreach (var sh in shaders)
            {
                int shaderHandle = LoadShader(sh.shaderPath, sh.shaderType);
                shaderHandles.Add(shaderHandle);
                GL.AttachShader(Handle, shaderHandle);
            }

            LinkProgram();

            //Clean up
            foreach(var shHandle in  shaderHandles)
            {
                GL.DetachShader(Handle, shHandle);
                GL.DeleteShader(shHandle);
            }
        }

        public void Use()
        {
            if (_inUse == Handle)
                return;
            GL.UseProgram(Handle);
            _inUse = Handle;
        }

        private int LoadShader(string path, ShaderType shaderType)
        {
            int Shader = GL.CreateShader(shaderType);
            string shaderSource = Resources.ReadResource(path);
            GL.ShaderSource(Shader, shaderSource);

            GL.CompileShader(Shader);
            GL.GetShader(Shader, ShaderParameter.CompileStatus, out int success);
            if(success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(Shader);
                Console.WriteLine(infoLog);
            }

            return Shader;
        }

        private void LinkProgram()
        {
            GL.LinkProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }
        }

        public int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(Handle, name);
        }

        public void LoadMat4(string name, Matrix4 matrix, bool transpose = true)
        {
            GL.ProgramUniformMatrix4(Handle, GetUniformLocation(name), transpose, ref matrix);
        }
        public void LoadVec3(string name, Vector3 vector)
        {
            GL.ProgramUniform3(Handle, GetUniformLocation(name), ref vector);
        }
        public void LoadVec2i(string name, Vector2i vector)
        {
            GL.ProgramUniform2(Handle, GetUniformLocation(name), ref vector);
        }
        public void LoadBool(string name, bool value)
        {
            GL.ProgramUniform1(Handle, GetUniformLocation(name), value?1:0);
        }
        public void LoadInt(string name, int value)
        {
            GL.ProgramUniform1(Handle, GetUniformLocation(name), value);
        }
        public void LoadFloat(string name, float value)
        {
            GL.ProgramUniform1(Handle, GetUniformLocation(name), value);
        }
        public void LoadMaterial(string name, Material material)
        {
            LoadVec3($"{name}.color", material.Color);
            LoadFloat($"{name}.ambient", material.ka);
            LoadFloat($"{name}.diffuse", material.kd);
            LoadFloat($"{name}.specular", material.ks);
            LoadFloat($"{name}.shininess", material.shinniness);
        }

        ~Shader()
        {
            if(!_disposed)
            {
                Console.WriteLine("GPU Leak! Did you forget to dispose() shader?");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                GL.DeleteProgram(Handle);
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

    }
}
