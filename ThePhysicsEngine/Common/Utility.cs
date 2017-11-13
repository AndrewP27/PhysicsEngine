using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collision.ThePhysicsEngine;
using System.Drawing;
using System.Windows.Forms;

namespace Collision.ThePhysicsEngine.Common
{
    public static class Utility
    {
        private static double DIFF_FOR_APPROX_EQUAL = 0.005;

        public static CollisionObj checkCollisionLineSegments(DoublePointArray line1, DoublePointArray line2, PaintEventArgs e)
        {

            // point00 will be the new orgin
            doublePoint point00 = new doublePoint(line1.getAt(0));
            doublePoint point01 = new doublePoint(line1.getAt(1));

            doublePoint point10 = new doublePoint(line2.getAt(0));
            doublePoint point11 = new doublePoint(line2.getAt(1));
            

            // Check for Vertical Lines
            if(isApproxEqual(point00.X, point01.X) && isApproxEqual(point10.X, point11.X))
            {
                return checkForLineOverlap(point00, point01, point10, point11,e);
            }

            if(isApproxEqual(point00.X, point01.X))
            {
                return collisionAlgorithm(point00, point01, point10, point11, e);
            }

            if (isApproxEqual(point10.X, point11.X))
            {
                return collisionAlgorithm(point10, point11, point00, point01, e);
            }

            return collisionAlgorithm(point00, point01, point10, point11, e);


            

        }

        public static CollisionObj collisionAlgorithm(doublePoint point00, doublePoint point01, doublePoint point10, doublePoint point11, PaintEventArgs e)
        {

            doublePoint point00Original = new doublePoint(point00);
            doublePoint point01Original = new doublePoint(point01);
            doublePoint point10Original = new doublePoint(point10);
            doublePoint point11Original = new doublePoint(point11);

            // Move Origin
            point10.shift(-point00.X, -point00.Y);
            point11.shift(-point00.X, -point00.Y);

            point01.shift(-point00.X, -point00.Y);
            point00.shift(-point00.X, -point00.Y);

            // Rotate Orientation
            double mag1 = Math.Sqrt(point01.X * point01.X + point01.Y * point01.Y);
            double mag2 = Math.Sqrt(point10.X * point10.X + point10.Y * point10.Y);
            double mag3 = Math.Sqrt(point11.X * point11.X + point11.Y * point11.Y);

            double angle1 = Math.Atan2(point01.Y, point01.X);
            double angle2 = Math.Atan2(point10.Y, point10.X);
            double angle3 = Math.Atan2(point11.Y, point11.X);

            point01.X = mag1;
            point01.Y = 0;

            point10.X = mag2 * Math.Cos(angle2 - angle1);
            point10.Y = mag2 * Math.Sin(angle2 - angle1);

            point11.X = mag3 * Math.Cos(angle3 - angle1);
            point11.Y = mag3 * Math.Sin(angle3 - angle1);

            // Check Collision
            if (!isApproxEqual(point11.Y, point10.Y) && !isApproxEqual(point11.X, point10.X))
            {
                double slopeInv = (point11.X - point10.X) / (point11.Y - point10.Y);
                double xintercept = point11.X - point11.Y * slopeInv;

                if (checkIsWithin(xintercept, point10.X, point11.X) && checkIsWithin(xintercept, point00.X, point01.X))
                {
                    double xCollision = xintercept * Math.Cos(angle1) + point00Original.X;
                    double yCollision = xintercept * Math.Sin(angle1) + point00Original.Y;

                    e.Graphics.DrawRectangle(Pens.Purple, (float)xCollision - 5, (float)yCollision - 5, 10, 10);
                    doublePoint[] collisionPoint = { new doublePoint(xCollision, yCollision) };

                    if(checkIsOnEndPoint(xintercept, point10.X, point11.X) && checkIsOnEndPoint(xintercept, point00.X, point01.X))
                        return new CollisionObj(CollisionType.PointPoint, new DoublePointArray(collisionPoint));
                    else if (checkIsOnEndPoint(xintercept, point10.X, point11.X) || checkIsOnEndPoint(xintercept, point00.X, point01.X))
                        return new CollisionObj(CollisionType.SegmentPoint, new DoublePointArray(collisionPoint));
                    else
                        return new CollisionObj(CollisionType.SegmentCross, new DoublePointArray(collisionPoint));
                }
                else
                    return null;
            }
            else if(!isApproxEqual(point11.X, point10.X))
            {
                point01.X = 0;
                point01.Y = mag1;

                point10.X = mag2 * Math.Cos(angle2 - angle1 + Math.PI/2);
                point10.Y = mag2 * Math.Sin(angle2 - angle1 + Math.PI / 2);

                point11.X = mag3 * Math.Cos(angle3 - angle1 + Math.PI / 2);
                point11.Y = mag3 * Math.Sin(angle3 - angle1 + Math.PI / 2);

                return checkForLineOverlap(point00, point01, point10, point11, point00Original, point01Original, point10Original, point11Original, e);
            }
            else
            {
                return checkForLineOverlap(point00, point01, point10, point11, point00Original, point01Original, point10Original, point11Original, e);
            }
        }

