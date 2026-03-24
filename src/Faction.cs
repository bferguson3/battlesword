using Godot;
using System;
using Godot.Collections;
using System.Linq;

public partial class Faction : Node3D
{
	Godot.Collections.Array units;
	// units[x].upgrades[] contains e.g. "G1"
	Godot.Collections.Array upgradePackages;
	// upgradePackages[x].uid == upgrades from above
	Godot.Collections.Array spells ;
	// "threshold" should be used for cost 
	Godot.Collections.Array specialRules;
	// just a list of tags and descriptions for the entire army 

	BSArmy army = new BSArmy();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Dictionary _u = (Dictionary)ResourceLoader.Load<Json>("res://src/7oi8zeiqfamiur21.json").Data;
		units = _u["units"].AsGodotArray();
		upgradePackages = _u["upgradePackages"].AsGodotArray();
		spells = _u["spells"].AsGodotArray();
		specialRules = _u["specialRules"].AsGodotArray();

		for (int j = 0; j < units.Count; j++)
		{
			BSUnit nu = new BSUnit();
			Dictionary ev = (Dictionary)units[j];
			nu.unitName = (string)ev["name"];
			nu.cost = (int)ev["cost"];
			nu.uid = (string)ev["id"];
			Dictionary bases = (Dictionary)ev["bases"];
			nu.baseSize = (int)bases["round"]; // apply (baseSize / 32) factor to scale. (assuming all sprites are 32px).
			nu.def = (int)ev["defense"]; // seven minus to get "display" val. i.e. 7 - 6+ = def of 1
			nu.power = (int)ev["quality"]; // same as above - 7-n to get "power". behind the scenes this value is good.
			nu.unitCt = (int)ev["size"];
			GD.Print(nu.unitName, " ", nu.cost, " D:", 7 - nu.def, " P:", 7 - nu.power,  " x",nu.unitCt);
			Godot.Collections.Array weaps = (Godot.Collections.Array)ev["weapons"];
			for(int k = 0; k < weaps.Count; k++)
			{
				Dictionary w = (Dictionary)weaps[k];
				BSLoadout l = new BSLoadout();
				l.name = (string)w["name"]; // OK so far
				int ct = (int)w["count"];
				l.range = (int)w["range"];
				l.attacks = (int)w["attacks"];
				Godot.Collections.Array sp = (Godot.Collections.Array)w["specialRules"];
				foreach(Dictionary sr in sp)
				{
					if ((string)sr["name"] == "AP")
					{
						l.ap = (int)sr["rating"];
					}
					else
					{
						BSAbility ab = new BSAbility();
						ab.name = (string)sr["name"];
						if (sr.ContainsKey("rating"))
							ab.value = (int)sr["rating"];
						ab.id = (string)sr["id"]; // for description reference later 
						
						l.abilities.Add(ab);
						// TODO finish me by enabling enum etc. 
					}
				}
				for(int h = 0; h < ct; h++)
					nu.loadouts.Add(l.Copy());
				// 
				if (l.range == 0)
					GD.Print("--", l.name, " x", ct, " R: - A:", l.attacks, " AP", l.ap);
				else 
					GD.Print("--", l.name, " x", ct, " R: ", l.range, " A:", l.attacks, " AP", l.ap);
				// the problem here is we have to add the weapons to individual models! 
				//nu.AddWeaponModel(null, l, ct); // (sprite image, loadout object, number)
				// alternatively, nu.PopulateWeaponLoadouts() after all done. 
				// If there are less than unitCt children after this, probably a bug in the data. 
			}
			// TODO remove "Fast" and add to Mv stat
			Godot.Collections.Array rules = (Godot.Collections.Array)ev["rules"];
			foreach(Dictionary r in rules)
			{	// id, name, rating like above 
				if((string)r["name"] == "Tough")
				{
					nu.heartsPerModel = (int)r["rating"];
				}
				else
				{
					BSAbility ab = new BSAbility
					{
						id = (string)r["id"],
						name = (string)r["name"]
					};
					if (r.ContainsKey("rating"))
						ab.value = (int)r["rating"];

					nu.abilities.Add(ab);
				}
			}
			GD.Print(nu.heartsPerModel);
			// TODO items[] < DEFAULT upgrades. can be swapped sometimes.
			//// items[x].content[y].id is the "id" from specialRules. .count, .name also important.
			////  .bases important too - e.g. can add bikes to infantry. we need to change sprite and scale for these. 
			// TODO upgrades[] < must be CHOSEN. sometimes overrides items[].
			/// upgrades[x] is a string referring to upgradePackages[y].uid
			
			army.units.Add(nu);
			
		}

		// for upgradePackages[]: < entire army's
		/// upgradePackages[n] < split by package e.g. A1, E1
		/// .sections[x] < split by option groups. one package can have several options (/sections)
		/// .options[y] < split by option selection. individual choices listed here
		/// .gains[z] < split by individual ability. bonuses for that option here 

		//spells 

		//specialRules 
		
	}
}
