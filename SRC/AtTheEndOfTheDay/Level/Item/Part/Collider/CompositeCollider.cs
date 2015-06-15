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
        public IList<Collider> Colliders = new List<Collider>();

        #region ColliderMethods
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

        #region PartMethods
        public override void Attach(Item item)
        {
            foreach (var colider in Colliders)
                colider.Attach(item);
        }
        public override void Detach(Item item)
        {
            foreach (var colider in Colliders)
                colider.Detach(item);
        }
        #endregion PartMethods
    }
}
