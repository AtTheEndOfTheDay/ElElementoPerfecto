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
    public class Magnet : Item
    {
        #region Constructors
        #region DefaultForceConstructors
        public Magnet(Game game)
            : this(game, 1f, Vector3.Empty, Vector3.Empty, 1f) { }
        public Magnet(Game game, Vector3 position)
            : this(game, 1f, position, Vector3.Empty, 1f) { }
        public Magnet(Game game, Vector3 position, Vector3 rotation)
            : this(game, 1f, position, rotation, 1f) { }

        public Magnet(Game game, Vector3 position, Single scale)
            : this(game, 1f, position, Vector3.Empty, scale) { }
        public Magnet(Game game, Vector3 position, Vector3 rotation, Single scale)
            : this(game, 1f, position, rotation, scale) { }
        #endregion DefaultForceConstructors

        #region DefaultConstructors
        public Magnet(Game game, Single force)
            : this(game, force, Vector3.Empty, Vector3.Empty, 1f) { }
        public Magnet(Game game, Single force, Vector3 position)
            : this(game, force, position, Vector3.Empty, 1f) { }
        public Magnet(Game game, Single force, Vector3 position, Vector3 rotation)
            : this(game, force, position, rotation, 1f) { }

        public Magnet(Game game, Single force, Single scale)
            : this(game, force, Vector3.Empty, Vector3.Empty, scale) { }
        public Magnet(Game game, Single force, Vector3 position, Single scale)
            : this(game, force, position, Vector3.Empty, scale) { }
        public Magnet(Game game, Single force, Vector3 position, Vector3 rotation, Single scale)
        {
            var mesh = game.GetMesh("Magnet");
            Add(new MeshStaticPart(mesh));
            var collider = new ObbTranslatedCollider(mesh);
            Add(collider);
            Scale = scale * Vector3Extension.One;
            Rotation = rotation;
            Position = position;
            _Obb = collider.Obb;
            _Force *= force;
        }
        #endregion DefaultConstructors
        #endregion Constructors

        #region ItemMethods
        public override void Build(Single deltaTime)
        {
            var input = GuiController.Instance.D3dInput;
            var stepR = deltaTime * BuildRotationSpeed;
            if (input.keyDown(Key.D))
                Rotation = Rotation.AddZ(-stepR);
            else if (input.keyDown(Key.A))
                Rotation = Rotation.AddZ(stepR);
        }
        private readonly TgcObb _Obb;
        private readonly Single _Force = 100000f;
        public override void Act(Interactive interactive, Single deltaTime)
        {
            var n = interactive.Position - Position;
            var d2 = n.LengthSq();
            n.Normalize();
            if (Vector3.Dot(n, _Obb.Orientation[1]) > .7f)
                interactive.Momentum -= n * (_Force / d2);
        }
        #endregion ItemMethods
    }
}
