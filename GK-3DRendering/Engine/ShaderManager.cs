using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine
{
    static class ShaderManager
    {
        public enum MyShaders
        {
            Default,
            FlagShader
        }

        private static Dictionary<MyShaders, Shader> _shaders = new();

        public static Shader GetShader(MyShaders shaderName)
        {
            return _shaders[shaderName];
        }

        public static void AddShader(MyShaders shaderName, Shader shader)
        {
            _shaders.Add(shaderName, shader);
        }


    }
}
