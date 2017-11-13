using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Collision.ThePhysicsEngine.Common
{
    public class doublePoint
    {
        public double X;
        public double Y;

        public doublePoint(double aX, double aY)
        {
            X = aX;
            Y = aY;
        }

        public doublePoint(doublePoint starter)
        {
            X = starter.X;
            Y = starter.Y;
        }

        public Point getPointSet()
        {
            return new Point((int)X, (int)Y);
        }
        public void shift(double deltaX, double deltaY)
        {
            X += deltaX;
            Y += deltaY;
        }

    }

    public class DoublePointArray
    {
        doublePoint[] myPoints;

        public DoublePointArray(doublePoint[] aPointSet)
        {
            myPoints = aPointSet;
        }

        public Point[] ToPointArray()
        {
            Point[] returnSet = new Point[myPoints.Length];

            for (int i = 0; i < myPoints.Length; i++)
            {
                returnSet[i] = myPoints[i].getPointSet();
            }

            return returnSet;
        }
        public int Length()
        {
            return myPoints.Length;
        }

        public doublePoint getAt(int aSlot)
        {
            return myPoints[aSlot];
        }

        public doublePoint[] ToDoublePointArray()
        {
            return myPoints;
        }

        public void Add(doublePoint aNewPoint)
        {
            doublePoint[] newSet = new doublePoint[myPoints.Length + 1];

            for (int i = 0; i < myPoints.Length; i++)
            {
                newSet[i] = myPoints[i];
            }

            newSet[myPoints.Length] = aNewPoint;

            myPoints = newSet;
        }

        public void Add(doublePoint aNewPoint, int aSlot)
        {
            doublePoint[] newSet = new doublePoint[myPoints.Length + 1];

            bool hasAddedIn = false;

            for (int i = 0; i < myPoints.Length + 1; i++)
            {
                if (hasAddedIn)
                    newSet[i] = myPoints[i - 1];
                else
                {
                    if (i == aSlot)
                    {
                        newSet[i] = aNewPoint;
                        hasAddedIn = true;
                    }
                    else
                        newSet[i] = myPoints[i];
                }
            }

            myPoints = newSet;
        }

    }
    public class RayVector
    {
        public double DeltaX;
        public double DeltaY;
        public double PosX;
        public double PosY;

        public RayVector(double aPosX, double aPosY, double aDeltaX, double aDeltaY)
        {
            DeltaX = aDeltaX;
            DeltaY = aDeltaY;
            PosX = aPosX;
            PosY = aPosY;
        }

        public RayVector(RayVector starter)
        {
            DeltaX = starter.DeltaX;
            DeltaY = starter.DeltaY;

            PosX = starter.PosX;
            PosY = starter.PosY;
        }


        public void shift(double deltaX, double deltaY)
        {
            PosX += deltaX;
            PosY += deltaY;
        }

    }

    public enum CollisionType { None, PointPoint, SegmentPoint, SegmentCross, LineOverlap, ContainsOtherArea, OtherContainsArea };

    public class CollisionObj
    {
        CollisionType myTypeOfCollision;
        DoublePointArray myPointSet;
        public CollisionObj(CollisionType aTypeOfCollision, DoublePointArray aPointSet)
        {
            myTypeOfCollision = aTypeOfCollision;
            myPointSet = aPointSet;
        }

        public CollisionType getMyCollisionType()
        {
            return myTypeOfCollision;
        }

        public DoublePointArray getPointSet()
        {
            return myPointSet;
        }
    }
}
