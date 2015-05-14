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
        const float CONST_VELOCIDAD = 100f;

        private Vector3 velocidad_f = new Vector3(0, -1.5f, 0);
        private Vector3 velocidad_w = new Vector3(0, 0, 0);
        //private Vector3 aceleracion_i = new Vector3(0, 0, 0);
        //private Vector3 aceleracion_w = new Vector3(0, 0, 0);

        private TgcTexture textPelota = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Pelotita.jpg");
        public TgcSphere esfera;

        public Pelota()
        {
            Vector3 iniPelota = new Vector3(0, 500, 0);

            esfera = new TgcSphere();
            esfera.Radius = (float)20;
            esfera.Position = iniPelota;
            esfera.LevelOfDetail = 4;
            esfera.BasePoly = TgcSphere.eBasePoly.ICOSAHEDRON;
            esfera.setTexture(textPelota);

           
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

        public void rebotar(Item item, float coef_rebote,ITransformObject objeto)
        {
            if (Vector3.Dot(velocidad_f - item.velocidad(), esfera.Position - objeto.Position) < 0)
            {
                velocidad_f.Y = (-velocidad_f.Y)*coef_rebote; //cambiar esto solo sirve para cuando rebota en el piso
            }
            
        }

        public void dispose()
        {
            esfera.dispose();
        }
    }
}
