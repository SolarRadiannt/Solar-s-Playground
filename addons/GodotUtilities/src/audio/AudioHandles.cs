using Godot;

namespace GodotUtilities.AudioManagement;

public struct SfxHandle(PooledAudioPlayer player, uint id)
{
    public readonly bool IsAlive => player.Playing && player.SpawnId == id;

    public readonly void SetPitch(float scale) { if (player.Playing) player.PitchScale    = scale;  }
    public readonly void SetVolume(float linear) { if (player.Playing) player.VolumeLinear = linear; }
}

public struct Sfx2DHandle(PooledAudioPlayer2D player, uint id)
{
    public readonly bool IsAlive => player.Playing && player.SpawnId == id;

    public readonly void SetPitch(float scale) { if (player.Playing) player.PitchScale = scale; }
    public readonly void SetVolume(float linear) { if (player.Playing) player.VolumeLinear = linear; }
    public readonly void SetPosition(Vector2 pos) { if (player.Playing) player.GlobalPosition = pos; }
}



