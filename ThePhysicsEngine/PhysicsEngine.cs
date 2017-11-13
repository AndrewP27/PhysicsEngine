using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Collision.ThePhysicsEngine.CollisionShapes;
using Collision.ThePhysicsEngine.Common;

namespace Collision.ThePhysicsEngine
{
    class PhysicsEngine
    {
        List<CollsionBaseShape> myCollsionShapes = new List<CollsionBaseShape>();
        public PhysicsEngine()
        {
            doublePoint[] insertPoints = { new doublePoint(30, 55), new doublePoint(95, 45), new doublePoint(105, 110), new doublePoint(20, 110), new doublePoint(65, 65) };
            DoublePointArray pointArray = new DoublePointArray(insertPoints);
            myCollsionShapes.Add(new CollisionPolygon(pointArray, 1));

            doublePoint[] insertPoints2 = { new doublePoint(45, 30), new doublePoint(85, 60), new doublePoint(95, 120), new doublePoint(45, 100), new doublePoint(80, 65) };
            DoublePointArray pointArray2 = new DoublePointArray(insertPoints2);
            myCollsionShapes.Add(new CollisionPolygon(pointArray2, 2));
        }

        public void drawCollisionShapes(PaintEventArgs e)
        {
            foreach (CollsionBaseShape i in myCollsionShapes)
            {
                i.drawSelf(e); 
            }
        }

        public void checkCollisions(PaintEventArgs e)
        {
            for (int i = 0; i < myCollsionShapes.Count; i++)
            {
                for (int j = 0; j < myCollsionShapes.Count; j++)
                {
                    if(i != j)
                        myCollsionShapes[i].checkCollision(myCollsionShapes[j], e);
                }
            }
        }
    }
}
