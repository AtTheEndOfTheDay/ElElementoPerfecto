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
    abstract class Item
    {
        bool enEscena = false;
        public TgcMesh mesh;
        TgcTexture textura;
        public TgcObb orientedBB;

        public Item() { }

        public Item(TgcMesh unMesh, TgcTexture texture)
        {
            mesh = unMesh;
            textura = texture;

            orientedBB = TgcObb.computeFromAABB(mesh.BoundingBox);
            orientedBB.setRenderColor(Color.Red);
        }

        public virtual void interactuarConPelota(Pelota pelota, float elapsedTime)
        { }

        public  virtual void iluminar()
        {
            mesh.Effect = GuiController.Instance.Shaders.TgcMeshPointLightShader;

            //Cargar variables shader de la luz
            mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
            mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(new Vector3(-13.1f, 10.5f, 10)));
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

        public void render()
        {
            if (enEscena)
            {
                if (EjemploAlumno.mostrarBBs)
                {
                    mesh.BoundingBox.render();
                }
                
                if (EjemploAlumno.mostrarOBBs)
                {
                    orientedBB.render();
                }
                mesh.render();
            }
        }
        
        public void dispose()
        {
            mesh.dispose();
        }

        public TgcBoundingBox getBB()
        {
            return mesh.BoundingBox;
        }

        public TgcObb getOBB()
        {
            return orientedBB;
        }

        public abstract float getCoefRebote(Vector3 normal);

        public abstract bool debeRebotar(TgcSphere esfera);

        public TgcTexture getTexture()
        {
            return textura;
        }

        public virtual void rotate(Vector3 rotacion)
        {
            Vector3 distanceBetweenCenters = orientedBB.Center - mesh.Position;

            distanceBetweenCenters = Vector3.TransformCoordinate(distanceBetweenCenters, Matrix.RotationYawPitchRoll(rotacion.Y, rotacion.X, rotacion.Z));
            orientedBB.Center = mesh.Position + distanceBetweenCenters;

            mesh.Rotation += rotacion;
            orientedBB.rotate(rotacion);
        }

        public Vector3 getPosition()
        {
            return mesh.Position;
        }

        public void setPosition(Vector3 newPosition)
        {
            move(newPosition - mesh.Position);
        }

        public virtual void move(Vector3 movement)
        {
            mesh.move(movement);
            orientedBB.move(movement);
        }

        public void llevarAContenedor()
        {
            move(getLugarDelContenedor() - mesh.Position);
        }

        //Cada mesh es distinto, por eso tiene un lugar distinto en el contenedor.
        public abstract Vector3 getLugarDelContenedor();

        public virtual void setenEscena(bool aparece)
        {
            enEscena = aparece;
        }

        public bool getenEscena()
        {
            return enEscena;
        }

    }
}

