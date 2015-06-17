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
    public abstract partial class Item : IGameComponent, IDisposable
    {
        #region Constants
        public const Single ScaleSizeFactor = 1f / 8f;
        public const Single BuildScalingSpeed = 4.5f;
        public const Single BuildRotationSpeed = 1.5f;
        public const Single BuildTranslationSpeed = 4.5f;
        public const int EffectVolume = 0;

        public static readonly Vector3 DefaultScale = Vector3Extension.One;
        public static readonly Vector3 DefaultRotation = Vector3.Empty;
        public static readonly Vector3 DefaultPosition = Vector3.Empty;
        public static readonly Matrix DefaultScaleMatrix = Matrix.Scaling(DefaultScale);
        public static readonly Matrix DefaultRotationMatrix = Matrix.RotationYawPitchRoll(DefaultRotation.X, DefaultRotation.Y, DefaultRotation.Z);
        public static readonly Matrix DefaultPositionMatrix = Matrix.Translation(DefaultPosition);
        #endregion Constants

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
        #endregion Constructors

        #region Properties
        private String _Name = String.Empty;
        public String Name
        {
            get { return _Name; }
            set { _Name = value ?? String.Empty; }
        }
        private String _Properties = String.Empty;
        public String Properties
        {
            get { return _Properties; }
            set { _Properties = value ?? String.Empty; }
        }
        #endregion Properties

        #region ResetMethods
        protected Vector3 SavedScale;
        protected Vector3 SavedRotation;
        protected Vector3 SavedPosition;
        public virtual void SaveValues()
        {
            SavedScale = _Scale;
            SavedRotation = _Rotation;
            SavedPosition = _Position;
        }
        public virtual void LoadValues()
        {
            Scale = SavedScale;
            Rotation = SavedRotation;
            Position = SavedPosition;
        }
        private Vector3 _MenuScaleCache = Vector3.Empty;
        public virtual void MenuTransform(Vector3 scale, Vector3 rotation, Vector3 position)
        {
            if (Scale != _MenuScaleCache)
            {
                var factor = scale.MemberwiseDiv(Scale).MinCoordinate();
                _MenuScaleCache = Scale = Math.Min(1, factor) * Scale;
            }
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

        #region Parts
        public ItemPart[] Parts { get { return _Parts.ToArray(); } }
        private ICollection<ItemPart> _Parts = new List<ItemPart>();
        protected Boolean emptyParts()
        {
            return (_Parts.Count == 0);
        }
        public ItemPart Add(ItemPart part)
        {
            _Parts.Add(part);
            part.Attach(this);
            return part;
        }
        public void Add(IEnumerable<ItemPart> parts)
        {
            foreach (var part in parts)
                Add(part);
        }
        public ItemPart Remove(ItemPart part)
        {
            _Parts.Remove(part);
            part.Detach(this);
            return part;
        }
        public void Remove(IEnumerable<ItemPart> parts)
        {
            foreach (var part in parts)
                Remove(part);
        }

        public Matrix RenderMatrix { get; private set; }
        public virtual void Render(Dx3D.Effect shader)
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
        #endregion Parts

        #region Colliders
        public Collider[] Colliders { get { return _Colliders.ToArray(); } }
        private ICollection<Collider> _Colliders = new List<Collider>();
        public Collider Add(Collider collider)
        {
            _Colliders.Add(collider);
            Add(collider as ItemPart);
            return collider;
        }
        public void Add(IEnumerable<Collider> colliders)
        {
            foreach (var collider in colliders)
                Add(collider);
        }
        public Collider Remove(Collider collider)
        {
            _Colliders.Remove(collider);
            Remove(collider as ItemPart);
            return collider;
        }
        public void Remove(IEnumerable<Collider> colliders)
        {
            foreach (var collider in colliders)
                Remove(collider);
        }

        public Boolean Intercepts(TgcRay ray)
        {
            return _Colliders.Any(c => c.Intercepts(ray));
        }
        public Boolean Collides(Item other)
        {
            return _Colliders.Any(c => other._Colliders.Any(oc => c.Collides(oc)));
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
        #endregion Colliders

        #region InteractionMethods
        public virtual void FindSiblings(Item[] items) { }
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
                        interactive.AddForceAt(r, weight * (normal - collision.Friction * ortho));
                        reacted = true;
                    }
                }
            return reacted;
        }
        #endregion InteractionMethods
    }
}
