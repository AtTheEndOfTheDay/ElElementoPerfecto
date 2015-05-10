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
        private TgcSprite[] objetos;
        Size screenSize = GuiController.Instance.Panel3d.Size;

        private string texturaMenu()
        {
            return EjemploAlumno.alumnoTextureFolder() + "menu3.jpg"; 
        }

        private string texturaObjeto()
        {
            return EjemploAlumno.alumnoTextureFolder() + "menu2.jpg";
        }


        public MenuObjetos()
        {               
            //Crear Sprite
            menu = new TgcSprite();
            menu.Texture = TgcTexture.createTexture(texturaMenu());
            objetos = new TgcSprite[10];
            for(var i = 0; i < 10; i++)
                objetos[i] = new TgcSprite();
            foreach (var sprite in objetos)
                sprite.Texture = TgcTexture.createTexture(texturaObjeto());

            //Ubicarlo centrado en la pantalla
            menu.Position = new Vector2(screenSize.Width * 0.8f, 0);
            menu.SrcRect = new Rectangle(100, 100, (int)Math.Round(screenSize.Width * 0.2), screenSize.Height);

                       
            //Modifiers para variar parametros del sprite
            GuiController.Instance.Modifiers.addFloat("Tamaño ancho en % de Screen", 0, 1, 0.2f);
        
        }

        public void renderMenu()
        {
            menu.render();
            for (var i = 0; i < 10; i++)
                objetos[i].render();
        }

        public void disposeMenu()
        {
            menu.dispose();
        }

        public void actualizarModifiers()
        {
            float heightRate = (float) GuiController.Instance.Modifiers["Tamaño ancho en % de Screen"];
            float esquinaSupIzqMenu = screenSize.Width * (1 - heightRate);
            int ladoObjeto = (int)Math.Round(screenSize.Width * heightRate * 0.5f);
            //Actualizar valores cargados en modifiers
            menu.Position = new Vector2(esquinaSupIzqMenu, 0);
            menu.SrcRect = new Rectangle(100, 100, (int)Math.Round(screenSize.Width * heightRate), screenSize.Height);
            for (var i = 0; i < 5; i++)
            {
                objetos[i].Position = new Vector2(esquinaSupIzqMenu, 0 + ladoObjeto * i);
                objetos[i].SrcRect = new Rectangle(100, 100, ladoObjeto, ladoObjeto);
            }
            for (var i = 5; i < 10; i++)
            {
                objetos[i].Position = new Vector2(esquinaSupIzqMenu + ladoObjeto, 0 + ladoObjeto * (i-5));
                objetos[i].SrcRect = new Rectangle(100, 100, ladoObjeto, ladoObjeto);
            }
            
        }

    }
}
