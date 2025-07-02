using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdvStatusTran : CmnSaveProc.SaveClass
{

	//public const int SAVE_SLOT_NUM = 10;

	//public static AdvStatus AdvData;
	//public static AdvSaveSlot Slot;

	// static AdvStatusTran(){
	// 	//Slot = load<AdvSaveSlot> ( );
	// 	//AdvData = new AdvStatus[SAVE_SLOT_NUM];
	// }

	public Dictionary<string, int> Flags;
	public string StoryName;
	public int StoryIndex;

	public AdvStatusTran() {
		Flags = new Dictionary<string, int>();
		StoryName = "";
		StoryIndex = 0;
	}

	#region UseNovelOnly
	public int SlotIndex;
	public long SaveDate;
	#endregion


	public void addFlag(string key, int val) {
		Flags[key] += val;
	}

	public void setFlag(string key, int val) {
		Flags[key] = val;
	}

	public int getFlag(string key) {
		if (Flags.ContainsKey(key)) {
			return Flags[key];
		}
		return 0;
	}

	// public class AdvStatus : SaveClass
	// {

	// }

	// public class AdvSaveSlot : SaveClass
	// {
	// 	public int[] SlotIndex;
	// 	public string SaveDate;
	// 	public string Thumbnail;
	// }

	// public static void load(){
	// 	AdvData =  load<AdvStatus> ( );
	// }

	// public static void load( int index ){
	// 	AdvData =  load<AdvStatus> ( );
	// }

	// public static void save(){
	// 	AdvData.save ();
	// }
	//	public static void save( int index ){
	//		AdvData.save (index);
	//	}
}