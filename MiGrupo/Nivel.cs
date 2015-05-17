
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
        List<ItemUsuario> objetosDelUsuario;

        Pelota pelota;
        Vector3 iniPelota = new Vector3(0, 100, 0);

        TgcBox fondo;
        Pared piso;
        Pared lateralDerecha;
        Pared lateralIzquierda; 
        
        Construccion construccion;
        Play play;
        Etapa etapa;
        TgcText2d textStage = new TgcText2d();
        
        public Nivel( TgcTexture textFondo, TgcTexture textPiso, TgcTexture textParedes,
                     Pelota pelotita, List<Item> itemsDelNivel, List<ItemUsuario> itemsDelUsuario)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
                        
            pelota = pelotita;
            objetosDelUsuario = itemsDelUsuario;
            TgcBox cajaCreadora = TgcBox.fromSize(new Vector3(-10, 5, 0), new Vector3(1, 1, 0), textPiso); 

            fondo = TgcBox.fromSize(new Vector3(2.85f, 0, 0), new Vector3(32, 20.75f, 0), textFondo);
            piso = new Pared(TgcBox.fromSize(new Vector3(2.85f, -10.25f, 0), new Vector3(32, 0.25f, 1f), textPiso), 1);
            lateralDerecha = new Pared(TgcBox.fromSize(new Vector3(-13.1f, 0, 0), new Vector3(0.25f, 20.75f, 1f), textParedes), 1);
            lateralIzquierda = new Pared(TgcBox.fromSize(new Vector3(18.8f, 0, 0), new Vector3(0.25f, 20.75f, 1f), textParedes), 1);

            itemsDelNivel.Add(piso);
            itemsDelNivel.Add(lateralDerecha);
            itemsDelNivel.Add(lateralIzquierda);

            menu = new MenuObjetos(objetosDelUsuario, TgcTexture.createTexture(EjemploAlumno.alumnoTextureFolder() + "Laja.jpg"));

            construccion = new Construccion(itemsDelNivel, pelota,cajaCreadora, menu);
            play = new Play(itemsDelNivel, pelota);
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

            foreach (ItemUsuario objeto in objetosDelUsuario)
            {
                objeto.render();
            }
                        
            GuiController.Instance.Drawer2D.beginDrawSprite();
                menu.renderMenu(objetosDelUsuario.Count);
                textStage.render();
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void dispose(){
            fondo.dispose();
            piso.dispose();
            lateralDerecha.dispose();
            lateralIzquierda.dispose();
            menu.disposeMenu(objetosDelUsuario.Count);
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
            foreach (ItemUsuario objeto in objetosDelUsuario)
            {
                objeto.mesh.Effect = GuiController.Instance.Shaders.TgcMeshPointLightShader;
            }

            foreach (ItemUsuario objeto in objetosDelUsuario)
            {
                //Cargar variables shader de la luz
                objeto.mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                objeto.mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(new Vector3(0, 10.5f, 10)));
                objeto.mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(GuiController.Instance.ThirdPersonCamera.getPosition()));
                objeto.mesh.Effect.SetValue("lightIntensity", 15);
                objeto.mesh.Effect.SetValue("lightAttenuation", 1);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                objeto.mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                objeto.mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                objeto.mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                objeto.mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                objeto.mesh.Effect.SetValue("materialSpecularExp", 10f);
            }
        }
    }
}
