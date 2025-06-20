using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectControllMng : MonoBehaviour
{
    /// <summary>
    /// 共通エフェクトキー
    /// </summary>
    public enum Key{
        Heal,
        Cure,
    }

    [SerializeField]
    private Camera EffectCamera;

    public IReadOnlyDictionary<string, EffectMng> Effects;

    public static EffectControllMng Singleton;

    void Awake() {
        if (Effects == null || Effects.Count == 0) {
            var chids = this.transform.GetComponentsInChildren<EffectMng>();
            var dic = new Dictionary<string, EffectMng>();
            for (int i = 0; i < chids.Length; i++) {
                dic.Add(chids[i].name, chids[i]);
            }
            Effects = dic;
        }

        Singleton = this;
    }

    public void showEffect( Key k, Vector3 posi ) {
        showEffect(k.ToString(), posi);
    }

    public void showEffect(string s, Vector3 posi) {
        showEffect(EffectCamera, s, posi);
    }

    public void showEffect( Camera cam, Key k, Vector3 posi) {
        showEffect(cam, k.ToString(), posi);
    }

    public void showEffect( Camera cam, string s, Vector3 posi) {
        if (Effects[s] != null && cam != null) {
            var w = UtilToolLib.changeScreenPosi(cam, posi);
            Effects[s].effect(w);
        }
    }

}
