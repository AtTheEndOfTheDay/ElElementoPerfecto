using System;
using System.Drawing;
using Microsoft.DirectX;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Ball : Interactive
    {
        #region Constructors
        public Ball()
        {
            var mesh = Game.Current.NewMesh("Ball");
            _MeshTextured = new MeshTemporalRecursivePart(Game.Current.NewMesh("BallTextured"));
            Add(_Mesh = new MeshTemporalRecursivePart(mesh));
            Add(new SphereCollider(mesh));
        }
        #endregion Constructors

        #region Properties
        private MeshTemporalRecursivePart _Mesh;
        public Color Color
        {
            get { return _Mesh.Color; }
            set { _Mesh.Color = value; }
        }
        private MeshTemporalRecursivePart _MeshTextured;
        private String _Texture;
        public String Texture
        {
            get { return _Texture; }
            set
            {
                if (_Texture == value) return;
                _Texture = value;
                if (String.IsNullOrWhiteSpace(value))
                    _SwapMeshes(false);
                else
                {
                    try
                    {
                        _MeshTextured.Texture = Game.Current.GetMaterial(value);
                        _SwapMeshes(true);
                    }
                    catch { _SwapMeshes(false); }
                }
            }
        }
        private Boolean _IsTextured = false;
        private void _SwapMeshes(Boolean isTextured)
        {
            if (_IsTextured == isTextured) return;
            if (_IsTextured = isTextured)
            {
                Add(_MeshTextured);
                Remove(_Mesh);
            }
            else
            {
                Add(_Mesh);
                Remove(_MeshTextured);
            }
        }
        #endregion Properties

        #region InteractiveMethods
        protected override Matrix ComputeBody()
        {
            var m_5 = .2f * Mass;
            var a2 = Scale.X * Scale.X;
            var b2 = Scale.Y * Scale.Y;
            var c2 = Scale.Z * Scale.Z;
            return new Matrix()
            {
                M11= m_5 * (b2 + c2),  M12= 0,                M13= 0,
                M21= 0,                M22= m_5 * (a2 + c2),  M23= 0,
                M31= 0,                M32= 0,                M33= m_5 * (a2 + b2),
                M44= 1,
            };
        }
        #endregion InteractiveMethods

        #region ResetMethods
        public override void LoadValues()
        {
            base.LoadValues();
            if (_IsTextured)
                _MeshTextured.Clear();
            else _Mesh.Clear();
        }
        #endregion ResetMethods

        #region ItemMethods
        public override void Animate(Single deltaTime)
        {
            if (_IsTextured)
                _MeshTextured.Update(deltaTime);
            else _Mesh.Update(deltaTime);
        }
        #endregion ItemMethods
    }
}
