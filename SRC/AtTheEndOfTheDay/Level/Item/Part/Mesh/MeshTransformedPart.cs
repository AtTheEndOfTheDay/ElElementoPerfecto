using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshTransformedPart : MeshPart
    {
        public MeshTransformedPart(TgcMesh mesh)
            : base(mesh) { }

        #region PartMethods
        public override void Attach(Item item)
        {
            Detach(item);
            item.ScaleChanged += ItemScaleChanged;
            item.RotationChanged += ItemRotationChanged;
            item.PositionChanged += ItemPositionChanged;
        }
        public override void Detach(Item item)
        {
            item.ScaleChanged -= ItemScaleChanged;
            item.RotationChanged -= ItemRotationChanged;
            item.PositionChanged -= ItemPositionChanged;
        }
        public override void Render(Item item, Effect shader)
        {
            Mesh.Transform = Matrix.Scaling(Scale)
                           * Matrix.RotationYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                           * Matrix.Translation(Position);
            base.Render(item, shader);
        }

        public Vector3 Scale = Item.DefaultScale;
        public Vector3 Rotation = Item.DefaultRotation;
        public Vector3 Position = Item.DefaultPosition;

        protected virtual void ItemScaleChanged(Item item)
        {
            Scale = item.Scale;
        }
        protected virtual void ItemRotationChanged(Item item)
        {
            Rotation = item.Rotation;
        }
        protected virtual void ItemPositionChanged(Item item)
        {
            Position = item.Position;
        }
        #endregion PartMethods
    }
}
