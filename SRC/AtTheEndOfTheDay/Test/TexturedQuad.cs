using AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement;
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

namespace AlumnoEjemplos.SRC.AtTheEndOfTheDay.Test
{
    
    /// <summary>
    /// Pared 3D plana que solo crece en dos dimensiones.
    /// </summary>
    public class TexturedQuad : IRenderObject
    {
        private CustomVertex.PositionTextured[] vertices;

        private Vector3 position;
        /// <summary>
        /// Origen de coordenadas de la pared.
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }


        private Vector3 size;
        /// <summary>
        /// Dimensiones de la pared.
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public Vector3 Size
        {
            get { return size; }
            set { size = value; }
        }


        private Vector3 rotation;
        /// <summary>
        /// Orientación de la pared.
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        private TgcTexture texture;
        /// <summary>
        /// Textura de la pared
        /// </summary>
        public TgcTexture Texture
        {
            get { return texture; }
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


        private Vector2 uvOffset;
        /// <summary>
        /// Offset UV de textura
        /// </summary>
        public Vector2 UVOffset
        {
            get { return uvOffset; }
            set { uvOffset = value; }
        }

        private bool enabled;
        /// <summary>
        /// Indica si la pared esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
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





        /// <summary>
        /// Crea una pared vacia.
        /// </summary>
        public TexturedQuad()
        {
            this.vertices = new CustomVertex.PositionTextured[6];
            this.position = Vector3.Empty;
            this.size = Vector3Extension.One;
            this.rotation = Vector3.Empty;
            this.enabled = true;
            this.uTile = 1;
            this.vTile = 1;
            this.alphaBlendEnable = true;
            this.uvOffset = new Vector2(0, 0);
            
            

            //Shader
            this.effect = GuiController.Instance.Shaders.VariosShader;
            this.technique = TgcShaders.T_POSITION_TEXTURED;

            updateValues();
        }

        /// <summary>
        /// Crea una pared con un punto de origen, el tamaño de la pared y la orientación de la misma, especificando
        /// el tiling de la textura
        /// </summary>
        /// <param name="origin">Punto de origen de la pared</param>
        /// <param name="size">Dimensiones de la pared. Uno de los valores será ignorado, según la orientación elegida</param>
        /// <param name="orientation">Orientacion de la pared</param>
        /// <param name="texture">Textura de la pared</param>
        /// <param name="uTile">Cantidad de tile de la textura en coordenada U</param>
        /// <param name="vTile">Cantidad de tile de la textura en coordenada V</param>
        public TexturedQuad(Vector3 position, Vector3 size, Vector3 rotation, TgcTexture texture, float uTile, float vTile)
            : this()
        {
            setTexture(texture);

            this.position = position;
            this.size = size;
            this.rotation = rotation;
            this.uTile = uTile;
            this.vTile = vTile;

            updateValues();
        }

        /// <summary>
        /// Crea una pared con un punto de origen, el tamaño de la pared y la orientación de la misma, con ajuste automatico
        /// de coordenadas de textura
        /// </summary>
        /// <param name="origin">Punto de origen de la pared</param>
        /// <param name="size">Dimensiones de la pared. Uno de los valores será ignorado, según la orientación elegida</param>
        /// <param name="orientation">Orientacion de la pared</param>
        /// <param name="texture">Textura de la pared</param>
        public TexturedQuad(Vector3 position, Vector3 size, TgcTexture texture)
            : this()
        {
            setTexture(texture);

            this.position = position;
            this.size = size;

            updateValues();
        }

        /// <summary>
        /// Configurar punto minimo y maximo de la pared.
        /// Se ignora un valor de cada punto según la orientación elegida.
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        /// <param name="min">Min</param>
        /// <param name="max">Max</param>
        //public void setExtremes(Vector3 min, Vector3 max)
        //{
        //    this.origin = min;
        //    this.size = Vector3.Subtract(max, min);
        //}

        /// <summary>
        /// Actualizar parámetros de la pared en base a los valores configurados
        /// </summary>
        public void updateValues()
        {

            //Calcular los 4 corners de la pared, segun el tipo de orientacion

            Vector3 bLeft = new Vector3(-size.X / 2, -size.Y / 2, 0);
            Vector3 bRight = new Vector3(size.X / 2, -size.Y / 2, 0);
            Vector3 tLeft = new Vector3(-size.X / 2, size.Y / 2, 0);
            Vector3 tRight = new Vector3(size.X / 2, size.Y / 2, 0);

            float offsetU = this.uvOffset.X;
            float offsetV = this.uvOffset.Y;

            //Primer triangulo
            vertices[0] = new CustomVertex.PositionTextured(bLeft, offsetU, offsetV + vTile);
            vertices[1] = new CustomVertex.PositionTextured(tLeft, offsetU, offsetV);
            vertices[2] = new CustomVertex.PositionTextured(tRight, offsetU + uTile, offsetV);

            //Segundo triangulo
            vertices[3] = new CustomVertex.PositionTextured(bLeft, offsetU , offsetV + vTile);
            vertices[4] = new CustomVertex.PositionTextured(tRight, offsetU + uTile, offsetV);
            vertices[5] = new CustomVertex.PositionTextured(bRight, offsetU + uTile, offsetV + vTile);

            Matrix t = Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.Translation(position);
            

            //Transformar todos los puntos
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position = Vector3.TransformCoordinate(vertices[i].Position, t);
            }

        }

        /// <summary>
        /// Configurar textura de la pared
        /// </summary>
        public void setTexture(TgcTexture texture)
        {
            if (this.texture != null)
            {
                this.texture.dispose();
            }
            this.texture = texture;
        }

        /// <summary>
        /// Renderizar la pared
        /// </summary>
        public void render()
        {
            if (!enabled)
                return;

            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            activateAlphaBlend();

            texturesManager.shaderSet(effect, "texDiffuseMap", texture);
            texturesManager.clear(1);
            GuiController.Instance.Shaders.setShaderMatrixIdentity(this.effect);
            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionTextured;
            effect.Technique = this.technique;

            //Render con shader
            effect.Begin(0);
            effect.BeginPass(0);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 2, vertices);
            effect.EndPass();
            effect.End();

            resetAlphaBlend();
        }

        /// <summary>
        /// Activar AlphaBlending, si corresponde
        /// </summary>
        protected void activateAlphaBlend()
        {
            Device device = GuiController.Instance.D3dDevice;
            if (alphaBlendEnable)
            {
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.AlphaBlendEnable = true;
            }
        }

        /// <summary>
        /// Desactivar AlphaBlending
        /// </summary>
        protected void resetAlphaBlend()
        {
            Device device = GuiController.Instance.D3dDevice;
            device.RenderState.AlphaTestEnable = false;
            device.RenderState.AlphaBlendEnable = false;
        }



        /// <summary>
        /// Liberar recursos de la pared
        /// </summary>
        public void dispose()
        {
            texture.dispose();
        }

        /// <summary>
        /// Crear un nuevo Wall igual a este
        /// </summary>
        /// <returns>Wall clonado</returns>
        //public TexturedQuad clone()
        //{
        //    TexturedQuad cloneWall = new TexturedQuad();
        //    cloneWall.position = this.position;
        //    cloneWall.size = this.size;
        //    cloneWall.rotation = this.rotation;
        //    cloneWall.uTile = this.uTile;
        //    cloneWall.vTile = this.vTile;
        //    cloneWall.alphaBlendEnable = this.alphaBlendEnable;
        //    cloneWall.uvOffset = this.uvOffset;
        //    cloneWall.setTexture(this.texture.clone());

        //    updateValues();
        //    return cloneWall;
        //}

    }
}
