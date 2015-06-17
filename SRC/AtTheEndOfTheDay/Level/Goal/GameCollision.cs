using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class ItemCollision : Goal
    {
        public ItemCollision(Game game) : base(game) { }
        private Item[] _Items;
        public String[] Items { get; set; }
        public override void FindTargets(Item[] items)
        {
            if (Items != null && Items.Length > 0)
            {
                _Items = null;
                var targets = new Item[Items.Length];
                for (var index = 0; index < Items.Length; index++)
                {
                    var item = items.FirstOrDefault(i => i.Name == Items[index]);
                    if (item == null) return;
                    targets[index] = item;
                }
                _Items = targets;
            }
        }
        public override Boolean IsMeet
        {
            get { return _Items != null && _Items.All(a => _Items.All(b => a.Collides(b))); }
        }
        public override void Dispose()
        {
            if (_Items == null) return;
            foreach (var item in _Items)
                item.Dispose();
        }
    }
}
