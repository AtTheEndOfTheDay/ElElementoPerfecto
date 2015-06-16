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
    class AnimatedQuad
    {
        Size frameSize;
        int totalFrames;
        float currentTime;
        float animationTimeLenght;
        float vTile;
        float uTile;
        float textureWidth;
        float textureHeight;
        int firstFrame;

        protected bool enabled;
        /// <summary>
        /// Indica si el sprite esta habilitado para ser renderizada
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        protected bool playing;
        /// <summary>
        /// Arrancar o parar avance de animacion
        /// </summary>
        public bool Playing
        {
            get { return playing; }
            set { playing = value; }
        }

        TexturedQuad texturedQuad;

        protected float frameRate;
        /// <summary>
        /// Velocidad de la animacion medida en cuadros por segundo.
        /// </summary>
        public float FrameRate
        {
            get { return frameRate; }
        }

        protected int currentFrame;
        /// <summary>
        /// Frame actual de la textura animada
        /// </summary>
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        /// <summary>
        /// Posicion del sprite
        /// </summary>
        public Vector3 Position
        {
            get { return texturedQuad.Position; }
            set { texturedQuad.Position = value; }
        }


        /// <summary>
        /// Factor de escala en X e Y
        /// </summary>
       /*
        public Vector2 Scaling
        {
            get { return wall.Scaling; }
            set { sprite.Scaling = value; }
        }
        */
        /// <summary>
        /// Angulo de rotación en radianes
        /// </summary>
        
        public Vector3 Rotation
        {
            get { return texturedQuad.Rotation; }
            set { texturedQuad.Rotation = value; }
        }
        
        public Vector2 Size
        {
            get { return texturedQuad.Size; }
            set { texturedQuad.Size = value; }
        }
        /// <summary>
        /// Crear un nuevo Wall animado
        /// </summary>
        /// <param name="texturePath">path de la textura animada</param>
        /// <param name="frameSize">tamaño de un tile de la animacion</param>
        /// <param name="totalFrames">cantidad de frames que tiene la animacion</param>
        /// <param name="frameRate">velocidad en cuadros por segundo</param>
        public AnimatedQuad(string texturePath, Size frameSize, int totalFrames, float frameRate, Vector2 quadSize, Vector3 quadPosition, int firstFrame)
        {
            this.enabled = false;
            this.currentFrame = firstFrame;
            this.frameSize = frameSize;
            this.totalFrames = totalFrames;
            this.currentTime = 0;
            this.playing = true;
            this.firstFrame = firstFrame;

            //Crear textura
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture texture = TgcTexture.createTexture(d3dDevice, texturePath);

            //Calcular valores de frames de la textura
            textureWidth = texture.Width;
            textureHeight = texture.Height;
            uTile = frameSize.Width / textureWidth;
            vTile = frameSize.Height / textureHeight;
            //Quad
            texturedQuad = new TexturedQuad(quadPosition, quadSize, Vector3.Empty, texture, uTile, vTile);
            texturedQuad.AlphaBlendEnable = true;

            setFrameRate(frameRate);
        }

        /// <summary>
        /// Cambiar la velocidad de animacion
        /// </summary>
        public void setFrameRate(float frameRate)
        {
            this.frameRate = frameRate;
            animationTimeLenght = (float)totalFrames / frameRate;
        }


        public void updateValues()
        {
            texturedQuad.updateValues();
        }

        /// <summary>
        /// Actualizar frame de animacion
        /// </summary>
        public void update()
        {
            if (!enabled)
                return;

            //Avanzar tiempo
            if (playing)
            {
                currentTime += GuiController.Instance.ElapsedTime;
                if ((currentTime > animationTimeLenght) && (totalFrames != 0))
                {
                    //Reiniciar al llegar al final
                    currentTime = 0;
                    enabled = false;
                }
             
            }

            int realTotalFrames = (int)(1 / vTile) * (int)(1 / uTile);
            //Obtener cuadro actual
            if (currentFrame != ((int)(currentTime * frameRate) + firstFrame) % realTotalFrames)
            {
                currentFrame = (int)(currentTime * frameRate) + firstFrame % realTotalFrames;

                texturedQuad.UVOffset = new Vector2(uTile * (currentFrame % (int)(1 / uTile)), vTile * (currentFrame / (int)(1 / uTile)));
            }
        }


        public void initAnimation()
        {
            currentTime = 0;
            enabled = true;
        }

        public void keepPlaying()
        {
            if (enabled)
                return;
            initAnimation();
        }

        public void stopAnimation()
        {
            currentTime = 0;
            enabled = false;
        }

        /// <summary>
        /// Renderizar Sprite.
        /// Se debe llamar primero a update().
        /// Sino se dibuja el ultimo estado actualizado.
        /// </summary>
        public void render()
        {
            if (!enabled)
                return;

            //Dibujar wall
            texturedQuad.render();
        }

        /// <summary>
        /// Actualiza la animacion y dibuja el Sprite
        /// </summary>
        public void updateAndRender()
        {
            update();
            render();
        }

        /// <summary>
        /// Liberar recursos
        /// </summary>
        public void dispose()
        {
            texturedQuad.dispose();
        }

    }
}
