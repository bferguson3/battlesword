using Godot;


public partial class BSAbility : Node3D
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

	public override void _Ready()
	{
	}

}
