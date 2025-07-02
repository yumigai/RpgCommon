using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMast : MulitiUseListMast {

    public string Image;

    public static HelpMast[] List;

    public static void load() {
        List = load<HelpMast>();
    }
}
