using Godot;


public partial class BSAbility : Node3D
{
	public enum BSType { Weapon, Character };
	public BSType myType = BSType.Weapon;
	
	public string name = "";
	public int value = 0;
	public string id = "";

	// "name" will become AbilityType, may not be necessary after all if its just a str 
	// "value" is for e.g. AP
	// "id" is unique identifier id 

// Fast, Slow, Tough, Deadly should not be used. 
	public enum AbilityType { Rending, Blast, Reliable, Purge, Unstoppable, 
		Deadly, Devout, Fear, Tough, Shielded, 
		Fast, Impact, Transport, Scout, Strider, 
		Hero };
	[Export]
	public AbilityType abilityType = AbilityType.Blast;

	public BSAbility Copy()
	{
		BSAbility b = new BSAbility();
		b.myType = this.myType;
		b.name = this.name;
		b.value = this.value;
		b.id = this.id;
		b.abilityType = this.abilityType;
		return b;
	}

	public override void _Ready()
	{
	}

}
