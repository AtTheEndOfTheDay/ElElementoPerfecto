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
    public class Menu : Item
    {
        #region Constants
        public static readonly Vector3 DefaultItemRotation = new Vector3(.7f, 0, .7f);
        public static readonly Vector3 DefaultItemRotationSpeed = new Vector3(0, .7f, 0);
        #endregion Constants

        #region Constructors
        public Menu(Game game)
            : base(game)
        {
            Add(_Colider = new ObbCollider() { Color = Color.White });
            ItemRotation = DefaultItemRotation;
            ItemRotationSpeed = DefaultItemRotationSpeed;
        }
        #endregion Constructors

        #region Items
        private readonly List<Item> _Items = new List<Item>();
        public Item Add(Item item)
        {
            _Items.Add(item);
            item.SaveValues();
            return item;
        }
        public T Add<T>(T items)
            where T : IEnumerable<Item>
        {
            foreach (var item in items)
                Add(item);
            return items;
        }
        public Item Remove(Item item)
        {
            _Items.Remove(item);
            item.LoadValues();
            return item;
        }
        public T Remove<T>(T items)
            where T : IEnumerable<Item>
        {
            foreach (var item in items)
                Remove(item);
            return items;
        }
        #endregion Items

        #region Properties
        public Vector3 ItemRotation { get; set; }
        public Vector3 ItemRotationSpeed { get; set; }
        public ObbCollider _Colider;
        public Vector3 _ItemStart;
        private Vector3 _ItemSize = Item.DefaultScale;
        public Vector3 ItemSize
        {
            get { return _ItemSize; }
            set
            {
                _ItemSize = value;
                var is_2 = .5f * _ItemSize;
                _Colider.Extents = is_2;
                _ItemStart = is_2 - is_2.MemberwiseMult(Scale);
            }
        }
        #endregion Properties

        #region ItemMethods
        public Item Pick(TgcRay ray)
        {
            var item = _Items.FirstOrDefault(i => i.Intercepts(ray));
            if (item != null) Remove(item);
            return item;
        }
        public override void Render(Dx3D.Effect shader)
        {
            base.Render(shader);
            foreach (var item in _Items)
                item.Render(shader);
        }
        public override void Dispose()
        {
            base.Dispose();
            foreach (var item in _Items)
                item.Dispose();
        }
        public override void Animate(Single deltaTime)
        {
            ItemRotation += deltaTime * RotationSpeed;
            var rotation = Rotation + ItemRotation;
            var start = Position + _ItemStart;
            var v = new Vector3(0, 0, 0);
            for (var i = 0; v.Z < Scale.Z && i < _Items.Count; v.Z++)
                for (v.Y = 0; v.Y < Scale.Y && i < _Items.Count; v.Y++)
                    for (v.X = 0; v.X < Scale.X && i < _Items.Count; v.X++, i++)
                        _Items[i].MenuTransform(rotation, start + v.MemberwiseMult(_ItemSize));
        }
        #endregion ItemMethods
    }
}
