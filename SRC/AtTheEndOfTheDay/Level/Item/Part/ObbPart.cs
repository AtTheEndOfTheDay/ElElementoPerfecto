using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using Microsoft.DirectX;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class ObbPart : ItemPart
    {
        #region Constructors
        public readonly TgcObb Obb;
        public readonly Vector3 Extents;
        public ObbPart(TgcMesh mesh)
            : this(TgcObb.computeFromAABB(mesh.BoundingBox)) { }
        public ObbPart(TgcObb obb)
        {
            Obb = obb;
            Extents = obb.Extents;
            obb.setRenderColor(MeshPart.Color);
        }
        #endregion Constructors

        #region PartMethods
        public void Attach(Item item)
        {
            Detach(item);
            item.ScaleChanged += ItemScaleChanged;
            item.RotationChanged += ItemRotationChanged;
            item.PositionChanged += ItemPositionChanged;
        }
        public void Detach(Item item)
        {
            item.ScaleChanged -= ItemScaleChanged;
            item.RotationChanged -= ItemRotationChanged;
            item.PositionChanged -= ItemPositionChanged;
        }
        public void Render(Item item, Effect shader)
        {
            if (Collider.IsVisible)
            {
                //Obb.Effect = shader;
                Obb.render();
            }
        }
        public void Dispose()
        {
            Obb.dispose();
        }

        public Vector3 Scale = Item.DefaultScale;
        public Vector3 Position = Item.DefaultPosition;
        public Matrix Orientation
        {
            get { return Obb.GetOrientation(); }
            set { Obb.SetOrientation(value); }
        }

        protected virtual void ItemScaleChanged(Item item)
        {
            Obb.Extents = Extents.MemberwiseMult(Scale = item.Scale);
        }
        protected virtual void ItemRotationChanged(Item item)
        {
            Orientation = item.RotationMatrix;
        }
        protected virtual void ItemPositionChanged(Item item)
        {
            Obb.Center = Position = item.Position;
        }
        #endregion PartMethods
    }
}
