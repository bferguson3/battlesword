using System.Collections.Generic;
using Godot;

public partial class BSArmy : Node3D
{
	public List<BSUnit> units = new List<BSUnit>();
}


public partial class GameMaster : Node3D
{

	public Dictionary<string,string> BlessedSistersSprites = new Dictionary<string, string>
	{ //0, 3, 8, 9, 10, 11, 13
		{"Exo-Suit High Sister", "exosuit60.png"}, //60
		{"High Destroyer Sister",""}, // 40
		{"Celestial High Sister",""}, // 32 
		{"High Sister","sister1.png"}, // 32
		{"Fanatic Superior",""}, // 25
		{"Novice Leader",""}, // 25
		{"Novice Sisters",""}, // dual ccw 25mm
		{"Fanatic Sisters",""}, // chain sword
		{"Warrior Sisters","warriorsister_25.png"}, // rifles
		{"Vanguard Sisters","sistervanguard_25.png"}, // rifles
		{"Protector Sisters","protectorsister_25.png"}, // pistol/spear
		{"Pistoleer Sisters","pistoleersister_25.png"}, // pistols 
		{"Assault Sisters",""}, // flying, swd pistol
		{"Celestial Warrior Sisters","celestialsister_25.png"}, // heavy rifle
		{"Support Sisters",""}, // flamers 25mm
		{"Destroyer Sisters",""}, // ccw only 40mm
		{"Biker Fanatics",""}, //60x35
		{"Biker Sisters",""}, //60x35
		{"APC",""},
		{"Procession Altar",""},
		{"Infernal APC",""},
		{"Organ Tank",""},
		{"Battle Tank",""},
		{"Exo-Suit",""}, // 60
		{"Assault Walker",""}, //120x92
		{"Support Walker",""}, // 120x92
		{"Blessed Titan",""}, // 160x122
		{"Constance",""},
		
	};

	public enum BSPlayerState { NONE, SELECTING_UNIT, UNIT_MENU_ROOT }
	public enum BSGamePhase {CHANGING_TURNS, COMMAND, MOVE, SHOOT, CHARGE, FIGHT, END };
	public enum PlayerTurn { LOCAL, PC_OPPONENT, NPC }
	public BSPlayerState playerState;
	public PlayerTurn whoseTurn;
	public BSGamePhase activePhase;

	public BSArmy player_army = new BSArmy();
	public Faction player_faction = new Faction();
	public BSArmy enemy_army = new BSArmy();
	public Faction enemy_faction = new Faction();

	// TEMPLATE OF ALL MODELS 
	PackedScene modelScn = GD.Load<PackedScene>("res://scenes/model.tscn");


	public override void _Ready()
	{
		GD.Randomize();

		// Make this a proper flow: 
		AdvanceTurn();
		SetPhase(BSGamePhase.MOVE);

		// Test: all armies are battle sisters 
		player_faction.Name = "PlayerFaction"; 	// change the flag here to adjust which faction they actually are. 
		AddChild(player_faction); 
		enemy_faction.Name = "EnemyFaction";
		AddChild(enemy_faction); // Initializing these objects basically copies all their army data into GD stuff. 

		// test out just spawning one...
		//SpawnUnit(player_faction.factionUnits[0]);
		SpawnUnit(player_faction.factionUnits[3]);
		SpawnUnit(player_faction.factionUnits[8]);
		SpawnUnit(player_faction.factionUnits[9]);
		SpawnUnit(player_faction.factionUnits[10]);
		SpawnUnit(player_faction.factionUnits[11]);
		SpawnUnit(player_faction.factionUnits[13]);
		
	}

	public void AdvanceTurn()
	{
		if(whoseTurn == PlayerTurn.LOCAL)
			whoseTurn = PlayerTurn.NPC;
		else 
			whoseTurn = PlayerTurn.LOCAL;
	}

	public void SetPhase(BSGamePhase phase)
	{
		activePhase = phase;

		if(activePhase == BSGamePhase.MOVE)
		{
			// enable selectability of units 
			//GetNode<BSUnit>("bnun unit").isSelectable = true;
			//GetNode<BSUnit>("bnun unit3").isSelectable = true;
		}
	}

	public override void _Process(double delta)
	{
		
		Dice d = new Dice(5); // 5D6+0
		d.Roll();
		//Dice dd = new Dice(3);
		//dd.Roll();
		//d = d + dd;
		//GD.Print(d.results.ToArray().Stringify());
		
	}

	public BSUnit SpawnUnit(BSUnit unit)
	{
	// default scale is meant for 32x32 px sprites with scale of 1.0
	// so, battlenun-body must be scaled e.g. 1.875 for 60px
	// nothing else is scaled. assume sprite px size is correct. 
		BSUnit u = new BSUnit();
		// instantiate sprites for each model in the unit by instntiating the model Scene 
		u.unitName = unit.unitName;
		u.Name = unit.unitName;
		for(int k = 0; k < unit.loadouts.Count; k++)
		{
			u.loadouts.Add(unit.loadouts[k].Copy());	
			//GD.Print(unit.loadouts[k].name);
		}
		for(int k = 0; k < unit.abilities.Count; k++)
		{
			u.abilities.Add(unit.abilities[k].Copy());	
			//GD.Print(unit.abilities[k].name);
		}
		for(int i = 0; i < unit.unitCt; i++)
		{
			var m = modelScn.Instantiate<BSModel>();
			float scale = (float)unit.baseSize / 32.0f;
			// this is where we assign Sprite and Size/ scale if needed!
			m.Texture = ResourceLoader.Load<Texture2D>("res://assets/" + BlessedSistersSprites[unit.unitName]);
			m.Name = unit.unitName + "_" + i.ToString();
			Node3D body = m.GetNode<Node3D>("battlenun-body");
			body.Scale = new Vector3(scale, scale, scale);
			if (i < 5)
				m.Position = new Vector3(i * scale / 3.0f, scale / 6.0f, 0.0f);
			else 
				m.Position = new Vector3((i - 5) * scale / 3.0f, scale / 6.0f, scale / 3.0f);
			m.GetDefaultLoadout(u); // gets weapons and abilities from "unit master" aka defaults. 
			// hearts
			m.currentHearts = unit.heartsPerModel;
			m.maxHearts = unit.heartsPerModel;
			//GD.Print("hearts: ", m.maxHearts);
			
			u.AddChild(m);
			//m.SetCircleSize(unit.baseSize);
		}
		u.uid = unit.uid;
		u.cost = unit.cost;
		u.baseSize = unit.baseSize;
		u.power = unit.power;
		u.def = unit.def;
		u.unitCt = unit.unitCt;
		u.heartsPerModel = unit.heartsPerModel;
		u.move = unit.move;	

		// TODO: 
		// Copy ITEMS
		// Copy UPGRADES 
		// (RULES is copied via abilities[] above)

		// then add entire unit as children to this GameMaster
		u.isSelectable = true;
		AddChild(u);
		
		u.myUnits = u.GetChildren();
		foreach (BSModel s in u.myUnits)
		{
			s.SetColor(new Color(0.66f, 0.66f, 1.0f));
			//GD.Print(u.unitName, u.baseSize);
			s.SetCircleSize(u.baseSize);
		}
		return u;
	}
	
}
