using System.Numerics;

namespace Common.Physics
{
    public class Particle
    {
        public Particle(float mass, float charge)
        {
            this.Mass = mass;
            this.Charge = charge;

            this.Position = new Vector3(0, 0, 0);
            this.Velocity = new Vector3(0, 0, 0);
        }

        public Particle(float mass, float charge, Vector3 position)
        {
            this.Mass = mass;
            this.Charge = charge;

            this.Position = position;
            this.Velocity = new Vector3(0, 0, 0);
        }

        public Particle(float mass, float charge, Vector3 position, Vector3 velocity)
        {
            this.Mass = mass;
            this.Charge = charge;

            this.Position = position;
            this.Velocity = velocity;
        }

        public float Mass { get; set; }

        public float Charge { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Velocity { get; set; }
    }
}
