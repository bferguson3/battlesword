using Godot;
using System;

public partial class MapSelector : Sprite3D
{
	public double lastDelta;
	float moveSpeed = 2.0f;
	Camera3D cam;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam = (Camera3D)GetNode("/root/Node3D/BattleCamera");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var p = cam.GlobalBasis.X.Normalized();
		Position = cam.Position * p;
			
	}

}
