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
        TgcTexture laja = TgcTexture.createTexture(alumnoTextureFolder() + "Laja.jpg");
        TgcTexture texturaCannon = TgcTexture.createTexture(alumnoTextureFolder() + "Cannon.png");
        TgcTexture texturaMagnet = TgcTexture.createTexture(alumnoTextureFolder() + "Magnet.png");
        TgcTexture texturaSpring = TgcTexture.createTexture(alumnoTextureFolder() + "Spring.png");

        TgcSceneLoader loader = new TgcSceneLoader();
        TgcScene scene;

        Pared piso;
        Pared lateralDerecha;
        Pared lateralIzquierda;
        Cannon cannon;
        Cannon cannon2;
        Magnet magnet1;
        Magnet magnet2;
        Spring spring1;

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
            cannon.dispose();
            cannon2.dispose();
            magnet1.dispose();
            magnet2.dispose();
            spring1.dispose();

            scene.disposeAll();
        }

        private void setParedes(TgcTexture textura, List<Item> itemsDeNivel)
        {
            piso = Pared.CrearPared(new Vector3(2.85f, -9.9f, 1), new Vector3(30, 0.1f, 1f), new Vector3(0, 0, 0), textura, "piso");
            lateralDerecha = Pared.CrearPared(new Vector3(-12.42f, 0, 1), new Vector3(0.01f, 20.75f, 1f), new Vector3(0, 0, 0), textura, "lateralDerecha");
            lateralIzquierda = Pared.CrearPared(new Vector3(17.9f, 0, 1), new Vector3(0.01f, 20.75f, 1f), new Vector3(0, 0, 0), textura, "lateralIzquierda");
            itemsDeNivel.Add(piso);
            itemsDeNivel.Add(lateralDerecha);
            itemsDeNivel.Add(lateralIzquierda);
        }

        public MenuObjetos menuNivel(List<Item> itemsDelUsuario)
        {

            return new MenuObjetos(itemsDelUsuario, laja);
        }

        public Pelota pelotaNivel(int nivel)
        {
            switch (nivel)
            {
                case 1:
                    return new Pelota(0.5f, new Vector3(16, -8, 1), metal);
                case 2:
                    return new Pelota(1f, new Vector3(10, 5, 1), madera);
                default:
                    return new Pelota(0.5f, new Vector3(16, -8, 1), metal);
            }
        }


        public void iniciarNivel(int nivel, List<Item> itemsDeNivel, List<Item> itemsDelUsuario, MenuObjetos menu, TgcBox fondo, TgcBox contenedor)
        {
            switch (nivel)
            {
                case 1:

                    //Items del Usuario
                    itemsDelUsuario.Clear();
                    magnet1 = Magnet.CrearMagnet(scene.Meshes[3].clone("magnet1"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    magnet2 = Magnet.CrearMagnet(scene.Meshes[3].clone("magnet2"), texturaMagnet, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi));
                    spring1 = Spring.CrearSpring(scene.Meshes[1].clone("spring1"), texturaSpring, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0));
                    itemsDelUsuario.Add(magnet1);
                    itemsDelUsuario.Add(magnet2);
                    itemsDelUsuario.Add(spring1);
                    //Fin Items del Usuario;
    
                    //Items del Nivel
                    itemsDeNivel.Clear();
                    setParedes(madera, itemsDeNivel);
                    cannon = Cannon.CrearCannon(scene.Meshes[0].clone("cannon1"), scene.Meshes[2].clone("baseCannon1"), texturaCannon, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi / 4), new Vector3 (16, -8f, 1f));
                    cannon.setenEscena(true);
                    cannon.cargado = true;
                    itemsDeNivel.Add(cannon);
                    itemsDeNivel.Add(cannon.baseCannon);
                    //Fin Items del Nivel


                    fondo.setTexture(madera);
                    break;
                case 2:
                    //Items del Usuario 
                    itemsDelUsuario.Clear();
                    cannon2 = Cannon.CrearCannon(scene.Meshes[0].clone("Cannon2"), scene.Meshes[2].clone("baseCannon1"), texturaCannon, Color.Black, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, pi / 4));
                    itemsDelUsuario.Add(cannon2);
                    //Fin Items del Usuario  
                
                    //Items del Nivel
                    itemsDeNivel.Clear();
                    setParedes(metal, itemsDeNivel);
                    Pared obstaculo1 = Pared.CrearPared(new Vector3(-10, -6, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, 0), madera, "Obstaculo1");
                    Pared obstaculo2 = Pared.CrearPared(new Vector3(-5, -4, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, 0), madera, "Obstaculo2");
                    Pared obstaculo3 = Pared.CrearPared(new Vector3(5, -2, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, 0), madera, "Obstaculo3");
                    Pared obstaculo4 = Pared.CrearPared(new Vector3(10, 0, 1), new Vector3(5, 0.25f, 1), new Vector3(0, 0, FastMath.PI / 4), madera, "Obstaculo4");
                    itemsDeNivel.Add(obstaculo1);
                    itemsDeNivel.Add(obstaculo2);
                    itemsDeNivel.Add(obstaculo3);
                    itemsDeNivel.Add(obstaculo4);
                    itemsDeNivel.Add(cannon2.baseCannon);
                    //Fin Items del Nivel
                    
                    fondo.setTexture(metal);
                    break;
            }
            
        }

        public void reiniciar(int nivel, Pelota pelota) 
            {
                pelota.reiniciar();
                switch (nivel) 
                { 
                    case 1:
                        cannon.cargado = true; 
                        break; 
                    case 2:                        
                        break; 
                } 
            } 


    }
}
