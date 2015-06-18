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
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Spring : Item
    {
        #region Constants
        private static readonly Object[] _SignalNull = { };
        #endregion Constants

        #region Constructors
        public Spring()
        {
            var mesh = Game.Current.GetMesh("Spring");
            Add(new MeshStaticPart(mesh));
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
        private readonly ObbTranslatedCollider _Obb;
        protected override void ReceiveCollision(Vector3 point, float approachVel, Vector3 normal)
        {
        }
        #endregion ItemMethods
    }
}
