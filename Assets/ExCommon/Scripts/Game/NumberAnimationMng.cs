using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 数字をカウントアップ・カウントダウンさせる
/// </summary>
public class NumberAnimationMng : MonoBehaviour
{
    /// <summary>
    /// カウントアップ単位
    /// </summary>
    private const float COUNTUP_TIME_UNIT = 0.01f;

    [SerializeField]
    public Text NumberTxt;

    [SerializeField]
    public int StartNum;

    [SerializeField]
    public int EndNum;

    [SerializeField,Header("完了までにかかる最大時間")]
    public float Time = 2f;

    [SerializeField]
    public bool StartToAnimation;

    private int NowNum;

    private int addNum;

    private void Awake() {
        if (NumberTxt == null) {
            NumberTxt = GetComponent<Text>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (StartToAnimation) {
            init();
            AnimationStart();
        }
    }

    void init() {
        NowNum = StartNum;
        int sign = (int)Mathf.Sign(EndNum - StartNum);

        addNum = (int)Mathf.Ceil((float)(EndNum - StartNum) / Time * COUNTUP_TIME_UNIT);
        addNum = addNum == 0 ? sign : addNum;
        NumberTxt.text = NowNum.ToString();
    }

    public void AnimationStart() {

        StartCoroutine(AnimationProcess());
    }

    public void AnimationStart( int start, int end ) {
        StartNum = start;
        EndNum = end;
        init();
        AnimationStart();
    }


    /// <summary>
    /// 実体処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator AnimationProcess() {

        int count_num = (int)(Time / COUNTUP_TIME_UNIT)+1;
        for (var i = 0; i < count_num; i++) {
            yield return new WaitForSeconds(COUNTUP_TIME_UNIT);

            if ((addNum > 0 && NowNum < EndNum) || (addNum < 0 && NowNum > EndNum)) {
                NowNum += addNum;
                NowNum = addNum > 0 ? Mathf.Clamp(NowNum, NowNum, EndNum) : Mathf.Clamp(NowNum, EndNum, NowNum);
                NumberTxt.text = NowNum.ToString();
            } else {
                break;
            }
        }
    }


}
