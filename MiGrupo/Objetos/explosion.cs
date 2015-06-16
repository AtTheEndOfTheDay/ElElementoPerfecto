﻿using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.MiGrupo.Objetos
{
    class AnimatedWall
    {
        
        /*
        TgcPlaneWall explosion;
        TgcTexture expTexture = TgcTexture.createTexture(alumnoTextureFolder() + "Explosion.png");
        float timeLastFrame;
        int framesPerSecond = 16;
        int actualFrame = 0;
        int amountOfFrames = 16;
        float timeOfAnimation = 1f;
        float timeFromBeginning = 0f;
        float tile

        public Explosion() 
        {
            explosion = new TgcPlaneWall(new Vector3(0, 0, 0), new Vector3(2, 2, 0), TgcPlaneWall.Orientations.XYplane, expTexture, 0.25f, 0.25f);
            timeLastFrame = 0f;
        }

        public void initAnimation() 
        {
            timeFromBeginning = 0;
        }

        public void render(float elapsedTime) {
            
            timeFromBeginning += elapsedTime;
            if(timeOfAnimation >= timeFromBeginning)
            {
                actualFrame = (int)Math.Floor(timeFromBeginning * framesPerSecond) % amountOfFrames;
            
                explosion.UVOffset = Vector3
            }
            



        }


        public static string alumnoTextureFolder()
        {
            return GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\";
        }*/

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

        TgcPlaneWall wall;
        /// <summary>
        /// Sprite con toda la textura a animar
        /// </summary>
        /*
        public TgcPlaneWall Wall
        {
            get { return wall; }
        }*/

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
        public Vector3 Origin
        {
            get { return wall.Origin; }
            set { wall.Origin = value; }
        }

        public Vector3 Position
        {
            get { return wall.Origin + Vector3.Multiply(wall.Size, 0.5f); }
            set { wall.Origin = value - Vector3.Multiply(wall.Size, 0.5f); }
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
        /*
        public float Rotation
        {
            get { return wall.Rotation; }
            set { sprite.Rotation = value; }
        }
        */
        /// <summary>
        /// Crear un nuevo Wall animado
        /// </summary>
        /// <param name="texturePath">path de la textura animada</param>
        /// <param name="frameSize">tamaño de un tile de la animacion</param>
        /// <param name="totalFrames">cantidad de frames que tiene la animacion</param>
        /// <param name="frameRate">velocidad en cuadros por segundo</param>
        public AnimatedWall(string texturePath, Size frameSize, int totalFrames, float frameRate, Vector3 wallSize, Vector3 originWall, int firstFrame)
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
            int realTotalFrames = (int)(1 / vTile) * (int)(1 / uTile);
            /*if (realTotalFrames > totalFrames)
            {
                throw new Exception("Error en AnimatedWall. No coinciden la cantidad de frames y el tamaño de la textura: " + totalFrames);
            }
            */
            //Wall
            wall = new TgcPlaneWall(originWall, wallSize, TgcPlaneWall.Orientations.XYplane, texture, uTile, vTile);
            wall.AlphaBlendEnable = true;

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
            wall.updateValues();
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
                if (currentTime > animationTimeLenght)
                {
                    //Reiniciar al llegar al final
                    currentTime = 0;
                    enabled = false;
                }
             
            }

            //Obtener cuadro actual
            if (currentFrame != (int)(currentTime * frameRate) + firstFrame)
            {
                currentFrame = (int)(currentTime * frameRate) + firstFrame;

                wall.UVOffset = new Vector2(uTile * (currentFrame % (int)(1 / uTile)), vTile * (currentFrame / (int)(1 / uTile)));
                wall.updateValues();
            }
        }


        public void initAnimation()
        {
            currentTime = 0;
            enabled = true;
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
            wall.render();
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
            wall.dispose();
        }
    }
}
