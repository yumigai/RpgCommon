using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldMast : MulitiUseListMast
{

	public string Tag;
	public int[] NextIds;
	public string Map;

	public static WorldMast[] List;

	public static void load(){
		List = load<WorldMast> ();
	}

}