using UnityEngine;
using System.Collections;

public class BuffTran
{

    public const float SLIP_POWER = 0.2f; //攻撃に対するスリップダメージの割合（毒など）

    public enum TYPE
    {
        NON,
        CRASH_OUT, //クラッシュ（行動不能・防御力半減・回避不能）
        HP_SLIP, //スリップダメージor回復
        MP_SLIP, //スリップダメージor回復
        HIT, //命中アップorダウン
        SWAY, //回避アップorダウン
        //HIT_SWAY_BUFF, //命中・回避アップorダウン
        ATK, //攻撃アップorダウン
        DEF, //防御アップorダウン 
        MAG, //魔攻撃アップorダウン
        REG, //魔防御アップorダウン
        HIT_ATK, //命中・攻撃アップorダウン
        HIT_SWAY, //命中・回避アップorダウン
        DEF_REG, //防御・抵抗アップorダウン
        POISON, //
        STAN, //スタン（通常攻撃が確率で失敗）
        PANIC, //混乱
        CURSE, //
        ALL
    };

    public enum FIELD_TYPE
    {
        BATTLE,
        DUNGEON,
        DUAL,
        ALL
    };

    public TYPE Type { get; set; }
    public FIELD_TYPE FieldType { get; set; }
    public int Turn { get; set; }
    public float Value { get; set; }

    public BuffTran(TYPE type, FIELD_TYPE field, int turn, float value)
    {
        FieldType = field;
        Type = type;
        Turn = turn;
        Value = value;
    }

    /// <summary>
    /// バフターン経過
    /// </summary>
    /// <param name=“field”></param>
    /// <returns></returns>
    public int passTurn(FIELD_TYPE field)
    {
        if (FieldType == field)
        {
            Turn--;
            return Turn;
        }
        return Turn;
    }

}