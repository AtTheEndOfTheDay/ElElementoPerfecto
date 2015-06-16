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
    public class Repulsor : Item
    {
        #region Constructors
        #region DefaultForceConstructors
        public Repulsor(Game game)
            : this(game, 1f, Vector3.Empty, Vector3.Empty, 1f) { }
        public Repulsor(Game game, Vector3 position)
            : this(game, 1f, position, Vector3.Empty, 1f) { }
        public Repulsor(Game game, Vector3 position, Vector3 rotation)
            : this(game, 1f, position, rotation, 1f) { }

        public Repulsor(Game game, Vector3 position, Single radius)
            : this(game, 1f, position, Vector3.Empty, radius) { }
        public Repulsor(Game game, Vector3 position, Vector3 rotation, Single radius)
            : this(game, 1f, position, rotation, radius) { }
        #endregion DefaultForceConstructors

        #region DefaultConstructors
        public Repulsor(Game game, Single force)
            : this(game, force, Vector3.Empty, Vector3.Empty, 1f) { }
        public Repulsor(Game game, Single force, Vector3 position)
            : this(game, force, position, Vector3.Empty, 1f) { }
        public Repulsor(Game game, Single force, Vector3 position, Vector3 rotation)
            : this(game, force, position, rotation, 1f) { }

        public Repulsor(Game game, Single force, Single radius)
            : this(game, force, Vector3.Empty, Vector3.Empty, radius) { }
        public Repulsor(Game game, Single force, Vector3 position, Single radius)
            : this(game, force, position, Vector3.Empty, radius) { }
        public Repulsor(Game game, Single force, Vector3 position, Vector3 rotation, Single radius)
        {
            var mesh = game.NewMesh("Ball", Color.FromArgb(0,75,0,0));//TODO: Make Mesh
            Add(new MeshStaticPart(mesh)); 
            Add(new SphereCollider(mesh));
            Scale = radius * Vector3Extension.One;
            _Force *= force;
        }
        #endregion DefaultConstructors
        #endregion Constructors

        #region ItemMethods
        private readonly Single _Force = 100000f;
        private readonly Single _RepulsionDistance = 1000f;//TODO: que lo levante del .lvl
        public override void Act(Interactive interactive, Single deltaTime)
        {
            var n = interactive.Position - Position;
            var d2 = n.LengthSq();
            n.Normalize();
            if (d2 < _RepulsionDistance)
                interactive.Momentum += n * (_Force / d2);
        }
        #endregion ItemMethods
    }
}
