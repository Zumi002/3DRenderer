using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK_3DRendering.Engine.Components.Renderers;
using OpenTK.Mathematics;

namespace GK_3DRendering.Engine.Objects
{
    class Plane : GameObject
    {
        private Vertex[] vertices =
        {
            new (new Vector3(1f,0,1f),new Vector3(0,1f,0)),    // 0
            new (new Vector3(-1f,0,1f),new Vector3(0,1f,0)),   // 1
            new (new Vector3(1f,0,-1f),new Vector3(0,1f,0)),   // 2
            new (new Vector3(-1f,0,-1f),new Vector3(0,1f,0)),  // 3
            new (new Vector3(1f,0,1f),new Vector3(0,-1f,0)),   // 4
            new (new Vector3(-1f,0,1f),new Vector3(0,-1f,0)),  // 5
            new (new Vector3(1f,0,-1f),new Vector3(0,-1f,0)),  // 6 
            new (new Vector3(-1f,0,-1f),new Vector3(0,-1f,0))  // 7
        };
        private int[] indices =
        {
           0,2,1,
           1,2,3
        };
        public Plane(Scene scene) : base(scene)
        {
            Mesh mesh = new Mesh(vertices, indices);

            Renderer = new MeshRenderer(mesh, ShaderManager.GetShader(ShaderManager.MyShaders.Default), new Material(), this, true);

            Name = "Plane";
        }

    }
}
