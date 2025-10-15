using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMng : MonoBehaviour {

    public enum TAG {
        FLOOR,
        WALL,
        ROOF,
        GATE,
        GIMMICK_FLOOR
    }

    [SerializeField]
    public TAG Tag;

    [SerializeField]
    public GameObject[] Obstracts;

    [SerializeField]
    public Transform ObstractPosi;

    [SerializeField]
    public int PutPer = 30;

    [SerializeField]
    public bool IsRotateObstract;

    [SerializeField]
    public Renderer Rend;

    void Start()
    {
        putObject();
    }

    public void SetMaterial(Material material) {
        if (Rend != null && material != null) {
            Rend.material = material;
        }
    }

    public void SetMaterial( string path ){
        if (!string.IsNullOrEmpty(path)){
            SetMaterial( Resources.Load<Material>(GameConst.MAP_MATERIAL_PATH + path));
        }
    }

    public void SetMaterial( Material floor, Material wall, Material roof, Material gate, Material gimmick) {
        switch (Tag) {
            case TAG.FLOOR:
                SetMaterial(floor);
                break;
            case TAG.WALL:
                SetMaterial(wall);
                break;
            case TAG.ROOF:
                SetMaterial(roof);
                break;
            case TAG.GATE:
                SetMaterial(gate);
                break;
            case TAG.GIMMICK_FLOOR:
                SetMaterial(gimmick);
                break;
        }
    }

    protected void putObject()
    {
        if (ObstractPosi != null)
        {
            int judge = Random.Range(0, 100);
            if (judge < PutPer)
            {
                if (Obstracts.Length > 0)
                {
                    int rand = Random.Range(0, Obstracts.Length);
                    GameObject obj = Instantiate(Obstracts[rand]) as GameObject;
                    obj.transform.parent = ObstractPosi;
                    obj.transform.localPosition = Obstracts[rand].transform.localPosition;
                    obj.transform.localRotation = Obstracts[rand].transform.localRotation;
                    if (IsRotateObstract)
                    {
                        int rote = Random.Range(0, 4);
                        obj.transform.Rotate(new Vector3());
                    }
                }
            }
        }
    }

}
