namespace SolFramework.Tools;
using System;
using System.Collections.Generic;
using Godot;

public struct Modifier
{
	public readonly float Additive = 0;
	public readonly float Multiplier = 1;
	public readonly float Flat = 0;
	public Modifier() {}
}


public class StatValue
{
	private float _baseValue;
	private float _value;
	private readonly Dictionary<string, Modifier> _modifiers = new();

	public Dictionary<string, Modifier>.ValueCollection Modifiers => _modifiers.Values;
	public float Value => _value;
	public float BaseValue {
	get { return _baseValue; }
	set {
		_baseValue = value;
		UpdateCalculation();
	}}

	public StatValue(float baseValue)
	{
		_baseValue = baseValue;
		_value = baseValue;
	}

	public StatValue SetModifier(string name, Modifier modifier)
	{
		_modifiers[name] = modifier;
		UpdateCalculation();
		return this;
	}
	public bool HasModifier(string name) =>
		_modifiers.ContainsKey(name);
	

	public StatValue RemoveModifier(string name)
	{
		_modifiers.Remove(name);
		UpdateCalculation();
		return this;
	}

	private void UpdateCalculation()
	{
		float finalAdd = 0;
		float finalFlat = 0;
		float finalMult = 1;

		foreach (var (_, modifier) in _modifiers)
		{
			finalAdd += modifier.Additive;
			finalFlat += modifier.Flat;
			finalMult *= modifier.Multiplier;
		}

		float finalValue = Mathf.Max(0, _baseValue * (1 + finalAdd) * finalMult + finalFlat);
		_value = finalValue;
	}
}
