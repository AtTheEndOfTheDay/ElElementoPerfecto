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
        #region Constructors
        public Menu(Game game, Vector3 position, Vector3 slots, Vector3 step)
            : this(game, position, slots, step, Vector3.Empty) { }
        public Menu(Game game, Vector3 position, Vector3 slots, Vector3 step, Vector3 rotation)
        {
            var halfStep = .5f * step;
            var obb = new TgcObb() { Extents = halfStep.Abs() };
            Add(new ObbCollider(obb));
            obb.setRenderColor(Color.White);
            obb.SetOrientation();
            _Step = step;
            _Start = halfStep - halfStep.MemberwiseMult(slots);
            Scale = slots;
            Rotation = rotation;
            Position = position;
        }
        #endregion Constructors

        #region Fields
        private readonly Vector3 _Step;
        private readonly Vector3 _Start;
        private Vector3 _ItemRotation = new Vector3(.7f, 0, .7f);
        private static readonly Vector3 _RotationSpeed = new Vector3(0, .7f, 0);
        private readonly List<Item> _Items = new List<Item>();
        public void Add(Item item)
        {
            _Items.Add(item);
            item.SaveValues();
        }
        public void Add(IEnumerable<Item> items)
        {
            foreach (var item in items)
                Add(item);
        }
        public void Remove(Item item)
        {
            _Items.Remove(item);
            item.LoadValues();
        }
        public void Remove(IEnumerable<Item> items)
        {
            foreach (var item in items)
                Remove(item);
        }
        #endregion Fields

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
            _ItemRotation += deltaTime * _RotationSpeed;
            var rotation = Rotation + _ItemRotation;
            var start = Position + _Start;
            var v = new Vector3(0, 0, 0);
            for (var i = 0; v.Z < Scale.Z && i < _Items.Count; v.Z++)
                for (v.Y = 0; v.Y < Scale.Y && i < _Items.Count; v.Y++)
                    for (v.X = 0; v.X < Scale.X && i < _Items.Count; v.X++, i++)
                        _Items[i].MenuTransform(rotation, start + v.MemberwiseMult(_Step));
        }
        #endregion ItemMethods
    }
}
