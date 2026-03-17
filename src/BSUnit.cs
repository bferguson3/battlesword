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
	public bool isHighlighted;
	
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
	public int def = 1; // 1 is equivalent to 6+. 	2	3	4	5	// nothing has 6.
	// 													5+	4+	3+	2+
	public int unitCt = 10;		// how many models in this unit?
	public int heartsPerModel = 1; // 
	public int[] unitHearts; // how many hearts per unit? (if tough keyword, else 1)
	
	public int move = 6; // lowest probably 6, maybe 4, max 20ish
	
	public List<BSLoadout> loadouts = new List<BSLoadout>();
	public List<BSAbility> abilities = new List<BSAbility>();

	public bool isSelectable = false;
	/// 

	public override void _Ready()
	{
		// init health array 
		unitHearts = new int[unitCt];
		for(int a = 0; a < unitCt; a++) unitHearts[a] = heartsPerModel;

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

	public void Flash(Color c)
	{
		foreach(BSModel s in myUnits)
			s.Flash(c);
		
	}
	public void FlashOff()
	{
		foreach(BSModel s in myUnits)
			s.FlashOff();
	}

    public override void _PhysicsProcess(double delta)
    {
		if(los_get_collisions)
		{
			foreach(BSModel b in tgtModels)
			{
				tgtLosObj = b.myLines; //b.GetNode<Node>("battlenun-body/battlenun-collider/LOSNodes").GetChildren();
				int los_ct = 8;
				foreach (RayCast3D r in tgtLosObj){
					if (!r.IsColliding())
						los_ct--;
					r.TargetPosition = Vector3.Zero;
				}
							
				if(los_ct == 8)
				{	GD.Print(b.Name, " is hidden");
					b.inCoverDisplay = true;
				} else if(los_ct == 0)
				{	GD.Print(b.Name, " is fully visible");
				} else
				{	GD.Print(b.Name, " is in cover (", los_ct, "/8)");
					b.inCoverDisplay = true;
				}
			}
			los_get_collisions = false;
		}
		if(calculateLOS) // idiotic "kill one cycle" process
		{				// because we have to wait for TargetPosition to update
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
		foreach(BSModel b in tgtModels)
		{
			tgtLosObj = b.myLines;//b.GetNode<Node>("battlenun-body/battlenun-collider/LOSNodes").GetChildren();
			foreach (RayCast3D r in tgtLosObj)
			{
				r.TargetPosition = eyeSource.GlobalPosition - r.GlobalPosition;
			}	
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(processLineOfSight)
		{ // should only need 1 frame trigger 
			processLineOfSight = false;
			// make sure we loop this PROPERLY! might have to wait!
			GetLOS(GetNode<BSModel>("battlenun5"), GetNode<BSUnit>("/root/Node3D/bnun unit2"));
		}
		
	}
}
