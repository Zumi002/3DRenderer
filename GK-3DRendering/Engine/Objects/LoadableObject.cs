using GK_3DRendering.Engine.Components.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine.Objects
{
    class LoadableObject : GameObject
    {
        MeshLoader meshLoader;
        public LoadableObject(string resourcePath, Scene scene) : base(scene)
        {
            meshLoader = new MeshLoader();
            
            Mesh mesh = meshLoader.Load(resourcePath);
            Renderer = new MeshRenderer(mesh, ShaderManager.GetShader(ShaderManager.MyShaders.Default), new Material(), this);

            Name = "LoadableObject";
        }
    }
}
