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
        private MeshStaticPart _Mesh;
        private const Single _ForceFactor = 100000f;
        private const Single _RepulsionFactor = 100f;
        #region Constructors
        public Repulsor()
        {
            var mesh = Game.Current.NewMesh("Ball");
            Add(_Mesh = new MeshStaticPart(mesh));
            _Mesh.Color = Color.FromArgb(0, 75, 0, 0);
            Add(new SphereCollider(mesh));
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
        private Single _RepulsionDistance;
        private Single _RepulsionDistancePow2;
        public Single RepulsionDistance
        {
            get { return _RepulsionDistance; }
            set
            {
                _RepulsionDistance = value;
                _RepulsionDistancePow2 = value * value;
            }
        }
        #endregion Properties

        #region ItemMethods
        
        public override void Act(Interactive interactive, Single deltaTime)
        {
            var n = interactive.Position - Position;
            var d2 = n.LengthSq();
            n.Normalize();
            if (d2 < _RepulsionFactor * _RepulsionDistancePow2)
                interactive.Momentum += n * (_ForceReal / d2);
        }
        #endregion ItemMethods
    }
}
