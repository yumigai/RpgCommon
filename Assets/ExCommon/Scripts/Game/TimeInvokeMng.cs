using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeInvokeMng : MonoBehaviour
{
    /// <summary>
    /// 指定時間後、アクション
    /// </summary>
    /// <param name="go"></param>
    /// <param name="callback"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static TimeInvokeMng TimerAction( GameObject go, System.Action callback, float time ) {
        var timer = GetTimer(go, callback);
        timer.TimerAction(callback, time);
        return timer;
    }

    public static TimeInvokeMng FrameEndAction(GameObject go, System.Action callback) {
        var timer = GetTimer(go, callback);
        timer.FrameAction(callback);
        return timer;
    }

    protected static TimeInvokeMng GetTimer(GameObject go, System.Action callback) {
        var timer = go.GetComponent<TimeInvokeMng>();
        if (timer == null) {
            timer = go.AddComponent<TimeInvokeMng>();
        }

        return timer;
    }

    /// <summary>
    /// 指定時間後、破棄
    /// </summary>
    /// <param name="go"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static TimeInvokeMng TimerDestroy(GameObject go, float time) {
        return TimerAction(go,()=>{ Destroy(go); },time);
    }

    /// <summary>
    /// 指定時間後、隠す
    /// </summary>
    /// <param name="go"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static TimeInvokeMng TimerHide(GameObject go, float time) {
        return TimerAction(go, () => { go.SetActive(false); }, time);
    }


    public void TimerAction(System.Action callback, float time) {
        StartCoroutine(Invoke(callback, time));
    }

    public void FrameAction(System.Action callback) {
        StartCoroutine(Invoke(callback));
    }

    IEnumerator Invoke(System.Action callback, float time) {
        yield return new WaitForSeconds(time);
        callback();
    }

    IEnumerator Invoke(System.Action callback) {
        yield return new WaitForEndOfFrame();
        callback();
    }
}
