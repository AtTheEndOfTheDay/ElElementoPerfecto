﻿
using System;
using System.Collections.Generic;
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
    class Nivel
    {
        MenuObjetos menu;
        List<Item> objetosDeMenu;

        Pelota pelota;
        Vector3 iniPelota = new Vector3(0, 100, 0);

        TgcBox fondo;
        Pared piso;
        Pared lateralDerecha;
        Pared lateralIzquierda; 
        
        Construccion construccion;
        Play play;
        Stage etapa;
        TgcText2d textStage = new TgcText2d();

        public Nivel( TgcTexture textFondo, TgcTexture textPiso, TgcTexture textParedes,
                     Pelota pelotita, List<Item> itemsDelNivel, List<Item> itemsDelUsuario)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            pelota = pelotita;
            objetosDeMenu = itemsDelUsuario;

            fondo = TgcBox.fromSize(new Vector3(2.85f, 0, 0), new Vector3(32, 20.75f, 0), textFondo);
            piso = new Pared(TgcBox.fromSize(new Vector3(2.85f, -10.25f, 0), new Vector3(32, 0.25f, 0.25f), textPiso), 1);
            lateralDerecha = new Pared(TgcBox.fromSize(new Vector3(-13.1f, 0, 0), new Vector3(0.25f, 20.75f, 0.25f), textParedes), 1);
            lateralIzquierda = new Pared(TgcBox.fromSize(new Vector3(18.8f, 0, 0), new Vector3(0.25f, 20.75f, 0.25f), textParedes), 1);

            itemsDelNivel.Add(piso);
            itemsDelNivel.Add(lateralDerecha);
            itemsDelNivel.Add(lateralIzquierda);

            construccion = new Construccion(itemsDelNivel, pelota);
            play = new Play(itemsDelNivel, pelota);
            etapa = construccion;

            menu = new MenuObjetos(objetosDeMenu, TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Laja.jpg"));

            textStage.Color = Color.White;
            textStage.Position = new Point(0, 0);
            textStage.Text = "Construccion";

        }

        public void reiniciar(){
            pelota.reiniciar();
            etapa = construccion;
            textStage.Text = etapa.getNombre();
        }

        public void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcD3dInput input = GuiController.Instance.D3dInput;

            fondo.render();
            if ((input.keyDown(Key.Space))&&(etapa.Equals(construccion)))
            {
                etapa = play;
                textStage.Text = etapa.getNombre();
            }
            else
            {
                if ((input.keyDown(Key.C)) && (etapa.Equals(play)))
                {
                    reiniciar();
                }
                    
            }           
            etapa.interaccion(input,elapsedTime);

            etapa.aplicarMovimientos(elapsedTime);

            etapa.render();
            
            GuiController.Instance.Drawer2D.beginDrawSprite();
            menu.renderMenu(objetosDeMenu.Count);
                textStage.render();
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void dispose(){
            fondo.dispose();
            piso.dispose();
            lateralDerecha.dispose();
            lateralIzquierda.dispose();
            menu.disposeMenu(objetosDeMenu.Count);
            pelota.dispose();
        }
    }
}