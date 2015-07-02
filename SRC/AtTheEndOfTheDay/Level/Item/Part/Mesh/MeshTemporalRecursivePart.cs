using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshTemporalRecursivePart : MeshPart
    {
        private Int32 _CurrentIndex = 0;
        private readonly Single _DeltaTime;
        private readonly Matrix[] _Snapshots;
        private readonly Int32 _Last = 0;
        private readonly Single _DeltaAlpha = 0f;
        public MeshTemporalRecursivePart(TgcMesh mesh, Int32 snapshots = 100)
            : base(mesh)
        {
            _Last = snapshots - 1;
            _Snapshots = new Matrix[snapshots];
            _DeltaAlpha = .666f / snapshots;
        }
        public void Clear()
        {
            _Snapshots[_Last] = Matrix.Zero;
            _Snapshots[0] = _Snapshots[_CurrentIndex];
            _CurrentIndex = 0;
        }
        public override void Attach(Item item) { }
        public override void Detach(Item item) { }
        public void Update(Single deltaTime) { }
        public override void Render(Item item, Effect shader)
        {
            Mesh.Transform = item.RenderMatrix;
            base.Render(item, shader);
            _Snapshots[_CurrentIndex] = item.RenderMatrix;
            _CurrentIndex = _CurrentIndex == _Last ? 0 : _CurrentIndex + 1;
            if (!Game.Current.IsTemporalEffectEnabled) return;
            var alpha = .666f;
            Game.Current.SetAlpha(shader, .3999f);
            for (var i = _CurrentIndex - 1; -1 < i; i--)
                alpha = _RenderSnapshot(item, shader, i, alpha);
            if (_Snapshots[_Last] != Matrix.Zero)
                for (var i = _Last; _CurrentIndex < i; i--)
                    alpha = _RenderSnapshot(item, shader, i, alpha);
            Game.Current.SetAlpha(shader, 1f);
        }
        private Single _RenderSnapshot(Item item, Effect shader, Int32 index, Single alpha)
        {
            alpha -= _DeltaAlpha;
            var scale = Matrix.Identity;
            scale.Scale(alpha, alpha, alpha);
            Mesh.Transform = scale * _Snapshots[index];
            base.Render(item, shader);
            return alpha;
        }
    }
}
