using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class StageAreaMng : MonoBehaviour
{

    public const int MAP_OUTLINE_SIZE = 2;

    public enum AREA_TYPE
    {
        RANDOM,
        STATIC,
        ROOM_RANDOM,
    }

    public enum BACK_GROUND_TYPE
    {
        TILE,
        UP,
        UP_RIGHT,
    }

    [SerializeField]
    public Transform StageBase;

    [SerializeField]
    public StageRoomMng[] RoomPrefabs;

    [SerializeField]
    public int MaxRoomNum;

    [SerializeField, Header("固定かランダムか")]
    private AREA_TYPE AreaType;

    [SerializeField, Header("背景タイプ")]
    private BACK_GROUND_TYPE BackType = BACK_GROUND_TYPE.UP_RIGHT;

    [SerializeField]
    private Transform BackGround;

    [SerializeField]
    private StageTileMng BackGroundTile;

    [SerializeField]
    public float MapUnitSize = 9; //マップサイズ単位

    [SerializeField]
    public GameObject EnemyPrefab;

    [SerializeField, Header("部屋をランダム回転させるか")]
    public bool isRandomRotate = true;

    [SerializeField, Header("nullでも可")]
    public GameObject KeyPrefab; //nullの場合、共通の値を使用

    [SerializeField, Header("nullでも可")]
    public GameObject GatePrefab; //nullの場合、共通の値を使用

    [SerializeField, Header("nullでも可")]
    public Sprite MapConnectImage;

    [System.NonSerialized]
    public List<StageRoomMng> Rooms = new List<StageRoomMng>();

    private int[] BlockRates;

    [System.NonSerialized]
    public List<Rect> Map = new List<Rect>();

    [System.NonSerialized]
    public Dictionary<Vector2Int, Rect> ConnectRoad = new Dictionary<Vector2Int, Rect>();

    [System.NonSerialized]
    public Vector3 AreaMin = new Vector3();

    [System.NonSerialized]
    public Vector3 AreaMax = new Vector3();

    [System.NonSerialized]
    public bool IsBaked = false; //Bake済みか

    //[System.NonSerialized]
    //public System.Action CallbackBaked;

    void Awake() {
        initSetup();
    }

    public void initSetup() {

        IsBaked = false;

        StageBase = StageBase == null ? this.transform : StageBase;

        switch (AreaType) {
            case AREA_TYPE.RANDOM:
            break;

            case AREA_TYPE.STATIC:
            break;
            case AREA_TYPE.ROOM_RANDOM:
            break;
        }

        if (AreaType == AREA_TYPE.RANDOM) {
            BlockRates = RoomPrefabs.Select(it => it.Rate).ToArray();

            for (int i = 0; i < MaxRoomNum; i++) {
                SetRoom();
            }

            setBackGround();

            StartCoroutine(bakeStage());
        } else {

        }
    }

    /// <summary>
    /// ルーム作成
    /// </summary>
    /// <returns></returns>
    private StageRoomMng MakeRoom() {

        int index = UtilToolLib.getRateRandom(BlockRates);

        GameObject obj = Instantiate(RoomPrefabs[index].gameObject) as GameObject;
        obj.SetActive(true);
        obj.transform.SetParent(StageBase);

        StageRoomMng mng = obj.GetComponent<StageRoomMng>();
        mng.RoomName = RoomPrefabs[index].name;
        mng.Section = this;

        return mng;
    }

    /// <summary>
    /// ルーム作成
    /// </summary>
    /// <param name="connected_room"></param>
    /// <param name="connect"></param>
    /// <returns></returns>
    public void SetRoom() {

        StageRoomMng mng = MakeRoom();

        Rect map = new Rect(new Vector2Int(), mng.RoomSize);

        if (Rooms.Count > 0) {

            int roop_count = 10;

            for (int i = 0; i < roop_count; i++) {

                StageRoomMng connected_to_room = Rooms.Where(it => it.ConnectionPosis.Count() > 0).OrderBy(_ => System.Guid.NewGuid()).FirstOrDefault();

                int conned_index = Random.Range(0, connected_to_room.ConnectionPosis.Count());
                Transform connect = connected_to_room.ConnectionPosis[conned_index];

                int add_connect_index = Random.Range(0, mng.ConnectionPosis.Count());
                Transform add_connect = mng.ConnectionPosis[add_connect_index];

                Vector3 rotate = add_connect.eulerAngles - connect.eulerAngles;
                rotate.y = 180 - Mathf.RoundToInt(rotate.y);

                //UtilToolLib.rotatePivot(mng.gameObject, add_connect.position, connect.position, rotate, StageBase);

                if (isRandomRotate) {
                    UtilToolLib.rotatePivot(mng.gameObject, add_connect.position, connect.position, rotate, StageBase);
                } else {
                    UtilToolLib.rotatePivot(mng.gameObject, add_connect.position, connect.position, Vector3.zero, StageBase);
                }

                map = getMap(mng);

                if (!Map.Any(it => it.Overlaps(map))) {

                    bool same_room = (connected_to_room.RoomName == mng.RoomName);
                    ConnectMakeHole(mng, add_connect_index, (mng.RoomName == connected_to_room.RoomName));
                    ConnectMakeHole(connected_to_room, conned_index, same_room);
                    int connected_index = Rooms.IndexOf(connected_to_room);
                    connectedMap(map, connected_index, same_room);
                    Map.Add(map);
                    Rooms.Add(mng);
                    mng.name = map.ToString();
                    break;
                }

                if (i >= roop_count - 1) {
                    Destroy(mng.gameObject);
                }
            }
        } else {
            map = getMap(mng);
            Map.Add(map);
            Rooms.Add(mng);
            mng.name = map.ToString();
        }
    }

    public void bakeNav() {
        NavMeshSurface nav = GetComponent<NavMeshSurface>();
        if (nav != null) {
            nav.BuildNavMesh();
        }

        IsBaked = true;
    }

    /// <summary>
    /// 部屋を生成する際に１～フレーム遅れが出るので、Bakeを遅らせる
    /// </summary>
    /// <returns></returns>
    public IEnumerator bakeStage() {
        yield return new WaitForEndOfFrame();
        bakeNav();
    }

    /// <summary>
    /// 部屋接続
    /// </summary>
    /// <param name="room"></param>
    /// <param name="index"></param>
    /// <param name="remove_wall"></param>
    private void ConnectMakeHole(StageRoomMng room, int index, bool remove_wall) {
        if (index >= 0) {
            Destroy(room.GetGate(index)?.gameObject);
            if (remove_wall) {
                var wall = room.GetWall(index);
                if (wall != null) {
                    Destroy(wall.gameObject);
                }
                //柱表示制御
                foreach (var pillar in room.Pillars) {
                    if (pillar.Relation != null) {
                        pillar.Relation.Remove(room.ConnectionPosis[index].gameObject);
                        if (pillar.Relation.Count == 0) {
                            pillar.gameObject.SetActive(false);
                        }
                    }
                }
            }
            room.ConnectionPosis.RemoveAt(index);
        }
    }

    /// <summary>
    /// ミニマップ作成
    /// </summary>
    /// <param name="add_room"></param>
    /// <param name="connected_room"></param>
    private void connectedMap(Rect map, int connected_index, bool same_room) {

        Rect connected_map = Map[connected_index];

        Vector2 posi = new Vector2();
        Vector2 size = new Vector2(MAP_OUTLINE_SIZE * 2, MAP_OUTLINE_SIZE * 2);

        if (Mathf.RoundToInt(map.center.y) == Mathf.RoundToInt(connected_map.center.y)) {
            posi.x = (map.center.x > connected_map.x ? map.x : connected_map.x) - MAP_OUTLINE_SIZE;
            posi.y = map.center.y - Mathf.Min(map.size.y, connected_map.size.y) / 2;
            size.y = Mathf.Min(map.size.y, connected_map.size.y);
        } else {
            posi.y = (map.center.y > connected_map.y ? map.y : connected_map.y) - MAP_OUTLINE_SIZE;
            posi.x = map.center.x - Mathf.Min(map.size.x, connected_map.size.x) / 2;
            size.x = Mathf.Min(map.size.x, connected_map.size.x);
        }

        //MapToRoad.Add(new Vector2Int(ConnectRoad.Count, connected_index));

        //ConnectRoad.Add(new Rect(posi, size));

        ConnectRoad.Add(new Vector2Int(Map.Count, connected_index), new Rect(posi, size));
    }

    /// <summary>
    /// ミニマップ取得
    /// </summary>
    /// <param name="room"></param>
    /// <param name="connected_room"></param>
    /// <returns></returns>
    private Rect getMap(StageRoomMng room) {

        Vector2 posi = new Vector2Int();

        Vector2 size = room.RoomSize;
        Vector2Int rotate = Vector2Int.RoundToInt(room.transform.eulerAngles);
        if (rotate.y == 90 || rotate.y == -90 || rotate.y == 270) {
            size = new Vector2(size.y, size.x);
        }

        //Rectでoverrap判定するためにsizeで調整。Rectは左上基準なのでそれで合わせないとずれる
        posi = new Vector2(room.transform.position.x - size.x / 2, (room.transform.position.z - size.y / 2));

        return new Rect(posi, size);
    }

    /// <summary>
    /// 背景位置調整
    /// </summary>
    private void setBackGround() {

        AreaMin.x = Map.Min(it => it.x);
        AreaMax.x = Map.Max(it => (it.x + it.width));
        AreaMin.z = Map.Min(it => it.y);
        AreaMax.z = Map.Max(it => (it.y + it.height));
        AreaMin.y = Rooms.Min(it => it.transform.position.y);
        AreaMax.y = Rooms.Max(it => it.transform.position.y);

        if ((BackType == BACK_GROUND_TYPE.UP || BackType == BACK_GROUND_TYPE.UP_RIGHT) && BackGround != null) {
            float x = BackType == BACK_GROUND_TYPE.UP ? AreaMin.x + AreaMax.x / 2 : AreaMax.x;
            BackGround.transform.position = new Vector3(x, AreaMin.y, AreaMax.z);
        }

        if (BackType == BACK_GROUND_TYPE.TILE && BackGroundTile != null) {
            StageTileMng.SpreadTileArea(BackGroundTile, new Rect(AreaMin.x, AreaMin.z, AreaMax.x - AreaMin.x, AreaMax.z - AreaMin.z), Map);
        }
    }

}