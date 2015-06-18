using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshImmutableePart : MeshTransformedPart
    {
        public MeshImmutableePart(TgcMesh mesh)
            : base(mesh) { }
        protected override void Item_PositionChanged(Item item) { }
        protected override void Item_RotationChanged(Item item) { }
        protected override void Item_ScaleChanged(Item item) { }
    }
}
