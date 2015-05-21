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
    class NivelFactory
    {
        const float pi = (float)Math.PI;

        TgcTexture madera = TgcTexture.createTexture(alumnoTextureFolder() + "Madera.jpg");
        TgcTexture metal = TgcTexture.createTexture(alumnoTextureFolder() + "Metal.jpg");
        TgcTexture metal2 = TgcTexture.createTexture(alumnoTextureFolder() + "METAL_t.jpg");
        TgcTexture laja = TgcTexture.createTexture(alumnoTextureFolder() + "Laja.jpg");
        TgcTexture texturaCannon = TgcTexture.createTexture(alumnoTextureFolder() + "Cannon.png");
        TgcTexture texturaMagnet = TgcTexture.createTexture(alumnoTextureFolder() + "Magnet.png");
        TgcTexture texturaSpring = TgcTexture.createTexture(alumnoTextureFolder() + "Spring.png");
        TgcTexture texturaPasarDeNivel = TgcTexture.createTexture(alumnoTextureFolder() + "PasasteDeNivel.jpg");
        TgcTexture texturaGanaste = TgcTexture.createTexture(alumnoTextureFolder() + "Ganaste!!!.jpg");

        TgcSceneLoader loader = new TgcSceneLoader();
        TgcScene scene;

        Pared piso;
        Pared techo;
        Pared lateralDerecha;
        Pared lateralIzquierda;
        
        //Lvl1
        Cannon lvl1Cannon;
        Magnet lvl1Magnet;
        
        //lvl2
        Spring lvl2Spring;
        Spring lvl2Spring1;
        Spring lvl2Spring2;

        //Lvl3
        Magnet lvl3Magnet; 
        Magnet lvl3Magnet1;
        Magnet lvl3Magnet2;
   
        //Lvl4
        Magnet lvl4Magnet;
        Spring lvl4Spring;

        //Lvl5
        Magnet lvl5Magnet; 
        Magnet lvl5Magnet1;
        Spring lvl5Spring;

        //Lvl6
        Magnet lvl6Magnet;
        Magnet lvl6Magnet1;
        Magnet lvl6Magnet2;
        Magnet lvl6Magnet3;
        Spring lvl6Spring;
        Spring lvl6Spring1;
        Spring lvl6Spring2;
        Spring lvl6Spring3;
        Spring lvl6Spring4;

        public static string alumnoTextureFolder()
        {
            return GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\";
        }

        public NivelFactory()
        {
            scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "MeshCreator\\Meshes\\Elements-TgcScene.xml");
        }

        public void dispose()
        {
            
            scene.disposeAll();
        }

        private void setParedes(TgcTexture textura, List<Item> itemsDeNivel)
        {
            Size screenSize = GuiController.Instance.Panel3d.Size;
            if ((screenSize.Width * 9/ screenSize.Height * 16) == 1)
            {
                piso = Pared.CrearPared(new Vector3(2.7f, -9.9f, 1), new Vector3(30.4f, 0.1f, 1f), new Vector3(0, 0, 0), textura, "piso");
                techo = Pared.CrearPared(new Vector3(2.7f, 9.9f, 1), new Vector3(30.4f, 0.1f, 1f), new Vector3(0, 0, 0), textura, "piso");
                lateralDerecha = Pared.CrearPared(new Vector3(-12.42f, 0, 1), new Vector3(0.01f, 20f, 1f), new Vector3(0, 0, 0), textura, "lateralDerecha");
                lateralIzquierda = Pared.CrearPared(new Vector3(17.9f, 0, 1), new Vector3(0.01f, 20f, 1f), new Vector3(0, 0, 0), textura, "lateralIzquierda");
            }
            else 
            {
                piso = Pared.CrearPared(new Vector3(2.7f, -9.9f, 1), new Vector3(30.9f, 0.1f, 1f), new Vector3(0, 0, 0), textura, "piso");
                techo = Pared.CrearPared(new Vector3(2.7f, 9.9f, 1), new Vector3(30.9f, 0.1f, 1f), new Vector3(0, 0, 0), textura, "piso");
                lateralDerecha = Pared.CrearPared(new Vector3(-12.8f, 0, 1), new Vector3(0.01f, 20f, 1f), new Vector3(0, 0, 0), textura, "lateralDerecha");
                lateralIzquierda = Pared.CrearPared(new Vector3(18.7f, 0, 1), new Vector3(0.01f, 20f, 1f), new Vector3(0, 0, 0), textura, "lateralIzquierda");
            }
            itemsDeNivel.Add(piso);
            itemsDeNivel.Add(techo);
            itemsDeNivel.Add(lateralDerecha);
            itemsDeNivel.Add(lateralIzquierda);
        }

        public void iniciarNivel(int nivel, out TgcSprite cartel, out TgcBox objetoGanador, out Pelota pelota, List<Item> itemsDeNivel, List<Item> itemsDelUsuario, out MenuObjetos menu, TgcBox fondo)
        {
            cartel = new TgcSprite();
            Size screenSize = GuiController.Instance.Panel3d.Size;
            cartel.SrcRect = new Rectangle();
            cartel.Position = new Vector2((int)Math.Round(screenSize.Width * 0.2f), (int)Math.Round(screenSize.Height * 0.4f));
            switch (nivel)
            {
                case 1:
                    objetoGanador = TgcBox.fromSize(new Vector3(-3, -2, 1), new Vector3(1, 1, 1), Color.FromArgb(125, Color.RoyalBlue));
                    objetoGanador.AlphaBlendEnable = true;
                    pelota = new Pelota(0.5f, new Vector3(16, -8, 1), metal);
                    cartel.Texture = texturaPasarDeNivel;

                    //Items del Usuario
                    itemsDelUsuario.Clear();
                    lvl1Magnet = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl1Magnet"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    itemsDelUsuario.Add(lvl1Magnet);
                    //Fin Items del Usuario;
    
                    //Items del Nivel
                    itemsDeNivel.Clear();
                    setParedes(madera, itemsDeNivel);
                    Pared lvl1Obstaculo = Pared.CrearPared(new Vector3(-3, -4, 1), new Vector3(3.5f, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl1Obstaculo");
                    Pared lvl1Obstaculo1 = Pared.CrearPared(new Vector3(0, -3, 1), new Vector3(2, 0.25f, 1), new Vector3(0, 0, pi / 4), madera, "lvl1Obstaculo1");
                    Pared lvl1Obstaculo2 = Pared.CrearPared(new Vector3(0, -1, 1), new Vector3(0.25f, 2, 1), new Vector3(0, 0, pi / 4), madera, "lvl1Obstaculo2");
                    Pared lvl1Obstaculo3 = Pared.CrearPared(new Vector3(-1.5f, 6, 1), new Vector3(0.25f, 7.75f, 1), new Vector3(0, 0, 0), madera, "lvl1Obstaculo3");
                    Pared lvl1Obstaculo4 = Pared.CrearPared(new Vector3(-5, 3f, 1), new Vector3(0.25f, 13.5f, 1), new Vector3(0, 0, 0), madera, "lvl1Obstaculo4");
                    lvl1Cannon = Cannon.CrearCannon(scene.Meshes[0].clone("cannon1"), scene.Meshes[2].clone("baseCannon1"), texturaCannon, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 1.03f ), new Vector3 (16, -8f, 1f));
                    lvl1Cannon.setenEscena(true);
                    lvl1Cannon.cargado = true;
                    lvl1Cannon.potencia = 2;
                    itemsDeNivel.Add(lvl1Obstaculo);
                    itemsDeNivel.Add(lvl1Obstaculo1);
                    itemsDeNivel.Add(lvl1Obstaculo2);
                    itemsDeNivel.Add(lvl1Obstaculo3);
                    itemsDeNivel.Add(lvl1Obstaculo4);
                    itemsDeNivel.Add(lvl1Cannon);
                    itemsDeNivel.Add(lvl1Cannon.baseCannon);
                    //Fin Items del Nivel


                    fondo.setTexture(madera);
                    break;
                case 2:
                    objetoGanador = TgcBox.fromSize(new Vector3(-8, 6, 1), new Vector3(1, 1, 1), Color.FromArgb(101, Color.Blue));
                    objetoGanador.AlphaBlendEnable = true;
                    pelota = new Pelota(0.5f, new Vector3(12, 0, 1), metal2);
                    cartel.Texture = texturaPasarDeNivel;

                    //Items del Usuario 
                    itemsDelUsuario.Clear();
                    lvl2Spring = Spring.CrearSpring(scene.Meshes[1].clone("lvl2Spring"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    lvl2Spring1 = Spring.CrearSpring(scene.Meshes[1].clone("lvl2Spring1"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    lvl2Spring2 = Spring.CrearSpring(scene.Meshes[1].clone("lvl2Spring2"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    itemsDelUsuario.Add(lvl2Spring);
                    itemsDelUsuario.Add(lvl2Spring1);
                    itemsDelUsuario.Add(lvl2Spring2);
                    //Fin Items del Usuario  
                
                    //Items del Nivel
                    itemsDeNivel.Clear();
                    setParedes(madera, itemsDeNivel);
                    //Fin Items del Nivel
                    
                    fondo.setTexture(madera);
                    break;

                case 3:
                    objetoGanador = TgcBox.fromSize(new Vector3(-10, 7, 1), new Vector3(1, 1, 1), Color.FromArgb(125, Color.RoyalBlue));
                    objetoGanador.AlphaBlendEnable = true;
                    pelota = new Pelota(0.5f, new Vector3(15, -7, 1), metal);
                    cartel.Texture = texturaPasarDeNivel;

                    //Items del Usuario
                    itemsDelUsuario.Clear();
                    lvl3Magnet = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl3Magnet"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl3Magnet1 = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl3Magnet1"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl3Magnet2 = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl3Magnet2"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    itemsDelUsuario.Add(lvl3Magnet);
                    itemsDelUsuario.Add(lvl3Magnet1);
                    itemsDelUsuario.Add(lvl3Magnet2);
                    //Fin Items del Usuario;

                    //Items del Nivel
                    itemsDeNivel.Clear();
                    setParedes(madera, itemsDeNivel);
                    Pared lvl3Obstaculo = Pared.CrearPared(new Vector3(-5, 5, 1), new Vector3(0.3f, 10, 1), new Vector3(0, 0, 0), metal, "lvl3Obstaculo");
                    Pared lvl3Obstaculo1 = Pared.CrearPared(new Vector3(5, -5, 1), new Vector3(0.3f, 10, 1), new Vector3(0, 0, 0), metal, "lvl3Obstaculo1");
                    itemsDeNivel.Add(lvl3Obstaculo);
                    itemsDeNivel.Add(lvl3Obstaculo1);
                    //Fin Items del Nivel

                    fondo.setTexture(madera);
                    break;
                case 4:
                    objetoGanador = TgcBox.fromSize(new Vector3(13, 0, 1), new Vector3(1, 1, 1), Color.FromArgb(101, Color.Blue));
                    objetoGanador.AlphaBlendEnable = true;
                    pelota = new Pelota(0.5f, new Vector3(12, 8, 1), metal2);
                    cartel.Texture = texturaPasarDeNivel;

                    //Items del Usuario 
                    itemsDelUsuario.Clear();
                    lvl4Magnet = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl4Magnet"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl4Spring = Spring.CrearSpring(scene.Meshes[1].clone("lvl4Spring"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    itemsDelUsuario.Add(lvl4Magnet);
                    itemsDelUsuario.Add(lvl4Spring);
                    //Fin Items del Usuario  
                
                    //Items del Nivel
                    itemsDeNivel.Clear();
                    setParedes(metal, itemsDeNivel);
                    Pared lvl4Obstaculo = Pared.CrearPared(new Vector3(1.5f, -2, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl4Obstaculo");
                    Pared lvl4obstaculo1 = Pared.CrearPared(new Vector3(-4.5f, 2, 1), new Vector3(7, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl4obstaculo1");
                    Pared lvl4Obstaculo2 = Pared.CrearPared(new Vector3(7, 2, 1), new Vector3(6, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl4Obstaculo2");
                    Pared lvl4Obstaculo3 = Pared.CrearPared(new Vector3(13, 5, 1), new Vector3(8, 0.25f, 1), new Vector3(0, 0, pi / 4), madera, "lvl4Obstaculo3");
                    Pared lvl4Obstaculo4 = Pared.CrearPared(new Vector3(-8, -1, 1), new Vector3(6, 0.25f, 1), new Vector3(0, 0, pi / 2), madera, "lvl4Obstaculo4");
                    Pared lvl4Obstaculo5 = Pared.CrearPared(new Vector3(4, 0, 1), new Vector3(4, 0.25f, 1), new Vector3(0, 0, pi / 2), madera, "lvl4Obstaculo5");
                    Pared lvl4Obstaculo6 = Pared.CrearPared(new Vector3(-1, 0, 1), new Vector3(4, 0.25f, 1), new Vector3(0, 0, pi / 2), madera, "lvl4Obstaculo6");
                    Pared lvl4Obstaculo7 = Pared.CrearPared(new Vector3(-10.2f, -4.7f, 1), new Vector3(6, 0.25f, 1), new Vector3(0, 0, -pi / 4), madera, "lvl4Obstaculo7");
                    Pared lvl4Obstaculo8 = Pared.CrearPared(new Vector3(1.5f, -7f, 1), new Vector3(19, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl4Obstaculo8");
                    itemsDeNivel.Add(lvl4Obstaculo);
                    itemsDeNivel.Add(lvl4obstaculo1);
                    itemsDeNivel.Add(lvl4Obstaculo2);
                    itemsDeNivel.Add(lvl4Obstaculo3);
                    itemsDeNivel.Add(lvl4Obstaculo4);
                    itemsDeNivel.Add(lvl4Obstaculo5);
                    itemsDeNivel.Add(lvl4Obstaculo6);
                    itemsDeNivel.Add(lvl4Obstaculo7);
                    itemsDeNivel.Add(lvl4Obstaculo8);
                    //Fin Items del Nivel
                    
                    fondo.setTexture(metal);
                    break;

                case 5:
                    objetoGanador = TgcBox.fromSize(new Vector3(-6, -7, 1), new Vector3(1, 1, 1), Color.FromArgb(101, Color.Blue));
                    objetoGanador.AlphaBlendEnable = true;
                    pelota = new Pelota(0.5f, new Vector3(8, -3f, 1), metal2);
                    cartel.Texture = texturaPasarDeNivel;

                    //Items del Usuario 
                    itemsDelUsuario.Clear();
                    lvl5Magnet = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl5Magnet"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl5Magnet1 = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl5Magnet1"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl5Spring = Spring.CrearSpring(scene.Meshes[1].clone("lvl5Spring"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    itemsDelUsuario.Add(lvl5Magnet);
                    itemsDelUsuario.Add(lvl5Magnet1);
                    itemsDelUsuario.Add(lvl5Spring);
                    //Fin Items del Usuario  

                    //Items del Nivel
                    itemsDeNivel.Clear();
                    setParedes(metal, itemsDeNivel);
                    Pared lvl5Obstaculo = Pared.CrearPared(new Vector3(0, -5, 1), new Vector3(15, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl5Obstaculo");
                    Pared lvl5obstaculo1 = Pared.CrearPared(new Vector3(-7.5f, -7.5f, 1), new Vector3(0.25f, 5, 1), new Vector3(0, 0, 0), madera, "lvl5obstaculo1");
                    Pared lvl5Obstaculo2 = Pared.CrearPared(new Vector3(11.25f, -1.5f, 1), new Vector3(10, 0.25f, 1), new Vector3(0, 0, pi / 4), madera, "lvl5Obstaculo2");
                    itemsDeNivel.Add(lvl5Obstaculo);
                    itemsDeNivel.Add(lvl5obstaculo1);
                    itemsDeNivel.Add(lvl5Obstaculo2);
                    //Fin Items del Nivel

                    fondo.setTexture(metal);
                    break;

                case 6:
                    objetoGanador = TgcBox.fromSize(new Vector3(12, 7, 1), new Vector3(1, 1, 1), Color.FromArgb(101, Color.Blue));
                    objetoGanador.AlphaBlendEnable = true;
                    pelota = new Pelota(0.5f, new Vector3(8, -8f, 1), metal2);
                    cartel.Texture = texturaGanaste;

                    //Items del Usuario 
                    itemsDelUsuario.Clear();
                    lvl6Magnet = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl6Magnet"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl6Magnet1 = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl6Magnet1"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl6Magnet2 = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl6Magnet2"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl6Magnet3 = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl6Magnet"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    itemsDelUsuario.Add(lvl6Magnet);
                    itemsDelUsuario.Add(lvl6Magnet1);
                    itemsDelUsuario.Add(lvl6Magnet2);
                    itemsDelUsuario.Add(lvl6Magnet3);
                    lvl6Spring = Spring.CrearSpring(scene.Meshes[1].clone("lvl6Spring"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    lvl6Spring1 = Spring.CrearSpring(scene.Meshes[1].clone("lvl6Spring1"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    lvl6Spring2 = Spring.CrearSpring(scene.Meshes[1].clone("lvl6Spring2"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    lvl6Spring3 = Spring.CrearSpring(scene.Meshes[1].clone("lvl6Spring3"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    lvl6Spring4 = Spring.CrearSpring(scene.Meshes[1].clone("lvl6Spring4"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    itemsDelUsuario.Add(lvl6Spring);
                    itemsDelUsuario.Add(lvl6Spring1);
                    itemsDelUsuario.Add(lvl6Spring2);
                    itemsDelUsuario.Add(lvl6Spring3);
                    itemsDelUsuario.Add(lvl6Spring4);
                    //Fin Items del Usuario  

                    //Items del Nivel
                    itemsDeNivel.Clear();
                    setParedes(metal, itemsDeNivel);
                    Pared lvl6Obstaculo = Pared.CrearPared(new Vector3(8, 5, 1), new Vector3(20, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl5Obstaculo2");
                    Pared lvl6Obstaculo1 = Pared.CrearPared(new Vector3(-2, 0, 1), new Vector3(20, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl5Obstaculo2");
                    Pared lvl6Obstaculo2 = Pared.CrearPared(new Vector3(8, -5, 1), new Vector3(20, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl5Obstaculo2");
                    itemsDeNivel.Add(lvl6Obstaculo);
                    itemsDeNivel.Add(lvl6Obstaculo1);
                    itemsDeNivel.Add(lvl6Obstaculo2);
                    //Fin Items del Nivel

                    fondo.setTexture(metal);
                    break;

                default:
                    objetoGanador = TgcBox.fromSize(new Vector3(-5, -8, 1), new Vector3(1, 1, 1), Color.Blue);
                    pelota = new Pelota(0.5f, new Vector3(16, -8, 1), metal);
                    break;
            }

            menu = new MenuObjetos(itemsDelUsuario, laja);
        }

        public void reiniciar(int nivel, Pelota pelota) 
            {
                pelota.reiniciar();
                switch (nivel) 
                { 
                    case 1:
                        lvl1Cannon.cargado = true; 
                        break; 
                } 
            } 


    }
}
