using System;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Collision
    {
        public Collider ColliderA { get; private set; }
        public Collider ColliderB { get; private set; }
        public readonly Contact[] Contacts;
        public Single Friction { get; set; }
        public Single Restitution { get; set; }
        public Collision(Collider colliderA, Collider colliderB, params Contact[] contacts)
        {
            ColliderA = colliderA;
            ColliderB = colliderB;
            Contacts = contacts;
            Friction = colliderA.Friction + colliderB.Friction;
            Restitution = Math.Max(colliderA.Restitution, colliderB.Restitution);
        }
        public void Invert()
        {
            var c = ColliderA;
            ColliderA = ColliderB;
            ColliderB = c;
            foreach (var contact in Contacts)
                contact.Invert();
        }
    }
}
