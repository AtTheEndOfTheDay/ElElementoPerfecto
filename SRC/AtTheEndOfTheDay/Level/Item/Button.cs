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
    public class Button : Item
    {
        #region Constants
        #endregion Constants

        #region Constructors
        MeshStaticPart _Mesh;
        public Button()
        {
            var mesh = Game.Current.GetMesh("Wall");
            Add(_Mesh = new MeshStaticPart(mesh));
            _Mesh.Color = Color.FromArgb(0, 0, 0, 55);
            Add(_Obb = new ObbTranslatedCollider(mesh));
        }
        #endregion Constructors

        #region Properties
        private String _RelatedItemString;
        public String RelatedItem
        {
            get { return _RelatedItemString; }
            set
            {
                _RelatedItemString = value;
                //TODO:  _RelatedItem = econtrar el item del Related - Parser
            }
        }

        #endregion Properties

        #region ItemMethods
        private readonly ObbTranslatedCollider _Obb;
        private Item _RelatedItem;
        public override bool React(ItemCollision itemCollision, float deltaTime)
        {
            _RelatedItem.ButtonSignal();
            return base.React(itemCollision, deltaTime);
        }
        #endregion ItemMethods
    }
}
