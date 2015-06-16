using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo.Objetos
{
 
    /// <summary>
    /// Herramienta para crear un Quad 3D, o un plano con ancho y largo acotado,
    /// en base al centro, una normal, una rotacion respecto de la normal y una Textura.
    /// </summary>
    public class TexturedQuad : IRenderObject
    {

        #region Creacion


        #endregion


        readonly Vector3 ORIGINAL_DIR = new Vector3(0, 1, 0);

        VertexBuffer vertexBuffer;

        Vector3 center;
        /// <summary>
        /// Centro del plano
        /// </summary>
        public Vector3 Center
        {
            get { return center; }
            set { center = value; }
        }

        Vector3 normal;
        /// <summary>
        /// Normal del plano
        /// </summary>
        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        Vector2 size;
        /// <summary>
        /// Tamaño del plano, en ancho y longitud
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        /*
        Color color;
        /// <summary>
        /// Color del plano
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        */

        TgcTexture texture;
        /// <summary>
        /// Textura de la pared
        /// </summary>
        public TgcTexture Texture
        {
            get { return texture; }
        }

        private bool enabled;
        /// <summary>
        /// Indica si el plano habilitado para ser renderizado
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public Vector3 Position
        {
            get { return center; }
        }

        private bool alphaBlendEnable;
        /// <summary>
        /// Habilita el renderizado con AlphaBlending para los modelos
        /// con textura o colores por vértice de canal Alpha.
        /// Por default está deshabilitado.
        /// </summary>
        public bool AlphaBlendEnable
        {
            get { return alphaBlendEnable; }
            set { alphaBlendEnable = value; }
        }

        protected Effect effect;
        /// <summary>
        /// Shader del mesh
        /// </summary>
        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        protected string technique;
        /// <summary>
        /// Technique que se va a utilizar en el effect.
        /// Cada vez que se llama a render() se carga este Technique (pisando lo que el shader ya tenia seteado)
        /// </summary>
        public string Technique
        {
            get { return technique; }
            set { technique = value; }
        }


        float uTile;
        /// <summary>
        /// Cantidad de tile de la textura en coordenada U.
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public float UTile
        {
            get { return uTile; }
            set { uTile = value; }
        }

        float vTile;
        /// <summary>
        /// Cantidad de tile de la textura en coordenada V.
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public float VTile
        {
            get { return vTile; }
            set { vTile = value; }
        }


        bool autoAdjustUv;
        /// <summary>
        /// Auto ajustar coordenadas UV en base a la relación de tamaño de la pared y la textura
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public bool AutoAdjustUv
        {
            get { return autoAdjustUv; }
            set { autoAdjustUv = value; }
        }

        Vector2 uvOffset;
        /// <summary>
        /// Offset UV de textura
        /// </summary>
        public Vector2 UVOffset
        {
            get { return uvOffset; }
            set { uvOffset = value; }
        }



        public TexturedQuad()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionTextured), 6, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            this.center = Vector3.Empty;
            this.normal = new Vector3(0, 1, 0);
            this.size = new Vector2(10, 10);
            this.enabled = true;
            //this.color = Color.Blue;
            this.alphaBlendEnable = false;

            //Shader
            this.effect = GuiController.Instance.Shaders.VariosShader;
            this.technique = TgcShaders.T_POSITION_TEXTURED;
        }

        /// <summary>
        /// Actualizar parámetros del plano en base a los valores configurados
        /// </summary>
        public void updateValues()
        {
            CustomVertex.PositionTextured[] vertices = new CustomVertex.PositionTextured[6];

            float offsetU = this.uvOffset.X;
            float offsetV = this.uvOffset.Y;

            //Crear un Quad con dos triángulos sobre XZ con normal default (0, 1, 0)
            Vector3 min = new Vector3(-size.X / 2, 0, -size.Y / 2);
            Vector3 max = new Vector3(size.X / 2, 0, size.Y / 2);
            
            vertices[0] = new CustomVertex.PositionTextured(min, offsetU + uTile, offsetV + vTile);
            vertices[1] = new CustomVertex.PositionTextured(min.X, 0, max.Z, offsetU, offsetV + vTile);
            vertices[2] = new CustomVertex.PositionTextured(max, offsetU, offsetV);

            vertices[3] = new CustomVertex.PositionTextured(min, offsetU + uTile, offsetV + vTile);
            vertices[4] = new CustomVertex.PositionTextured(max, offsetU, offsetV);
            vertices[5] = new CustomVertex.PositionTextured(max.X, 0, min.Z, offsetU + uTile, offsetV);

            //Obtener matriz de rotacion respecto de la normal del plano
            normal.Normalize();
            float angle = FastMath.Acos(Vector3.Dot(ORIGINAL_DIR, normal));
            Vector3 axisRotation = Vector3.Cross(ORIGINAL_DIR, normal);
            axisRotation.Normalize();
            Matrix t = Matrix.RotationAxis(axisRotation, angle) * Matrix.Translation(center);

            //Transformar todos los puntos
            for (int i = 0; i < vertices.Length; i++)
			{
                vertices[i].Position = Vector3.TransformCoordinate(vertices[i].Position, t);
			}

            //Cargar vertexBuffer
            vertexBuffer.SetData(vertices, 0, LockFlags.None);
        }

        

        /// <summary>
        /// Renderizar el Quad
        /// </summary>
        public void render()
        {
            if (!enabled)
                return;

            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            texturesManager.clear(0);
            texturesManager.clear(1);

            GuiController.Instance.Shaders.setShaderMatrixIdentity(this.effect);
            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionTextured;
            effect.Technique = this.technique;
            d3dDevice.SetStreamSource(0, vertexBuffer, 0);

            //Render con shader
            effect.Begin(0);
            effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            effect.EndPass();
            effect.End();
        }

        /// <summary>
        /// Liberar recursos de la flecha
        /// </summary>
        public void dispose()
        {
            if (vertexBuffer != null && !vertexBuffer.Disposed)
            {
                vertexBuffer.Dispose();
            }
        }


    }
}
