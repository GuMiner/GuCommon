namespace Common.Mathematics
{
    // TODO rewrite appropriately using an interface instead of a function, and no weird double-vector type
    public class RK4Integration
    {
        // /// <summary>
        // /// Performs an RK4 integration step, returning the derivative vectors.
        // /// </summary>
        // private DoubleVector Evaluate(ParticleState particleState, DoubleVector currentState, DoubleVector derivative,
        //     float timeStep, Func<ParticleState, DoubleVector, Vector3> accelerationAction)
        // {
        //     DoubleVector updatedState = new DoubleVector(
        //         currentState.Vector + derivative.Vector * timeStep,
        //         currentState.IntegratedVector + derivative.IntegratedVector * timeStep);
        //     return new DoubleVector(currentState.IntegratedVector, accelerationAction(particleState, currentState));
        // }
        // 
        // /// <summary>
        // /// Performs the RK4 integration, returning the updated particle position info.
        // /// </summary>
        // public DoubleVector Integrate(Func<ParticleState, DoubleVector, Vector3> integrationAcceleration,
        //     ParticleState particleState, DoubleVector currentState, float timeStep)
        // {
        //     DoubleVector a = Evaluate(particleState, currentState, new DoubleVector(), 0, integrationAcceleration);
        //     DoubleVector b = Evaluate(particleState, currentState, a, timeStep * 0.5f, integrationAcceleration);
        //     DoubleVector c = Evaluate(particleState, currentState, b, timeStep * 0.5f, integrationAcceleration);
        //     DoubleVector d = Evaluate(particleState, currentState, c, timeStep, integrationAcceleration);
        // 
        //     Vector3 dposdt = (1.0f / 6.0f) * (a.Vector + 2.0f * (b.Vector + c.Vector) + d.Vector);
        //     Vector3 dveldt = (1.0f / 6.0f) * (a.IntegratedVector + 2.0f * (b.IntegratedVector + c.IntegratedVector) + d.IntegratedVector);
        // 
        //     return new DoubleVector(
        //         currentState.Vector + dposdt * timeStep,
        //         currentState.IntegratedVector + dveldt * timeStep);
        // }
        
        // MillUI
    }
}
