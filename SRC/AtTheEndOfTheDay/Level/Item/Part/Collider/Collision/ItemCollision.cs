using System;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class ItemCollision
    {
        public readonly Item Item;
        public readonly Interactive Interactive;
        public readonly Collision[] Collisions;

        public ItemCollision(Item item, Interactive interactive, Collision[] collisions)
        {
            this.Item = item;
            this.Interactive = interactive;
            this.Collisions = collisions;
        }
        public Boolean Reaction(Single deltaTime)
        {
            return Item.React(this, deltaTime);
        }
    }
}
