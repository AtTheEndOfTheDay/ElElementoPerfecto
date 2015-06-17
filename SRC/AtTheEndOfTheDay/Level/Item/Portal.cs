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
    public class Portal : Item
    {
        #region Constants
        #endregion Constants

        #region Constructors
        MeshStaticPart _Mesh;
        MeshImmutableePart _Receptor;
        ObbCollider _Obb;
        public Portal()
        {
            var mesh = Game.Current.NewMesh("WallTextured"); 
            Add(_Mesh = new MeshStaticPart(mesh) { Texture = Game.Current.GetMaterial("BluePortal.png") });
            Add(_Obb = new ObbCollider(mesh));
            var receptorMesh = Game.Current.NewMesh("WallTextured");
            Add(_Receptor = new MeshImmutableePart(receptorMesh) { Texture = Game.Current.GetMaterial("OrangePortal.png") });
        }
        public override void LoadValues()
        {
            if (emptyParts())
            {
                Add(_Mesh);
                Add(_Receptor);
                Add(_Obb);
            }
            base.LoadValues();
        }
        #endregion Constructors

        #region Properties
        public Vector3 ReceptorPosition
        {
            get { return _Receptor.Position; }
            set
            {
                _Receptor.Position = value;
            }
        }
        public Vector3 ReceptorScale
        {
            get { return _Receptor.Scale; }
            set
            {
                _Receptor.Scale = value;
            }
        }
        public Vector3 ReceptorRotation
        {
            get { return _Receptor.Rotation; }
            set
            {
                _Receptor.Rotation = value;
            }
        }
        #endregion Properties

        #region ItemMethods
        public override Boolean React(ItemCollision itemCollision, Single deltaTime)
        {
            if (itemCollision.Item != this) return false;
            var reacted = true;
            var interactive = itemCollision.Interactive;
            interactive.Position = _Receptor.Position;
            Remove(_Mesh);
            Remove(_Receptor);
            Remove(_Obb);
            return reacted;
        }
        #endregion ItemMethods
    }
}
