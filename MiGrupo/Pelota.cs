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
        const float CONST_VELOCIDAD = 10f;
        
        private Vector3 velocidadMovimiento;
        private Vector3 velocidadRotacion;
        private Vector3 iniPelota;
        private Vector3 velocidadInicialMovimiento = new Vector3(-1f, -1.5f, 0);
        private Vector3 velocidadInicialRotacion = new Vector3(0, 0, 0);

        public TgcSphere esfera = new TgcSphere();

        private PelotaCollisionManager manejadorDeColiciones = new PelotaCollisionManager();

        public Pelota(float radio, Vector3 posicionInicial, TgcTexture textPelota)
        {
            iniPelota = posicionInicial;
            esfera.Radius = radio;
            esfera.Position = iniPelota;
            esfera.LevelOfDetail = 4;
            esfera.BasePoly = TgcSphere.eBasePoly.ICOSAHEDRON;
            esfera.setTexture(textPelota);

            velocidadMovimiento = velocidadInicialMovimiento;
            velocidadRotacion = velocidadInicialRotacion;
           
        }

        public void interactuar(TgcD3dInput input,float elapsedTime)
        {
            //velocidadMovimiento.X = 0;
            velocidadRotacion.Z = 0;

            if (input.keyDown(Key.A))
            {
                velocidadMovimiento.X += 1f * elapsedTime;
                velocidadRotacion.Z += -15;
            }
            else if (input.keyDown(Key.D))
            {
                velocidadMovimiento.X -= 1f * elapsedTime;
                velocidadRotacion.Z = 15;
            }

            velocidadMovimiento.Y += -0.5f * elapsedTime;

        }

        public void aplicarMovimientos(float elapsedTime, List<Item> itemsInScenario)
        {
            esfera.rotateZ(velocidadRotacion.Z * elapsedTime);

            velocidadMovimiento = manejadorDeColiciones.ConsiderarColicionesCon(esfera, itemsInScenario, velocidadMovimiento, CONST_VELOCIDAD * elapsedTime, 0);

            esfera.move(velocidadMovimiento * CONST_VELOCIDAD * elapsedTime);
        }

        public void render()
        {

            esfera.render();
            esfera.updateValues();
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
