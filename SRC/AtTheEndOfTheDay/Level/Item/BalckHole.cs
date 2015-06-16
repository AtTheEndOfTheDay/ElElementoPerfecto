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
    public class BlackHole : Item
    {
        private const Single _MaxScaleFactor = 1.2f;
        private const Single _MinScaleFactor = 0.8f;
        #region Constructors
        #region DefaultForceConstructors
        public BlackHole(Game game)
            : this(game, 1f, Vector3.Empty, Vector3.Empty, 1f) { }
        public BlackHole(Game game, Vector3 position)
            : this(game, 1f, position, Vector3.Empty, 1f) { }
        public BlackHole(Game game, Vector3 position, Vector3 rotation)
            : this(game, 1f, position, rotation, 1f) { }

        public BlackHole(Game game, Vector3 position, Single radius)
            : this(game, 1f, position, Vector3.Empty, radius) { }
        public BlackHole(Game game, Vector3 position, Vector3 rotation, Single radius)
            : this(game, 1f, position, rotation, radius) { }
        #endregion DefaultForceConstructors

        #region DefaultConstructors
        public BlackHole(Game game, Single force)
            : this(game, force, Vector3.Empty, Vector3.Empty, 1f) { }
        public BlackHole(Game game, Single force, Vector3 position)
            : this(game, force, position, Vector3.Empty, 1f) { }
        public BlackHole(Game game, Single force, Vector3 position, Vector3 rotation)
            : this(game, force, position, rotation, 1f) { }

        public BlackHole(Game game, Single force, Single radius)
            : this(game, force, Vector3.Empty, Vector3.Empty, radius) { }
        public BlackHole(Game game, Single force, Vector3 position, Single radius)
            : this(game, force, position, Vector3.Empty, radius) { }
        public BlackHole(Game game, Single force, Vector3 position, Vector3 rotation, Single radius)
        {
            var mesh = game.NewMesh("Ball", "BlackHole.jpg");//TODO: Make mesh
            Add(new MeshStaticPart(mesh));
            Add(new SphereCollider(mesh));
            Scale = radius * Vector3Extension.One;
            _MaxScale = Scale.X * _MaxScaleFactor;
            _MinScale = Scale.X * _MinScaleFactor;
            _Force *= force;
        }
        #endregion DefaultConstructors
        #endregion Constructors

        #region ItemMethods
 
        private Single _MaxScale;
        private Single _MinScale;
        private Boolean _IsGrowing = true;
        public override void Animate(Single deltaTime)
        {
            var stepX = 0.2f * deltaTime; 
            var stepY = 0.3f * deltaTime;
            if (_IsGrowing)
            {
                Scale = Scale.AdvanceX(stepX, _MaxScale);
                Scale = Scale.AdvanceY(stepY, _MaxScale);
            }
            else
            {
                Scale = Scale.AdvanceX(stepX, _MinScale);
                Scale = Scale.AdvanceY(stepY, _MinScale);
            }
            if (Scale.X == _MinScale || Scale.X == _MaxScale)
                _IsGrowing = !_IsGrowing;
        }

        private readonly Single _Force = 100000f;
        private readonly Single _AtractionDistance = 100f;
        private Single _AtractionFactor;
        private Single _AtractionFactorPow2;
        public Single AtractionFactor
        {
            get { return _AtractionFactor; }
            set
            {
                _AtractionFactor = value;
                _AtractionFactorPow2 = value * value;
            }
        }//TODO: que lo levante del .lvl
        public override void Act(Interactive interactive, Single deltaTime)
        {
            var n = interactive.Position - Position;
            var d2 = n.LengthSq();
            n.Normalize();
            if (d2 < _AtractionDistance * 25/*_AtractionFactorPow2*/)
                interactive.Momentum -= n * (_Force / d2);
        }

        public override Boolean React(ItemCollision itemCollision, Single deltaTime)
        {
            if (itemCollision.Item != this) return false;
            var reacted = true;
            var interactive = itemCollision.Interactive;
            interactive.Position= new Vector3(0,0,Single.MaxValue);
            return reacted;
        }
        #endregion ItemMethods
    }
}
