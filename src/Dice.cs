using System.Collections.Generic;
using Godot;


public class Dice
{
	public int ct;
	public int mod;
	public int faces;
	public List<int> results;
	private int total;
	public int ones;
	public int crit_val; // Modify this and run GetCrits()
	private int _crits;

	public Dice(int _ct = 1, int _faces = 6, int _mod = 0)
	{
		results = new List<int>();
		total = 0;
		ones = 0;
		ct = _ct;
		mod = _mod;
		faces = _faces;
		crit_val = 6;
		_crits = 0;
		
	}

	public void Roll()
	{
		results = new List<int>();
		for(int i = 0; i < ct; i++)
		{
			var r = (int)(GD.Randi() % faces) + 1;
			results.Add(r);
			if(r >= crit_val) _crits ++;
			else if(r == 1) ones ++;
		}
	}

	public int Total()
	{
		total = 0;
		for(int i = 0; i < results.Count; i++) total += results[i];
		return total;
	}

	public int GetCrits() // just in case we change crit_val, recalculate here
	{
		_crits = 0;
		for(int i = 0; i < ct; i++)
		{
			if(results[i] >= crit_val) _crits++;
		}
		return _crits;
	}

	public int GetSuccesses(int tgt)
	{
		int suc = 0;
		foreach(int r in results)
		{
			if(r >= tgt) suc ++;
		}
		return suc;
	}

	public static Dice operator + (Dice da, Dice db) // crit target and faces taken from first argument.
	{
        Dice nd = new Dice(da.ct + db.ct, da.faces, da.mod + db.mod)
        {
            crit_val = da.crit_val,
            ones = da.ones + db.ones
        };
        for (int a = 0; a < da.ct; a++) nd.results.Add(da.results[a]);
		for(int b = 0; b < db.ct; b++) nd.results.Add(db.results[b]);
		nd.Total();
		nd.GetCrits();
		
		return nd;
	}

	public void Refresh()
	{
		ones = 0;
		foreach(int v in results){ if (v == 1) { ones++; } }
		GetCrits();
		Total();
	}

	//
	public int RemoveEqual(int e)
	{
		var l = results.RemoveAll(x => x == e);
		// recalculate: crits, ones, total
		Refresh();
		return l;
	}
	public int RemoveLessThan(int e)
	{
		var l = results.RemoveAll(x => x < e);
		Refresh();
		return l;
	}
	public int RemoveMoreThanOrEqual(int e)
	{
		var l = results.RemoveAll(x => x >= e);
		Refresh();
		return l;
	}
	public Dice GrabEqualTo(int e)
	{
        Dice _d = new Dice();
        foreach (int r in results){
			if (r == e){
				_d.results.Add(r);
			}
		}
		_d.ct = _d.results.Count; // get correct new number
		return _d;
	}
	public Dice GrabLessThan(int e)
	{
        Dice _d = new Dice();
        foreach (int r in results){
			if (r < e){
				_d.results.Add(r);
			}
		}
		_d.ct = _d.results.Count; // get correct new number
		return _d;
	}
	public Dice GrabUps(int e)
	{
        Dice _d = new Dice();
        foreach (int r in results){
			if (r >= e){
				_d.results.Add(r);
			}
		}
		_d.ct = _d.results.Count; // get correct new number
		return _d;
	}
	//

}
