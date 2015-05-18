
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
        List<Item> objetosDelUsuario;
        List<Item> objetosDelNivel;

        Pelota pelota;
        Vector3 iniPelota = new Vector3(0, 100, 0);

        TgcBox fondo;
        TgcBox contenedor;
        Pared piso;
        Pared lateralDerecha;
        Pared lateralIzquierda; 
        
        Construccion construccion;
        Play play;
        Etapa etapa;
        TgcText2d textStage = new TgcText2d();
        TgcTexture textMenu = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Laja.jpg");

        public Nivel( TgcTexture textFondo, TgcTexture textPiso, TgcTexture textParedes,
                     Pelota pelotita, List<Item> itemsDelNivel, List<Item> itemsDelUsuario)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
                        
            pelota = pelotita;
            objetosDelUsuario = itemsDelUsuario;
            objetosDelNivel = itemsDelNivel;
            TgcBox cajaCreadora = TgcBox.fromSize(new Vector3(-10, 5, 0), new Vector3(1, 1, 0), textPiso);

            contenedor = TgcBox.fromSize(new Vector3(-15.5f, -8.25f, 0.9f), new Vector3(5.5f, 4.5f, 0), textMenu);
            fondo = TgcBox.fromSize(new Vector3(2.85f, 0, 0), new Vector3(32, 20.75f, 0), textFondo);
            piso = new Pared(TgcBox.fromSize(new Vector3(2.85f, -10.25f, 0), new Vector3(32, 0.25f, 1f), textPiso).toMesh("piso"), textPiso);
            lateralDerecha = new Pared(TgcBox.fromSize(new Vector3(-13.1f, 0, 0), new Vector3(0.25f, 20.75f, 1f), textParedes).toMesh("lateralDerecha"), textParedes);
            lateralIzquierda = new Pared(TgcBox.fromSize(new Vector3(18.8f, 0, 0), new Vector3(0.25f, 20.75f, 1f), textParedes).toMesh("lateralIzquierda"), textParedes);

            objetosDelNivel.Add(piso);
            objetosDelNivel.Add(lateralDerecha);
            objetosDelNivel.Add(lateralIzquierda);

            menu = new MenuObjetos(objetosDelUsuario, textMenu);

            construccion = new Construccion(objetosDelNivel, pelota, menu, objetosDelUsuario);
            play = new Play(objetosDelNivel, objetosDelUsuario, pelota);
            etapa = construccion;

            textStage.Color = Color.White;
            textStage.Position = new Point(0, 0);
            textStage.Text = "Construccion";

        }

 
        public void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcD3dInput input = GuiController.Instance.D3dInput;

            fondo.render();
            contenedor.render();
            if ((input.keyDown(Key.Space))&&(etapa.Equals(construccion)))
            {
                etapa = play;
                textStage.Text = etapa.getNombre();
            }
            else
            {
                if ((input.keyDown(Key.C)) && (etapa.Equals(play)))
                {
                    pasarAConstruccion();
                }
                    
            }           
            
            etapa.interaccion(input,elapsedTime);
            etapa.aplicarMovimientos(elapsedTime);
            etapa.render();

            foreach (Item objeto in objetosDelUsuario)
            {
                objeto.iluminar();
            }

            foreach (Item objeto in objetosDelUsuario)
            {
                objeto.render();
            }
                        
            GuiController.Instance.Drawer2D.beginDrawSprite();
                menu.renderMenu(objetosDelUsuario.Count);
                textStage.render();
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void dispose(){
            foreach (var item in objetosDelNivel)
            {
                item.dispose();
            }
            fondo.Texture.dispose();
            fondo.dispose();
            contenedor.Texture.dispose();
            contenedor.dispose();
            menu.disposeMenu(objetosDelUsuario.Count);
            pelota.dispose();
        }
        private void pasarAConstruccion()
        {
            pelota.reiniciar();
            etapa = construccion;
            textStage.Text = etapa.getNombre();
        }
    }
}
