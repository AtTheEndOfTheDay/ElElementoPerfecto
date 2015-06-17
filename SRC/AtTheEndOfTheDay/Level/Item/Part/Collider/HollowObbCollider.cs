using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class HollowObbCollider : CompositeCollider
    {
        #region Constructors
        #region MeshConstructors
        public HollowObbCollider(TgcMesh mesh, Vector3 cornerScale)
            : this(mesh, cornerScale, cornerScale) { }
        public HollowObbCollider(TgcMesh mesh, Vector3 minCornerScale, Vector3 maxCornerScale)
        {
            var obb = TgcObb.computeFromAABB(mesh.BoundingBox);
            _HollowObbCollider(obb, obb.Center - mesh.Position, minCornerScale, maxCornerScale);
            obb.dispose();
        }
        #endregion MeshConstructors

        #region AabbConstructors
        public HollowObbCollider(TgcBoundingBox bb, Vector3 cornerScale)
            : this(bb, Vector3.Empty, cornerScale, cornerScale) { }
        public HollowObbCollider(TgcBoundingBox bb, Vector3 translation, Vector3 cornerScale)
            : this(bb, translation, cornerScale, cornerScale) { }
        public HollowObbCollider(TgcBoundingBox bb, Vector3 translation, Vector3 minCornerScale, Vector3 maxCornerScale)
        {
            var obb = TgcObb.computeFromAABB(bb);
            _HollowObbCollider(obb, translation, minCornerScale, maxCornerScale);
            obb.dispose();
        }
        #endregion AabbConstructors

        #region ObbConstructors
        public HollowObbCollider(TgcObb obb, Vector3 cornerScale)
            : this(obb, Vector3.Empty, cornerScale, cornerScale) { }
        public HollowObbCollider(TgcObb obb, Vector3 translation, Vector3 cornerScale)
            : this(obb, translation, cornerScale, cornerScale) { }
        public HollowObbCollider(TgcObb obb, Vector3 translation, Vector3 minCornerScale, Vector3 maxCornerScale)
        {
            _HollowObbCollider(obb, translation, minCornerScale, maxCornerScale);
        }
        #endregion ObbConstructors

        #region DefaultConstructor
        private void _HollowObbCollider(TgcObb obb, Vector3 translation, Vector3 minCornerScale, Vector3 maxCornerScale)
        {
            var e = obb.Extents;
            var o = obb.Orientation;
            var min = e.MemberwiseMult(minCornerScale);
            if (min.X > 0)
                _AddCollider(o, new Vector3(min.X, e.Y, e.Z), (e.X - min.X) * Vector3Extension.Left + translation);
            if (min.Y > 0)
                _AddCollider(o, new Vector3(e.X, min.Y, e.Z), (e.Y - min.Y) * Vector3Extension.Bottom + translation);
            if (min.Z > 0)
                _AddCollider(o, new Vector3(e.X, e.Y, min.Z), (e.Z - min.Z) * Vector3Extension.Back + translation);
            var max = e.MemberwiseMult(maxCornerScale);
            if (max.X > 0)
                _AddCollider(o, new Vector3(max.X, e.Y, e.Z), (e.X - max.X) * Vector3Extension.Right + translation);
            if (max.Y > 0)
                _AddCollider(o, new Vector3(e.X, max.Y, e.Z), (e.Y - max.Y) * Vector3Extension.Top + translation);
            if (max.Z > 0)
                _AddCollider(o, new Vector3(e.X, e.Y, max.Z), (e.Z - max.Z) * Vector3Extension.Front + translation);
        }
        private void _AddCollider(Vector3[] orientation, Vector3 extents, Vector3 translation)
        {
            Add(new ObbTranslatedCollider(new TgcObb()
            {
                Orientation = orientation,
                Extents = extents,
                Center = translation,
            }, translation));
        }
        #endregion DefaultConstructor
        #endregion Constructors
    }
}
