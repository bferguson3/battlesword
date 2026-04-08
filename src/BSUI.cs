using Godot;
using System;

public partial class BSUI : Control
{
	Control toolTip;
	// tooltip stuff:
	RichTextLabel unitTitle;
	RichTextLabel unitDescription;
	BSModel currentHighlight;
	Control descTip;
	Control nameTip;
	Control subMenu;
	PanelContainer smpc;

	public override void _Ready()
	{
		toolTip = GetNode<Control>("SubViewport/ToolTip");
		nameTip = GetNode<Control>("SubViewport/ToolTip/NameContainer");
		unitTitle = GetNode<RichTextLabel>("SubViewport/ToolTip/NameContainer/UnitTitle");
		descTip = GetNode<Control>("SubViewport/ToolTip/DescriptionContainer");
		unitDescription = GetNode<RichTextLabel>("SubViewport/ToolTip/DescriptionContainer/UnitDescription");
		subMenu = GetNode<Control>("SubViewport/SubMenu");
		smpc = GetNode<PanelContainer>("SubViewport/SubMenu/MenuContainer");
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

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.IsActionPressed("mouse_left"))
			{
				if(subMenu.Visible){
					if( (eventMouseButton.Position.X < (subMenu.Position.X))||
					(eventMouseButton.Position.Y > (subMenu.Position.Y + smpc.Size.Y)) ||
					(eventMouseButton.Position.Y < subMenu.Position.Y) || 
					(eventMouseButton.Position.X > (subMenu.Position.X + smpc.Size.X)))
					{
						if(currentHighlight != null){
							currentHighlight.Deselect();
							HideTooltip(currentHighlight);
						}
						subMenu.Visible = false;
					}
					else
					{
						GD.Print("ok");
					}
				}
				if(currentHighlight != null){
					subMenu.Visible = true;
					currentHighlight.myUnit.isSelected = true;
					subMenu.Position = new Vector2(eventMouseButton.Position.X - smpc.Size.X, eventMouseButton.Position.Y + 10);
				}
				GD.Print(eventMouseButton.Position, GetViewport().GetMousePosition(), subMenu.Position, smpc.Size);

				// once something is selected, current highlight does not go away
			}
			
		}
    }

	public void ShowTooltip(BSModel model)
	{
		//if(model != currentHighlight)
		//	currentHighlight.myUnit.isSelected = false;
		if(currentHighlight != null)
			if(currentHighlight != model)
			{
				if (currentHighlight.myUnit.isSelected)
				{
					return;
				}
			}
		currentHighlight = model;
		toolTip.ResetSize();
		descTip.ResetSize();
		nameTip.ResetSize();
			
		// show Panel and move to mouse position
		toolTip.Visible = true;
		string newText = "";
		unitTitle.Text = "[color=#ff5050] " + model.myUnit.unitName + " ";
		//if(model.myUnit.unitCt > 1)
			unitTitle.Text += "[/color] [" + model.myUnit.myModelNodes.Count + "/" + model.myUnit.unitCt + "] ";
		//else
		//	unitTitle.Text +="[/color] ";
		if(!subMenu.Visible){
			Vector2 pos = GetViewport().GetMousePosition();
			toolTip.SetPosition(new Vector2(pos.X + 20, pos.Y + 10));
		}
		if (model.maxHearts > 1)
			newText = " HP: [color=#00ff60]" + model.currentHearts + "/" + model.maxHearts + "[/color]\n ";
		else
			newText = " HP: [color=#00ff60] 1[/color]\n ";
		/*
		[e85545]Loadout[-]
		[c6c930]3x Rifles[-] (24", A1)
		[c6c930]3x CCW[-] (A1)

		[dc61ed]Special[-]
		Hold the Line
		*/
		newText += "[color=#e6e442]Pwr[/color] " + (7 - model.myUnit.power).ToString() + " / [color=#46e4e2]Def[/color] " + (7 - model.myUnit.def).ToString() +"\n\n "; 
		newText += "[color=#e85545]Loadout[/color]\n ";
		for(int i = 0; i < model.myLoadouts.Count; i++)
		{
			if(model.myLoadouts[i].range == 0)
				newText += "[color=#c6c930]" + model.myLoadouts[i].name + "[/color] (A" + model.myLoadouts[i].attacks.ToString() + ") ";
			else 
				newText += "[color=#c6c930]" + model.myLoadouts[i].name + "[/color] (" + model.myLoadouts[i].range.ToString() + "\", A" + model.myLoadouts[i].attacks.ToString() + ") ";
			if(model.myLoadouts[i].abilities.Count > 0)
			{
				newText += "[";
				foreach(var ab in model.myLoadouts[i].abilities)
					newText += ab.name + ", ";
				newText = newText.Substring(0, newText.Length - 2);
				newText +="] ";
			}
			newText +="\n ";
		}
		newText += "\n [color=#dc61ed]Special[/color]\n ";
		// unique upgrades:
		for(int i = 0; i < model.myAbilities.Count; i++)
		{
			newText += model.myAbilities[i].name + "\n ";
		}
		// unit-wide abilities:
		for(int i = 0; i < model.myUnit.abilities.Count; i++)
		{
			newText += model.myUnit.abilities[i].name + "\n ";
		}
		newText = newText.Substring(0, newText.Length - 2);

		unitDescription.Clear();
		unitDescription.AppendText(newText);
		
		if (descTip.Size.X > nameTip.Size.X) nameTip.Size = new Vector2(descTip.Size.X, nameTip.Size.Y);
		else descTip.Size = new Vector2(nameTip.Size.X, descTip.Size.Y);
	}
}
