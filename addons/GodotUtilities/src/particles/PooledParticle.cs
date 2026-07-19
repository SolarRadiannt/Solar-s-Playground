using Godot;
using GodotUtilities.Pooling;

namespace GodotUtilities.ParticlesManagement;

public partial class PooledParticle : GpuParticles2D
{
    public NodePool<PooledParticle> Pool { get; set; }

    public uint SpawnId { get; private set; }

    public override void _Ready()
    {
        Connect(GpuParticles2D.SignalName.Finished, Callable.From(OnFinished));
    }

    private void OnFinished()
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (Pool != null) Pool.Release(this);
        else QueueFree();
    }

    public void Emit(Vector2 position)
    {
        SpawnId++;
        GlobalPosition = position;
        
        // Restart() re-enables emission; setting Emitting first ensures the signal fires on completion
        Emitting = true;
        Restart();
    }

    public void Stop()
    {
        Emitting = false;
        ReturnToPool();
    }
}
