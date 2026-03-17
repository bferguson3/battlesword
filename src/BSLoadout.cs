using Godot;

using System.Collections.Generic;

public partial class BSLoadout : Node3D
{
	public string name;
	public int range;
	public int attacks;
	public int ap;
	public List<BSAbility> abilities = new List<BSAbility>();
	public override void _Ready()
	{
	}

}
