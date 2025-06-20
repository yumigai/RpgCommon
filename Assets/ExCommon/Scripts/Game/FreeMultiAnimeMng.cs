using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMultiAnimeMng : MonoBehaviour
{
    public enum TYPE {
        ROLE,
        MOVE,
        SCALE,
    }

    public enum END_TO {
        LOOP,
        REVERSE,
        STOP,
    }

    [SerializeField]
    public TYPE Type;

    [SerializeField]
    public END_TO EndTo;

    //[SerializeField]
    //public Vector3 RoleSpeed;

    //[SerializeField]
    //public Vector3 MoveSpeed;

    [SerializeField]
    public Vector3 Speed;

    [SerializeField,Tooltip("サイズの最小")]
    public Vector3 Min;

    [SerializeField, Tooltip("サイズの最大")]
    public Vector3 Max;

    private bool IsStop = false;

    void FixedUpdate() {

        if (IsStop) {
            return;
        }

        Transform t = this.transform;

        switch (Type) {
            case TYPE.ROLE:
                t.Rotate(Speed);
                break;
            case TYPE.MOVE:
                t.position += Speed;
                break;
            case TYPE.SCALE:
                ScaleAnime(t);
                break;
        }

        if (Speed != Vector3.zero) {

        }


    }

    /// <summary>
    /// 拡大縮小アニメ
    /// </summary>
    /// <param name="t"></param>
    protected void ScaleAnime( Transform t ) {

        bool is_end = false;

        t.localScale += Speed;
        Vector3 s = t.localScale;

        if (s.x > Max.x || s.y > Max.y || s.z > Max.z) {
            is_end = true;
            t.localScale = EndTo == END_TO.LOOP ? Min : Max;
        } else if (s.x < Min.x || s.y < Min.y || s.z < Min.z) {
            is_end = true;
            t.localScale = EndTo == END_TO.LOOP ? Max : Min;
        }

        if (is_end) {
            if (EndTo == END_TO.REVERSE) {
                Speed *= -1f;
            } 
            IsStop = EndTo == END_TO.STOP;
        }

    }

}
