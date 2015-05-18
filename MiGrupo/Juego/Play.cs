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
    class Play : Etapa
    {
        private List<Item> itemsDelNivel;
        private List<Item> itemsDelUsuario;
        private List<Item> items = new List<Item>();
        private Pelota pelota;

        public Play(List<Item> itemsNivel, List<Item> itemsUsuario, Pelota unaPelota)
        {
            itemsDelNivel = itemsNivel ;
            itemsDelUsuario = itemsUsuario;
            foreach (Item item in itemsDelNivel)
                items.Add(item);
            foreach (Item item in itemsDelUsuario)
                items.Add(item);
            pelota = unaPelota;
        }

        void Etapa.interaccion(TgcD3dInput input, float elapsedTime)
        {
            pelota.interactuar(input, elapsedTime);
        }

        void Etapa.aplicarMovimientos(float elapsedTime)
        {
            pelota.aplicarMovimientos(elapsedTime, items);

        }

        void Etapa.render()
        {
            pelota.render();

            foreach (var item in items)
            {
                item.render();
            }
        }

        String Etapa.getNombre()
        {
            return "Play";
        }

    }
}
