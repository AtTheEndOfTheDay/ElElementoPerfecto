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
        protected readonly TgcBoundingSphere Sphere;
        public SphereCollider(TgcMesh mesh)
            : this(TgcBoundingSphere.computeFromMesh(mesh)) { }
        public SphereCollider(TgcBoundingSphere sphere)
        {
            Sphere = sphere;
            _Radius = sphere.Radius;
            sphere.setRenderColor(Collider.DefaultColiderColor);
        }
        #endregion Constructors

        #region Properties
        public override Color Color
        {
            get { return base.DefaultColiderColor; }
            set { Sphere.setRenderColor(base.DefaultColiderColor = value); }
        }
        private Single _Radius;
        public Single Radius
        {
            get { return _Radius; }
            set
            {
                if (_Radius == value) return;
                Sphere.setValues(Sphere.Center, Sphere.Radius * value / _Radius);
                _Radius = value;
            }
        }
        #endregion Properties

        #region PartMethods
        public override void Attach(Item item)
        {
            Detach(item);
            Item_ScaleChanged(item);
            Item_PositionChanged(item);
            item.ScaleChanged += Item_ScaleChanged;
            item.PositionChanged += Item_PositionChanged;
        }
        public override void Detach(Item item)
        {
            item.ScaleChanged -= Item_ScaleChanged;
            item.PositionChanged -= Item_PositionChanged;
        }
        public override void Render(Item item, Effect shader)
        {
            if (Game.Current.IsColliderVisible)
            {
                //TODO:Sphere.Effect = shader;
                Sphere.render();
            }
        }
        public override void Dispose()
        {
            Sphere.dispose();
        }

        protected virtual void Item_ScaleChanged(Item item)
        {
            Scale = Vector3Extension.One * item.Scale.MaxCoordinate();
            Sphere.setValues(Sphere.Center, _Radius * Scale.X);
        }
        protected virtual void Item_PositionChanged(Item item)
        {
            Sphere.setCenter(Position = item.Position);
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
