//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEditor;
using UnityEngine;

namespace MRTK.UX
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Parabola))]
    public class ParabolaInspector : LineBaseInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            Parabola p = (Parabola)target;

            Vector3 arrowPosition = p.FirstPoint + (Vector3.up * p.Height);
            Handles.color = Handles.yAxisColor;
            Handles.Label(p.FirstPoint + Vector3.up * 0.15f, "Height: " + p.Height + "\n(Drag to change)");
            Handles.DrawDottedLine(p.FirstPoint, arrowPosition, 5f);
            Vector3 newArrowPosition = Handles.FreeMoveHandle(arrowPosition, Quaternion.LookRotation(p.UpDirection.normalized), 0.05f, Vector3.zero, Handles.CircleHandleCap);
            p.Height = Vector3.Distance(p.FirstPoint, newArrowPosition);
            
            Handles.color = Color.cyan;
            p.FirstPoint = Handles.FreeMoveHandle(p.FirstPoint, Quaternion.identity, 0.05f, Vector3.zero, Handles.RectangleHandleCap);
            p.LastPoint = Handles.FreeMoveHandle(p.LastPoint, Quaternion.identity, 0.05f, Vector3.zero, Handles.RectangleHandleCap);
            
            Handles.color = Color.white;
            Vector3 lastPos = p.GetPoint(0f);
            Vector3 currentPos = Vector3.zero;

            for (int i = 1; i < 10; i++)
            {
                float normalizedDistance = (1f / (10 - 1)) * i;
                currentPos = p.GetPoint(normalizedDistance);
                Handles.DrawDottedLine(lastPos, currentPos, 5f);
                lastPos = currentPos;
            }
        }
    }
}
