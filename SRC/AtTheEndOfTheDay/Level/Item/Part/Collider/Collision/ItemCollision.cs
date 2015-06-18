using Microsoft.DirectX;
using System;
using System.Linq;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class ItemCollision
    {
        public readonly Item Item;
        public readonly Interactive Interactive;
        public readonly Collision[] Collisions;
        public Int32 TimesReacted { get; private set; }

        public ItemCollision(Item item, Interactive interactive, Collision[] collisions)
        {
            this.Item = item;
            this.Interactive = interactive;
            this.Collisions = collisions;
        }
        public Boolean ComputeReaction(Single deltaTime)
        {
            var reacted = false;
            foreach (var collision in Collisions)
                foreach (var contact in collision.Contacts)
                    if (Item.React(new ItemContactState(this, collision, contact, TimesReacted), deltaTime))
                        reacted = true;
            if (reacted) TimesReacted++;
            return reacted;
        }
        public Boolean AnyContact(Func<Contact, Boolean> predicate)
        {
            return Collisions.Any(collision => collision.Contacts.Any(predicate));
        }
        public Boolean AnyNormalDotVector(Vector3 vector, Func<Single, Boolean> predicate)
        {
            return AnyContact(contact => predicate(Vector3.Dot(contact.NormalAB, vector)));
        }
    }
}
