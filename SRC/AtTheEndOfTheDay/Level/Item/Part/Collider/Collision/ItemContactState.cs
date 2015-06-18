using System;
using Microsoft.DirectX;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class ItemContactState
    {
        public readonly Interactive Interactive;
        public readonly Collision Collision;
        public readonly Contact Contact;
        public readonly Int32 TimesReacted;
        public ItemContactState(ItemCollision itemCollision, Collision collision, Contact contact, Int32 timesReacted)
        {
            Interactive = itemCollision.Interactive;
            TimesReacted = timesReacted;
            Collision = collision;
            Contact = contact;
            Point = contact.PointB;
            Radius = Point - Interactive.Position;
            Normal = contact.NormalAB;
            Velocity = Interactive.GetVelocityAt(Radius);
            Approach = -Vector3.Dot(Normal, Velocity);
            Momentum = Interactive.GetForceAt(Radius);
            Weight = -Vector3.Dot(Normal, Momentum);
            Orthonormal = Normal.Orthonormal(Velocity);
        }
        public Vector3 Point { get; private set; }
        public Vector3 Radius { get; private set; }
        public Vector3 Normal { get; private set; }
        public void ApplyMinimumTranslation()
        {
            Interactive.Position += Contact.MinimumTranslation;
        }
        public Vector3 Orthonormal { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Single Approach { get; private set; }
        public Single ComputeApproachVelocity()
        {
            return -Vector3.Dot(Normal, Interactive.GetVelocityAt(Radius));
        }
        public Vector3 Momentum { get; private set; }
        public Single Weight { get; private set; }
        public Vector3 Restitution
        {
            get { return (1 + Collision.Restitution) * Approach * Normal; }
        }
        public void ApplyRestitution()
        {
            Interactive.AddVelocityAt(Radius, Restitution);
        }
        public Vector3 Friction
        {
            get { return Weight * (Normal - Collision.Friction * Orthonormal); }
        }
        public void ApplyFriction()
        {
            Interactive.AddForceAt(Radius, Friction);
        }
    }
}
