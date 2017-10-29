namespace Common.Cnc
{
    /// <summary>
    /// Defines a relative-motion linear mill command.
    /// </summary>
    public class LinearMillCommand
    {
        /// <summary>
        /// Creates a new command with the specified ID, delta positioning, and speed.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xDelta"></param>
        /// <param name="yDelta"></param>
        /// <param name="zDelta"></param>
        /// <param name="aDelta"></param>
        /// <param name="speed"></param>
        public LinearMillCommand(int id, int xDelta, int yDelta, int zDelta, int aDelta, int speed)
        {
            this.Id = id;
            this.XSteps = xDelta;
            this.YSteps = yDelta;
            this.ZSteps = zDelta;
            this.ASteps = aDelta;
            this.Speed = speed;
        }

        public int Id { get; }

        public int XSteps { get; }

        public int YSteps { get; }

        public int ZSteps { get; }

        public int ASteps { get; }

        public int Speed { get; }
    }
}
