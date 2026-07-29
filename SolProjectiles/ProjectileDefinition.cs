using Godot;

[GlobalClass]
public partial class ProjectileDefinition : Resource
{
    [Export] public PackedScene Scene { get; set; }
    [Export] public float Speed { get; set; }
    [Export] public int Damage { get; set; }
    [Export] public int MaxDistance { get; set; }
}