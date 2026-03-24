using Godot;
using System;
using System.Collections.Generic;

public partial class BSUnit : Node3D
{
	[Export]
	public Color _baseColor;
	[Export]
	public bool FlipX;

	public Godot.Collections.Array<Node> myUnits; // list of my children 

// I think we can avoid assigning this - these just need a complete BSModel scene to work, which we can do later.
// Line of sight shit. eyes and rays are set per unit, so... 
	RayCast3D eyeSource;
	BSUnit unitTgt;
	BSModel modelTgt;
	bool calculateLOS;

	Godot.Collections.Array<Node> tgtModels;
	Godot.Collections.Array<Node> tgtLosObj;

	int tgtModelsCounted;
	bool los_get_collisions;
	public bool isHighlighted;
	
	[Export]
	public bool processLineOfSight;
//

///
	/// Game Stat Stuff 
	/// 
	public string unitName = "UnitName";

	public bool isLeader = false;
	public bool hasLeader = false;
	public BSUnit partnerUnit = null;
	public string uid = "";
	public int cost = 0;
	public int baseSize = 32;
	
	public int power = 1; // Q
	public int def = 1; // 1 is equivalent to 6+. 	2	3	4	5	   // nothing has 6 (1+)
	// 												5+	4+	3+	2+     // so 7 - army val.
	public int unitCt = 10;		// how many models in this unit?
	public int heartsPerModel = 1; // 
	
	public int move = 6; // lowest probably 6, maybe 4, max 20ish
	
	public List<BSLoadout> loadouts = new List<BSLoadout>();
	public List<BSAbility> abilities = new List<BSAbility>();

	public bool isSelectable = false;
///
/// 
	public BSUnit InstantiateMe()
	{
		BSUnit b = new BSUnit();
		b._baseColor = this._baseColor;
		b.FlipX = this.FlipX;
		//b.myUnits = // we dont assign this now; we instantiate models as we need them later
		b.unitName = this.unitName;
		//b.isLeader = this.isLeader;
		//b.hasLeader = this.hasLeader;
		b.uid = this.uid;
		b.cost = this.cost;
		b.baseSize = this.baseSize;
		b.power = this.power;
		b.def = this.def;
		b.unitCt = this.unitCt;
		b.heartsPerModel = this.heartsPerModel;
		b.move = this.move;
		// TODO DUPLICATE LOADOUTS? 
		// TODO DUPLICATE ABILITIES? 
		// think about this when you are awake 
		
		return b;
	}
	// Fixme
	public List<string> items = new List<string>();
	public List<string> rules = new List<string>();
	//public List<string> weapons = new List<string>();
	public List<string> upgrades = new List<string>();
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
