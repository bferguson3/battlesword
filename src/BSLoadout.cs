using Godot;

using System.Collections.Generic;

public partial class BSLoadout : Node3D
{
	public string name = "CCW";
	public int range = 0;
	public int attacks = 1;
	public int ap = 0;
	public int dmg = 1;
	public List<BSAbility> abilities = new List<BSAbility>();

	public BSLoadout Copy()
	{
		BSLoadout l = new BSLoadout();

		l.name = this.name;
		l.range = this.range;
		l.attacks = this.attacks;
		l.ap = this.ap;
		l.dmg = this.dmg;

		l.abilities = this.abilities; // REFERENCE ONLY! FIXME?

		return l;
	}

	public override void _Ready()
	{
	}

}
