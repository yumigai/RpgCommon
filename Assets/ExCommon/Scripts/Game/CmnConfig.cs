using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CmnConfig {

	public static string MAIN_GAME_SCENE = "MainGame";
	public static string RESULT_AFTER_SCENE = "MainGame";
	public const string TITLE_SCENE = "TitleScene";
	public const string READY_SCENE = "ReadyScene";
	public const string COLLECTION_SCENE = "CollectionScene";
	public const string CONFIG_SCENE = "ConfigScene";
	public const string MISSION_SCENE = "MissionScene";
	public const string RESULT_SCENE = "ResultScene";
	public const string TUTORIAL_SCENE = "TutorialScene";
	public const string SELECT_LANG_SCENE = "SelectLangScene";



	public const string BEFORE_TITLE_SCENE_NAME = "CmnBeforeTitleScene";

	public const string SERVER_DOMAIN = "maemuke.s2.xrea.com";

	public const string SERVER_HTTP = "http://" + SERVER_DOMAIN;

	public const string SERVER_APPLI_ICON_DATA = SERVER_HTTP + "/ex_appli/Icon/";

    public const string RESOURCE_PREFAB = "Prefab/";

    public const string RESOURCE_MUSIC = "Sound/Music/";

    public const string RESOURCE_SE = "Sound/Se/";

    public const int REVIEW_LATER_NEXT_PLAYCOUNT = 3;

    /// <summary>
    /// アナログ・方向入力でないボタン最後
    /// </summary>
    public const int LastPadButton = (int)GamePadButton.Back;

	public enum SAVE_NAME{
		CMN_SOUND_CONFIG,
		CMN_PLAY_COUNT,
		CMN_IS_REVIEW,
		LANGUAGE,
	}

    /// <summary>
    /// 物理・論理キー
    /// </summary>
    public enum GamePadButton {
        Decision,
        Cancel,
        A,
        B,
        X,
        Y,
        L1,
        L2,
        L3,
        R1,
        R2,
        R3,
        Start,
        Back,
        Horizontal,
        Vertical,
        KeyAll,
        /// <summary>
        /// ↑物理キーここまで
        /// ↓論理キー（同時押し、複数入力可等）
        /// </summary>
        LogicKey = KeyAll+1,
        DualAxis,
        L1R1, //L1R1どちらも
    }

    /// <summary>
    /// 方向
    /// </summary>
    public enum Axis {
        Neutral,
        Up,
        Down,
        Left,
        Right,
    }



}
