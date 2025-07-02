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
        DESTROY_ALL,
        GET_TREASURE,
        LIVED,
        GURD,
        ALL
    }

    public enum KIND
    {
        SUB_MISSION,
        MAIN_MISSION,
        STORY,

        BOSS
    }

    public enum FEATURE //特徴
    {
        NON,//基本的に使用しない
        INIT_STAGE, //初期解放ステージ
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
    public int FieldSize; //バトル時のモンスター出現数
    public int EnemyNum;
    public int EncountMapId;
    public FEATURE[] Feature = new FEATURE[0]; //特徴
    public int[] NextIds;

    public static StageMast[] List;

    /// <summary>
    /// NextIdsが一つの場合のみ使用（分岐など、複数オープンしない）
    /// </summary>
    public int NextId {
        get {
            return NextIds[0];
        }
    }

    public ChapterMast Chapter => ChapterMast.List.FirstOrDefault(it => it.Id == ChapterId);

    public static int getStageIndex(int id) {
        return System.Array.FindIndex(List, it => it.Id == id);
    }

    public static void load() {
        List = load<StageMast>();
    }

}