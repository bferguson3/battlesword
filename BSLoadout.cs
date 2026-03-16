using Godot;
using System;
using System.Collections.Generic;

public partial class BSLoadout : Node
{
	public string name;
	public int range;
	public int attacks;
	public int ap;
	public List<BSAbility> abilities = new List<BSAbility>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
