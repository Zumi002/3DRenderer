using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjLoader;
using ObjLoader.Loader.Loaders;
using OpenTK.Mathematics;

namespace GK_3DRendering.Engine
{
    class MeshLoader
    {
        ObjLoaderFactory objLoaderFactory = new();
        public MeshLoader() 
        {
        }

        public Mesh Load(string resourcePath)
        {
            var result = LoadFromResource(resourcePath);
            return Convert(result);
        }


        public LoadResult LoadFromResource(string resourcePath)
        {
            Stream s = Resources.GetResourceStream(resourcePath);

            var objLoader = objLoaderFactory.Create(new MaterialNullStreamProvider());

            return objLoader.Load(s);
        }

        public Mesh Convert(LoadResult result)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();
            foreach(var group in result.Groups)
            {
                foreach(var face in group.Faces)
                {
                    int triangle = 0;
                    int firstIdx = vertices.Count;
                    for(int i = 0; i<face.Count; i++)
                    {
                        if (triangle >= 3)
                        {
                            indices.Add(firstIdx);
                            indices.Add(vertices.Count - 1);
                        }
                        var faceVertex = face[i];
                        var v = result.Vertices[faceVertex.VertexIndex-1];
                        var n = result.Normals[faceVertex.NormalIndex-1];
                        var t = result.Textures[faceVertex.TextureIndex-1];
                        vertices.Add(new Vertex(
                                     new Vector3(v.X, v.Y, v.Z),
                                     new Vector3(n.X, n.Y, n.Z),
                                     new Vector2(t.X, t.Y)));
                        indices.Add(vertices.Count-1);
                        triangle++;
                        
                    }
                }
            }
            return new Mesh(vertices, indices);
        }
    }
}
