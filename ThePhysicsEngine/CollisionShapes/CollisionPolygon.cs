using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Collision.ThePhysicsEngine.Common;

namespace Collision.ThePhysicsEngine.CollisionShapes
{
    public class CollisionPolygon : CollsionBaseShape
    {
        DoublePointArray pointSet;
        Pen outlinePen = new Pen(Brushes.Black);
        Brush fillBrush = Brushes.Aqua;
        int my_ID;
        double my_Area = -1;
        doublePoint my_Centroid = null;

        bool DEBUG_SHOW_CENTROID = true;
        bool DEBUG_SHOW_OVERLAP = false;

        public CollisionPolygon(DoublePointArray setOfPoints, int anID)
        {
            pointSet = setOfPoints;
            my_ID = anID;
            outlinePen.Width = 1F;
            getCentroid();
        }

        public override void drawSelf(PaintEventArgs e)
        {
            if (pointSet != null)
            {
                if (!DEBUG_SHOW_OVERLAP)
                    e.Graphics.FillPolygon(fillBrush, pointSet.ToPointArray());
                e.Graphics.DrawPolygon(outlinePen, pointSet.ToPointArray());
            }

            if (DEBUG_SHOW_CENTROID)
            {
                Point centroid = my_Centroid.getPointSet();
                Point[] testSet = { centroid, new Point(centroid.X+1, centroid.Y), new Point(centroid.X + 1, centroid.Y+1), new Point(centroid.X , centroid.Y + 1) };
                //e.Graphics.DrawPolygon(Pens.Red, testSet);
            }

            

        }

        public override double getArea()
        {
            if (my_Area == -1)
            {
                double Area = 0;
                for (int i = 0; i < pointSet.Length(); i++)
                {
                    doublePoint thisPoint;
                    doublePoint nextPoint;
                    if (i == pointSet.Length()-1)
                    {
                         thisPoint = pointSet.getAt(i);
                         nextPoint = pointSet.getAt(0);
                    }
                    else
                    {
                         thisPoint = pointSet.getAt(i);
                         nextPoint = pointSet.getAt(i + 1);
                    }

                    Area += thisPoint.X * nextPoint.Y - nextPoint.X * thisPoint.Y;
                }

                my_Area = Area * 0.5;
            }
            return my_Area;
        }

        public override doublePoint getCentroid()
        {
            if (my_Centroid == null)
            {
                double XCord = 0;
                double YCord = 0;

                double area = getArea();

                for (int i = 0; i < pointSet.Length(); i++)
                {

                    doublePoint thisPoint;
                    doublePoint nextPoint;

                    if (i == pointSet.Length() - 1)
                    {

                        thisPoint = pointSet.getAt(i);
                        nextPoint = pointSet.getAt(0);
                    }
                    else
                    {
                        thisPoint = pointSet.getAt(i);
                        nextPoint = pointSet.getAt(i + 1);

                    }
                    XCord += (thisPoint.X + nextPoint.X) * (thisPoint.X * nextPoint.Y - nextPoint.X * thisPoint.Y);
                    YCord += (thisPoint.Y + nextPoint.Y) * (thisPoint.X * nextPoint.Y - nextPoint.X * thisPoint.Y);
                }
                my_Centroid = new doublePoint(XCord / 6 / area, YCord / 6 / area);
            }
            return my_Centroid;

        }

        public override void checkCollision(CollsionBaseShape otherPoly, PaintEventArgs e)
        {
            List<CollisionObj> myCollsions = new List<CollisionObj>();

            List<DoublePointArray> myLineSegments = getAllLineSegments();

            List<DoublePointArray> otherLineSegments = otherPoly.getAllLineSegments();
            foreach (DoublePointArray i in otherLineSegments)
            {
                foreach (DoublePointArray j in myLineSegments)
                {
                    CollisionObj thisCollision = Utility.checkCollisionLineSegments(i, j, e);
                    if (thisCollision != null)
                        myCollsions.Add(thisCollision);
                }
            }

            if (myCollsions.Count == 0)
            {
                CollisionObj thisCollision = checkOverlap(otherPoly, e);
                if (thisCollision != null)
                    myCollsions.Add(thisCollision);
            }

        }

