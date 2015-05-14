using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.MiGrupo
{
    interface Stage
    {
        void render();

        Stage interaccion(TgcD3dInput input, float elapsedTime);

        void aplicarMovimientos(float elapsedTime);

        void mostrarStage();

    }
}
