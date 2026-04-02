using Godot;
using System;

public partial class BSUI : Control
{
	Control toolTip;
	// tooltip stuff:
	RichTextLabel unitTitle;
	RichTextLabel unitDescription;
	BSModel currentHighlight;

	public override void _Ready()
	{
		toolTip = GetNode<Control>("SubViewport/MainControl/ToolTip");
		unitTitle = GetNode<RichTextLabel>("SubViewport/MainControl/ToolTip/UnitTitle");
		unitDescription = GetNode<RichTextLabel>("SubViewport/MainControl/ToolTip/UnitDescription");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void HideTooltip(BSModel unit)
	{
		if (currentHighlight == unit)
		{
			toolTip.Visible = false;
			currentHighlight = null;	
		}
		
	}

	public void ShowTooltip(BSModel model)
	{
		currentHighlight = model;
		// show Panel and move to mouse position
		toolTip.Visible = true;
		unitTitle.Text = "[color=#ff5050] " + model.myUnit.unitName;
		Vector2 pos = GetViewport().GetMousePosition();
		toolTip.SetPosition(new Vector2(pos.X + 20, pos.Y + 10));
		unitDescription.Text = "HP: [color=#00ff60]" + model.currentHearts + "/" + model.maxHearts;
		//toolTip.SetPosition(GetMousePosition());
		// update with data 
	}
}
