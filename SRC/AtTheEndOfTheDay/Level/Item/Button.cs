using System;
using System.Linq;
using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Utils.Sound;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Button : Item
    {
        #region Constants
        private static readonly Object[] _SignalNull = { };
        #endregion Constants

        #region Constructors
        private readonly ObbTranslatedCollider _Collider;
        public Button()
        {
            var mesh = Game.Current.GetMesh("Torus");
            Add(new MeshStaticPart(mesh));
            Add(new MeshStaticPart(Game.Current.GetMesh("Cylinder")));
            Add(_Collider = new ObbTranslatedCollider(mesh));
        }
        #endregion Constructors

        #region Properties
        private Item _RelatedItem;
        private String _RelatedItemName;
        public String RelatedItem
        {
            get { return _RelatedItemName; }
            set
            {
                _RelatedItemName = value;
            }
        }
        private TgcStaticSound _SoundEffect;
        private String _SoundString;
        public String SoundEffect
        {
            get { return _SoundString; }
            set
            {
                _SoundString = value;
                _SoundEffect = Game.Current.GetSound(_SoundString, EffectVolume);
            }
        }
        private Object[] _Signal = _SignalNull;
        public Object[] Signal
        {
            get { return _Signal; }
            set { _Signal = value ?? _SignalNull; }
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
        public override void FindSiblings(Item[] items)
        {
            if (_RelatedItemName != null)
            {
                _RelatedItem = null;
                _RelatedItem = items.FirstOrDefault(i => i.Name.IgnoreCaseEquals(_RelatedItemName));
                if (_RelatedItem == null) return;
            }
        }
        protected override void OnCollision(ItemCollision itemCollision)
        {
            if (_RelatedItem != null
            && itemCollision.AnyNormalDotVector(_Collider.Top, dot => dot.Abs().TolerantEquals(1)))
            {
                _RelatedItem.Signal(_Signal);
            }
            if (_SoundEffect != null)
                _SoundEffect.play(false);
        }
        #endregion ItemMethods
    }
}
