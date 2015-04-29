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

namespace AlumnoEjemplos.MiGrupo
{
    class MenuObjetos
    {
        private TgcSprite menu;
        Size screenSize = GuiController.Instance.Panel3d.Size;
        //Size textureSize;

        private string texturaMenu()
        {
            return EjemploAlumno.alumnoTextureFolder() + "menu3.jpg"; 
        }

        public MenuObjetos()
        {               
            //Crear Sprite
            menu = new TgcSprite();
            menu.Texture = TgcTexture.createTexture(texturaMenu());
            //textureSize = menu.Texture.Size;

            //Ubicarlo centrado en la pantalla
            menu.Position = new Vector2(0,screenSize.Height* 0.8f);
            menu.SrcRect = new Rectangle(100, 100, screenSize.Width,(int) Math.Round(screenSize.Height * 0.2));

                       
            //Modifiers para variar parametros del sprite
            //GuiController.Instance.Modifiers.addVertex2f("position", new Vector2(0, 0), new Vector2(screenSize.Width, screenSize.Height), menu.Position);
            //GuiController.Instance.Modifiers.addVertex2f("scaling", new Vector2(0, 0), new Vector2(4, 4), menu.Scaling);
            GuiController.Instance.Modifiers.addFloat("Tamaño altura en % de Screen", 0, 1, 0.2f);
        
        }

        public void renderMenu()
        {
            menu.render();
        }

        public void disposeMenu()
        {
            menu.dispose();
        }

        public void actualizarModifiers()
        {
            float heightRate = (float) GuiController.Instance.Modifiers["Tamaño altura en % de Screen"];
            //Actualizar valores cargados en modifiers
            menu.Position = new Vector2(0, screenSize.Height * (1 - heightRate));
            menu.SrcRect = new Rectangle(100, 100, screenSize.Width, (int)Math.Round(screenSize.Height * heightRate));
            
        }

    }
}