        public static CollisionObj checkForLineOverlap(doublePoint point00, doublePoint point01, doublePoint point10, doublePoint point11, doublePoint pointOriginal00, doublePoint pointOriginal01, doublePoint pointOriginal10, doublePoint pointOriginal11, PaintEventArgs e)
        {
            // One Direction
            if (checkIsWithin(point00.Y, point10.Y, point11.Y) && isApproxEqual(point00.X, point10.X))
            {
                if (checkIsWithin(point01.Y, point10.Y, point11.Y))
                {
                    doublePoint[] CollisionSet = { pointOriginal00, pointOriginal01 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point10.Y, point00.Y, point01.Y))
                {
                    doublePoint[] CollisionSet = { pointOriginal00, pointOriginal10 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { pointOriginal00, pointOriginal11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }
            else if (checkIsWithin(point01.Y, point10.Y, point11.Y) && isApproxEqual(point00.X, point10.X))
            {
                if (checkIsWithin(point00.Y, point10.Y, point11.Y))
                {
                    doublePoint[] CollisionSet = { pointOriginal01, pointOriginal00 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point10.Y, point00.Y, point01.Y))
                {
                    doublePoint[] CollisionSet = { pointOriginal01, pointOriginal10 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { pointOriginal01, pointOriginal11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }
            else if (checkIsWithin(point10.Y, point00.Y, point01.Y) && isApproxEqual(point00.X, point10.X))
            {
                if (checkIsWithin(point00.Y, point10.Y, point11.Y))
                {
                    doublePoint[] CollisionSet = { pointOriginal10, pointOriginal00 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point01.Y, point00.Y, point01.Y))
                {
                    doublePoint[] CollisionSet = { pointOriginal10, pointOriginal01 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { pointOriginal10, pointOriginal11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }

            // The other Direction
            if (checkIsWithin(point00.X, point10.X, point11.X) && isApproxEqual(point00.Y, point10.Y))
            {
                if (checkIsWithin(point01.X, point10.X, point11.X))
                {
                    doublePoint[] CollisionSet = { pointOriginal00, pointOriginal01 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point10.X, point00.X, point01.X))
                {
                    doublePoint[] CollisionSet = { pointOriginal00, pointOriginal10 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { pointOriginal00, pointOriginal11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }
            else if (checkIsWithin(point01.X, point10.X, point11.X) && isApproxEqual(point00.Y, point10.Y))
            {
                if (checkIsWithin(point00.X, point10.X, point11.X))
                {
                    doublePoint[] CollisionSet = { pointOriginal01, pointOriginal00 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point10.X, point00.X, point01.X))
                {
                    doublePoint[] CollisionSet = { pointOriginal01, pointOriginal10 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { pointOriginal01, pointOriginal11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }
            else if (checkIsWithin(point10.X, point00.X, point01.X) && isApproxEqual(point00.Y, point10.Y))
            {
                if (checkIsWithin(point00.X, point10.X, point11.X))
                {
                    doublePoint[] CollisionSet = { pointOriginal10, pointOriginal00 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point01.X, point10.X, point11.X))
                {
                    doublePoint[] CollisionSet = { pointOriginal10, pointOriginal01 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { pointOriginal10, pointOriginal11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }
            
            return null;
        }

        public static CollisionObj checkForLineOverlap(doublePoint point00, doublePoint point01, doublePoint point10, doublePoint point11, PaintEventArgs e)
        {
            // One Direction
            if (checkIsWithin(point00.Y, point10.Y, point10.Y) && isApproxEqual(point00.X, point10.X))
            {
                if (checkIsWithin(point01.Y, point10.Y, point10.Y))
                {
                    doublePoint[] CollisionSet = { point00, point01 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point10.Y, point00.Y, point01.Y))
                {
                    doublePoint[] CollisionSet = { point00, point10 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { point00, point11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }
            else if (checkIsWithin(point01.Y, point10.Y, point10.Y) && isApproxEqual(point00.X, point10.X))
            {
                if (checkIsWithin(point00.Y, point10.Y, point10.Y))
                {
                    doublePoint[] CollisionSet = { point01, point00 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point10.Y, point00.Y, point01.Y))
                {
                    doublePoint[] CollisionSet = { point01, point10 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { point01, point11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }
            else if (checkIsWithin(point10.Y, point10.Y, point10.Y) && isApproxEqual(point00.X, point10.X))
            {
                if (checkIsWithin(point00.Y, point10.Y, point10.Y))
                {
                    doublePoint[] CollisionSet = { point10, point00 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point01.Y, point00.Y, point01.Y))
                {
                    doublePoint[] CollisionSet = { point10, point01 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { point10, point11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }

            // The other Direction
            if (checkIsWithin(point00.X, point10.X, point10.X) && isApproxEqual(point00.Y, point10.Y))
            {
                if (checkIsWithin(point01.X, point10.X, point10.X))
                {
                    doublePoint[] CollisionSet = { point00, point01 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point10.X, point00.X, point01.X))
                {
                    doublePoint[] CollisionSet = { point00, point10 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { point00, point11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }
            else if (checkIsWithin(point01.X, point10.X, point10.X) && isApproxEqual(point00.Y, point10.Y))
            {
                if (checkIsWithin(point00.X, point10.X, point10.X))
                {
                    doublePoint[] CollisionSet = { point01, point00 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point10.X, point00.X, point01.X))
                {
                    doublePoint[] CollisionSet = { point01, point10 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { point01, point11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }
            else if (checkIsWithin(point10.X, point10.X, point10.X) && isApproxEqual(point00.Y, point10.Y))
            {
                if (checkIsWithin(point00.X, point10.X, point10.X))
                {
                    doublePoint[] CollisionSet = { point10, point00 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else if (checkIsWithin(point01.X, point00.X, point01.X))
                {
                    doublePoint[] CollisionSet = { point10, point01 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
                else
                {
                    doublePoint[] CollisionSet = { point10, point11 };
                    paintCollisionSet(CollisionSet, e);
                    return new CollisionObj(CollisionType.LineOverlap, new DoublePointArray(CollisionSet));
                }
            }

            return null;
        }

        public static bool checkIsWithin(double numberExamined, double range1, double range2)
        {
            if(range1 > range2)
            {
                if ((numberExamined <= range1 || isApproxEqual(numberExamined,range1)) && (numberExamined >= range2) || isApproxEqual(numberExamined, range2))
                    return true;
                else
                    return false;
            }
            else if (range2 > range1)
            {
                if (numberExamined <= range2 && numberExamined >= range1)
                    return true;
                else
                    return false;
            }
            else
            {
                if (isApproxEqual(numberExamined,range1))
                    return true;
                else
                    return false;
            }
        }

        public static bool checkIsOnEndPoint(double numberExamined, double range1, double range2)
        {
            if (isApproxEqual(numberExamined, range2) || isApproxEqual(numberExamined, range1))
                return true;
            else
                return false;
        }

        private static void paintCollisionSet(doublePoint[] CollisionSet, PaintEventArgs e)
        {
            foreach (doublePoint i in CollisionSet)
            {
                e.Graphics.DrawRectangle(Pens.Purple, (float)i.X - 5, (float)i.Y - 5, 10, 10);
            }
        }

        public static bool isApproxEqual(double number1, double number2)
        {
            return (Math.Abs(number1 - number2) < DIFF_FOR_APPROX_EQUAL);
        }
    }
}
