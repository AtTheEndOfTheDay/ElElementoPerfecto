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
        protected readonly TgcObb Obb;
        public ObbCollider(Game game)
            : this(game, new TgcObb() { Extents = Item.DefaultScale })
        {
            Obb.SetOrientation();
        }
        public ObbCollider(Game game, TgcMesh mesh)
            : this(game, TgcObb.computeFromAABB(mesh.BoundingBox)) { }
        public ObbCollider(Game game, TgcBoundingBox aabb)
            : this(game, TgcObb.computeFromAABB(aabb)) { }
        public ObbCollider(Game game, TgcObb obb)
            :base(game)
        {
            Obb = obb;
            _Extents = obb.Extents;
            obb.setRenderColor(Collider.DefaultColiderColor);
        }
        #endregion Constructors

        #region Properties
        public override Color Color
        {
            get { return base.Color; }
            set { Obb.setRenderColor(base.Color = value); }
        }
        private Vector3 _Extents;
        public Vector3 Extents
        {
            get { return _Extents; }
            set
            {
                if (_Extents == value) return;
                Obb.Extents = Obb.Extents
                    .MemberwiseMult(value)
                    .MemberwiseDiv(_Extents);
                _Extents = value;
            }
        }
        public Vector3[] Orientation
        {
            get { return Obb.Orientation; }
            set { Obb.Orientation = value; }
        }
        #endregion Properties

        #region PartMethods
        public override void Attach(Item item)
        {
            Detach(item);
            Item_ScaleChanged(item);
            Item_RotationChanged(item);
            Item_PositionChanged(item);
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

        protected virtual void Item_ScaleChanged(Item item)
        {
            Obb.Extents = _Extents.MemberwiseMult(Scale = item.Scale);
        }
        protected virtual void Item_RotationChanged(Item item)
        {
            Rotation = item.Rotation;
            Obb.SetOrientation(item.RotationMatrix);
        }
        protected virtual void Item_PositionChanged(Item item)
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
