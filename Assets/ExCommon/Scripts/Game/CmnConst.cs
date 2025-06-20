using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CmnConst {
	
	public enum SCENE
	{
		MainGame,
		TitleScene,
		ConfigScene,
		TutorialScene,
		CollectionScene,
		HelpScene,
		ReadyScene,
		SelectLangScene,
		HomeScene,
		StoryScene,
        StoryListScene,
		MenuScene,
		EndingScene,
        QuestScene,
        ItemScene,
        ItemMenuScene,
        CharaScene,
		CharaMenuScene,
		CharaDetailScene,
        AreaSelectScene,
        StageSelectScene,
        SelectFloorScene,
        ResultScene,
        ResultAfterScene,
        BattleScene,
        QuestLogScene,
        ArchiveScene,
        ShopScene,
		ShopMenuScene,
        UpgradeScene,
		TeamEditScene,
		RecordScene,
		RankingScene,
		ErrorScene,
        BossScene,
        NowLoadingScene,
        OpeningScene,
        InitScene,
        KeyConfigScene,
        EquipScene,
        SystemScene,
        SaveScene,
    }

	public enum Lang
	{
		Jp,
		En
	}


	public const string BEFORE_TITLE_SCENE_NAME = "CmnBeforeTitleScene";

	public const string SERVER_DOMAIN = "maemuke.s2.xrea.com";

	public const string SERVER_HTTP = "http://" + SERVER_DOMAIN;

	public const string SERVER_APPLI_ICON_DATA = SERVER_HTTP + "/ex_appli/Icon/";

	//public const int CSV_BODY = 1;

	//public const int TSV_BODY = 1;

    public const int REVIEW_LATER_NEXT_PLAYCOUNT = 3;

	public readonly static string[] RARITY_COLOR;

	public readonly static float[] DROP_CATEGORY_PERCENT;

	public enum SAVE_NAME{
		CMN_SOUND_CONFIG,
		CMN_PLAY_COUNT,
		CMN_IS_REVIEW,
		LANGUAGE,
	}

	public static class Path{

		public const string TXT = "Text/";

		public const string SYSTEM_FILE_PATH = TXT + "System";

		public const string INIT_PLAYER_FILE_PATH = TXT + "Player";

		public const string INIT_GAME_STATUS_FILE_PATH = TXT + "InitGameStatus";

		public const string STAGE_FILE_PATH = TXT + "Stage";

		public const string TARGET_FILE_PATH = TXT + "Target";

		public const string TARGET_LV_FILE_PATH = TXT + "TargetLevel";

        public const string STORY_FILE_PATH = TXT + "Storys/";

        public const string STORY_LIST_FILE_PATH = STORY_FILE_PATH + "StoryList";

        public const string LEVEL_PARAM = TXT + "LevelParam";

		public const string ENEMY_PARAM = TXT + "Enemy";

		public const string UNIT_PATH = TXT + "UnitWrap";

		public const string DUNGEON_TEXT_PATH = TXT + "Dungeon";

		public const string SKILL_PATH = TXT + "Skill";
		public const string ENEMY_SKILL_PATH = TXT + "EnemySkill";
		public const string PLAYER_SKILL_PATH = TXT + "PlayerSkill";

		public const string CHAPTER_PATH = TXT + "Chapter";

        public const string ARCHIVE_TXT = TXT + "Archive/";

        public const string STAGE_BACK_IMG_PATH	= "Sprite/Stage/";

		public const string TARGET_IMG_PATH = "Sprite/Target/";

		public const string PREFAB = "Prefab/";

        public const string EFFECT_PREFAB = PREFAB + "Effects/";

        public const string EFFECTMNG_PREFAB = EFFECT_PREFAB + "Mng/";

		public const string MUSIC = "Sound/Music/";

		public const string SE = "Sound/Se/";

		public const string ADV_IMG_PATH = "Sprite/Adv/";

		public const string IMG_PATH = "Sprite/";

		public const string ADV_IMG_ICON = ADV_IMG_PATH + "Icon/";
		public const string ADV_IMG_BACK = ADV_IMG_PATH + "Back/";
		public const string ADV_IMG_EVENT = ADV_IMG_PATH + "Event/";
		public const string ADV_IMG_STAND = ADV_IMG_PATH + "Chara/";

        public const string IMG_CHARA = IMG_PATH + "Chara/";
		public const string IMG_CHARA_ICON = IMG_CHARA + "Icon/";
		public const string IMG_CHARA_LINE = IMG_CHARA + "Line/";
		public const string IMG_CHARA_BUSTUP = IMG_CHARA + "BustUp/";
		public const string IMG_CHARA_STAND = IMG_CHARA + "Stand/";

		public const string IMG_ITEM = IMG_PATH + "Item/";
		public const string IMG_ITEM_ICON = IMG_ITEM + "Icon/";

		public const string IMG_BACKGROUND = IMG_PATH + "BackGround/";

        public const string IMG_ARCHIVE = IMG_PATH + "Archives/";

        public const string IMG_ENEMY = IMG_PATH + "Enemy/";

		public const string ICON_PATH	= IMG_PATH + "Icon/";

		public const string ICON_ITEM_PATH = ICON_PATH + "Item/";

		public const string ICON_SKILL_PATH = ICON_PATH + "Skill/";

		public const string ICON_STAGE_PATH = ICON_PATH + "Stage/";

		public const string MAP_PATH = IMG_PATH + "Map/";
        
        public const string IMG_ELEMENT = ICON_PATH + "Element/";

        public const string GAMEPAD_IMG = IMG_PATH + "GamePad/Pad/";

        public const string KEYBOARD_IMG = IMG_PATH + "GamePad/KeyBoard/";
    }

}
