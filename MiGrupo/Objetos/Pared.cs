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
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.MiGrupo
{
    class Pared : Item
    {
        public static Pared CrearPared(Vector3 posicion, Vector3 size, Vector3 rotacion, TgcTexture texture, String nombre)
        {
            Pared auxPared;

            TgcMesh auxMesh = TgcBox.fromSize(new Vector3(0, 0, 0), size, texture).toMesh(nombre);
            auxMesh.Position = posicion;
            
            auxPared = new Pared(auxMesh,texture);

            ((Item) auxPared).rotate(rotacion);

            return auxPared;
        }
        
        public Pared(TgcMesh mesh, TgcTexture texture) :base(mesh,texture)
        {
            enEscena = true;
        }

        public override Vector3 interactuarConPelota()
        {
            return new Vector3(0, 0, 0);
        }
        
        public override bool debeRebotar(TgcSphere esfera)
        {
            return true;
        }
        public override float getCoefRebote(Vector3 normal)
        {
            return 0.5f;
        }
        public override void llevarAContenedor()
        {
            mesh.move(new Vector3(-15.5f - mesh.Position.X, -8f - mesh.Position.Y, 1 - mesh.Position.Z));
        }
    }
}
