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
    class Pared : Item
    {
        public TgcBox caja;

        public Pared(TgcBox unaCaja)
        {
            caja = unaCaja;
        }

        void Item.interactuar(TgcD3dInput input,float elapsedTime)
        {}

        Vector3 Item.interactuarConPelota()
        {
            return new Vector3(0, 0, 0);
        }

        void Item.iluminar()
        {
            //No se ilumina
        }

        void Item.render()
        {
            caja.render();
        }
        
        Vector3 Item.velocidad()
        { return new Vector3(0,0,0); }

        public void dispose()
        {
            caja.dispose();
        }
        public TgcTexture getTexture()
        {
            return caja.Texture;
        }

        TgcBoundingBox Item.getBB()
        {
            return caja.BoundingBox;
        }

        float Item.getCoefRebote()
        {
            return 0.5f;
        }

        bool Item.debeRebotar(TgcSphere esfera)
        {
            return true;
        }
    }
}
