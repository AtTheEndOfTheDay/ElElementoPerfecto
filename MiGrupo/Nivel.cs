
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

        TgcScene scene;

        public Nivel( TgcTexture textFondo, TgcTexture textPiso, TgcTexture textParedes,
                     Pelota pelotita, List<Item> itemsDelNivel, List<Item> itemsDelUsuario,
                        TgcScene escenario)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            scene = escenario;
            
            pelota = pelotita;
            objetosDeMenu = itemsDelUsuario;

            fondo = TgcBox.fromSize(new Vector3(2.85f, 0, 0), new Vector3(32, 20.75f, 0), textFondo);
            piso = new Pared(TgcBox.fromSize(new Vector3(2.85f, -10.25f, 0), new Vector3(32, 0.25f, 1f), textPiso), 1);
            lateralDerecha = new Pared(TgcBox.fromSize(new Vector3(-13.1f, 0, 0), new Vector3(0.25f, 20.75f, 1f), textParedes), 1);
            lateralIzquierda = new Pared(TgcBox.fromSize(new Vector3(18.8f, 0, 0), new Vector3(0.25f, 20.75f, 1f), textParedes), 1);

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

            iluminar();

            foreach (TgcMesh mesh in scene.Meshes)
            {
                mesh.render();
            }
                        
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
        public void reiniciar()
        {
            pelota.reiniciar();
            etapa = construccion;
            textStage.Text = etapa.getNombre();
        }

        private void iluminar()
        {
            foreach (TgcMesh mesh in scene.Meshes)
            {
                mesh.Effect = GuiController.Instance.Shaders.TgcMeshPointLightShader;
            }

            foreach (TgcMesh mesh in scene.Meshes)
            {
                //Cargar variables shader de la luz
                mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(new Vector3(0, 10.5f, 10)));
                mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(GuiController.Instance.ThirdPersonCamera.getPosition()));
                mesh.Effect.SetValue("lightIntensity", 15);
                mesh.Effect.SetValue("lightAttenuation", 1);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                mesh.Effect.SetValue("materialSpecularExp", 10f);
            }
        }
    }
}
