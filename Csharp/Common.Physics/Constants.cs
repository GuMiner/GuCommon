using System;

namespace Common.Physics
{
    /// <summary>
    /// Holds physical constants common across all of physics
    /// </summary>
    public class Constants
    {
        public static readonly double LightSpeed = 2.99792458e8; // m/s
        public static readonly double Gravity = 6.67408e-11;     // Nm^2/kg^2
        public static readonly double Coulomb = 8.98755e9;       // Nm^2/C^2
        public static readonly double Magnetic = 4 * Math.PI * 1e-7;  // Tm/A

        public static readonly double ElectronCharge = 1.6021766208e-19; // C
        public static readonly double ElectronMass = 9.10938356e-31; // kg
    }
}
