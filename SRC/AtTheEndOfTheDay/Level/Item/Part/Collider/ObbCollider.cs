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
        public readonly TgcObb Obb;
        public ObbCollider()
            : this(new TgcObb() { Extents = Item.DefaultScale })
        {
            Obb.SetOrientation();
        }
        public ObbCollider(TgcMesh mesh)
            : this(TgcObb.computeFromAABB(mesh.BoundingBox)) { }
        public ObbCollider(TgcBoundingBox aabb)
            : this(TgcObb.computeFromAABB(aabb)) { }
        public ObbCollider(TgcObb obb)
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
            set { base.Color = value; if (Obb != null) Obb.setRenderColor(value); }
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
        public Vector3 Right
        {
            get { return Obb.Orientation[0]; }
            set { Obb.Orientation[0] = value; }
        }
        public Vector3 Left
        {
            get { return -Right; }
            set { Right = -value; }
        }
        public Vector3 Top
        {
            get { return Obb.Orientation[1]; }
            set { Obb.Orientation[1] = value; }
        }
        public Vector3 Bottom
        {
            get { return -Top; }
            set { Top = -value; }
        }
        public Vector3 Front
        {
            get { return Obb.Orientation[2]; }
            set { Obb.Orientation[2] = value; }
        }
        public Vector3 Back
        {
            get { return -Front; }
            set { Front = -value; }
        }
        public void SetOrientation()
        {
            Obb.SetOrientation();
        }
        public void SetOrientation(Matrix orientation)
        {
            Obb.SetOrientation(orientation);
        }
        public void SetOrientation(Vector3 rotation)
        {
            Obb.SetOrientation(rotation);
        }
        public void SetOrientation(Vector3 o0, Vector3 o1, Vector3 o2)
        {
            Obb.SetOrientation(o0, o1, o2);
        }
        public Vector3 ClosestPoint(Vector3 v)
        {
            return Obb.ClosestPoint(v);
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
            if (Game.Current.IsColliderVisible)
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
