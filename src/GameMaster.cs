using System.Collections.Generic;
using Godot;

public class BSArmy
{
	public List<BSUnit> units;
}

public partial class GameMaster : Node3D
{
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

	public override void _Ready()
	{
		// Fixme I guess
		
		AdvanceTurn();
		SetPhase(BSGamePhase.MOVE);

		GD.Randomize();

		AddChild(player_faction); // to start it 
		
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

		}
	}

	public override void _Process(double delta)
	{
		
		Dice d = new Dice(5); // 5D6+0
		d.Roll();
		Dice dd = new Dice(3);
		dd.Roll();
		d = d + dd;
		GD.Print(d.results.ToArray().Stringify());
		
	}
	
}
