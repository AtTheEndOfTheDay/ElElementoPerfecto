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
    public class Acelerator : Item
    {
        #region Constants
        private const Single _ForceFactor = 100f;
        #endregion Constants

        #region Constructors
        MeshStaticPart _Mesh;
        ObbCollider _Obb;
        public Acelerator()
        {
            var mesh = Game.Current.NewMesh("WallTextured");
            Add(_Mesh = new MeshStaticPart(mesh) { Texture = Game.Current.GetMaterial("Acelerator.jpg") });
            Add(_Obb = new ObbCollider(mesh));
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
        #endregion Properties

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
        public override Boolean React(ItemCollision itemCollision, Single deltaTime)
        {
            if (itemCollision.Item != this) return false;
            var reacted = false;
            var interactive = itemCollision.Interactive;
            interactive.Momentum += _Obb.Orientation[0] * (_ForceReal);
            return reacted;
        }
        #endregion ItemMethods
    }
}
