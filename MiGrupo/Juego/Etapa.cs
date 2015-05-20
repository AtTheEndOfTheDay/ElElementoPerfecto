using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.MiGrupo
{
    interface Etapa
    {
        void interaccion(TgcD3dInput input, float elapsedTime);
        String getNombre();
    }
}
