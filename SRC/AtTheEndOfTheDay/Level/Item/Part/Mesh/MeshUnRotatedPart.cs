using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshUnRotatedPart : MeshTransformedPart
    {
        public MeshUnRotatedPart(TgcMesh mesh)
            : base(mesh) { }
        protected override void Item_RotationChanged(Item item) { }
    }
}
