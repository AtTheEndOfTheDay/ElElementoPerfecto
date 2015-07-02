using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public abstract partial class Item : IGameComponent, IDisposable
    {
        #region Constants
        public const Int32 EffectVolume = -500;
        public const Single ScaleSizeFactor = 1f / 8f;
        public const Single BuildScalingSpeed = 4.5f;
        public const Single BuildRotationSpeed = 1.5f;
        public const Single BuildTranslationSpeed = 4.5f;

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
                OnScaleChanged();
            }
        }
        protected virtual void OnScaleChanged()
        {
            RiseEvent(ScaleChanged);
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
                OnRotationChanged();
            }
        }
        protected virtual void OnRotationChanged()
        {
            RiseEvent(RotationChanged);
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
                OnPositionChanged();
            }
        }
        protected virtual void OnPositionChanged()
        {
            RiseEvent(PositionChanged);
        }
        #endregion TransformProperties

        #region Parts
        protected Boolean IsEmptyOfParts { get { return (_Parts.Count == 0); } }
        public ItemPart[] Parts { get { return _Parts.ToArray(); } }
        private ICollection<ItemPart> _Parts = new List<ItemPart>();
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
        public void ClearParts()
        {
            Remove(_Parts.ToArray());
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
        protected Boolean IsEmptyOfColliders { get { return (_Colliders.Count == 0); } }
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
        public void ClearColiders()
        {
            Remove(_Colliders.ToArray());
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
            if (collisions.Count == 0) return null;
            var itemCollision = new ItemCollision(this, interactive, collisions.ToArray());
            OnCollision(itemCollision);
            return itemCollision;
        }
        #endregion Colliders

        #region InteractionMethods
        public virtual void Signal(Object[] signal) { }
        public virtual void FindSiblings(Item[] items) { }
        public virtual void Build(Single deltaTime) { }
        public virtual void Animate(Single deltaTime) { }
        public virtual void StaticCollision(Item item) { }
        public virtual void Act(Interactive interactive, Single deltaTime) { }
        protected virtual void OnCollision(ItemCollision itemCollision) { }
        public virtual Boolean React(ItemContactState contactState, Single deltaTime)
        {
            var reacted = false;
            if (contactState.TimesReacted == 0)
            {
                OnContact(contactState, deltaTime);
                reacted = true;
            }
            if (contactState.Weight > 0)
            {
                OnRestingContact(contactState);
                reacted = true;
            }
            if (contactState.Approach > 0)
            {
                OnRestitutiveContact(contactState);
                reacted = true;
            }
            return reacted;
        }
        protected virtual void OnContact(ItemContactState contactState, Single deltaTime)
        {
            contactState.ApplyMinimumTranslation();
        }
        protected virtual void OnRestitutiveContact(ItemContactState contactState)
        {
            contactState.ApplyRestitution();
        }
        protected virtual void OnRestingContact(ItemContactState contactState)
        {
            contactState.ApplyFriction();
        }
        public virtual void Simulate(Single deltaTime) { }
        #endregion InteractionMethods
    }
}
