using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class ObbCollider : Collider
    {
        #region Constructors
        public Vector3 Extents;
        public readonly TgcObb Obb;
        public ObbCollider(TgcMesh mesh)
            : this(TgcObb.computeFromAABB(mesh.BoundingBox)) { }
        public ObbCollider(TgcObb obb)
        {
            Obb = obb;
            Extents = obb.Extents;
            obb.setRenderColor(Collider.Color);
        }
        #endregion Constructors

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
            if (Collider.IsVisible)
            {
                //TODO:Obb.Effect = shader;
                Obb.render();
            }
        }
        public override void Dispose()
        {
            Obb.dispose();
        }

        public Vector3 Scale = Item.DefaultScale;
        public Vector3 Position = Item.DefaultPosition;
        public Vector3 Rotation = Item.DefaultRotation;

        protected virtual void ItemScaleChanged(Item item)
        {
            Obb.Extents = Extents.MemberwiseMult(Scale = item.Scale);
        }
        protected virtual void ItemRotationChanged(Item item)
        {
            Rotation = item.Rotation;
            Obb.SetOrientation(item.RotationMatrix);
        }
        protected virtual void ItemPositionChanged(Item item)
        {
            Obb.Center = Position = item.Position;
        }
        #endregion PartMethods

        #region ColliderMethods
        public override Boolean Intercepts(TgcRay ray)
        {
            var v = new Vector3();
            return TgcCollisionUtils.intersectRayObb(ray, Obb, out v);
        }
        public override Boolean Collides(Collider other)
        {
            var sc = other as SphereCollider;
            if (sc != null)
                return TgcCollisionUtils.testSphereOBB(sc.Sphere, Obb);
            var oc = other as ObbCollider;
            if (oc != null)
                return TgcCollisionUtils.testObbObb(oc.Obb, Obb);
            return base.Collides(other);
        }
        public override Collision Collide(Collider other)
        {
            var sc = other as SphereCollider;
            if (sc != null)
                return _SphereCollide(sc);
            var oc = other as ObbCollider;
            if (oc != null)
                return _ObbCollide(oc);
            return base.Collide(other);
        }
        private Collision _SphereCollide(SphereCollider sc)
        {
            var center = sc.Sphere.Center;
            var radius = sc.Sphere.Radius;
            var closest = Obb.ClosestPoint(center);
            var normal = center - closest;
            var depth = radius - normal.Length();
            if (depth < 0) return null;
            normal.Normalize();
            return new Collision(this, sc
                , new Contact(closest, center - radius * normal, normal, depth)
            );
        }
        private Collision _ObbCollide(ObbCollider oc)
        {
            //TODO: Obb Collide Obb
            return null;
        }
        #endregion ColliderMethods
    }
}
