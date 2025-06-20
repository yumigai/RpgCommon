using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect2dMng : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Effects;

    private int EffectCount = 0;

    public GameObject effect(Vector3 posi )
    {
        if (EffectCount >= Effects.Length)
        {
            EffectCount = 0;
        }

        GameObject eff;

        if (Effects[EffectCount] == null)
        {
            eff = Instantiate(this.gameObject) as GameObject;
            Effects[EffectCount] = eff;
            eff.transform.localScale = this.transform.localScale;
            eff.transform.parent = this.transform;
            eff.layer = this.gameObject.layer;

        }
        else
        {
            eff = Effects[EffectCount];
        }

        eff.transform.position = posi;
        eff.SetActive(true);

        EffectCount++;

        return eff;
    }
}
