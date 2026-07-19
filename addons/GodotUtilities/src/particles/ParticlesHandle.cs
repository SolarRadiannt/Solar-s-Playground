using Godot;

namespace GodotUtilities.ParticlesManagement;

public struct ParticlesHandle(PooledParticle particles, uint id)
{
    public readonly bool IsAlive => 
        GodotObject.IsInstanceValid(particles) &&
        particles.Emitting && 
        particles.SpawnId == id;

    public readonly bool IsInGroup(StringName group) =>
        IsAlive && particles.IsInGroup(group);
     
    public readonly void Stop()
    { 
        if (IsAlive) particles.Stop(); 
    }

    public readonly void SetPosition(Vector2 position)
    {
        if (IsAlive) particles.GlobalPosition = position; 
    }

    public readonly void SetDirection(Vector2 direction)
    {
        if (IsAlive) particles.SetDirection(direction);
    }
    
    public readonly void SetChildIndex(int index)
    {
        if (IsAlive) particles.GetParent()?.MoveChild(particles, index);
    }

    public readonly SignalAwaiter WaitToFinish()
    {
        if (!IsAlive)
            GD.PushError($"WaitToFinish called on a dead handle — awaiter will never resolve");

        return particles.ToSignal(particles, GpuParticles2D.SignalName.Finished);
    }

}

