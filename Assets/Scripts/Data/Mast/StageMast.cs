using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageMast : MulitiUseListMast
{

    public enum GAME_RULE
    {
        GOAL,
        DESTROY_BOSS,
        GET_KEY_AND_GOAL,
        DESTROY_ENEMY,
        GET_TREASURE,
        LIVED,
        GURD,
        ALL
    }

    public enum KIND
    {
        MAIN_MISSION,
        STORY,
        BOSS,
        SUB_MISSION,
    }

    public enum FEATURE //特徴
    {
        NON,//基本的に使用しない
        INIT_STAGE, //初期解放ステージ
        LAST_STAGE,
    }

    public int ChapterId;
    public int MaxRoomNum;
    public int FloorNum; //階数・固定マップなら使わんかも
    public string Image;
    public GAME_RULE Rule;
    public int NeedKeyNum;
    public KIND Kind;
    public string Info;
    public string StagePrefab;
    public string Bgm;
    public string Story;
    public string AfterStory;
    public string SpecialEventTag;
    public string SpecialStory;
    public string SpecialAfterStory;
    public int FieldSize; //バトル時のモンスター出現数
    public int StageLv;
    public int EnemyNum;
    public int FoeNum; // tuujou ha 1
    public string EncountMapTag;
    public string FoeTag; // foe no enemy mast id
    public string UnitTag; // dare ni kannren suru stage ka. kore ga settei sarete iruto, party ni inai to archive ha ochinai
    public string ArchiveTag; // drop suru archive tag
    public FEATURE[] Feature = new FEATURE[0]; //特徴
    public int[] NextIds;

    public static IReadOnlyList<StageMast> List;

    /// <summary>
    /// NextIdsが一つの場合のみ使用（分岐など、複数オープンしない）
    /// </summary>
    public int NextId {
        get {
            return NextIds[0];
        }
    }

    public ChapterMast Chapter => ChapterMast.List.FirstOrDefault(it => it.Id == ChapterId);

    public string GetStory() {
        if (EventActionMast.judgeEvent(SpecialEventTag)) {
            return SpecialStory;
        }
        return Story;
    }

    public string GetAfterStory() {
        if (EventActionMast.judgeEvent(SpecialEventTag)) {
            return SpecialAfterStory;
        }
        return AfterStory;
    }


    public static void load() {
        List = load<StageMast>();
    }

}