using System;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshTemporalRecursivePart : MeshPart
    {
        private Int32 _CurrentIndex = 0;
        private Single _CurrentTime = -1f;
        private readonly Single _DeltaTime;
        private readonly Matrix[] _Snapshots;
        private readonly Int32 _Last = 0;
        private readonly Single _DeltaAlpha = 0f;
        public MeshTemporalRecursivePart(TgcMesh mesh, Int32 snapshots = 7, Single deltaTime = .03f)
            : base(mesh)
        {
            _Last = snapshots - 1;
            _CurrentTime = _DeltaTime = deltaTime;
            _Snapshots = new Matrix[snapshots];
            _DeltaAlpha = 1 / snapshots;
        }
        public void Update(Single deltaTime)
        {
            _CurrentTime -= deltaTime;
        }
        public void Clear()
        {
            _Snapshots[_Last] = Matrix.Zero;
            _Snapshots[0] = _Snapshots[_CurrentIndex];
            _CurrentIndex = 0;
        }
        public override void Attach(Item item) { }
        public override void Detach(Item item) { }
        public override void Render(Item item, Effect shader)
        {
            if (_CurrentTime < 0)
            {
                _CurrentTime = _DeltaTime;
                _Snapshots[_CurrentIndex] = item.RenderMatrix;
                _CurrentIndex = _CurrentIndex == _Last ? 0 : _CurrentIndex + 1;
            }
            var alpha = 1f;
            for (var i = _CurrentIndex; -1 < i; i--)
            {
                Mesh.Transform = _Snapshots[i];
                base.Render(item, shader);
                alpha -= _DeltaAlpha;
            }
            if (_Snapshots[_Last] != Matrix.Zero)
                for (var i = _Last; _CurrentIndex < i; i--)
                {
                    Mesh.Transform = _Snapshots[i];
                    base.Render(item, shader);
                    alpha -= _DeltaAlpha;
                }
            Mesh.Transform = item.RenderMatrix;
            base.Render(item, shader);
        }
    }
}
