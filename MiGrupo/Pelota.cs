﻿using System;
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
        const float CONST_VELOCIDAD = 40f;
        
        private Vector3 velocidadMovimiento;
        private Vector3 velocidadRotacion;
        private Vector3 iniPelota = new Vector3(0, 10, 0);
        private Vector3 velocidadInicialMovimiento = new Vector3(0, -1.5f, 0);
        private Vector3 velocidadInicialRotacion = new Vector3(0, 0, 0);

        private TgcTexture textPelota = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Pelotita.jpg");
        public TgcSphere esfera = new TgcSphere();


        public Pelota()
        {
            esfera.Radius = 2f;
            esfera.Position = iniPelota;
            esfera.LevelOfDetail = 4;
            esfera.BasePoly = TgcSphere.eBasePoly.ICOSAHEDRON;
            esfera.setTexture(textPelota);

            velocidadMovimiento = velocidadInicialMovimiento;
            velocidadRotacion = velocidadInicialRotacion;
           
        }

        public void interactuar(TgcD3dInput input,float elapsedTime)
        {
            velocidadMovimiento.X = 0;
            velocidadRotacion.Z = 0;

            if (input.keyDown(Key.A))
            {
                velocidadMovimiento.X += 1;
                velocidadRotacion.Z += -15;
                //pelota.rotateZ(-15 * elapsedTime);
            }
            else if (input.keyDown(Key.D))
            {
                velocidadMovimiento.X = -1;
                velocidadRotacion.Z = 15;
                //pelota.rotateZ(15 * elapsedTime);
            }

            velocidadMovimiento.Y += -0.5f * elapsedTime;
            //pelota.rotateX(velocidad_w.X * elapsedTime);
            //pelota.rotateY(velocidad_w.Y * elapsedTime);

        }

        public void aplicarMovimientos(float elapsedTime)
        {
            esfera.rotateZ(velocidadRotacion.Z * elapsedTime);
            esfera.move(velocidadMovimiento * CONST_VELOCIDAD * elapsedTime);
        }

        public void render()
        {

            esfera.render();
            esfera.updateValues();
        }

        public void rebotar(Item item, float coef_rebote,Vector3 normal)
        {
            if (Vector3.Dot(velocidadMovimiento - item.velocidad(), normal) < 0)
            {
                velocidadMovimiento.Y = (-velocidadMovimiento.Y)*coef_rebote; //cambiar esto solo sirve para cuando rebota en el piso
            }
            
        }

        public void reiniciar()
        {
            esfera.Position = iniPelota;
            velocidadMovimiento = velocidadInicialMovimiento;
            velocidadRotacion = velocidadInicialRotacion;
        }
        public void dispose()
        {
            esfera.dispose();
        }
    }
}