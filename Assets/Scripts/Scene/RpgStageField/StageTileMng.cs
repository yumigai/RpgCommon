using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageTileMng : MonoBehaviour {

    /// <summary>
    /// 上下左右の外周の大きさ
    /// </summary>
    private const int OUT_LINE_SIZE = 2;

    [SerializeField]
    public Vector2Int Size = new Vector2Int(3, 3);

    [SerializeField]
    public GameObject[] ObjectPrefabs;

    [SerializeField]
    public Transform[] ObjectPosis;


    /// <summary>
    /// 指定エリアに敷き詰める
    /// </summary>
    /// <param name="source"></param>
    /// <param name="area"></param>
    /// <param name="map">除外範囲エリア</param>
    public static void SpreadTileArea(StageTileMng source, Rect area, List<Rect> map = null) {

        Vector2Int loop = new Vector2Int( (int)(Mathf.Ceil( area.width / source.Size.x  ) + OUT_LINE_SIZE*2) , (int)(Mathf.Ceil( area.height / source.Size.y )) + OUT_LINE_SIZE*2);
        Transform parent = source.transform.parent;

        for (int i = 0; i < loop.x; i++) {
            for (int j = 0; j < loop.y; j++) {

                Vector2 posi = new Vector2((area.position.x - source.Size.x * OUT_LINE_SIZE) + i * source.Size.x, ( area.position.y - source.Size.y * OUT_LINE_SIZE) + j * source.Size.y);
                Rect tile_area = new Rect( new Vector2( posi.x - source.Size.x / 2, posi.y - source.Size.y / 2 ), source.Size);

                if (map != null && map.Any( it => it.Overlaps(tile_area) ) ) {
                    continue;
                }

                GameObject obj = Instantiate(source.gameObject) as GameObject;
                obj.transform.localScale = source.transform.lossyScale;
                obj.transform.SetParent(parent);
                
                obj.transform.position = new Vector3( posi.x, parent.transform.position.y, posi.y);
                obj.GetComponent<StageTileMng>().putObject();
            }
        }

        Destroy(source.gameObject);

    }

    /// <summary>
    /// オブジェクト配置
    /// </summary>
    private void putObject() {
        if (ObjectPosis.Length > 0 && ObjectPrefabs.Length > 0) {
            for (int i = 0; i < ObjectPosis.Length; i++) {
                int index = Random.Range(0, ObjectPrefabs.Length);
                GameObject obj = Instantiate(ObjectPrefabs[index].gameObject) as GameObject;
                obj.transform.localScale = ObjectPrefabs[index].transform.localScale; //先にスケール設定
                obj.transform.SetParent(ObjectPosis[i]);
                obj.transform.localPosition = ObjectPrefabs[index].transform.localPosition;
            }
        }
    }
}
