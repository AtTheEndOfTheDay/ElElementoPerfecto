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
        TgcTexture texturaMenu = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Menu.jpg");

        public MenuObjetos()
        {               
            //Creacion de Sprites
            menu = new TgcSprite();
            objetos = new TgcSprite[10];
            for(var i = 0; i < 10; i++)
                objetos[i] = new TgcSprite();

            //Asginar Texturas
            menu.Texture = texturaMenu;
            for (var i = 0; i < 3; i++)
                objetos[i].Texture = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Cannon.png");
            for (var i = 3; i < 7; i++)
                objetos[i].Texture = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Magnet.png");
            for (var i = 7; i < 10; i++)
                objetos[i].Texture = TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Spring.png");

            //Esta pensado para una resolucion con aspect ratio 16:9
            //Dibujo Menu
            float ladoIzqMenu = screenSize.Width * (1 - 0.2f);
            float anchoMenu = screenSize.Width - ladoIzqMenu;
            menu.Position = new Vector2(ladoIzqMenu, 0);
            menu.SrcRect = new Rectangle(0, 0, (int)Math.Round(anchoMenu), screenSize.Height);

            //Dibujo Objetos de Menu
            float ladoObjeto = anchoMenu * 0.5f;
            for (var i = 0; i < 10; i++)
            {
                objetos[i].Position = new Vector2(ladoIzqMenu, 0 + ladoObjeto * i);
                objetos[i].Scaling = new Vector2(ladoObjeto / objetos[0].Texture.Width, ladoObjeto / objetos[0].Texture.Height);
            }
            for (var i = 5; i < 10; i++)
            {
                objetos[i].Position = new Vector2(ladoIzqMenu + ladoObjeto, 0 + ladoObjeto * (i - 5));
                objetos[i].Scaling = new Vector2(ladoObjeto / objetos[0].Texture.Width, ladoObjeto / objetos[0].Texture.Height);
            }
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
            for (var i = 0; i < 10; i++)
                objetos[i].dispose();
        }
    }
}
