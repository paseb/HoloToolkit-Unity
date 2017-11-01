using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.UX
{
    public class HighlightMaterialFocus : InteractibleHighlight, IPointerSpecificFocusable
    {
        public void OnFocusEnter(PointerSpecificEventData eventData)
        {
            HasFocus = true;
        }

        public void OnFocusExit(PointerSpecificEventData eventData)
        {
            HasFocus = false;
        }
    }
}