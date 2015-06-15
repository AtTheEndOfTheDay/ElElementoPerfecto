using System;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class SphereCollider : Collider
    {
        #region Constructors
        public Single Radius;
        public readonly TgcBoundingSphere Sphere;
        public SphereCollider(TgcMesh mesh)
            : this(TgcBoundingSphere.computeFromMesh(mesh)) { }
        public SphereCollider(TgcBoundingSphere sphere)
        {
            Sphere = sphere;
            Radius = sphere.Radius;
            sphere.setRenderColor(Collider.Color);
        }
        #endregion Constructors

        #region PartMethods
        public override void Attach(Item item)
        {
            Detach(item);
            item.ScaleChanged += ItemScaleChanged;
            item.PositionChanged += ItemPositionChanged;
        }
        public override void Detach(Item item)
        {
            item.ScaleChanged -= ItemScaleChanged;
            item.PositionChanged -= ItemPositionChanged;
        }
        public override void Render(Item item, Effect shader)
        {
            if (Collider.IsVisible)
            {
                //TODO:Sphere.Effect = shader;
                Sphere.render();
            }
        }
        public override void Dispose()
        {
            Sphere.dispose();
        }

        public Single Scale = GetSingleScale(Item.DefaultScale);
        public Vector3 Position = Item.DefaultPosition;

        protected virtual void ItemScaleChanged(Item item)
        {
            Scale = GetSingleScale(item.Scale);
            Sphere.setValues(Sphere.Center, Radius * Scale);
        }
        protected virtual void ItemPositionChanged(Item item)
        {
            Sphere.setCenter(Position = item.Position);
        }
        protected static Single GetSingleScale(Vector3 s)
        {
            return Math.Max(Math.Max(s.X, s.Y), s.Z);
        }
        #endregion PartMethods

        #region ColliderMethods
        public override Boolean Intercepts(TgcRay ray)
        {
            return TgcCollisionUtils.testRaySphere(ray, Sphere);
        }
        public override Boolean Collides(Collider other)
        {
            var sc = other as SphereCollider;
            if (sc != null)
                return TgcCollisionUtils.testSphereSphere(this.Sphere, sc.Sphere);
            return base.Collides(other);
        }
        public override Collision Collide(Collider other)
        {
            var sc = other as SphereCollider;
            if (sc != null)
                return _SphereCollide(sc);
            return base.Collide(other);
        }
        private Collision _SphereCollide(SphereCollider sc)
        {
            var center = Sphere.Center;
            var radius = Sphere.Radius;
            var scCenter = sc.Sphere.Center;
            var scRadius = sc.Sphere.Radius;
            var normal = scCenter - center;
            var depth = scRadius + radius - normal.Length();
            if (depth < 0) return null;
            normal.Normalize();
            return new Collision(this, sc
                , new Contact(
                    center + radius * normal,
                    scCenter - scRadius * normal,
                    normal, depth
           ));
        }
        #endregion ColliderMethods
    }
}
