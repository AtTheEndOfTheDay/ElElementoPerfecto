using TgcViewer.Utils.TgcSceneLoader;

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
