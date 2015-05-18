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
    class Cannon : Item
    {
        public float potencia = 0.6f;
        public bool cargado = false;

        public Cannon(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion, Vector3 movimiento)
            : base(unMesh, texture)
        {
            mesh.setColor(uncolor);
            mesh.Scale = escalado;
            mesh.Rotation = rotacion;
            mesh.move(movimiento);
            enEscena = false;
        }
        
        public Cannon(TgcMesh unMesh, TgcTexture texture, Color uncolor, Vector3 escalado, Vector3 rotacion)
            : base(unMesh, texture)
        {
            mesh.setColor(uncolor);
            mesh.Scale = escalado;
            mesh.Rotation = rotacion;
            llevarAContenedor();
            enEscena = false;
        }

        public override Vector3 interactuarConPelota()
        {
            return new Vector3(-potencia, potencia, 0);
        }

        public override bool debeRebotar(TgcSphere esfera)
        {
            if (cargado)
            {
                cargado = false;
                return false;
            }                
            else
                return true;
        }

        public override float getCoefRebote(Vector3 normal)
        {
            return 0.25f;
        }
        
    }
}
