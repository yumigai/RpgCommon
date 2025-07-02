using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageRoomMng : MonoBehaviour
{

    /// <summary>
    /// 門のゲームオブジェクトの親
    /// </summary>
    public const string GATE_BASE = "Gate";

    /// <summary>
    /// 壁のゲームオブジェクトの親
    /// </summary>
    public const string WALL_BASE = "Wall";

    [SerializeField]
    public Vector2Int RoomSize = new Vector2Int(3, 3);

    [SerializeField]
    public AudioClip StepSe;

    [SerializeField]
    public int Rate;

    [SerializeField, Header("GateとWallの名前の子オブジェクトが必要")]
    public List<Transform> ConnectionPosis;

    [SerializeField]
    public List<RelationObjectMng> Pillars;

    [SerializeField]
    public Transform[] Respawns;

    [SerializeField]
    public StageObjectSetMng[] ObjectSets;

    [SerializeField]
    public Sprite MapImage;

    [System.NonSerialized]
    public StageAreaMng Section;

    [System.NonSerialized]
    public string RoomName;

    public Transform RewpawnCenter {
        get {
            if (Respawns.Length > 0) {
                return Respawns[0];
            }
            return null;
        }
    }

    public void Start() {
        if (RewpawnCenter != null) {
            putObjectSets();
        }
    }

    /// <summary>
    /// レート差によるステージオブジェクト設置
    /// </summary>
    /// <param name="objes"></param>
    /// <returns></returns>
    //private void putObject( StageObjectMng[] objes, Transform trans, bool random_rotate = false ) {
    //    if (objes != null && objes.Length > 0) {
    //        int[] rates = objes.Select(it => it.Rates).ToArray();
    //        int index = UtilToolLib.getRateRandom(rates);
    //        if (index >= 0) {
    //            GameObject obj = Instantiate(objes[index].gameObject) as GameObject;
    //            obj.transform.position = trans.position;
    //            obj.transform.Rotate(transform.localRotation.eulerAngles);
    //            if (random_rotate) {
    //                float rote = Random.Range(0, 360);
    //                obj.transform.Rotate(new Vector3(0f, rote, 0f));
    //            }
    //            obj.transform.SetParent(Section?.transform);
    //        }
    //    }
    //}
    public Transform GetGate(int index) {
        return ConnectionPosis[index].Find(StageRoomMng.GATE_BASE);
    }

    public Transform GetWall(int index) {
        return ConnectionPosis[index].Find(StageRoomMng.WALL_BASE);
    }

    /// <summary>
    /// オブジェクトセットを配置
    /// </summary>
    private void putObjectSets() {
        if (ObjectSets != null && ObjectSets.Length > 0) {
            int[] rates = ObjectSets.Select(it => it.Rates).ToArray();
            int index = UtilToolLib.getRateRandom(rates);
            if (index >= 0) {
                GameObject obj = Instantiate(ObjectSets[index].gameObject) as GameObject;
                obj.transform.SetParent(this.transform);
                obj.transform.localPosition = ObjectSets[index].transform.localPosition;
                obj.transform.localEulerAngles = Vector3.zero;
                StageObjectSetMng mng = obj.GetComponent<StageObjectSetMng>();

                //var sorted_north = ConnectionPosis.OrderBy(it => (it.localPosition.z)).FirstOrDefault();

                for (int i = 0; i < mng.WallObjects.Length; i++) {
                    Transform parent_wall = ConnectionPosis.Find(it => it.name == mng.WallObjects[i].name);

                    if (parent_wall == null) {
                        Destroy(mng.WallObjects[i].gameObject);
                    } else {
                        mng.WallObjects[i].transform.SetParent(parent_wall);
                    }
                }
            }
        }
    }




}