using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract partial class Item : IDisposable
    {
        #region DefaultTransformation
        protected const Single BuildRotationSpeed = 1.5f;
        protected const Single BuildScalingSpeed = 4.5f;
        protected static readonly Vector3 BuildMinScale = .2f * Vector3Extension.One;

        public static readonly Vector3 DefaultScale = new Vector3(1, 1, 1);
        public static readonly Vector3 DefaultRotation = Vector3.Empty;
        public static readonly Vector3 DefaultPosition = Vector3.Empty;
        public static readonly Matrix DefaultScaleMatrix = Matrix.Scaling(DefaultScale);
        public static readonly Matrix DefaultRotationMatrix = Matrix.RotationYawPitchRoll(DefaultRotation.X, DefaultRotation.Y, DefaultRotation.Z);
        public static readonly Matrix DefaultPositionMatrix = Matrix.Translation(DefaultPosition);
        #endregion DefaultTransformation

        #region Item.EventHandler
        public delegate void EventHandler(Item item);
        private void RiseEvent(EventHandler eventHandler)
        {
            if (eventHandler != null)
                eventHandler(this);
        }
        #endregion Item.EventHandler

        #region Constructors
        public Item()
        {
            ScaleMatrix = DefaultScaleMatrix;
            RotationMatrix = DefaultRotationMatrix;
            PositionMatrix = DefaultPositionMatrix;
        }
        public virtual void Init(IList<Item> gameItems, IList<Item> userItems)
        {
            SaveValues();
        }
        #endregion Constructors

        #region ResetMethods
        private Vector3 _SavedScale;
        private Vector3 _SavedRotation;
        private Vector3 _SavedPosition;
        public virtual void SaveValues()
        {
            _SavedScale = _Scale;
            _SavedRotation = _Rotation;
            _SavedPosition = _Position;
        }
        public virtual void LoadValues()
        {
            Scale = _SavedScale;
            Rotation = _SavedRotation;
            Position = _SavedPosition;
        }
        public virtual void MenuTransform(Vector3 rotation, Vector3 position)
        {
            Scale = DefaultScale;
            Rotation = rotation;
            Position = position;
        }
        #endregion ResetMethods

        #region TransformProperties
        public Matrix ScaleMatrix { get; private set; }
        private Vector3 _Scale = DefaultScale;
        public event EventHandler ScaleChanged;
        public Vector3 Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale == value) return;
                _Scale = value;
                ScaleMatrix = Matrix.Scaling(value);
                RiseEvent(ScaleChanged);
            }
        }
        public Matrix RotationMatrix { get; private set; }
        private Vector3 _Rotation = DefaultRotation;
        public event EventHandler RotationChanged;
        public Vector3 Rotation
        {
            get { return _Rotation; }
            set
            {
                if (_Rotation == value) return;
                _Rotation = value;
                RotationMatrix = Matrix.RotationYawPitchRoll(value.Y, value.X, value.Z);
                RiseEvent(RotationChanged);
            }
        }
        public Matrix PositionMatrix { get; private set; }
        private Vector3 _Position = DefaultPosition;
        public event EventHandler PositionChanged;
        public Vector3 Position
        {
            get { return _Position; }
            set
            {
                if (_Position == value) return;
                _Position = value;
                PositionMatrix = Matrix.Translation(value);
                RiseEvent(PositionChanged);
            }
        }
        #endregion TransformProperties

        #region PartMethods
        private ICollection<IPart> _Parts = new List<IPart>();
        public void Add(IPart part)
        {
            _Parts.Add(part);
            part.Attach(this);
        }
        public void Remove(IPart part)
        {
            _Parts.Remove(part);
            part.Detach(this);
        }

        public Matrix RenderMatrix { get; private set; }
        public virtual void Render(Dx3D.Effect shader = null)
        {
            RenderMatrix = ScaleMatrix * RotationMatrix * PositionMatrix;
            foreach (var p in _Parts)
                p.Render(this, shader);
        }
        public virtual void Dispose()
        {
            foreach (var p in _Parts)
                p.Dispose();
        }
        #endregion PartMethods

        #region ColliderMethods
        private ICollection<Collider> _Colliders = new List<Collider>();
        public void Add(Collider collider)
        {
            _Colliders.Add(collider);
            Add(collider as IPart);
        }
        public void Remove(Collider collider)
        {
            _Colliders.Remove(collider);
            Add(collider as IPart);
        }

        public Boolean Intercepts(TgcRay ray)
        {
            return _Colliders.Any(c => c.Intercepts(ray));
        }
        public Boolean Collides(Item other)
        {
            foreach (var c in _Colliders)
                foreach (var oc in other._Colliders)
                    if (c.Collides(oc))
                        return true;
            return false;
        }
        public ItemCollision Collide(Interactive interactive)
        {
            if (this == interactive) return null;
            var collisions = new List<Collision>();
            foreach (var c in _Colliders)
                foreach (var ic in interactive._Colliders)
                {
                    var collision = c.Collide(ic);
                    if (collision != null)
                        collisions.Add(collision);
                }
            return collisions.Count == 0 ? null
                : new ItemCollision(this, interactive, collisions.ToArray());
        }
        #endregion ColliderMethods

        #region InteractionMethods
        public virtual void Build(Single deltaTime) { }
        public virtual void Animate(Single deltaTime) { }
        public virtual void Act(Interactive interactive, Single deltaTime) { }
        public virtual Boolean React(ItemCollision itemCollision, Single deltaTime)
        {
            if (itemCollision.Item != this) return false;
            var reacted = false;
            var interactive = itemCollision.Interactive;
            foreach (var collision in itemCollision.Collisions)
                foreach (var contact in collision.Contacts)
                {
                    var normal = contact.NormalAB;
                    interactive.Position += normal * contact.Depth;
                    var r = contact.PointB - interactive.Position;
                    var velocity = interactive.GetVelocityAt(r);
                    var approachVel = -Vector3.Dot(normal, velocity);
                    if (approachVel > 0)
                    {
                        interactive.AddVelocityAt(r, (1 + collision.Restitution) * approachVel * normal);
                        reacted = true;
                    }
                    var force = interactive.GetForceAt(r);
                    var weight = -Vector3.Dot(normal, force);
                    if (weight > 0)
                    {
                        var ortho = normal.Orthonormal(velocity);
                        //var orthoVel = Vector3.Dot(ortho, velocity);
                        interactive.AddForceAt(r, weight * (normal - collision.Friction * ortho));
                        reacted = true;
                    }
                }
            return reacted;
        }
        #endregion InteractionMethods
    }
}
