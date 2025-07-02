using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FieldUIMng : MonoBehaviour
{

    /// <summary>
    /// Miniマップの部屋間隔
    /// </summary>
    public const int MAP_PURSE = 1;

    [SerializeField]
    public Transform EnemyBase;

    [SerializeField]
    public Image ToClearIcon;

    [SerializeField]
    public Sprite[] ToClearSprites;

    [SerializeField]
    public Text ToClearMax;

    [SerializeField]
    public Text ToClearNum;

    [SerializeField]
    public Text StageTitle;

    [SerializeField]
    public Text StageTerm;

    [SerializeField]
    public Transform MapBase;

    [SerializeField]
    public Transform MiniMapRoom;

    [SerializeField]
    public Image HeroRader;

    [SerializeField]
    public GameObject EnemyRaderPrefab;

    [SerializeField]
    public Transform EnemyRaderBase;

    [SerializeField]
    public CharaImgGroupMng CharaGroup;

    [SerializeField]
    public FieldObjectInfoMng StageClearGuid;

    [System.NonSerialized]
    private int[] NowNums;

    [System.NonSerialized]
    public List<RectTransform> MapRooms = new List<RectTransform>();

    [System.NonSerialized]
    public Dictionary<Vector2Int, RectTransform> MapRoads = new Dictionary<Vector2Int, RectTransform>();

    [System.NonSerialized]
    public List<Transform> EnemyRaders = new List<Transform>();

    [System.NonSerialized]
    private Vector2 MapDetection = new Vector2();

    public static FieldUIMng Singleton;



    private void Awake() {
        Singleton = this;
    }

    // Use this for initialization
    void Start() {

        initInfo();

        EnemyRaderPrefab.gameObject.SetActive(false);

    }

    protected void FixedUpdate() {
        updateMapRader();
    }

    public void makeMiniMap(StageAreaMng StageArea) {

        //ミニマップ作成
        for (int i = 0; i < StageArea.Map.Count; i++) {
            RectTransform rect = addMiniMapRoom(StageArea.Map[i], StageArea.Rooms[i].MapImage, StageAreaMng.MAP_OUTLINE_SIZE);
            var outline = rect.gameObject.AddComponent<Outline>();
            outline.effectDistance = new Vector2(StageAreaMng.MAP_OUTLINE_SIZE, StageAreaMng.MAP_OUTLINE_SIZE);
            rect.name = "map:" + i;
            MapRooms.Add(rect);
        }
        //for (int i = 0; i < StageArea.ConnectRoad.Count; i++) {
        //    RectTransform rect = addMiniMapRoom(StageArea.ConnectRoad[i], StageAreaMng.MAP_OUTLINE_SIZE);
        //    rect.name = "road:" + i;
        //}
        foreach (var road in StageArea.ConnectRoad) {
            RectTransform rect = addMiniMapRoom(road.Value, StageArea.MapConnectImage, StageAreaMng.MAP_OUTLINE_SIZE);
            MapRoads.Add(road.Key, rect);
            rect.name = "road:" + road.Key.x + ":" + road.Key.y;
        }

        //float map_min_x = MapRooms.Min(it => it.localPosition.x);
        //float map_max_x = MapRooms.Max(it => it.localPosition.x);
        //float map_min_y = MapRooms.Min(it => it.localPosition.y);
        //float map_max_y = MapRooms.Max(it => it.localPosition.y);

        float map_min_x = StageArea.Map.Min(it => it.position.x);
        float map_max_x = StageArea.Map.Max(it => it.position.x);
        float map_min_y = StageArea.Map.Min(it => it.position.y);
        float map_max_y = StageArea.Map.Max(it => it.position.y);

        float min_x = StageArea.Rooms.Min(it => it.transform.localPosition.x);
        float max_x = StageArea.Rooms.Max(it => it.transform.localPosition.x);
        float min_y = StageArea.Rooms.Min(it => it.transform.localPosition.z);
        float max_y = StageArea.Rooms.Max(it => it.transform.localPosition.z);

        Vector2 map_size = new Vector2(map_max_x - map_min_x, map_max_y - map_min_y) * MiniMapRoom.transform.localScale;
        Vector2 size = new Vector2(max_x - min_x, max_y - min_y) * StageArea.transform.localScale;

        MapDetection = map_size / size;
    }


    /// <summary>
    /// ミニマップ設定
    /// </summary>
    public RectTransform addMiniMapRoom(Rect room, Sprite sp, int sub = MAP_PURSE) {
        GameObject img_obj = new GameObject();
        Image img = img_obj.AddComponent<Image>();
        img.sprite = sp;
        img_obj.transform.SetParent(MiniMapRoom);
        RectTransform rect = img_obj.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(room.position.x + room.size.x / 2, room.position.y + room.size.y / 2, 0);
        rect.localScale = Vector3.one;
        rect.localRotation = new Quaternion();
        rect.sizeDelta = room.size;
        //MapRooms.Add(rect);
        rect.sizeDelta = new Vector2(room.size.x - sub, room.size.y - sub);
        var col2d = rect.gameObject.AddComponent<BoxCollider2D>();
        col2d.size = rect.sizeDelta;
        img.enabled = false;
        return rect;
    }


    private void initInfo() {
        CharaGroup.CreateGroup(SaveMng.Quest.ActiveParty);

        StageTitle.text = StageFieldSceneMng.StageData.Name;
        //switch (StageFieldSceneMng.StageData.Rule)
        //{
        //    case StageMast.GAME_RULE.DESTROY_ALL:
        //        StageTerm.text = "敵を全滅させよ";
        //        break;
        //    case StageMast.GAME_RULE.GET_TREASURE:
        //        StageTerm.text = "カギを" + MainProcess.StageData.NeedKeyNum +"個回収せよ"; 
        //        break;
        //}
    }

    /// <summary>
    /// 敵追加
    /// </summary>
    /// <param name="ene"></param>
    public void addEnemy(FieldEnemyMng ene) {
        //GameObject obj = Instantiate(EnemyInfo.gameObject) as GameObject;
        //obj.GetComponent<EnemyInfoMng>().setInit(ene);
        //obj.transform.parent = EnemyBase;

        //レーダー用
        GameObject enemy_rader = Instantiate(EnemyRaderPrefab);
        enemy_rader.SetActive(true);
        enemy_rader.transform.SetParent(EnemyRaderBase);
        enemy_rader.transform.localScale = Vector3.one;
        EnemyRaders.Add(enemy_rader.transform);

    }

    /// <summary>
    /// マップレーダー更新
    /// </summary>
    protected void updateMapRader() {
        Transform hero = FieldPlayerMng.hero().transform;
        Vector3 posi = hero.localPosition;
        MapBase.transform.localPosition = new Vector3(-posi.x * MapDetection.x, -posi.z * MapDetection.y, 0);
        Quaternion rotate = HeroRader.transform.localRotation;
        rotate.z = -hero.localRotation.y;
        HeroRader.transform.eulerAngles = new Vector3(0, 0, -hero.eulerAngles.y);
        updateEnemyMapRader();

    }

    /// <summary>
    /// 敵レーダー更新
    /// </summary>
    public void updateEnemyMapRader() {

        for (int i = 0; i < EnemyRaders.Count(); i++) {
            if (i < StageFieldSceneMng.Singleton.Enemys.Count()) {
                Vector3 posi = StageFieldSceneMng.Singleton.Enemys[i].transform.localPosition;
                EnemyRaders[i].transform.localPosition = new Vector3(posi.x * MapDetection.x, posi.z * MapDetection.y, 0);
            } else {
                Destroy(EnemyRaders[i].gameObject);
                EnemyRaders.RemoveAt(i);
            }
        }
    }

    public void onMapCenter(GameObject game_object) {


        var index = MapRooms.FindIndex(it => it.gameObject == game_object);
        if (index >= 0) {
            var on_map = MapRooms[index];
            var img = on_map.GetComponent<Image>();
            if (img != null && img.enabled == false) {
                img.enabled = true;
                var connects = MapRoads.Where(it => it.Key.x == index || it.Key.y == index);
                foreach (var connect in connects) {
                    connect.Value.GetComponent<Image>().enabled = true;
                }
            }
        }
    }

    ///// <summary>
    ///// マップタイル更新
    ///// </summary>
    //private void updateMapTile() {
    //    var a = MapRooms.Find(it => it.rect.Contains(HeroRader.rectTransform.position));

    //    if (a != null) {
    //        a.gameObject.SetActive(true);
    //    }
    //}

}