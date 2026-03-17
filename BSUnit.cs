using Godot;
using System.Collections.Generic;

public partial class BSUnit : Node3D
{
	[Export]
	public Color _baseColor;
	[Export]
	public bool FlipX;

	RayCast3D eyeSource;
	BSUnit unitTgt;
	BSModel modelTgt;
	bool calculateLOS;

	Godot.Collections.Array<Node> tgtModels;
	Godot.Collections.Array<Node> tgtLosObj;

	public Godot.Collections.Array<Node> myUnits;

	int tgtModelsCounted;
	bool los_get_collisions;
	
	[Export]
	public bool processLineOfSight;

	///
	/// Game Stat Stuff 
	/// 
	public string unitName = "UnitName";

	public bool isLeader = false;
	public bool hasLeader = false;
	public BSUnit partnerUnit = null;
	
	public int power = 1; // Q
	public int shields = 1; // 1 is equivalent to 6+. 	2	3	4	5	// nothing has 6.
	// 													5+	4+	3+	2+
	public int cur_hearts = 10;
	public int max_hearts = 10; // this is toughness or # of models in this unit. 
	public int move = 6; // lowest probably 6, maybe 4, max 20ish
	
	public List<BSLoadout> loadouts = new List<BSLoadout>();
	public List<BSAbility> abilities = new List<BSAbility>();

	/// 

	public override void _Ready()
	{
		/*
		//abilities.Add(new BSAbility());
		//abilities[0].abilityType = BSAbility.AbilityType.Blast;
		//GD.Print(abilities[0].abilityType);
		loadouts.Add(new BSLoadout());
		loadouts[0].attacks = 3;
		loadouts[0].abilities.Add(new BSAbility());
		loadouts[0].abilities[0].abilityType = BSAbility.AbilityType.Blast;
		*/
		// TODO: When checking LOS on target sprite,
		//. iterate through all 8 of its LOS spots 
		//. 
		processLineOfSight = true;
		myUnits = FindChildren("*", "Sprite3D");
		foreach (BSModel s in myUnits)
		{
			//s.Modulate = _baseColor;
			s.SetColor(_baseColor);
		}
		foreach(Sprite3D s in myUnits)
		{
			s.FlipH = FlipX;
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		if(los_get_collisions)
		{
			foreach(BSModel b in tgtModels)
			{
				tgtLosObj = b.myLines; //b.GetNode<Node>("battlenun-body/battlenun-collider/LOSNodes").GetChildren();
				int los_ct = 8;
				foreach (RayCast3D r in tgtLosObj)
					if (!r.IsColliding())
						los_ct--;
							
				if(los_ct == 8)
				{	GD.Print(b.Name, " is hidden");
				} else if(los_ct == 0)
				{	GD.Print(b.Name, " is fully visible");
				} else
				{	GD.Print(b.Name, " is in cover (", los_ct, "/8)");
				}
			}
			los_get_collisions = false;
		}
		if(calculateLOS)
		{
			foreach(BSModel b in tgtModels)
			{
				tgtLosObj = b.myLines;//b.GetNode<Node>("battlenun-body/battlenun-collider/LOSNodes").GetChildren();
				foreach (RayCast3D r in tgtLosObj)
				{
					r.TargetPosition = eyeSource.GlobalPosition - r.GlobalPosition;
				}	
			}
			calculateLOS = false;
			los_get_collisions = true;
		}
    }

	private void GetLOS(BSModel src_m, BSUnit target_unit)
	{
		eyeSource = src_m.myEyes;//GetNode<Node>("battlenun-body").GetNode<RayCast3D>("eyeSource");
		unitTgt = target_unit;
		tgtModels = unitTgt.myUnits; //unitTgt.FindChildren("*", "Sprite3D");
		// make all models look at layer 5, but dont move them there yet 
		//foreach(BSModel b in tgtModels)
		//{
		//	b.SetCollisionMask(5, true);
		//}
		tgtLosObj = null;
		calculateLOS = true;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(processLineOfSight && Name == "bnun unit")
		{ // should only need 1 frame trigger 
			processLineOfSight = false;
			GetLOS(GetNode<BSModel>("battlenun5"), GetNode<BSUnit>("/root/Node3D/bnun unit2"));
		}
		
	}
}
