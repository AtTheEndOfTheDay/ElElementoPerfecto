using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshStaticPart : MeshPart
    {
        public MeshStaticPart(Game game, TgcMesh mesh)
            : base(game, mesh) { }
        public override void Attach(Item item) { }
        public override void Detach(Item item) { }
        public override void Render(Item item, Effect shader)
        {
            Mesh.Transform = item.RenderMatrix;
            base.Render(item, shader);
        }
    }
}
