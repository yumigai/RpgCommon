using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSimpleMng : MonoBehaviour {

    [SerializeField]
    public AudioClip[] StartSe;

    [SerializeField]
    public float LiveTime = 1f;

    public static void readyEffect(GameObject prefab) {
        GameObject eff = Instantiate(prefab) as GameObject;
        Destroy(eff);
    }
    public static void showEffect(Vector3 posi, Quaternion rotate, GameObject prefab, float time = 1f)
    {
        if (prefab == null) {
            return;
        }

        GameObject eff = Instantiate(prefab) as GameObject;
        eff.transform.localScale = prefab.transform.localScale;
        eff.layer = prefab.layer;
        eff.transform.position = posi;
        eff.transform.localRotation = rotate;
        EffectSimpleMng mng = eff.GetComponent<EffectSimpleMng>();
        if (mng == null)
        {
            Destroy(eff, time);
        }
    }

    public void Start()
    {
        if (StartSe != null && StartSe.Length >= 1)
        {
            int se_index = Random.Range(0, StartSe.Length);
            SoundMng.Instance.playSE(StartSe[se_index]);
        }
        Destroy(this.gameObject, LiveTime);
    }

}
