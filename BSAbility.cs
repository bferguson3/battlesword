using Godot;
using System;

public partial class BSAbility : Node
{
	public enum BSType { Weapon, Character };

	public BSType myType;
	public string name;

	public enum AbilityType { Rending, Blast, Reliable, Purge, Unstoppable, 
		Deadly, Devout, Fear, Tough, Shielded, 
		Fast, Impact, Transport, Scout, Strider, 
		Hero };

	[Export]
	public AbilityType abilityType;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
