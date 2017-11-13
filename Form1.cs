using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Collision.ThePhysicsEngine;
using System.Diagnostics;

namespace Collision
{
    public partial class Form1 : Form
    {

        System.Threading.Thread motionThread;

        static bool isRunningMotion = false;

        static PhysicsEngine myPhysicsEngine;

        public Form1()
        {
            InitializeComponent();
            myPhysicsEngine = new PhysicsEngine();


        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            myPhysicsEngine.drawCollisionShapes(e);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            myPhysicsEngine.checkCollisions(e);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!isRunningMotion)
            {
                motionThread = new Thread(() => updatePos());
                motionThread.Start();
            }
            pictureBox1.Refresh();
        }

        private void updatePos()
        {
            isRunningMotion = true;
            //myPhysicsEngine.tick();
            isRunningMotion = false;
        }
    }
}
