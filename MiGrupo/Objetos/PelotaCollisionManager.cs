

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;

namespace AlumnoEjemplos.MiGrupo
{
    class PelotaCollisionManager
    {

        private Vector3 normalColision;


        private Vector3 rebotar(TgcSphere esfera, Item item, Vector3 velocidad, float factorVelocidad)
        {

            normalColision = obtenerNormalDeColision(esfera.BoundingSphere, item.getOBB(), Vector3.Multiply(velocidad,factorVelocidad));

            if (Vector3.Dot(velocidad, normalColision) < 0)
            {
                Vector3 ortogColision = new Vector3(-normalColision.Y,normalColision.X,0);
                Vector3 proyectadoEnNormal;

                proyectadoEnNormal.X = -item.getCoefRebote(normalColision) * Vector3.Dot(velocidad, normalColision);
                proyectadoEnNormal.Y = Vector3.Dot(velocidad, ortogColision);
                proyectadoEnNormal.Z = 0;

                velocidad = Vector3.Multiply(normalColision,proyectadoEnNormal.X) + Vector3.Multiply(ortogColision,proyectadoEnNormal.Y);

            }

            return velocidad;
        }

        public Vector3 ConsiderarColicionesCon(TgcSphere esfera, List<Item> itemsInScenario, Vector3 velocidad, float factorVelocidad, int nivelRecursivo)
        {
            Vector3 movement = Vector3.Multiply(velocidad, factorVelocidad);

            if (nivelRecursivo > 5)
            {
                return new Vector3(0f,0f,0f);
            }

            TgcBoundingSphere testSphere = new TgcBoundingSphere(esfera.Position + movement, esfera.Radius);
            Vector3 newVelocidad = velocidad;

            foreach (Item item in itemsInScenario)
            {
                if (TgcCollisionUtils.testSphereOBB(testSphere, item.getOBB()))
                {
                    if (item.debeRebotar(esfera))
                        newVelocidad = rebotar(esfera, item, velocidad, factorVelocidad);
                    /*
                    else
                        newVelocidad = item.interactuarConPelota();
                    */
                    if (!newVelocidad.Equals(velocidad))
                    {
                        newVelocidad = ConsiderarColicionesCon(esfera, itemsInScenario, newVelocidad, factorVelocidad, nivelRecursivo + 1);
                    }
                    break;

                }
            }

            return newVelocidad;
        }


        private Vector3[] computeCornersObb(TgcObb obb)
        {
            Vector3[] corners = new Vector3[8];

            Vector3 eX = obb.Extents.X * obb.Orientation[0];
            Vector3 eY = obb.Extents.Y * obb.Orientation[1];
            Vector3 eZ = obb.Extents.Z * obb.Orientation[2];

            corners[0] = obb.Center - eX - eY - eZ;
            corners[1] = obb.Center - eX - eY + eZ;

            corners[2] = obb.Center - eX + eY - eZ;
            corners[3] = obb.Center - eX + eY + eZ;

            corners[4] = obb.Center + eX - eY - eZ;
            corners[5] = obb.Center + eX - eY + eZ;

            corners[6] = obb.Center + eX + eY - eZ;
            corners[7] = obb.Center + eX + eY + eZ;

            return corners;
        }


        private TgcBoundingBox.Face[] computeFacesObb(TgcObb obb)
        {
            TgcBoundingBox.Face[] auxFaces = new TgcBoundingBox.Face[6];
            Vector3[] obbCorners = computeCornersObb(obb);
            Vector3[] normales = new Vector3[6];

            normales[0] = obb.Orientation[1];
            normales[1] = -obb.Orientation[1];

            normales[2] = obb.Orientation[2];
            normales[3] = -obb.Orientation[2];
            
            normales[4] = obb.Orientation[0];
            normales[5] = -obb.Orientation[0];


            //eY positive
            auxFaces[0] = new TgcBoundingBox.Face();
            auxFaces[0].Extremes[0] = obbCorners[2];
            auxFaces[0].Extremes[1] = obbCorners[3];
            auxFaces[0].Extremes[2] = obbCorners[6];
            auxFaces[0].Extremes[3] = obbCorners[7];

            //eY negative 
            auxFaces[1] = new TgcBoundingBox.Face();
            auxFaces[1].Extremes[0] = obbCorners[0];
            auxFaces[1].Extremes[1] = obbCorners[1];
            auxFaces[1].Extremes[2] = obbCorners[4];
            auxFaces[1].Extremes[3] = obbCorners[5];

            //eZ positive
            auxFaces[2] = new TgcBoundingBox.Face();
            auxFaces[2].Extremes[0] = obbCorners[1];
            auxFaces[2].Extremes[1] = obbCorners[3];
            auxFaces[2].Extremes[2] = obbCorners[5];
            auxFaces[2].Extremes[3] = obbCorners[7];

            //eZ negative 
            auxFaces[3] = new TgcBoundingBox.Face();
            auxFaces[3].Extremes[0] = obbCorners[0];
            auxFaces[3].Extremes[1] = obbCorners[2];
            auxFaces[3].Extremes[2] = obbCorners[4];
            auxFaces[3].Extremes[3] = obbCorners[6];

            //eX positive
            auxFaces[4] = new TgcBoundingBox.Face();
            auxFaces[4].Extremes[0] = obbCorners[4];
            auxFaces[4].Extremes[1] = obbCorners[5];
            auxFaces[4].Extremes[2] = obbCorners[6];
            auxFaces[4].Extremes[3] = obbCorners[7];

            //eX negative 
            auxFaces[5] = new TgcBoundingBox.Face();
            auxFaces[5].Extremes[0] = obbCorners[0];
            auxFaces[5].Extremes[1] = obbCorners[1];
            auxFaces[5].Extremes[2] = obbCorners[2];
            auxFaces[5].Extremes[3] = obbCorners[3];


            for (int i = 0; i < 6; i++)
            {
                auxFaces[i].Plane = new Plane(normales[i].X, normales[i].Y, normales[i].Z, -Vector3.Dot(auxFaces[i].Extremes[0], normales[i]));
            }

            return auxFaces;
        }

