using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JemMng : MonoBehaviour
{

    public const float Speed = 15f;

    public const float CAN_GET_TIME = 1f;

    public const float JEM_DIFFUSION = 2f;
    public const float JEM_SPLASH = 4f;
    public const float JEM_HOP = 5f;

    [SerializeField]
    private SphereCollider MaterialColl;

    [SerializeField]
    public Rigidbody Rigid;

    [SerializeField]
    private GameObject Effect;

    [SerializeField]
    public GameObject Target;

    public System.Action GetCallback;

    private float RestGetTime = CAN_GET_TIME;

    private bool IsCase = false;

    private void Awake() {
        MaterialColl = GetComponent<SphereCollider>();
        Rigid = GetComponent<Rigidbody>();
    }


    //public void OnTriggerEnter(Collider other) {
    //    if (other.gameObject == Target && RestGetTime <= 0f) {
    //        if (IsCase) {
    //                getJem();

    //        }
    //    }
    //}

    private void OnTriggerStay(Collider other) {
        if (IsCase) {
            if (other.gameObject == Target && RestGetTime <= 0f) {
                getJem();
            }
        }
    }


    private void FixedUpdate() {
        
        if (IsCase) {
            RestGetTime -= Time.fixedDeltaTime;
            if (RestGetTime <= 0) {
                this.transform.LookAt(Target.transform);
                Rigid.velocity = this.transform.forward * Speed;
            }
        }
        if (BaseStageFieldSceneMng.Singleton != null 
            && BaseStageFieldSceneMng.Singleton.ButtomLine != null 
            && BaseStageFieldSceneMng.Singleton.ButtomLine.position.y > this.transform.position.y) {
            this.transform.position = RespawnMng.getAirRandom();
        }
    }

    public void injection() {
        if (!IsCase) {
            MaterialColl.isTrigger = true;
            Rigid.useGravity = false;
            Rigid.velocity = new Vector3(0f, JEM_HOP, 0f);
            IsCase = true;
        }
    }
    public void injection( System.Action callback ) {
        GetCallback = callback;
        injection();
    }

    private void getJem() {
        GetCallback();
        EffectSimpleMng.showEffect(Target.transform.position, new Quaternion(), Effect);
        Destroy(this.gameObject);
    }

    public static void jemSplash(JemMng jem, Vector3 posi, int min, int max) {
        if (jem == null) {
            return;
        }
        int num = Random.Range(min, max);

        num = num == 0 ? 1 : num;
        float angle = 360 / num;

        for (int i = 0; i < num; i++) {
            GameObject obj = Instantiate(jem.gameObject) as GameObject;
            obj.transform.position = posi;

            if (num == 1) {
                float rand = Random.Range(0, 360f);
                obj.transform.Rotate(new Vector3(0f, rand, 0f));
            } else {
                obj.transform.Rotate(new Vector3(0f, angle * i, 0f));
                obj.transform.Translate(obj.transform.forward * JEM_DIFFUSION);
            }
            JemMng mng = obj.GetComponent<JemMng>();
            Vector3 splash = obj.transform.forward * JEM_SPLASH + obj.transform.up * JEM_HOP;
            mng.Rigid.velocity = splash;

        }
    }
}
