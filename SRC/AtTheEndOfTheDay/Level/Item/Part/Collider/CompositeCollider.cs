using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class CompositeCollider : Collider
    {
        #region Colliders
        public Collider[] Colliders { get { return _Colliders.ToArray(); } }
        private ICollection<Collider> _Colliders = new List<Collider>();
        public Collider Add(Collider collider)
        {
            _Colliders.Add(collider);
            foreach (var item in _Items)
                collider.Attach(item);
            return collider;
        }
        public void Add(IEnumerable<Collider> colliders)
        {
            foreach (var part in colliders)
                Add(part);
        }
        public Collider Remove(Collider collider)
        {
            _Colliders.Remove(collider);
            foreach (var item in _Items)
                collider.Detach(item);
            return collider;
        }
        public void Remove(IEnumerable<Collider> colliders)
        {
            foreach (var part in colliders)
                Remove(part);
        }
        #endregion Colliders

        #region Properties
        public override Color Color
        {
            get { return base.Color; }
            set
            {
                if (base.Color == value) return;
                foreach (var colider in _Colliders)
                    colider.Color = value;
                base.Color = value;
            }
        }
        #endregion Properties

        #region PartMethods
        private ICollection<Item> _Items = new List<Item>();
        public override void Attach(Item item)
        {
            _Items.Remove(item);
            _Items.Add(item);
            foreach (var colider in Colliders)
                colider.Attach(item);
        }
        public override void Detach(Item item)
        {
            _Items.Remove(item);
            foreach (var colider in Colliders)
                colider.Detach(item);
        }
        public override void Render(Item item, Dx3D.Effect shader)
        {
            foreach (var colider in Colliders)
                colider.Render(item, shader);
        }
        public override void Dispose()
        {
            foreach (var colider in Colliders)
                colider.Dispose();
        }
        #endregion PartMethods

        #region ColliderMethods
        public override Boolean Intercepts(TgcRay ray)
        {
            return Colliders.Any(c => c.Intercepts(ray));
        }
        public override Boolean Collides(Collider other)
        {
            return Colliders.Any(c => c.Collides(other));
        }
        public override Collision Collide(Collider other)
        {
            var contacts = new List<Contact>();
            foreach (var colider in Colliders)
            {
                var collision = colider.Collide(other);
                if (collision != null)
                    contacts.AddRange(collision.Contacts);
            }
            return contacts.Count == 0 ? null
                : new Collision(this, other, contacts.ToArray());
        }
        #endregion ColliderMethods
    }
}
