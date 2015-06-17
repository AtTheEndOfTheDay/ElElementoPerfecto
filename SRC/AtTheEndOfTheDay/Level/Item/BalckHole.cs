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
        private MeshStaticPart _Mesh;
        private const Single _ForceFactor = 100000f;
        private const Single _RepulsionFactor = 100f;
        private const Single _MaxScaleFactor = 1.2f;
        private const Single _MinScaleFactor = 0.8f;
        #region Constructors
        public BlackHole(Game game)
            :base(game)
        {
            var mesh = game.NewMesh("BallTextured");
            Add(_Mesh = new MeshStaticPart(game, mesh));
            _Mesh.Texture = Game.GetMaterial("BlackHole.jpg");
            Add(new SphereCollider(game, mesh));
        }
        #endregion Constructors

        #region Properties
        private Single _Force;
        private Single _ForceReal;
        public Single Force
        {
            get { return _Force; }
            set
            {
                _Force = value;
                _ForceReal = value * _ForceFactor;
            }
        }
        private Single _AtractionDistance;
        private Single _AtractionDistancePow2;
        public Single AtractionDistance
        {
            get { return _AtractionDistance; }
            set
            {
                _AtractionDistance = value;
                _AtractionDistancePow2 = value * value;
            }
        }
        #endregion Properties

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

        public override void Act(Interactive interactive, Single deltaTime)
        {
            var n = interactive.Position - Position;
            var d2 = n.LengthSq();
            n.Normalize();
            if (d2 < _AtractionDistance * _AtractionDistancePow2)
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
