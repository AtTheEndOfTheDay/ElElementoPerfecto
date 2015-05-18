using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TgcViewer.Utils.Input;

using Microsoft.DirectX;

using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MiGrupo
{
    interface Item
    {
        void interactuar(TgcD3dInput input, float elapsedTime);
        Vector3 interactuarConPelota();
        void iluminar();
        void render();
        bool esMovil();
        void aplicarMovimientos(float elapsedTime);
        Vector3 velocidad();
        void dispose();
        TgcBoundingBox getBB();
        float getCoefRebote();
        bool debeRebotar(TgcSphere esfera);
    }
}
