using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class MeshTransformedPart : MeshPart
    {
        #region Constructors
        public MeshTransformedPart(TgcMesh mesh)
            : base(mesh)
        {
            Scale = Item.DefaultScale;
            Rotation = Item.DefaultRotation;
            Position = Item.DefaultPosition;
            RotationMatrix = Item.DefaultRotationMatrix;
        }
        #endregion Constructors

        #region PartMethods
        public override void Attach(Item item)
        {
            Detach(item);
            item.ScaleChanged += Item_ScaleChanged;
            item.RotationChanged += Item_RotationChanged;
            item.PositionChanged += Item_PositionChanged;
        }
        public override void Detach(Item item)
        {
            item.ScaleChanged -= Item_ScaleChanged;
            item.RotationChanged -= Item_RotationChanged;
            item.PositionChanged -= Item_PositionChanged;
        }
        public override void Render(Item item, Effect shader)
        {
            Mesh.Transform = Matrix.Scaling(Scale)
                           * Matrix.RotationYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                           * Matrix.Translation(Position);
            base.Render(item, shader);
        }

        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Position { get; set; }
        public Matrix RotationMatrix { get; set; }

        protected virtual void Item_ScaleChanged(Item item)
        {
            Scale = item.Scale;
        }
        protected virtual void Item_RotationChanged(Item item)
        {
            Rotation = item.Rotation;
            RotationMatrix = item.RotationMatrix;
        }
        protected virtual void Item_PositionChanged(Item item)
        {
            Position = item.Position;
        }
        #endregion PartMethods
    }
}
