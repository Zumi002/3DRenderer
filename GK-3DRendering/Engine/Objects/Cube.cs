using GK_3DRendering.Engine.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using GK_3DRendering.Engine.Components.Renderers;
using System.Data;
using OpenTK.Mathematics;
using GK_3DRendering.Engine.Components;
using System.Drawing;

namespace GK_3DRendering.Engine.Objects
{
    class Cube : GameObject
    {
        private Vertex[] vertices =
        {
            //side 1 - up
            new (new Vector3(1f,1f,1f), new Vector3(0,1f,0)),      // 0
            new (new Vector3(-1f,1f,1f), new Vector3(0,1f,0)),     // 1
            new (new Vector3(1f,1f,-1f), new Vector3(0,1f,0)),     // 2
            new (new Vector3(-1f,1f,-1f), new Vector3(0,1f,0)),    // 3

            //side 2 - down
            new (new Vector3(1f,-1f,1f), new Vector3(0,-1f,0)),     // 4
            new (new Vector3(-1f,-1f,1f), new Vector3(0,-1f,0)),    // 5
            new (new Vector3(1f,-1f,-1f), new Vector3(0,-1f,0)),    // 6
            new (new Vector3(-1f,-1f,-1f), new Vector3(0,-1f,0)),   // 7


            //side 3 - left
            new (new Vector3(-1f,1f,1f), new Vector3(-1f,0,0)),     // 8
            new (new Vector3(-1f,1f,-1f), new Vector3(-1f,0,0)),    // 9
            new (new Vector3(-1f,-1f,1f), new Vector3(-1f,0,0)),    // 10
            new (new Vector3(-1f,-1f,-1f), new Vector3(-1f,0,0)),   // 11


            //side 5 - right
            new (new Vector3(1f,1f,1f), new Vector3(1f,0,0)),       // 12
            new (new Vector3(1f,1f,-1f), new Vector3(1f,0,0)),      // 13
            new (new Vector3(1f,-1f,1f), new Vector3(1f,0,0)),      // 14
            new (new Vector3(1f,-1f,-1f), new Vector3(1f,0,0)),     // 15

            //side 5 - front
            new (new Vector3(1f,1f,1f), new Vector3(0,0,1f)),       // 16
            new (new Vector3(1f,-1f,1f), new Vector3(0,0,1f)),      // 17
            new (new Vector3(-1f,1f,1f), new Vector3(0,0,1f)),      // 18
            new (new Vector3(-1f,-1f,1f), new Vector3(0,0,1f)),     // 19

            //side 6 - back
            new (new Vector3(1f,1f,-1f), new Vector3(0,0,-1f)),     // 20
            new (new Vector3(1f,-1f,-1f), new Vector3(0,0,-1f)),    // 21
            new (new Vector3(-1f,1f,-1f), new Vector3(0,0,-1f)),    // 22
            new (new Vector3(-1f,-1f,-1f), new Vector3(0,0,-1f)),   // 23
        };
        private int[] indices =
        {
            //side 1 - up
            0,1,2,
            1,2,3,

            //side 2 - down
            4,5,6,
            5,6,7,

            //side 3 - left
            8,9,10,
            9,10,11,

            //side 4 - right
            12,13,14,
            13,14,15,

            //side 5 - front
            16,17,18,
            17,18,19,

            //side 6 - back
            20,21,22,
            21,22,23
        };

        public Cube(Scene scene, string name, Transform transform, Vector3 color) : base(scene, name, transform)
        {
            Mesh mesh = new Mesh(vertices, indices);

            Renderer = new MeshRenderer(mesh, ShaderManager.GetShader(ShaderManager.MyShaders.Default), new Material(color), this);
        }
        public Cube(Scene scene) : this(scene, "Cube")
        { }

        public Cube(Scene scene, string name) : this(scene, name, new Transform())
        { }

        public Cube(Scene scene, Transform transform) : base(scene, "Cube", transform)
        { }

        public Cube(Scene scene, string name, Transform transform) : this(scene, "Cube", transform, new Vector3(1))
        { }

        

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //Transform.Rotate(new Vector3(0.0002f, 0.0001f, 0));
        }
    }
}
