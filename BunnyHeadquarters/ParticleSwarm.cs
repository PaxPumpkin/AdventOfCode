using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class ParticleSwarm : AoCodeModule
    {
        public ParticleSwarm()
        {
            inputFileName = "particleswarm.txt";
            base.GetInput();
        }
        public override void DoProcess()
        {
            List<Particle> particles = new List<Particle>();
            
            inputFile.ForEach(line => particles.Add(new Particle(line)));
            // "rough" because if two particles have identical acceleration values, this only finds the first one. Ostensibly, we'd have to check again for "well, then which one started closer and was already going slowest, etc )
            // meh, the first guess was right anyway. It's 364
            FinalOutput.Add("Big Rough Prediction: " + particles.IndexOf(particles.Find(part => part.acceleration.MaxVector == particles.Min(x => x.acceleration.MaxVector))).ToString());
            int iterationsUntilHomeostasis = 0;
            // I'll do 500 for the first time through. It'll be good for a first guess, anyway. 
            // added the particles.Count>420 because I now know that is the final point. I was just wondering how long until that happens. It's 40 loops, so really short. 
            //for (iterationsUntilHomeostasis = 0; iterationsUntilHomeostasis < 500 && particles.Count > 420; iterationsUntilHomeostasis++)
            for (iterationsUntilHomeostasis = 0; iterationsUntilHomeostasis < 500 ; iterationsUntilHomeostasis++)
            {
                List<Particle> collided; // collided for each particle
                List<Particle> allCollided = new List<Particle>(); // all particles that are in a collision state "this move"
                // iterate all particles, find all that have my coordinates but are NOT ME. If I find any, add them (and myself) to the list of all particles that collided this move. 
                particles.ForEach(part1 => { collided = particles.FindAll(test => (test.position.Equals(part1.position) && !test.Equals(part1))); if (collided.Count > 0) { allCollided.AddRange(collided); allCollided.Add(part1); } });
                // remove all particles we found that collided this time around. 
                allCollided.ForEach(boom => particles.Remove(boom)); // all collided will be factors of sets of collisions since... if a collides with b, a and b are added to the list. Then b collided with a, so b and a are added again... List is bigger than it needs to be.
                // ok, next move!
                particles.ForEach(part1 => part1.Move());
            }
            FinalOutput.Add("After " + iterationsUntilHomeostasis.ToString() + " iterations, there are: " + particles.Count.ToString() + " particles left.");
        }
    }
    class Particle
    {
        public Coordinate position;
        public Velocity velocity;
        public Acceleration acceleration;
        public Particle(string defintion)
        {
            string[] defs = defintion.Split(new string[] { ", " }, StringSplitOptions.None);
            string pos = defs[0].Substring(2);
            string vel = defs[1].Substring(2);
            string acc = defs[2].Substring(2);
            position = new Coordinate(pos);
            velocity = new Velocity(vel);
            acceleration = new Acceleration(acc);

        }
        public void Move()
        {
            velocity.Accelerate(acceleration);
            position.Move(velocity);
        }
        public override bool Equals(object obj)
        {
            Particle px = (Particle)obj;
            return (px.position.Equals(this.position) && px.velocity.Equals(this.velocity) && px.acceleration.Equals(this.acceleration));
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    class Coordinate
    {
        public int x;
        public int y;
        public int z;
        public int FromZero { get { return (Math.Abs(x) + Math.Abs(y) + Math.Abs(z)); } }
        public Coordinate(string definition)
        {
            string[] defs = definition.Substring(1, definition.Length - 2).Split(new char[] { ',' });
            x = Int32.Parse(defs[0]);
            y = Int32.Parse(defs[1]);
            z = Int32.Parse(defs[2]);
        }
        public void Move(Velocity vel)
        {
            x += vel.x;
            y += vel.y;
            z += vel.z;
        }
        public override bool Equals(object obj)
        {
            Coordinate objx = (Coordinate)obj;
            return (objx.x == this.x && objx.y == this.y && objx.z == this.z);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
    struct Acceleration
    {
        public int x;
        public int y;
        public int z;
        public int MaxVector { get { return (Math.Abs(x) + Math.Abs(y) + Math.Abs(z)); } }
        public Acceleration(string definition)
        {
            string[] defs = definition.Substring(1, definition.Length - 2).Split(new char[] { ',' });
            x = Int32.Parse(defs[0]);
            y = Int32.Parse(defs[1]);
            z = Int32.Parse(defs[2]);
        }
        public override bool Equals(object obj)
        {
            Acceleration a = (Acceleration)obj;
            return (a.x == this.x && a.y == this.y && a.z == this.z);
        }
        public override int GetHashCode() // this seems silly to do for a struct...
        {
            return base.GetHashCode();
        }
    }
    class Velocity
    {
        public int x;
        public int y;
        public int z;
        public int MaxVector { get { return (Math.Abs(x) + Math.Abs(y) + Math.Abs(z)); } }
        public Velocity(string definition)
        {
            string[] defs = definition.Substring(1, definition.Length - 2).Split(new char[] { ',' });
            x = Int32.Parse(defs[0]);
            y = Int32.Parse(defs[1]);
            z = Int32.Parse(defs[2]);
        }
        public void Accelerate(Acceleration acc)
        {
            x += acc.x;
            y += acc.y;
            z += acc.z;
        }
        public override bool Equals(object obj)
        {
            Velocity a = (Velocity)obj;
            return (a.x==this.x && a.y==this.y && a.z==this.z);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
