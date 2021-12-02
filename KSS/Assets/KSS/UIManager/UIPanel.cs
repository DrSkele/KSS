using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : UIReciver
{
    private enum PanelEnum { theA, theB, theC }
    public override string[] GetEnumArray()
    {
        return Enum.GetNames(typeof(PanelEnum));
    }
}