        public override DoublePointArray getPointSet()
        {
            return pointSet;
        }

        public override doublePoint getFirstPoint()
        {
            return pointSet.getAt(0);
        }

        public override void addInPoint(doublePoint aNewPoint, PaintEventArgs e)
        {
            bool failed = true;

            my_Area = -1;
            my_Centroid = null;

            int iterator = 0;

            while (failed)
            {
                failed = false;

                DoublePointArray testArray = new DoublePointArray(this.pointSet.ToDoublePointArray());

                testArray.Add(aNewPoint, iterator);

                CollisionPolygon testOption = new CollisionPolygon(testArray, -1);

                List<DoublePointArray> myLineSegments = testOption.getAllLineSegments();

                for (int i = 0; i < myLineSegments.Count; i++)
                {
                    for (int j = i+2; j < myLineSegments.Count-1; j++)
                    {
                        CollisionObj thisCollision = Utility.checkCollisionLineSegments(myLineSegments[i], myLineSegments[j], e);
                        if (thisCollision != null)
                        {
                            failed = true;
                            break;
                        }
                    }
                    if (failed)
                        break;
                }

                if(failed)
                {
                    iterator++;
                    if (iterator >= testArray.Length())
                        break;
                }

            }

            if (failed)
                throw new Exception("NEW POINT HAS NOT BEEN ADDED!");

            pointSet.Add(aNewPoint, iterator);
        }

        private CollisionObj checkOverlap(CollsionBaseShape otherPoly, PaintEventArgs e)
        {
            DoublePointArray testArray = new DoublePointArray(this.pointSet.ToDoublePointArray());
            CollisionPolygon testObject = new CollisionPolygon(testArray, -1);
            testObject.addInPoint(otherPoly.getFirstPoint(), e);

            if (DEBUG_SHOW_OVERLAP)
                testObject.drawSelf(e);

            if(this.getArea() > testObject.getArea())
            {
                doublePoint[] otherCentroid = { otherPoly.getCentroid() };
                return new CollisionObj(CollisionType.ContainsOtherArea, new DoublePointArray(otherCentroid));
            }

            testArray = new DoublePointArray(otherPoly.getPointSet().ToDoublePointArray());
            testObject = new CollisionPolygon(testArray, -1);
            testObject.addInPoint(this.getFirstPoint(), e);

            if (DEBUG_SHOW_OVERLAP)
                testObject.drawSelf(e);

            if (otherPoly.getArea() > testObject.getArea())
            {
                doublePoint[] centroid = { this.getCentroid() };
                return new CollisionObj(CollisionType.OtherContainsArea, new DoublePointArray(centroid));
            }

            return null;
        }

        public override List<DoublePointArray> getAllLineSegments()
        {
            List<DoublePointArray> returnSet = new List<DoublePointArray>();

            for (int i = 0; i < pointSet.Length(); i++)
            {
                doublePoint[] points = new doublePoint[2];
                if(i == pointSet.Length()-1)
                {
                    points[0] = pointSet.getAt(i);
                    points[1] = pointSet.getAt(0);
                }
                else
                {
                    points[0] = pointSet.getAt(i);
                    points[1] = pointSet.getAt(i+1);
                }
                DoublePointArray lineSegment = new DoublePointArray(points);

                returnSet.Add(lineSegment);
            }

            return returnSet;
        }

        public RayVector getForceVector(List<CollisionObj> myCollisions)
        {
            if (myCollisions[0].getMyCollisionType() == CollisionType.OtherContainsArea)
            {
                doublePoint centroid = getCentroid();
            }
            if (myCollisions[0].getMyCollisionType() == CollisionType.OtherContainsArea)
            {
                doublePoint centroid = getCentroid();
            }


            return null;
        }
    }
}
