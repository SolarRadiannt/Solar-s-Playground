// AUTO-GENERATED — do not edit manually
using Godot;

namespace GodotUtilities.AudioManagement;

public partial class AudioManager
{
	/// <summary>
	/// Auto-generated StringName constants. Use these instead of raw strings to avoid typos and benefit from IDE autocomplete.
	/// </summary>
	public static class SfxName
	{
		/// <summary>
		/// Refers to <c>Step1</c>.
		/// </summary>
		public static readonly StringName Step1 = "Step1";
		/// <summary>
		/// Refers to <c>Step2</c>.
		/// </summary>
		public static readonly StringName Step2 = "Step2";
		/// <summary>
		/// Refers to <c>Step3</c>.
		/// </summary>
		public static readonly StringName Step3 = "Step3";
		/// <summary>
		/// Refers to <c>Step4</c>.
		/// </summary>
		public static readonly StringName Step4 = "Step4";
		/// <summary>
		/// Refers to <c>Step5</c>.
		/// </summary>
		public static readonly StringName Step5 = "Step5";
		/// <summary>
		/// Refers to <c>Step6</c>.
		/// </summary>
		public static readonly StringName Step6 = "Step6";
		/// <summary>
		/// Refers to <c>Step7</c>.
		/// </summary>
		public static readonly StringName Step7 = "Step7";
		/// <summary>
		/// Refers to <c>Step8</c>.
		/// </summary>
		public static readonly StringName Step8 = "Step8";
	}

	/// <summary>
	/// Comma-separated list of all names. Intended for use with <c>[Export(PropertyHint.Enum, SFX_NAMES)]</c>.
	/// </summary>
	public const string SFX_NAMES = "Step1,Step2,Step3,Step4,Step5,Step6,Step7,Step8";

	/// <summary>
	/// Maps each SfxName constant to its resource path. Used internally to load scenes on demand.
	/// </summary>
	public static readonly Godot.Collections.Dictionary<StringName, string> SfxPaths = new()
	{
		[SfxName.Step1] = "res://Audios/SFXs/Step1.ogg",
		[SfxName.Step2] = "res://Audios/SFXs/Step2.ogg",
		[SfxName.Step3] = "res://Audios/SFXs/Step3.ogg",
		[SfxName.Step4] = "res://Audios/SFXs/Step4.ogg",
		[SfxName.Step5] = "res://Audios/SFXs/Step5.ogg",
		[SfxName.Step6] = "res://Audios/SFXs/Step6.ogg",
		[SfxName.Step7] = "res://Audios/SFXs/Step7.ogg",
		[SfxName.Step8] = "res://Audios/SFXs/Step8.ogg",
	};

	/// <summary>
	/// Auto-generated StringName constants. Use these instead of raw strings to avoid typos and benefit from IDE autocomplete.
	/// </summary>
	public static class MusicName
	{
	}

	/// <summary>
	/// Comma-separated list of all names. Intended for use with <c>[Export(PropertyHint.Enum, MUSIC_NAMES)]</c>.
	/// </summary>
	public const string MUSIC_NAMES = "";

	/// <summary>
	/// Maps each MusicName constant to its resource path. Used internally to load scenes on demand.
	/// </summary>
	public static readonly Godot.Collections.Dictionary<StringName, string> MusicPaths = new()
	{
	};

	/// <summary>
	/// Auto-generated StringName constants. Use these instead of raw strings to avoid typos and benefit from IDE autocomplete.
	/// </summary>
	public static class AmbienceName
	{
	}

	/// <summary>
	/// Comma-separated list of all names. Intended for use with <c>[Export(PropertyHint.Enum, AMBIENCE_NAMES)]</c>.
	/// </summary>
	public const string AMBIENCE_NAMES = "";

	/// <summary>
	/// Maps each AmbienceName constant to its resource path. Used internally to load scenes on demand.
	/// </summary>
	public static readonly Godot.Collections.Dictionary<StringName, string> AmbiencePaths = new()
	{
	};

}