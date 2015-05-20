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

        List<Item> objetosUsuario;
        TgcSprite menu;
        TgcSprite[] objetos;
        Size screenSize = GuiController.Instance.Panel3d.Size;
        float ladoIzqMenu;
        float ladoObjeto;
        int cantidadObjetos;

        public MenuObjetos(List<Item> listaMenu, TgcTexture texturaMenu)
        {
            cantidadObjetos = listaMenu.Count;
            objetosUsuario = listaMenu;
            //Creacion de Sprites
            menu = new TgcSprite();
            objetos = new TgcSprite[cantidadObjetos];
            for (var i = 0; i < cantidadObjetos; i++)
                objetos[i] = new TgcSprite();
            
            //Asginar Texturas
            menu.Texture = texturaMenu;
            for (var i = 0; i < cantidadObjetos; i++)
                objetos[i].Texture = listaMenu[i].getTexture();
            
            //Dibujo Menu
            ladoIzqMenu = screenSize.Width * (1 - 0.15f);
            float anchoMenu = screenSize.Width - ladoIzqMenu;
            menu.Position = new Vector2(ladoIzqMenu, 0);
            menu.SrcRect = new Rectangle(0, 0, (int)Math.Round(anchoMenu), (int)Math.Round((screenSize.Height*0.8f)));

            //Dibujo Objetos de Menu
            ladoObjeto = screenSize.Height / ((cantidadObjetos / 2) + 1);
            if (ladoObjeto * 2 > anchoMenu)
            {
                ladoObjeto = anchoMenu / 2;
            }
            for (var i = 0; i < (cantidadObjetos / 2); i++)
            {
                objetos[i].Position = new Vector2(ladoIzqMenu, 0 + ladoObjeto * i);
                objetos[i].Scaling = new Vector2(ladoObjeto / objetos[i].Texture.Width, ladoObjeto / objetos[i].Texture.Height);
            }
            for (var i = (cantidadObjetos / 2); i < cantidadObjetos; i++)
            {
                objetos[i].Position = new Vector2(ladoIzqMenu + ladoObjeto, 0 + ladoObjeto * (i - (cantidadObjetos / 2)));
                objetos[i].Scaling = new Vector2(ladoObjeto / objetos[i].Texture.Width, ladoObjeto / objetos[i].Texture.Height);
            }
        }

        public void renderMenu(int cantidadObjetos)
        {
            menu.render();
            for (var i = 0; i < cantidadObjetos; i++)
                objetos[i].render();
        }

        public void disposeMenu(int cantidadObjetos)
        {
            menu.dispose();
            for (var i = 0; i < cantidadObjetos; i++)
            {
                objetos[i].dispose();
            }
                
        }

        internal void poneloEnPantallaSiToco(float mouseX, float mouseY)
        {
            //Reviso por cada sprite en el menu, si clickeo en uno para renderizar ese objeto
            for (var i = 0; i < cantidadObjetos;  i++)
            {
                if ((mouseX > objetos[i].Position.X) && (mouseX < (objetos[i].Position.X + ladoObjeto)) &&
                    (mouseY > objetos[i].Position.Y) && (mouseY < (objetos[i].Position.Y + ladoObjeto)))
                {
                    if (!objetosUsuario[i].getenEscena())
                    {
                        objetosUsuario[i].setenEscena(true);
                        objetosUsuario[i].llevarAContenedor();
                    }
                }
            }
        }
    }
}
