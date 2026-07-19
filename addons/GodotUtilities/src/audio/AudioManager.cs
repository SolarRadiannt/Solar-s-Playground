using System.Collections.Generic;
using GodotUtilities.Pooling;
using Godot;

namespace GodotUtilities.AudioManagement;

[Icon("uid://f1w6odifvj0o")]
public partial class AudioManager : Node
{
    private const string SFX_BUS = "SFX";
    private const string MUSIC_BUS = "Music";
    private const string MASTER_BUS = "Master";
    private const string AMBIENCE_BUS = "Ambience";

    public static AudioManager Instance { get; private set; }

    private readonly float SfxPoolTrimCooldown = 
        ProjectSettings.GetSetting("godot_utilities/audio/sfx_pool_trim_cooldown").AsSingle();

    private readonly Dictionary<StringName, AudioStream> sfxCache = new();
    private readonly Dictionary<StringName, AudioStream> musicCache = new();
    private readonly Dictionary<StringName, AudioStream> ambienceCache = new();

    private CrossfadeChannel musicChannel;
    private CrossfadeChannel ambienceChannel;

    private ObjectPool<PooledAudioPlayer> sfxPool;
    private ObjectPool<PooledAudioPlayer2D> sfxPool2D;

    public float SfxVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;
    public float MasterVolume { get; private set; } = 1f;
    public float AmbienceVolume { get; private set; } = 1f;

    private Timer timer;

    public override void _EnterTree()
    {
        Instance = this;

        musicChannel = new CrossfadeChannel(this, MUSIC_BUS);
        ambienceChannel = new CrossfadeChannel(this, AMBIENCE_BUS);
    }

    public override void _Ready()
    {
        CreatePool();
        CreatePoolTrimTimer();
    }

    #region Initialization

    private void CreatePool()
    {
        sfxPool   = new(CreatePooledPlayer<PooledAudioPlayer>);
        sfxPool2D = new(CreatePooledPlayer<PooledAudioPlayer2D>);
    }

    private void CreatePoolTrimTimer()
    {
        timer = new Timer { Autostart = true, OneShot = false, WaitTime = SfxPoolTrimCooldown };
        AddChild(timer);
        timer.Timeout += OnTimerTimeout;
    }

    private T CreatePooledPlayer<T>() where T : Node, new()
    {
        var player = new T(); 
        player.Set(AudioStreamPlayer.PropertyName.Bus, SFX_BUS);
        AddChild(player);
        return player;
    }

    #endregion

    #region Pool Trim

    private void OnTimerTimeout()
    {
        sfxPool.Trim(1);
        sfxPool2D.Trim(1);
    }

    #endregion

    #region Lazy Load

    private AudioStream GetSfx(StringName id) => FileSystem.GetOrLoad(id, sfxCache, SfxPaths);
    private AudioStream GetMusic(StringName id) => FileSystem.GetOrLoad(id, musicCache, MusicPaths);
    private AudioStream GetAmbience(StringName id) => FileSystem.GetOrLoad(id, ambienceCache, AmbiencePaths);

    #endregion

    #region Unload API

    public void UnloadSfx(StringName id) => sfxCache.Remove(id);
    public void UnloadMusic(StringName id) => musicCache.Remove(id);
    public void UnloadAmbience(StringName id) => ambienceCache.Remove(id);

    public void UnloadAllSfx() => sfxCache.Clear();
    public void UnloadAllMusic() => musicCache.Clear();
    public void UnloadAllAmbience() => ambienceCache.Clear();

    public void UnloadAll()
    {
        sfxCache.Clear();
        musicCache.Clear();
        ambienceCache.Clear();
    }

    #endregion

    #region Warm Up

    public void WarmUpSfx(params StringName[] ids)
    {
        foreach (var id in ids) GetSfx(id);
    }

    #endregion
}


