using Microsoft.DirectX;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public static class MatrixExtension
    {
        public static Vector3 Multiply(this Matrix m, Vector3 v)
        {
            return new Vector3(
                m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z,
                m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z,
                m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z
            );
        }
    }
}
