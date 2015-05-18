

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


        private Vector3 rebotar(TgcSphere esfera, float coefRebote, Vector3 velocidad, float factorVelocidad, TgcBoundingBox bbObstaculo)
        {

            normalColision = obtenerNormalDeColision(esfera.BoundingSphere, bbObstaculo, Vector3.Multiply(velocidad,factorVelocidad));

            if (Vector3.Dot(velocidad, normalColision) < 0)
            {
                Vector3 ortogColision = new Vector3(-normalColision.Y,normalColision.X,0);
                Vector3 proyectadoEnNormal;

                proyectadoEnNormal.X = -coefRebote * Vector3.Dot(velocidad, normalColision);
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
                if (TgcCollisionUtils.testSphereAABB(testSphere, item.getBB()))
                {
                    if (item.debeRebotar(esfera))
                        newVelocidad = rebotar(esfera, item.getCoefRebote(), velocidad, factorVelocidad, item.getBB());
                    else
                        newVelocidad = item.interactuarConPelota();
                    if (!newVelocidad.Equals(velocidad))
                    {
                        newVelocidad = ConsiderarColicionesCon(esfera, itemsInScenario, newVelocidad, factorVelocidad, nivelRecursivo + 1);
                    }
                    break;

                }
            }

            return newVelocidad;
        }


        private Vector3 obtenerNormalDeColision(TgcBoundingSphere bEsfera, TgcBoundingBox obstaculoBB, Vector3 movementVector)
        {
            float menorDistancia = float.MaxValue;
            TgcBoundingBox.Face collisionFace = null;
            TgcBoundingBox.Face bbFaceTransformada = new TgcBoundingBox.Face();

            Vector3 pNormal;


            TgcBoundingBox.Face[] bbFaces = obstaculoBB.computeFaces();

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
                if (!pointInBounbingBoxFace(puntoDeInterseccion, bbFaceTransformada))
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

        private bool pointInBounbingBoxFace(Vector3 p, TgcBoundingBox.Face bbFace)
        {
            Vector3 min = bbFace.Extremes[0];
            Vector3 max = bbFace.Extremes[3];

            return p.X >= min.X && p.Y >= min.Y && p.Z >= min.Z &&
               p.X <= max.X && p.Y <= max.Y && p.Z <= max.Z;
        }


    }
}
