using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EffectMng : MonoBehaviour{

	[SerializeField]
    public GameObject EffectPrefab;
	[SerializeField]
    private GameObject[] Effects;
	[SerializeField]
	private bool isAnimeEndToDisactive = true;

    [SerializeField]
    public AudioClip[] StartSe;

    [System.NonSerialized]
	private ParticleSystem[] Particles;

	[System.NonSerialized]
	private Animator[] Animes;

	[System.NonSerialized]
	private Text[] Message;

	private float AnimeLong;

    private int EffectCount = 0;

    /// <summary>
    /// エフェクト生成＆実行
    /// </summary>
    /// <param name="path"></param>
    /// <param name="posi"></param>
    /// <param name="rotate"></param>
    /// <param name="parent"></param>
    /// <param name="se_order"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static EffectMng makeEffectMng( string path, Transform parent) {
        EffectMng mng;

        var tra = parent.Find(path);

        if (tra == null) {
            var prefab = Resources.Load(CmnConst.Path.EFFECTMNG_PREFAB + path) as GameObject;
            var obj = Instantiate(prefab);
            obj.name = path;
            obj.transform.SetParent(parent);
            mng = obj.GetComponent<EffectMng>();
        } else {
            mng = tra.GetComponent<EffectMng>();
        }

        return mng;
    }

    public void Start()
    {
        if (Effects == null || Effects.Length == 0) {
            Effects = new GameObject[1];
        }
        EffectCount = 0;
		Particles = new ParticleSystem[Effects.Length];
		Animes = new Animator[Effects.Length];
		Message = new Text[Effects.Length];
    }

    private void OnDisable() {
        System.Array.ForEach(Effects, it => Destroy(it?.gameObject));
    }

    public static void showEffect(Vector3 posi, Quaternion rotate, GameObject prefab, float time = 1f )
    {
        GameObject eff = Instantiate(prefab) as GameObject;
        eff.transform.localScale = prefab.transform.localScale;
        eff.layer = prefab.layer;
        eff.transform.position = posi;
        eff.transform.localRotation = rotate;
        Destroy(eff, time);
    }

	public GameObject effect( Vector3 posi, Quaternion rotate = new Quaternion(), int se_order = -1, string message = null )
    {
        if (EffectCount >= Effects.Length)
        {
            EffectCount = 0;
        }

		GameObject eff;

		if (Effects [EffectCount] == null) {
			eff = Instantiate (EffectPrefab) as GameObject;
			Effects [EffectCount] = eff;
			Particles [EffectCount] = eff.GetComponentInChildren<ParticleSystem> ();
			Animes [EffectCount] = eff.GetComponentInChildren<Animator> ();
			Message [EffectCount] = eff.GetComponentInChildren<Text> ();
			eff.transform.localScale = EffectPrefab.transform.localScale;
			eff.transform.SetParent(this.transform);
            eff.layer = this.gameObject.layer;
            var childs = eff.GetComponentsInChildren<Transform>();

            for (int i = 0; i < childs.Length; i++) {
                childs[i].gameObject.layer = this.gameObject.layer;
            }

			if (Animes [EffectCount] != null && isAnimeEndToDisactive && EffectCount == 0) {
				AnimeLong = Animes [EffectCount].GetCurrentAnimatorStateInfo (0).length;
			}

		} else {
			eff = Effects [EffectCount];
		}

        eff.transform.position = posi;
       	eff.transform.localRotation = rotate;
        eff.transform.eulerAngles += EffectPrefab.transform.eulerAngles;

        if (Animes [EffectCount] != null) {
//			Effects [EffectCount].SetActive (false);
			Effects [EffectCount].SetActive (true);
			if (isAnimeEndToDisactive) {
				StartCoroutine (animeEndWithDisActive (EffectCount));
			}
//			Animes [EffectCount].SetTrigger (AnimeTrriger);
		}else if (Particles [EffectCount] != null) {
			Particles [EffectCount].Play ();
		}

		if (Message [EffectCount] != null && !string.IsNullOrEmpty (message)) {
			Message [EffectCount].text = message;
		}


		if(StartSe != null && StartSe.Length >= 1 )
        {
			int se_index = se_order;
			if (se_index < 0 || se_index >= StartSe.Length ) {
				se_index = Random.Range (0, StartSe.Length);
			}
			SoundMng.Instance.playSE(StartSe[se_index]);
        }
        EffectCount++;

		return eff;
    }

	private IEnumerator animeEndWithDisActive( int index ){
		yield return new WaitForSeconds (AnimeLong);
		Effects [index].SetActive (false);
	}
}
