using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Collision.ThePhysicsEngine.Common;

namespace Collision.ThePhysicsEngine.CollisionShapes
{
    public abstract class CollsionBaseShape
    {
        public abstract void drawSelf(PaintEventArgs e);

        public abstract doublePoint getCentroid();

        public abstract doublePoint getFirstPoint();

        public abstract double getArea();

        public abstract void checkCollision(CollsionBaseShape otherPoly, PaintEventArgs e);

        public abstract void addInPoint(doublePoint aNewPoint, PaintEventArgs e);

        public abstract List<DoublePointArray> getAllLineSegments();

        public abstract DoublePointArray getPointSet();
    }
}
