using UnityEngine;
using System.Collections;

public class CmnAnimeMng : MonoBehaviour {

    public enum END_AFTER_TYPE
    {
        NON,
        DESTROY,
        DISACTIVE,
    }

//    [SerializeField]
//    public bool InitShow;

    [SerializeField]
    public END_AFTER_TYPE EndTo;

    [SerializeField]
    public float ToDestroyTime;

    [SerializeField]
    public AudioClip StartSe;

    public float TimeCount { get; set; }

    void Start()
    {
        //this.gameObject.SetActive(InitShow);
        show();
    }

    public void show()
    {
        if (StartSe != null)
        {
            SoundMng.Instance.playSE(StartSe);
        }
    }

    void FixedUpdate()
    {
        if (ToDestroyTime > 0f)
        {
            TimeCount += Time.deltaTime;
            if( TimeCount >= ToDestroyTime)
            {
                animeEnd();
            }
        }
        
    }

    public void animeEnd()
    {
        TimeCount = 0f;

        switch ( EndTo)
        {
            case END_AFTER_TYPE.DESTROY:
                Destroy(this.gameObject);
                break;
            case END_AFTER_TYPE.DISACTIVE:
                this.gameObject.SetActive(false);
                break;
        } 
    }

}
