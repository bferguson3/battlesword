using Godot;
using System;
using System.Collections.Generic;


public partial class BSUnit : Node3D
{
	[Export]
	public Color _baseColor;
	[Export]
	public bool FlipX;

	BSModel unit1;
	RayCast3D eyeCaster;
	Node3D unitTgt;
	BSModel modelTgt;
	bool calculateLOS;

	Godot.Collections.Array<Node> tgtModels;
	Godot.Collections.Array<Node> tgtLosObj;

	Godot.Collections.Array<Node> myUnits;

	int tgtModelsCounted;
	bool los_step_two;
	bool LOScomplete;
	//int positionsCounted;
	[Export]
	public bool reset_los_flag;

	/// Game Stat Stuff 
	/// 
	public string unitName;

	public bool isLeader;
	public bool hasLeader;
	public BSUnit partnerUnit;
	
	public int power;
	public int shields;
	public int cur_hearts;
	public int max_hearts; // this is toughness or # of models in this unit. 
	
	public List<BSLoadout> loadouts = new List<BSLoadout>();
	public List<BSAbility> abilities = new List<BSAbility>();

	/// 

	// Called when the node enters the scene tree for the first time.
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
		los_step_two = false;
		
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
		if (los_step_two)
		{
			foreach(var model in tgtModels){
				modelTgt = (BSModel)model;
				tgtLosObj = modelTgt.GetNode<Node3D>("battlenun-body/battlenun-collider/LOSNodes").GetChildren();
				var ct = 8;
				foreach (RayCast3D r in tgtLosObj)
				{
					if(r.IsColliding())
					{
						ct -= 1;
					}
					r.TargetPosition = new Vector3(0, 0, 0);
				}
				if(ct == 8)
					GD.Print(modelTgt.Name, " is fully visible");
				else if(ct < 8)
				{
					modelTgt.inCoverDisplay = true;
					if(ct == 0)
						GD.Print(modelTgt.Name, " is hidden");
					else
						GD.Print(modelTgt.Name, "is in cover (", 8-ct, " of 8)");
				}
				
				los_step_two = false;
				calculateLOS = false;
				foreach(BSModel t in tgtModels)
				{
					t.ColliderOn();
				}
				foreach(BSModel t in myUnits)
					t.ColliderOn();
				LOScomplete = true;
			}
		}
		if(calculateLOS){
			if (unitTgt != null)
			{
				if(modelTgt != null)
				{
					foreach (RayCast3D r in tgtLosObj)
					{
						r.TargetPosition = eyeCaster.GlobalPosition - r.GlobalPosition;
					}
					modelTgt.ColliderOff();
					tgtModelsCounted++;
					if(tgtModelsCounted >= tgtModels.Count)
					{
						modelTgt = null;
						los_step_two = true;
						// now that everyone is looking at you, you can count all the collisions
						//GD.Print("done");
					}
					else
					{
						modelTgt = (BSModel)tgtModels[tgtModelsCounted];
						modelTgt.ColliderOn();
						tgtLosObj = modelTgt.GetNode<Node3D>("battlenun-body/battlenun-collider/LOSNodes").GetChildren();
					}
				}
			}
		}
    }

	private void GetLOS(BSModel src_m, BSUnit target_unit)
	{
		//if (src_m == null)
		//	src_m =  // for active model only 
		eyeCaster = src_m.GetNode<Node>("battlenun-body").GetNode<RayCast3D>("EyeCaster");
		foreach (BSModel b in myUnits)
		{
			b.ColliderOff();
		}
		src_m.ColliderOn();

		unitTgt = target_unit;

		if (tgtModels == null){
			
			tgtModels = unitTgt.FindChildren("*", "Sprite3D");
			tgtModelsCounted = 0;
			
			foreach(BSModel t in tgtModels)
			{
				t.ColliderOff();
			}

			tgtLosObj = null;
			modelTgt = (BSModel)tgtModels[tgtModelsCounted];
			modelTgt.ColliderOn();
			tgtLosObj = modelTgt.GetNode<Node>("battlenun-body/battlenun-collider/LOSNodes").GetChildren();
			calculateLOS = true;
		}
	}

	public void ResetLOS()
	{
		LOScomplete = false;
		tgtModels = null;
		calculateLOS = false;
		los_step_two = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// HOW IT WORKS NOW: 
		/// If LOScomplete is TRUE, we are NOT performing any LOS calculations.
		///  Set reset_los_flag to true, and this will set all flags next frame
		///  to re-process the physics. Then wait for LOScomplete to be true,
		///  and you will know the new LOS data has been updated.
		if(reset_los_flag)
		{
			reset_los_flag = false;
			ResetLOS();
		}
		if(LOScomplete == false){
			if (Name == "bnun unit"){ // Make sure to filter this properly
				GetLOS(GetNode<BSModel>("battlenun5"), GetNode<BSUnit>("/root/Node3D/bnun unit2"));
			}
		} 
		else
		{	// "I have completed my LOS calc."
			
		}
		
	}
}
