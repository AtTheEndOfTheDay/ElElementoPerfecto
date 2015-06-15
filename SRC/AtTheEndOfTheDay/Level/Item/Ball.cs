using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Ball : Interactive
    {
        #region Constructors
        private const String _MeshName = "Ball";

        #region TexturedConstructors
        public Ball(Game game, String material)
            : this(game, Vector3.Empty, Vector3.Empty, 1, 1, material) { }
        public Ball(Game game, Single radius, String material)
            : this(game, Vector3.Empty, Vector3.Empty, radius, 1, material) { }
        public Ball(Game game, Single radius, Single mass, String material)
            : this(game, Vector3.Empty, Vector3.Empty, radius, mass, material) { }
        public Ball(Game game, Vector3 position, String material)
            : this(game, position, Vector3.Empty, 1, 1, material) { }
        public Ball(Game game, Vector3 position, Single radius, String material)
            : this(game, position, Vector3.Empty, radius, 1, material) { }
        public Ball(Game game, Vector3 position, Single radius, Single mass, String material)
            : this(game, position, Vector3.Empty, radius, mass, material) { }
        public Ball(Game game, Vector3 position, Vector3 velocity, String material)
            : this(game, position, velocity, 1, 1, material) { }
        public Ball(Game game, Vector3 position, Vector3 velocity, Single radius, String material)
            : this(game, position, velocity, radius, 1, material) { }
        public Ball(Game game, Vector3 position, Vector3 velocity, Single radius, Single mass, String material)
            : this(position, velocity, radius, mass, game.NewMesh(_MeshName, material)) { }
        #endregion TexturedConstructors

        #region ColorConstructors
        public Ball(Game game, Color color)
            : this(game, Vector3.Empty, Vector3.Empty, 1, 1, color) { }
        public Ball(Game game, Single radius, Color color)
            : this(game, Vector3.Empty, Vector3.Empty, radius, 1, color) { }
        public Ball(Game game, Single radius, Single mass, Color color)
            : this(game, Vector3.Empty, Vector3.Empty, radius, mass, color) { }
        public Ball(Game game, Vector3 position, Color color)
            : this(game, position, Vector3.Empty, 1, 1, color) { }
        public Ball(Game game, Vector3 position, Single radius, Color color)
            : this(game, position, Vector3.Empty, radius, 1, color) { }
        public Ball(Game game, Vector3 position, Single radius, Single mass, Color color)
            : this(game, position, Vector3.Empty, radius, mass, color) { }
        public Ball(Game game, Vector3 position, Vector3 velocity, Color color)
            : this(game, position, velocity, 1, 1, color) { }
        public Ball(Game game, Vector3 position, Vector3 velocity, Single radius, Color color)
            : this(game, position, velocity, radius, 1, color) { }
        public Ball(Game game, Vector3 position, Vector3 velocity, Single radius, Single mass, Color color)
            : this(position, velocity, radius, mass, game.NewMesh(_MeshName, color)) { }
        #endregion ColorConstructors

        #region DefaultConstructors
        public Ball(Game game)
            : this(game, Vector3.Empty, Vector3.Empty, 1, 1, Color.Purple) { }
        public Ball(Game game, Single radius)
            : this(game, Vector3.Empty, Vector3.Empty, radius, 1, Color.Purple) { }
        public Ball(Game game, Single radius, Single mass)
            : this(game, Vector3.Empty, Vector3.Empty, radius, mass, Color.Purple) { }
        public Ball(Game game, Vector3 position)
            : this(game, position, Vector3.Empty, 1, 1, Color.Purple) { }
        public Ball(Game game, Vector3 position, Single radius)
            : this(game, position, Vector3.Empty, radius, 1, Color.Purple) { }
        public Ball(Game game, Vector3 position, Single radius, Single mass)
            : this(game, position, Vector3.Empty, radius, mass, Color.Purple) { }
        public Ball(Game game, Vector3 position, Vector3 velocity)
            : this(game, position, velocity, 1, 1, Color.Purple) { }
        public Ball(Game game, Vector3 position, Vector3 velocity, Single radius)
            : this(game, position, velocity, radius, 1, Color.Purple) { }
        public Ball(Game game, Vector3 position, Vector3 velocity, Single radius, Single mass)
            : this(game, position, velocity, radius, mass, Color.Purple) { }
        private Ball(Vector3 position, Vector3 velocity, Single radius, Single mass, TgcMesh mesh)
        {
            Add(new MeshStaticPart(mesh));
            Add(new SphereCollider(mesh));
            Scale = radius * Vector3Extension.One;
            Position = position;
            Velocity = velocity;
        }
        #endregion DefaultConstructors
        #endregion Constructors

        #region InteractiveMethods
        protected override Matrix ComputeBody()
        {
            var m_5 = .2f * Mass;
            var a2 = Scale.X * Scale.X;
            var b2 = Scale.Y * Scale.Y;
            var c2 = Scale.Z * Scale.Z;
            return new Matrix()
            {
                M11= m_5 * (b2 + c2),  M12= 0,                M13= 0,
                M21= 0,                M22= m_5 * (a2 + c2),  M23= 0,
                M31= 0,                M32= 0,                M33= m_5 * (a2 + b2),
                M44= 1,
            };
        }
        #endregion InteractiveMethods
    }
}
