using System;
using System.Drawing;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class MeshPart : ItemPart
    {
        #region Constructors
        public readonly TgcMesh Mesh;
        public MeshPart(TgcMesh mesh)
        {
            Mesh = mesh;
            Mesh.AutoTransformEnable = false;
            Mesh.AutoUpdateBoundingBox = false;
        }
        #endregion Constructors

        #region Properties
        public override Color Color
        {
            get { return base.Color; }
            set { base.Color = value; if (Mesh != null) Mesh.setColor(value); }
        }
        public TgcTexture Texture
        {
            get
            {
                if (Mesh.DiffuseMaps == null || Mesh.DiffuseMaps.Length < 1)
                    return null;
                return Mesh.DiffuseMaps[0];
            }
            set
            {
                if (Mesh.DiffuseMaps == null || Mesh.DiffuseMaps.Length < 1)
                    return;
                Mesh.DiffuseMaps[0].dispose();
                Mesh.DiffuseMaps[0] = value;
            }
        }
        #endregion Properties

        #region PartMethods
        public override void Render(Item item, Effect shader)
        {
            if (Game.Current.IsMeshVisible)
            {
                Mesh.Effect = shader;
                Mesh.render();
            }
        }
        public override void Dispose()
        {
            if (Mesh.Enabled)
                Mesh.dispose();
        }
        #endregion PartMethods
    }
}
