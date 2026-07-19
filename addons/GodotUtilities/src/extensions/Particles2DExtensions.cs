using Godot;

namespace GodotUtilities;

public static class Particles2DExtensions
{
    public static void SetDirection(this GpuParticles2D particles, Vector2 direction)
    {
        if (particles.ProcessMaterial is ParticleProcessMaterial material)
            material.Direction = new Vector3(direction.X, direction.Y, 0f);
    }

    public static void SetGravity(this GpuParticles2D particles, Vector2 value)
    {
        if (particles.ProcessMaterial is ParticleProcessMaterial material)
            material.Gravity = new Vector3(value.X, value.Y, 0f);
    }

    public static void EmitOneShot(this GpuParticles2D particles)
    {
        if (!particles.OneShot)  
            Log.Warn($"Particles: '{particles.Name}' isn't assigned to be one shot");

        particles.Emitting = true;
        particles.Restart();
    }
}

