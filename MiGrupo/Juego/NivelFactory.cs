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
        
        //Lvl2
        Cannon lvl1Cannon;
        Magnet lvl1Magnet;
        
        //Lvl2
        Magnet lvl2Magnet;
        Magnet lvl2Magnet2;
        Spring lvl2Spring;
        Spring lvl2Spring2;


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
            piso.dispose();
            lateralDerecha.dispose();
            lateralIzquierda.dispose();
            lvl1Cannon.dispose();
            lvl1Magnet.dispose();
            if (lvl2Magnet2 != null)
            {
                lvl2Magnet.dispose();
                lvl2Magnet2.dispose();
                lvl2Spring.dispose();
                lvl2Spring2.dispose();
            }
            
            scene.disposeAll();
        }

        private void setParedes(TgcTexture textura, List<Item> itemsDeNivel)
        {
            piso = Pared.CrearPared(new Vector3(2.7f, -9.9f, 1), new Vector3(30.4f, 0.1f, 1f), new Vector3(0, 0, 0), textura, "piso");
            techo  = Pared.CrearPared(new Vector3(2.7f, 9.9f, 1), new Vector3(30.4f, 0.1f, 1f), new Vector3(0, 0, 0), textura, "piso");
            lateralDerecha = Pared.CrearPared(new Vector3(-12.42f, 0, 1), new Vector3(0.01f, 20f, 1f), new Vector3(0, 0, 0), textura, "lateralDerecha");
            lateralIzquierda = Pared.CrearPared(new Vector3(17.9f, 0, 1), new Vector3(0.01f, 20f, 1f), new Vector3(0, 0, 0), textura, "lateralIzquierda");
            itemsDeNivel.Add(piso);
            itemsDeNivel.Add(techo);
            itemsDeNivel.Add(lateralDerecha);
            itemsDeNivel.Add(lateralIzquierda);
        }

        public void iniciarNivel(int nivel, out TgcSprite cartel, out TgcBox objetoGanador, out Pelota pelota, List<Item> itemsDeNivel, List<Item> itemsDelUsuario, out MenuObjetos menu, TgcBox fondo, TgcBox contenedor)
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
                    objetoGanador = TgcBox.fromSize(new Vector3(13, 0, 1), new Vector3(1, 1, 1), Color.FromArgb(101, Color.Blue));
                    objetoGanador.AlphaBlendEnable = true;
                    pelota = new Pelota(0.5f, new Vector3(12, 8, 1), metal2);
                    cartel.Texture = texturaGanaste;

                    //Items del Usuario 
                    itemsDelUsuario.Clear();
                    lvl2Magnet = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl2Magnet"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl2Magnet2 = Magnet.CrearMagnet(scene.Meshes[3].clone("lvl2Magnet2"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    lvl2Spring = Spring.CrearSpring(scene.Meshes[1].clone("spring1"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    lvl2Spring2 = Spring.CrearSpring(scene.Meshes[1].clone("lvl2Spring2"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    itemsDelUsuario.Add(lvl2Magnet);
                    itemsDelUsuario.Add(lvl2Magnet2);
                    itemsDelUsuario.Add(lvl2Spring);
                    itemsDelUsuario.Add(lvl2Spring2);
                    //Fin Items del Usuario  
                
                    //Items del Nivel
                    itemsDeNivel.Clear();
                    setParedes(metal, itemsDeNivel);
                    Pared lvl2Obstaculo = Pared.CrearPared(new Vector3(1.5f, -2, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl2Obstaculo");
                    Pared lvl2obstaculo1 = Pared.CrearPared(new Vector3(-4.5f, 2, 1), new Vector3(7, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl2obstaculo1");
                    Pared lvl2Obstaculo2 = Pared.CrearPared(new Vector3(7, 2, 1), new Vector3(6, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl2Obstaculo2");
                    Pared lvl2Obstaculo3 = Pared.CrearPared(new Vector3(12, 4, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, FastMath.PI / 4), madera, "lvl2Obstaculo3");
                    Pared lvl2Obstaculo4 = Pared.CrearPared(new Vector3(-8, -1, 1), new Vector3(6, 0.25f, 1), new Vector3(0, 0, FastMath.PI / 2), madera, "lvl2Obstaculo4");
                    Pared lvl2Obstaculo5 = Pared.CrearPared(new Vector3(4, 0, 1), new Vector3(4, 0.25f, 1), new Vector3(0, 0, FastMath.PI / 2), madera, "lvl2Obstaculo5");
                    Pared lvl2Obstaculo6 = Pared.CrearPared(new Vector3(-1, 0, 1), new Vector3(4, 0.25f, 1), new Vector3(0, 0, FastMath.PI / 2), madera, "lvl2Obstaculo6");
                    Pared lvl2Obstaculo7 = Pared.CrearPared(new Vector3(-10, -5, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, -FastMath.PI / 4), madera, "lvl2Obstaculo7");
                    Pared lvl2Obstaculo8 = Pared.CrearPared(new Vector3(1.5f, -7f, 1), new Vector3(19, 0.25f, 1), new Vector3(0, 0, 0), madera, "lvl2Obstaculo8");
                    itemsDeNivel.Add(lvl2Obstaculo);
                    itemsDeNivel.Add(lvl2obstaculo1);
                    itemsDeNivel.Add(lvl2Obstaculo2);
                    itemsDeNivel.Add(lvl2Obstaculo3);
                    itemsDeNivel.Add(lvl2Obstaculo4);
                    itemsDeNivel.Add(lvl2Obstaculo5);
                    itemsDeNivel.Add(lvl2Obstaculo6);
                    itemsDeNivel.Add(lvl2Obstaculo7);
                    itemsDeNivel.Add(lvl2Obstaculo8);
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
                    case 2:   
                        break; 
                } 
            } 


    }
}
