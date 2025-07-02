using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMast {

    public const string IconPath = CmnConst.Path.IMG_PATH + "FoodIcon/";

    //public string Img;
    public float Heal;
    public string Name;

    public FoodMast( float heal, string name)
    {
        //Img = img;
        Heal = heal;
        Name = name;
    }

    public static FoodMast[] List = new FoodMast[]{
        new FoodMast( 0.2f, "雑穀おにぎり" ),
        new FoodMast( 0.2f, "燕麦のお好み焼き" ),
        new FoodMast( 0.2f, "雑穀がゆ" ),
        new FoodMast( 0.3f, "アナジャコのから揚げ" ),
        new FoodMast( 0.2f, "クラゲの和え物" ),
        new FoodMast( 0.2f, "木の根と皮の煮物" ),
        new FoodMast( 0.4f, "ドブ魚の焼き物" ),
        new FoodMast( 0.4f, "ハゲワシ卵のオムレツ" ),
        new FoodMast( 0.4f, "ハダカデバネズミのメンチカツ" ),
        new FoodMast( 0.2f, "苔と野草のサラダ" ),
        new FoodMast( 0.7f, "オオネズミのステーキ" ),
        new FoodMast( 0.5f, "モグラの串焼き" ),
        new FoodMast( 0.7f, "ミュータント牛丼" ),
        new FoodMast( 0.5f, "肉じゃが" ),
        new FoodMast( 1f, "天空のパン" ),

    };

}
