using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryMst : MasterCmn
{

	//	private const char PURSE_MARK = ':';

	//	private const string IS_TURN_TRUE = "1";

	public int Id;
	public string Tag;
	public int Priority;
	public string Name;
	public string Kana;
	public string Icon;
	public string StoryText;
	public string BackImage;
	public string EventImage;
	public string Effect;
	public string Se;
	public string Bgm;
	public string Anime;
	public string[] StImage;
	public string[] StFace;
	public string[] StPosi;
	public bool[] StTurn;

	public string NeedFlag;

	public int NeedFlagValue;

	public string[] SelectText;
	public string[] SelectFlag;
	public int[] SelectFlagValue;

	// #region selectButton
	// 	public string[] TjSelect;
	// 	public string[] TjTarget;
	// 	public string[] TjFlg;
	// #endregion

	[System.NonSerialized]
	public StandCharaData[] _Stand;

	// [System.NonSerialized]
	// public TagJumpData[] _TagJump;

	public StandCharaData[] Stand {
		get {
			if (_Stand == null) {
				setStand();
			}
			return _Stand;
		}
		private set {
			_Stand = value;
		}
	}
	//public TagJumpData[] TagJump{ get{ if(_TagJump==null){ setTagJump();} return _TagJump; }  private set{ _TagJump = value; } }


	public class StandCharaData
	{
		public string Image;
		public string Face;
		public string Position;
		public bool isTurn;
	}

	public class TagJumpData
	{
		public string SelectWord;
		public string TargetTag;
		public string Flg;
	}

	public StoryMst[] StoryLine;

	public void setStand() {
		Stand = new StandCharaData[StImage.Length];
		for (int i = 0; i < StImage.Length; i++) {
			Stand[i] = new StandCharaData();
			Stand[i].Image = StImage[i];
			Stand[i].Face = StFace[i];
			Stand[i].Position = StPosi[i];
			Stand[i].isTurn = (StTurn == null || i >= StTurn.Length) ? false : StTurn[i];
		}
	}

	// public void setTagJump(){
	//     if(TjSelect == null)
	//     {
	//         return;
	//     }

	// 	TagJump = new TagJumpData[TjSelect.Length];
	// 	for( int i = 0; i < TjSelect.Length; i++ ){
	//         TagJump[i] = new TagJumpData();
	//         TagJump[i].SelectWord = TjSelect[i];
	// 		TagJump[i].TargetTag = TjTarget[i];
	// 		TagJump[i].Flg = TjFlg[i];
	// 	}
	// }

}
