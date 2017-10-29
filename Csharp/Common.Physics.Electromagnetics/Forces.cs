using System.Numerics;

namespace Common.Physics.Electromagnetics
{
    /// <summary>
    /// Performs E&M force calculations.
    /// </summary>
    public class Forces
    {
        /// <summary>
        /// Computes the Lorentz force, F = qE + q(v x B), of a particle
        /// </summary>
        public static Vector3 LorentzForce(Particle particle, Vector3 electricField, Vector3 magneticField)
        {
            Vector3 electricForce = particle.Charge * electricField;
            Vector3 magneticForce = particle.Charge * Vector3.Cross(particle.Velocity, magneticField);
            return electricForce + magneticForce;
        }
    }
}
