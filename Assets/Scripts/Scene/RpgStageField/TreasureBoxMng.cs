using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBoxMng : BreakObstractMng {

    [SerializeField]
    public JemMng Jem;

    [SerializeField]
    public int MinJem;

    [SerializeField]
    public int MaxJem;

    override public bool breakObject()
    {
        base.breakObject();

        //if(Jem.Type == JemMng.TYPE.MONEY)
        //{
        //    Jem.DirectValue = StageFieldSceneMng.StageData.TreasureCoin;
        //}

        JemMng.jemSplash(Jem, EffectPosi.transform.position, MinJem, MaxJem );

        return true;
    }
    
}
