using Godot;


public partial class BSAbility : Node3D
{
	public enum BSType { Weapon, Character };

	public BSType myType = BSType.Weapon;
	public string name = "";
	public int value = 0;
	public string id = "";

	public enum AbilityType { Rending, Blast, Reliable, Purge, Unstoppable, 
		Deadly, Devout, Fear, Tough, Shielded, 
		Fast, Impact, Transport, Scout, Strider, 
		Hero };

	[Export]
	public AbilityType abilityType = AbilityType.Blast;

	public override void _Ready()
	{
	}

}
