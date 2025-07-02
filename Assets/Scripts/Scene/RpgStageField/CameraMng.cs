using UnityEngine;
using System.Collections;

public class CameraMng : MonoBehaviour {

    public const int RAY_ON_COUNT = 10;

    [SerializeField]
    GameObject Xray;

	[SerializeField]
	string[] TargetLayerMask;

    [System.NonSerialized]
    public Transform LookTarget;

	private int LayerMask;

    public int HideCount{ get; set; }

    private Vector3 BasePosi;

    public static CameraMng Singleton;

    private void Awake() {
        Singleton = this;
    }

    // Use this for initialization
    void Start () {
        
    }

    public void setLookTarget(Transform target) {

        LookTarget = target;

        HideCount = 0;
        LayerMask = UnityEngine.LayerMask.GetMask(TargetLayerMask);
        BasePosi = this.transform.position;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (LookTarget == null) {
            return;
        }

        this.transform.position = LookTarget.position + BasePosi;

        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;

        float maxDistance = 100;

		if (Physics.Raycast(ray, out hit, maxDistance, LayerMask ))
        {
            if (hit.transform == LookTarget || hit.collider.transform.parent.gameObject == LookTarget.gameObject)
            {
                if (Xray.activeSelf)
                {
                    Xray.SetActive(false);
                }
                HideCount = 0;
            }
            else
            {
                if (!Xray.activeSelf)
                {
                    HideCount++;
                    if (HideCount > RAY_ON_COUNT)
                    {
                        Xray.SetActive(true);
                    }
                }
            }
        }
    }
}
