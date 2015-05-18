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
    abstract class ItemUsuario : Item
    {
        public bool enEscena = false;
        public TgcMesh mesh;
        TgcTexture textura;
        public ItemUsuario(TgcMesh unMesh, TgcTexture texture)
        {
            mesh = unMesh;
            textura = texture;
        }

        void Item.interactuar(TgcD3dInput input, float elapsedTime)
        {
            //No Implementado
        }

        void Item.interactuarConPelota(TgcD3dInput input, float elapsedTime, Pelota pelota)
        {
            interactuarConPelota(input, elapsedTime, pelota);
        }
        void Item.iluminar()
        {
            iluminar();
        }
        void Item.render()
        {
            render();
        }

        bool Item.esMovil()
        {
            return true;
        }

        void Item.aplicarMovimientos(float elapsedTime)
        {
            //No Implementado
        }

        Vector3 Item.velocidad()
        { 
            return new Vector3(0, 0, 0); 
        }
        void Item.dispose()
        {
            mesh.dispose();
        }

        public TgcTexture getTexture()
        {
            return textura;
        }

        internal void render()
        {
            if (enEscena)
            {
                mesh.render();
            }     
        }

        public abstract void interactuarConPelota(TgcD3dInput input, float elapsedTime, Pelota pelota);

        public void iluminar()
        {
            mesh.Effect = GuiController.Instance.Shaders.TgcMeshPointLightShader;

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

        internal void llevarAContenedor()
        {
            mesh.move ( new Vector3 ( -14.75f- mesh.Position.X, -8.5f - mesh.Position.Y, 0));
        }
    }
}
