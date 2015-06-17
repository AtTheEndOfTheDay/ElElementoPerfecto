using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class AnimatedQuad : IDisposable
    {
        #region Constructors
        public Boolean _IsEnabled;
        private Single _CurrentTime;
        public AnimatedQuad()
        {
            IsPlaying = true;
        }
        #endregion Constructors

        #region Properties
        public Int32 FirstFrame { get; set; }
        public Int32 CurrentFrame { get; set; }
        public Boolean IsPlaying { get; set; }
        private Single _AnimationLength;
        private void UpdateAnimationLength()
        {
            _AnimationLength = _FrameRate == 0 ? 0 : (Single)_TotalFrames / _FrameRate;
        }
        private Int32 _TotalFrames;
        public Int32 TotalFrames
        {
            get { return _TotalFrames; }
            set
            {
                if (_TotalFrames == value) return;
                _TotalFrames = value;
                UpdateAnimationLength();
            }
        }
        private Single _FrameRate;
        public Single FrameRate
        {
            get { return _FrameRate; }
            set
            {
                if (_FrameRate == value) return;
                _FrameRate = value;
                UpdateAnimationLength();
            }
        }
        private TexturedQuad _TexturedQuad = new TexturedQuad() { IsAlphaBlendEnabled = true };
        private Size _TextureSize = new Size(1, 1);
        public TgcTexture Texture
        {
            get { return _TexturedQuad.Texture; }
            set
            {
                _TexturedQuad.Texture = value;
                _TextureSize = value == null ? new Size(1, 1) : value.Size;
            }
        }
        public Vector3 Position
        {
            get { return _TexturedQuad.Position; }
            set { _TexturedQuad.Position = value; }
        }
        public Vector2 Size
        {
            get { return _TexturedQuad.Size; }
            set { _TexturedQuad.Size = value; }
        }
        public Matrix RotationMatrix
        {
            get { return _TexturedQuad.RotationMatrix; }
            set { _TexturedQuad.RotationMatrix = value; }
        }
        private Size _TileInverse = new Size(1, 1);
        private Size _FrameSize = new Size(1, 1);
        private Int32 _RealTotalFrames = 1;
        public Size FrameSize
        {
            get { return _FrameSize; }
            set
            {
                if (_FrameSize == value) return;
                _FrameSize = value;
                var tile = _TexturedQuad.Tile = new Vector2((Single)value.Width / (Single)_TextureSize.Width, (Single)value.Height / (Single)_TextureSize.Height);
                _TileInverse = new Size((Int32)(1 / tile.X), (Int32)(1 / tile.Y));
                _RealTotalFrames = _TileInverse.Width * _TileInverse.Height;
            }
        }
        #endregion Properties

        #region AnimationMethods
        public void Update(Single deltaTime)
        {
            if (!_IsEnabled) return;
            if (IsPlaying)
            {
                _CurrentTime += deltaTime;
                if (_TotalFrames != 0 && _CurrentTime > _AnimationLength)
                {
                    _CurrentTime = 0;
                    _IsEnabled = false;
                }
            }
            //Obtener cuadro actual
            if (CurrentFrame != ((Int32)(_CurrentTime * FrameRate) + FirstFrame) % _RealTotalFrames)
            {
                CurrentFrame = (Int32)(_CurrentTime * FrameRate) + FirstFrame % _RealTotalFrames;
                _TexturedQuad.UVOffset = new Vector2(_TexturedQuad.Tile.X * (CurrentFrame % _TileInverse.Width), _TexturedQuad.Tile.Y * (CurrentFrame / _TileInverse.Height));
            }
        }
        public void Start()
        {
            _CurrentTime = 0;
            _IsEnabled = true;
        }
        public void Stop()
        {
            _CurrentTime = 0;
            _IsEnabled = false;
        }
        public void Render()
        {
            if (!_IsEnabled) return;
            _TexturedQuad.Render();
        }
        public void Dispose()
        {
            _TexturedQuad.Dispose();
        }
        #endregion AnimationMethods
    }
}
