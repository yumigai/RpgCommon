using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMng : MonoBehaviour {

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

        public void setMaterial( string path )
    {
        if (!string.IsNullOrEmpty(path) && Rend != null)
        {
            Rend.material = Resources.Load<Material>(GameConst.MAP_MATERIAL_PATH + path);
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
