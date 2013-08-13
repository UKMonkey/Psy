using System.Collections.Generic;
using Psy.Core.EpicModel;
using SlimMath;

namespace Psy.Graphics.Models
{
    public class CompiledModel
    {
        public class Animation
        {
            public readonly List<Frame> Frames;
            public readonly float Duration;

            public Animation(int frameCount, float duration)
            {
                Frames = new List<Frame>(frameCount);
                Duration = duration;
            }
        }

        public class Anchor
        {
            public int Id;
            public string Name;
            public Vector3 Position;
            public Quaternion Rotation;

            public Anchor(int id, string name, Vector3 position, Quaternion rotation)
            {
                Id = id;
                Name = name;
                Position = position;
                Rotation = rotation;
            }
        }

        public class Frame
        {
            public readonly List<ModelPartState> ModelPartStates;
            public readonly List<AnchorState> AnchorStates; 

            public Frame(int modelPartCount)
            {
                ModelPartStates = new List<ModelPartState>(modelPartCount);
                AnchorStates = new List<AnchorState>(modelPartCount);
            }
        }

        public class ModelPartState
        {
            public Quaternion Rotation;
            public Vector3 Position;

            public ModelPartState(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }

        public class AnchorState
        {
            public Quaternion Rotation;
            public Vector3 Position;

            public AnchorState(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }

        public class Triangle
        {
            public int Vertex1Index;
            public int Vertex2Index;
            public int Vertex3Index;
        }

        public class Mesh
        {
            public readonly int MaterialId;

            /// <summary>
            /// Links the vertex to the pivot with the animation.
            /// </summary>
            public readonly int[] VertexPivotIndex;

            /// <summary>
            /// Texture coordinates for rendering.
            /// </summary>
            public readonly Vector2[] TextureCoordinateBuffer;

            public readonly Triangle[] Triangles;
            public readonly Vector3[] Vertices;

            public Mesh(int vertexCount, int materialId)
            {
                MaterialId = materialId;
                Triangles = new Triangle[vertexCount / 3];
                Vertices = new Vector3[vertexCount];
                TextureCoordinateBuffer = new Vector2[vertexCount];
                VertexPivotIndex = new int[vertexCount];
            }
        }

        public readonly string Name;
        public readonly List<Mesh> Meshes;
        public readonly List<Anchor> Anchors;
        public readonly Dictionary<AnimationType, Animation> Animations;

        public CompiledModel(string name)
        {
            Name = name;
            Animations = new Dictionary<AnimationType, Animation>();
            Meshes = new List<Mesh>();
            Anchors = new List<Anchor>();
        }

        
    }
}