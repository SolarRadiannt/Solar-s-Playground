using Godot;
using GodotUtilities.Pooling;
using System.Collections.Generic;

namespace GodotUtilities.ParticlesManagement;

[Icon("uid://cnexoerk05mmp")]
public partial class ParticlesManager : Node
{
    private const float TRIM_DURATION = 5f;
    private const int KEEP_COUNT = 2;

    public static ParticlesManager Instance { get; private set; }

    public Node ParticlesParent
    {
        get
        {
            particlesParent ??= GetTree().CurrentScene;
            return particlesParent;
        }
        set => particlesParent = value;
    }

    private readonly Dictionary<StringName, NodePool<PooledParticle>> pools = new();

    private Cooldown trimTimer = new(TRIM_DURATION);
    private Node particlesParent;

    public override void _EnterTree() => Instance = this;

    public override void _Ready()
    {
        trimTimer.Start();
    }

    public override void _Process(double dt)
    {
        trimTimer.Tick(dt);

        if (trimTimer.IsReady)
        {
            TrimUnused();
            trimTimer.Start();
        }
    }

    private bool TryGetOrBuildPool(StringName id, out NodePool<PooledParticle> result)
    {
        if (pools.TryGetValue(id, out result))
            return true;
        
        if (ParticlePaths.TryGetValue(id, out string path))
        {
            var particlesScene = ResourceLoader.Load<PackedScene>(path);
            NodePool<PooledParticle> pool = null; 

            pool = new(
                particlesScene, ParticlesParent,
                onInstantiate: p => p.Pool = pool
            );
            
            pools[id] = pool;
            result = pool;
            return true;
        }

        GD.PushError(nameof(ParticlesManager), $": Particle Id '{id}' doesn't have a path. make sure to regenerate files");
        return false;
    }

    private void TrimUnused()
    {
        foreach (var (_, pool) in pools)
        {
            if (pool.FreeCount > KEEP_COUNT)
                pool.Trim(KEEP_COUNT);
        }
    }

    public ParticlesHandle Emit(StringName id, Vector2 position)
    {
        if (!TryGetOrBuildPool(id, out var pool)) 
            return default;

        var particles = pool.Get();
        particles.Emit(position);
        return new(particles, particles.SpawnId);
    }

    public void WarmUp(params StringName[] ids) => WarmUpv(ids);
    public void Unload(params StringName[] ids) => Unloadv(ids);

    public void WarmUpv(StringName[] ids)
    {
        foreach (var id in ids)
            Emit(id, Vector2.Zero);
    }

    public void Unloadv(StringName[] ids)
    {
        foreach (var id in ids)
        {
            if (!pools.TryGetValue(id, out var pool)) continue;
            pool.Destroy();
            pools.Remove(id);
        }
    }

    public void Clear()
    {
        foreach (var (_, pool) in pools)
            pool.Destroy();
        pools.Clear();
    }

}

