using System;
using System.Drawing;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class MeshPart : ItemPart
    {
        public static Boolean IsVisible = true;
        public static readonly Color Color = Color.Red;

        public readonly TgcMesh Mesh;
        public MeshPart(TgcMesh mesh)
        {
            Mesh = mesh;
            Mesh.AutoTransformEnable = false;
            Mesh.AutoUpdateBoundingBox = false;
        }

        public abstract void Attach(Item item);
        public abstract void Detach(Item item);

        public virtual void Render(Item item, Effect shader)
        {
            if (MeshPart.IsVisible)
            {
                Mesh.Effect = shader;
                Mesh.render();
            }
        }
        public void Dispose()
        {
            if (Mesh.Enabled)
                Mesh.dispose();
        }
    }
}
