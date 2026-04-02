using System.Collections.Generic;
using Godot;

public partial class BSModel : Sprite3D
{
	public CollisionObject3D myCollider;
	public RayCast3D myEyes;
	private CollisionShape3D myShape;
	public Godot.Collections.Array<Node> myLines;
	MeshInstance3D outerCircle;
	MeshInstance3D innerCircle;

	public BSUnit myUnit;

	// The units attack will drop by this amt when this model dies...
	public List<BSLoadout> myLoadouts = new List<BSLoadout>();
	// Unique upgrades, etc. 
	public List<BSAbility> myAbilities = new List<BSAbility>();

	public int currentHearts;
	public int maxHearts;

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

		outerCircle = GetNode<MeshInstance3D>("OuterCircle");
		innerCircle = GetNode<MeshInstance3D>("InnerCircle");
	}

	public void SetCircleSize(int mm)
	{
		// First, we assume that we are properly scaled at this point. 
		// both circles need to be placed downward on the Y in 1/2 the mm size. 
		// e.g. -0.16 for a 32mm model. 
		outerCircle.TopLevel = false;
		outerCircle.Position = new Vector3(0, (float)mm * -0.005f, 0); // mm to 0.01 and half again
		outerCircle.TopLevel = true;
		innerCircle.Position = new Vector3(0, (float)mm * -0.005f, 0);
		// then it needs to be resized. mesh.radius needs to be again 1/2 the mm size.
		SphereMesh s = new SphereMesh();
		s.Radius = (float)mm * 0.005f; // 60 == 0.3
		s.Height = (float)mm * 0.01f;
		innerCircle.Mesh = s;
		SphereMesh s2 = new SphereMesh();//outerCircle.Mesh as SphereMesh;
		s2.Radius = s.Radius + (0.254f * 6.0f); // six inch radius 
		s2.Height = s2.Radius / 2.0f;
		outerCircle.Mesh = s2;
	}

	public void GetDefaultLoadout(BSUnit u)
	{
		myLoadouts = new List<BSLoadout>();
		for(int i = 0; i < u.loadouts.Count; i++)
		{
			myLoadouts.Add(u.loadouts[i].Copy());
		}
		myAbilities = new List<BSAbility>();
		for(int i = 0; i < u.abilities.Count; i++)
		{
			myAbilities.Add(u.abilities[i].Copy());
		}
	}

	public void SetColor(Color c)
	{
		Modulate = c;
		mySavedColor = c;
	}

	public void MouseEnter()
	{
		if(myUnit.isSelectable)
			if(!myUnit.isHighlighted)
			{
				if (!flashingColor)
				{
					//GD.Print("dgb");
					myUnit.Flash(new Color("#008800"));			
				}
				else {
					targetClr = new Color("#008800");
				}
				myUnit.isHighlighted = true;
				myUnit.lastPointedModel = this;
			}
	}

	public void MouseExit()
	{
		if(myUnit.isHighlighted)
		{
			myUnit.isHighlighted = false;
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
		colorMod = new Color(0, 0, 0);
		Modulate = mySavedColor;
	}
	
	
	public override void _Process(double delta)
	{
		if(flashingColor){
			if(flashingUp){
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
				if(colorMod.R < 0.1) if(colorMod.G < 0.1) if(colorMod.B < 0.1)
				{ //colorMod = new Color(0, 0, 0);
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
		// FIX ME !!!
			if(!colorDown){
				colorMult -= (float)delta * 0.25f;
				if (colorMult > 0.75f)
				{
					Color _c = (mySavedColor + colorMod) * colorMult;
					Modulate = _c;
				}
				else 
					colorDown = true;
			}
			else
			{
				colorMult += (float)delta * 0.25f;
				if (colorMult < 1.0f)
				{
					Color _c = (mySavedColor + colorMod) * colorMult;
					Modulate = _c;
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
