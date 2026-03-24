using Godot;

using System.Collections.Generic;

public partial class BSLoadout : Node3D
{
	public string name;
	public int range;
	public int attacks;
	public int ap;
	public List<BSAbility> abilities = new List<BSAbility>();

	public BSLoadout Copy()
	{
		BSLoadout l = new BSLoadout();

		l.name = this.name;
		l.range = this.range;
		l.attacks = this.attacks;
		l.ap = this.ap;

		l.abilities = this.abilities; // REFERENCE ONLY! FIXME?

		return l;
	}

	public override void _Ready()
	{
	}

}
