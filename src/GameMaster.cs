using System.Collections.Generic;
using Godot;


public partial class GameMaster : Node3D
{
	public enum BSGameState { NONE, SELECTING_UNIT, UNIT_MENU_ROOT }
	public enum PlayerTurn { LOCAL, PC_OPPONENT, NPC }
	public BSGameState gameState;
	public PlayerTurn whoseTurn;

	public override void _Ready()
	{
		gameState = BSGameState.SELECTING_UNIT;
		whoseTurn = PlayerTurn.LOCAL;

		GD.Randomize();
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
