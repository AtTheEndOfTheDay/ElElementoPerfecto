using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.MiGrupo
{
    class Pelota
    {
        const float CONST_VELOCIDAD = 400f;
        
        private Vector3 velocidad_f;
        private Vector3 velocidad_w;
        //private Vector3 aceleracion_i = new Vector3(0, 0, 0);
        //private Vector3 aceleracion_w = new Vector3(0, 0, 0);
        private Vector3 iniPelota = new Vector3(0, 100, 0);
        private Vector3 iniVel_f = new Vector3(0, -1.5f, 0);
        private Vector3 iniVel_w = new Vector3(0, 0, 0);

        private TgcTexture textPelota = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Pelotita.jpg");
        public TgcSphere esfera;

        public Pelota()
        {
            
            esfera = new TgcSphere();
            esfera.Radius = 20f;
            esfera.Position = iniPelota;
            esfera.LevelOfDetail = 4;
            esfera.BasePoly = TgcSphere.eBasePoly.ICOSAHEDRON;
            esfera.setTexture(textPelota);

            velocidad_f = iniVel_f;
            velocidad_w = iniVel_w;
           
        }

        public void interactuar(TgcD3dInput input,float elapsedTime)
        {
            velocidad_f.X = 0;
            velocidad_w.Z = 0;

            if (input.keyDown(Key.A))
            {
                velocidad_f.X += 1;
                velocidad_w.Z += -15;
                //pelota.rotateZ(-15 * elapsedTime);
            }
            else if (input.keyDown(Key.D))
            {
                velocidad_f.X = -1;
                velocidad_w.Z = 15;
                //pelota.rotateZ(15 * elapsedTime);
            }

            velocidad_f.Y += -0.5f * elapsedTime;
            //pelota.rotateX(velocidad_w.X * elapsedTime);
            //pelota.rotateY(velocidad_w.Y * elapsedTime);

        }

        public void aplicarMovimientos(float elapsedTime)
        {
            esfera.rotateZ(velocidad_w.Z * elapsedTime);
            esfera.move(velocidad_f * CONST_VELOCIDAD * elapsedTime);
        }

        public void render()
        {

            esfera.render();
            esfera.updateValues();
        }

        public void rebotar(Item item, float coef_rebote,Vector3 normal)
        {
            if (Vector3.Dot(velocidad_f - item.velocidad(), normal) < 0)
            {
                velocidad_f.Y = (-velocidad_f.Y)*coef_rebote; //cambiar esto solo sirve para cuando rebota en el piso
            }
            
        }

        public void reiniciar()
        {
            esfera.Position = iniPelota;
            velocidad_f = iniVel_f;
            velocidad_w = iniVel_w;
        }
        public void dispose()
        {
            esfera.dispose();
        }
    }
}
