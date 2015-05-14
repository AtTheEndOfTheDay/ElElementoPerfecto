using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils._2D;
using Microsoft.DirectX;

namespace AlumnoEjemplos.MiGrupo
{

    class Construccion : Stage
    {
        private List<Item> itemsInScenario = new List<Item>();
        private TgcSprite cartel;
        TgcTexture texturaCartel = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "cartelConstruccion.png");
        private Pelota pelota;

        public Construccion(List<Item> unosItems,Pelota unaPelota)
        {
            itemsInScenario = unosItems;

            pelota = unaPelota;
            cartel = new TgcSprite();
            cartel.Texture = texturaCartel;
            cartel.Position = new Vector2(0, 0);
        }

        Stage Stage.interaccion(TgcD3dInput input, float elapsedTime)
        {


            //aqui iría lo de arrastrar los objetos de la barra a la pantalla
            //poder mover la lista en el menu de elementos, etc
            return this;
        }

        void Stage.aplicarMovimientos(float elapsedTime)
        {
            //supongo que nada se mueve por si solo cuando construyo, sino debería ir aquí
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
