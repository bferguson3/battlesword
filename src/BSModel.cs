using Godot;

public partial class BSModel : Sprite3D
{
	public CollisionObject3D myCollider;
	public RayCast3D myEyes;
	private CollisionShape3D myShape;
	public Godot.Collections.Array<Node> myLines;

	public BSUnit myUnit;

	[Export]
	public bool inCoverDisplay;
	private float colorMult;
	private bool colorDown;
	private Color mySavedColor;

	public void SetCollisionMask(int layer, bool v)
	{
		
		myCollider.SetCollisionMaskValue(layer, v);
	}
	public void SetCollisionLayer(int l, bool v)
	{
		myCollider.SetCollisionLayerValue(l, v);
	}
	
	public override void _Ready()
	{
		colorMult = 1.0f;
		myCollider = (CollisionObject3D)GetNode("battlenun-body");	
		myShape = (CollisionShape3D)GetNode("battlenun-body/battlenun-collider");
		myLines = myShape.GetNode<Node>("LOSNodes").GetChildren();
		myEyes = GetNode<Node>("battlenun-body").GetNode<RayCast3D>("EyeCaster");
		myUnit = GetParent<BSUnit>();

		myCollider.SetRayPickable(true);
		myCollider.MouseEntered += MouseEnter;
		myCollider.MouseExited += MouseExit;

		mySavedColor = Modulate;
	}

	public void SetColor(Color c)
	{
		Modulate = c;
		mySavedColor = c;
	}

	public void MouseEnter()
	{
		if(!myUnit.isHighlighted)
		{
			if(!flashingColor)
				myUnit.Flash(new Color("#008800"));
			else
				targetClr = new Color("#008800");
			myUnit.isHighlighted = true;
		}
	}

	public void MouseExit()
	{
		if(myUnit.isHighlighted)
		{
			myUnit.isHighlighted = false;
			//myUnit.FlashOff();
		}
	}

	Color colorMod = new Color(0, 0, 0);
	Color targetClr = new Color(0, 0.5f, 0);
	bool flashingUp = true;
	bool flashingColor = false;
	// increment colorMods each R, G, B if they are less than target value. 
	// set modulate to mySavedC + colorMod R, G, B. 
	public void Flash(Color c)
	{
		flashingUp = true;
		
		targetClr = c;
		colorMod = new Color(0, 0, 0);

		flashingColor = true;
	}
	public void FlashOff()
	{
		flashingColor = false;
		Modulate = mySavedColor;
	}
	
	
	public override void _Process(double delta)
	{
		if(flashingColor){
			if(flashingUp){
				//GD.Print("up");
				if(colorMod.R < targetClr.R) colorMod.R += (float)delta ;
				if(colorMod.G < targetClr.G) colorMod.G += (float)delta ;
				if(colorMod.B < targetClr.B) colorMod.B += (float)delta ;
				if(colorMod.R >= targetClr.R) if(colorMod.G >= targetClr.G) if(colorMod.B >= targetClr.B)
				{
					flashingUp = false;
				}
			}
			else
			{
				if(colorMod.R > 0) colorMod.R -= (float)delta ;
				if(colorMod.G > 0) colorMod.G -= (float)delta ;
				if(colorMod.B > 0) colorMod.B -= (float)delta ;
				//GD.Print(colorMod.R, colorMod.G, colorMod.B);
				if(colorMod.R < 0.1) if(colorMod.G < 0.1) if(colorMod.B < 0.1)
				{
					//colorMod = new Color(0, 0, 0);
					flashingUp = true;
					if (!myUnit.isHighlighted)
					{	
						FlashOff();
					}
				}
			}

			Modulate = mySavedColor + colorMod;
		}
		
		
		
		if(inCoverDisplay)
		{	// flash me grey 
			if(!colorDown){
				colorMult -= (float)delta * 0.25f;
				if (colorMult > 0.75f)
				{
					Modulate = new Color(mySavedColor.R * colorMult, mySavedColor.G *colorMult, mySavedColor.B * colorMult);
				}
				else 
					colorDown = true;
			}
			else
			{
				colorMult += (float)delta * 0.25f;
				if (colorMult < 1.0f)
				{
					Modulate = new Color(mySavedColor.R * colorMult, mySavedColor.G *colorMult, mySavedColor.B * colorMult);
				}else 
					colorDown = false;
			}
		}
	}

	public void ColliderOff()
	{
		myShape.SetDeferred("disabled", true);
	}

	public void ColliderOn()
	{
		myShape.SetDeferred("disabled", false);
	}
}
