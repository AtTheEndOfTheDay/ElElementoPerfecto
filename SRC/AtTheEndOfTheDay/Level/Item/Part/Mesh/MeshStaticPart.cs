using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshStaticPart : MeshPart
    {
        public MeshStaticPart(TgcMesh mesh)
            : base(mesh) { }
        public override void Attach(Item item) { }
        public override void Detach(Item item) { }
        public override void Render(Item item, Effect shader)
        {
            Mesh.Transform = item.RenderMatrix;
            base.Render(item, shader);
        }
    }
}
