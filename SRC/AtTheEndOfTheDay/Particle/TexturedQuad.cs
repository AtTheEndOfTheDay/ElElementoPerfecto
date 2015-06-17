using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class TexturedQuad : IDisposable
    {
        #region Constructors
        private Boolean _OldValues = true;
        private CustomVertex.PositionTextured[] _Vertices = new CustomVertex.PositionTextured[6];
        public TexturedQuad()
        {
            Enabled = true;
            IsAlphaBlendEnabled = true;
            Tile = new Vector2(1f, 1f);
            Shader = GuiController.Instance.Shaders.VariosShader;
            Technique = TgcShaders.T_POSITION_TEXTURED;
        }
        #endregion Constructors

        #region Properties
        public Effect Shader { get; set; }
        public String Technique { get; set; }
        public Vector2 Tile { get; set; }
        public Vector2 UVOffset { get; set; }
        public Boolean Enabled { get; set; }
        public Boolean IsAlphaBlendEnabled { get; set; }
        private void _SetAlphaBlend(Boolean isAlphaEnabled)
        {
            Device device = GuiController.Instance.D3dDevice;
            device.RenderState.AlphaTestEnable = isAlphaEnabled;
            device.RenderState.AlphaBlendEnable = isAlphaEnabled;
        }
        private Vector3 _Position;
        public Vector3 Position
        {
            get { return _Position; }
            set { _Position = value; _OldValues = true; }
        }
        private Vector2 _Size;
        public Vector2 Size
        {
            get { return _Size; }
            set { _Size = value; _OldValues = true; }
        }
        private Matrix _RotationMatrix = Matrix.Identity;
        public Matrix RotationMatrix
        {
            get { return _RotationMatrix; }
            set { _RotationMatrix = value; _OldValues = true; }
        }
        private TgcTexture _Texture;
        public TgcTexture Texture
        {
            get { return _Texture; }
            set
            {
                if (_Texture != null)
                    _Texture.dispose();
                _Texture = value;
            }
        }
        public void Dispose()
        {
            Texture = null;
        }
        #endregion Properties

        #region TextureMethods
        public void Render()
        {
            if (!Enabled) return;
            if (_OldValues)
            {
                _UpdateValues();
                _OldValues = false;
            }
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;
            _SetAlphaBlend(IsAlphaBlendEnabled);
            texturesManager.shaderSet(Shader, "texDiffuseMap", _Texture);
            texturesManager.clear(1);
            GuiController.Instance.Shaders.setShaderMatrixIdentity(this.Shader);
            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionTextured;
            Shader.Technique = Technique;
            //Render con shader
            Shader.Begin(0);
            Shader.BeginPass(0);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 2, _Vertices);
            Shader.EndPass();
            Shader.End();
            _SetAlphaBlend(false);
        }
        public void _UpdateValues()
        {
            //TODO:Calcular los 4 corners de la pared, segun el tipo de orientacion
            Vector3 bLeft = new Vector3(-_Size.X / 2, -_Size.Y / 2, 0);
            Vector3 bRight = new Vector3(_Size.X / 2, -_Size.Y / 2, 0);
            Vector3 tLeft = new Vector3(-_Size.X / 2, _Size.Y / 2, 0);
            Vector3 tRight = new Vector3(_Size.X / 2, _Size.Y / 2, 0);
            //Primer triangulo
            _Vertices[0] = new CustomVertex.PositionTextured(bLeft, UVOffset.X, UVOffset.Y + Tile.Y);
            _Vertices[1] = new CustomVertex.PositionTextured(tLeft, UVOffset.X, UVOffset.Y);
            _Vertices[2] = new CustomVertex.PositionTextured(tRight, UVOffset.X + Tile.X, UVOffset.Y);
            //Segundo triangulo
            _Vertices[3] = new CustomVertex.PositionTextured(bLeft, UVOffset.X, UVOffset.Y + Tile.Y);
            _Vertices[4] = new CustomVertex.PositionTextured(tRight, UVOffset.X + Tile.X, UVOffset.Y);
            _Vertices[5] = new CustomVertex.PositionTextured(bRight, UVOffset.X + Tile.X, UVOffset.Y + Tile.Y);
            //Transformar todos los puntos
            for (int i = 0; i < _Vertices.Length; i++)
            {
                _Vertices[i].Position = Vector3.TransformCoordinate(_Vertices[i].Position, _RotationMatrix);
            }
        }
        #endregion TextureMethods
    }
}