        private Vector3 obtenerNormalDeColision(TgcBoundingSphere bEsfera, TgcObb obstaculoOBB, Vector3 movementVector)
        {
            float menorDistancia = float.MaxValue;
            TgcBoundingBox.Face collisionFace = null;
            TgcBoundingBox.Face bbFaceTransformada = new TgcBoundingBox.Face();

            Vector3 pNormal;


            TgcBoundingBox.Face[] bbFaces = computeFacesObb(obstaculoOBB);

            foreach (TgcBoundingBox.Face bbFace in bbFaces)
            {
                pNormal = TgcCollisionUtils.getPlaneNormal(bbFace.Plane);

                //ensanchar las paredes

                //TODO en vez de ensanchar iría convertirlo en un cuarto de circunferencia
                for (int i = 0; i < 4; i++)
                {
                    bbFaceTransformada.Extremes[i] = bbFace.Extremes[i] + Vector3.Multiply(pNormal, bEsfera.Radius);

                    for (int j = 0; j < 4; j++)
                    {
                        //si no es el opuesto ni es el mismo vertice
                        if ((j != 3 - i) && (j != i))
                        {
                            //muevo el vertice la distancia del radio en direccion opuesta a el otro vertice
                            bbFaceTransformada.Extremes[i] += Vector3.Multiply(Vector3.Normalize(bbFace.Extremes[i] - bbFace.Extremes[j]), bEsfera.Radius);
                        }
                    }

                }




                bbFaceTransformada.Plane = new Plane(pNormal.X, pNormal.Y, pNormal.Z, -Vector3.Dot(pNormal, bbFaceTransformada.Extremes[0]));



                //proyectar rayo del centro de la pelota con su movimiento
                TgcRay movementRay = new TgcRay(bEsfera.Center, movementVector);
                float distanciaInterseccion;
                Vector3 puntoDeInterseccion;
                //Si no intersecta con el plano de la pared => descarta la cara
                if (!TgcCollisionUtils.intersectRayPlane(movementRay, bbFaceTransformada.Plane, out distanciaInterseccion, out puntoDeInterseccion))
                {
                    if (!TgcCollisionUtils.intersectRayPlane(movementRay, bbFace.Plane, out distanciaInterseccion, out puntoDeInterseccion))
                    {
                        continue;
                    }

                    //TODO esta entre los dos planos (el transformado y el original) => puede que pege en el vertice

                    return Vector3.Normalize(-movementVector);
                }

                //Si el punto intersectado con el plano no esta incluido en la cara => descarto la cara
                if (!pointInOBBFace(puntoDeInterseccion, bbFaceTransformada, obstaculoOBB))
                {
                    continue;
                }

                if (distanciaInterseccion < menorDistancia)
                {
                    collisionFace = bbFace;
                    menorDistancia = distanciaInterseccion;
                }


            }

            //si colisiona con alguna de las caras => devuelvo su normal
            if (collisionFace != null)
            {
                return TgcCollisionUtils.getPlaneNormal(collisionFace.Plane);

            }
            else
            {
                return Vector3.Empty;
            }
        }


        private bool pointInOBBFace(Vector3 pPrima, TgcBoundingBox.Face bbFace, TgcObb obb)
        {
            const float TOLERANCIA = 0.001f;

            Vector3 p = obb.toObbSpace(pPrima);

            Vector3 min = obb.toObbSpace(bbFace.Extremes[0]);
            Vector3 max = obb.toObbSpace(bbFace.Extremes[3]);

            return (p.X - min.X) >= -TOLERANCIA && (p.Y - min.Y) >= -TOLERANCIA && (p.Z - min.Z) >= -TOLERANCIA &&
                (p.X - max.X) <= TOLERANCIA && (p.X - max.X) <= TOLERANCIA && (p.X - max.X) <= TOLERANCIA;

        }


    }
}
