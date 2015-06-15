using System;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class GameCollision : IGoal
    {
        private Item _ItemA;
        private Item _ItemB;
        public GameCollision(IList<Item> gameItems, IList<Item> userItems, Int32 aIndex, Int32 bIndex)
        {
            _ItemA = gameItems[aIndex];
            _ItemB = gameItems[bIndex];
        }
        public Boolean IsMeet
        {
            get
            {
                return _ItemA.Collides(_ItemB);
            }
        }
    }
}
