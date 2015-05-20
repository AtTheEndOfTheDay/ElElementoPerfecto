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
        private List<Item> items = new List<Item>();
        private Pelota pelota;

        public Play(List<Item> itemsDelNivel, List<Item> itemsDelUsuario, Pelota unaPelota)
        {
            //Creo una lista generica con todos los items del nivel
            foreach (Item item in itemsDelNivel)
                items.Add(item);
            foreach (Item item in itemsDelUsuario)
                items.Add(item);
            pelota = unaPelota;
        }

        void Etapa.interaccion(TgcD3dInput input, float elapsedTime)
        {
            pelota.interactuar(input, elapsedTime);

            pelota.aplicarMovimientos(elapsedTime, items);
        }

        String Etapa.getNombre()
        {
            return "Play";
        }

    }
}
