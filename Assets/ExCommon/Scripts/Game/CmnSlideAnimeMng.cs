using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CmnSlideAnimeMng : MonoBehaviour {

    public enum END_TO
    {
        NON,
        REPEAT,
        REVERSE,
		DESTROY,
    }

	[SerializeField]
	public RectTransform[] TargetPoints;

    [SerializeField]
    public float Speed;

	[SerializeField]
	public float[] MultiSpeeds;

    [SerializeField]
    public END_TO EndTo;

    [SerializeField]
    public UnityEvent FinishEvent;

    [SerializeField,Header("有効時に開始するか")]
    public bool IsEnableAndMove = true;

    [System.NonSerialized]
    public Vector3[] MovePoints;

    public System.Action Callback;

	private Vector3 InitPosition;
    private int[] MaxMoveCounts;
    private Vector3[] MoveDirect;
    private int MoveCount;
    private RectTransform rectTrans;
    private int NowMoveIndex;


    void Start()
    {
        initSetting();

        moveSetting();

    }

    private void OnEnable() {
        if (IsEnableAndMove) {
            NowMoveIndex = 0;
            if(rectTrans != null && InitPosition != null) {
                rectTrans.position = InitPosition;
            }
        }
    }


    protected void initSetting()
    {
        if (TargetPoints != null && TargetPoints.Length > 0)
        {
            MovePoints = new Vector3[TargetPoints.Length];
            for (int i = 0; i < MovePoints.Length; i++)
            {
                MovePoints[i] = TargetPoints[i].GetComponent<RectTransform>().position;
            }
            InitPosition = MovePoints[0];
        }
        rectTrans = this.GetComponent<RectTransform>();
    }

    protected void moveSetting()
    {
        MoveCount = 0;
        if (MovePoints.Length > 1)
        {
            MaxMoveCounts = new int[MovePoints.Length - 1];
            MoveDirect = new Vector3[MovePoints.Length - 1];

            for (int i = 0; i < MaxMoveCounts.Length; i++)
            {

                float move = Speed;
                if (MultiSpeeds != null && MultiSpeeds.Length > i)
                {
                    move = MultiSpeeds[i];
                }

                float dist = Vector2.Distance(MovePoints[i], MovePoints[i + 1]);
                MoveDirect[i] = (MovePoints[i + 1] - MovePoints[i]) * (move / dist);
                MaxMoveCounts[i] = (int)Mathf.Ceil(dist / move);

            }
        }

        rectTrans.position = InitPosition;
        TargetPoints[0].GetComponent<RectTransform>().position = InitPosition;
    }

    void FixedUpdate()
    {
        if( NowMoveIndex < MoveDirect.Length)
        {
            if (MoveCount >= MaxMoveCounts[NowMoveIndex])
            {
                NowMoveIndex++;
                MoveCount = 0;
            }
            else
            {
                rectTrans.position += MoveDirect[NowMoveIndex];
                MoveCount++;
            }

            if ( NowMoveIndex >= MovePoints.Length - 1)
            {

                if (FinishEvent != null && FinishEvent.GetPersistentEventCount() > 0) {
                    FinishEvent.Invoke();
				}
                switch (EndTo)
                {
                    case END_TO.REPEAT:
                        NowMoveIndex = 0;
						rectTrans.position = InitPosition;
                        break;
                    case END_TO.REVERSE:
                        break;
					case END_TO.DESTROY:
						Destroy (this.gameObject);
						break;
                }

            }

        }

    }

}
