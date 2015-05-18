﻿using System;
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
        private List<Item> itemsInScenario;
        private Pelota pelota;

        public Play(List<Item> unosItems,Pelota unaPelota)
        {
            itemsInScenario = unosItems ;
            pelota = unaPelota;
        }

        void Etapa.interaccion(TgcD3dInput input, float elapsedTime)
        {
            pelota.interactuar(input, elapsedTime);

            foreach (var item in itemsInScenario)
            {
                item.interactuar(input,elapsedTime);
            }

        }

        void Etapa.aplicarMovimientos(float elapsedTime)
        {
            pelota.aplicarMovimientos(elapsedTime,itemsInScenario);

        }

        void Etapa.render()
        {
            pelota.render();

            foreach (var item in itemsInScenario)
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
