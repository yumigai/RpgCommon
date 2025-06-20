using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeExitCntlMng : AnimeParamCntlMng
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        setAnimeParam(animator, Key, ParamKey, ChangeBool);
    }

}
