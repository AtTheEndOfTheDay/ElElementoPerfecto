using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract class MeshPart : ItemPart
    {
        #region Constructors
        public readonly TgcMesh Mesh;
        private readonly String _Technique;
        public MeshPart(TgcMesh mesh)
        {
            Mesh = mesh;
            mesh.AlphaBlendEnable = true;
            mesh.AutoTransformEnable = false;
            mesh.AutoUpdateBoundingBox = false;
            _Technique = mesh.Technique;
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
                Mesh.Technique = Game.Current.IsToonShaderEnabled ? "NormalMap" : _Technique;
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
