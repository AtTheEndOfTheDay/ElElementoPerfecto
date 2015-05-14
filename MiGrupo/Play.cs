using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils._2D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class Play : Stage
    {
        private List<Item> itemsInScenario;
        private Pelota pelota;
        private TgcSprite cartel;

        TgcTexture texturaCartel = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "cartelPlay.png");

        public Play(List<Item> unosItems,Pelota unaPelota)
        {
            itemsInScenario = unosItems ;
            pelota = unaPelota;
            cartel = new TgcSprite();
            cartel.Texture = texturaCartel;
            cartel.Position = new Vector2(0, 0);
        }

        Stage Stage.interaccion(TgcD3dInput input, float elapsedTime)
        {
            pelota.interactuar(input, elapsedTime);

            foreach (var item in itemsInScenario)
            {
                item.interactuar(input,elapsedTime);
                item.interactuarConPelota(input, elapsedTime, pelota);
            }

            return this;
        }

        void Stage.aplicarMovimientos(float elapsedTime)
        {
            pelota.aplicarMovimientos(elapsedTime);

            foreach(var item in itemsInScenario)
            {
                item.aplicarMovimientos(elapsedTime);
            }
        }

        void Stage.render()
        {
            pelota.render();

            foreach (var item in itemsInScenario)
            {
                item.render();
            }
        }

        void Stage.mostrarStage()
        {
            cartel.render();
        }

    }
}
