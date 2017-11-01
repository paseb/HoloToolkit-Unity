﻿using HoloToolkit.Unity;
using MRTK.UX;
using UnityEngine;

namespace MRTK.UX
{
    [RequireComponent(typeof(Parabola))]
    [RequireComponent(typeof(DistorterGravity))]
    public class ParabolicPointer : NavigationPointer
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (parabolaMain == null)
                parabolaMain = gameObject.GetComponent<Parabola>();

            parabolaMainRenderers = parabolaMain.GetComponentsInChildren<MRTK.UX.LineRenderer>();

            distorterGravity = GetComponent<DistorterGravity>();
            parabolaMain.AddDistorter(distorterGravity);
        }

        protected override void UpdateRays()
        {
            if (parabolaMain == null)
                return;

            // Make sure our array will hold
            if (rays == null || rays.Length != lineCastResolution)
                rays = new RayStep[lineCastResolution];

            // Set up our rays
            // Turn off gravity so we get accurate rays
            distorterGravity.enabled = false;

            Vector3 lastPoint = parabolaMain.GetUnclampedPoint(0f);
            Vector3 currentPoint = Vector3.zero;
            for (int i = 1; i < rays.Length; i++)
            {
                float normalizedDistance = (1f / rays.Length) * i;
                currentPoint = parabolaMain.GetUnclampedPoint(normalizedDistance);
                rays[i] = new RayStep(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }

            // Re-enable gravity if we're looking at a hotspot
            distorterGravity.enabled = (HitResult == NavigationSurfaceResultEnum.HotSpot);
        }

        public override void UpdatePointer() {

            // Set up our parabola
            Vector3 parabolaTarget = PointerOrigin + (PointerDirection * parabolaDistance) + (Vector3.down * parabolaDropDist);
            parabolaMain.FirstPoint = PointerOrigin;
            parabolaMain.LastPoint = parabolaTarget;

            // Use the results from the last update to set our HitResult
            float clearWorldLength = 0f;
            HitResult = NavigationSurfaceResultEnum.None;
            distorterGravity.enabled = false;
            targetHotSpot = null;

            if (InteractionEnabled)
            {
                parabolaMain.enabled = true;

                // If we hit something
                if (Result.End.Object != null)
                {
                    // Check if it's in our valid layers
                    if ((Result.End.Object.layer & validLayers) != 0)
                    {
                        // See if it's a hot spot
                        if (NavigationPointer.CheckForHotSpot(Result.End.Object, out targetHotSpot) && targetHotSpot.IsActive)
                        {
                            HitResult = NavigationSurfaceResultEnum.HotSpot;
                            // Turn on gravity, point it at hotspot
                            distorterGravity.WorldCenterOfGravity = targetHotSpot.Position;
                            distorterGravity.enabled = true;
                        }
                        else
                        {
                            // If it's NOT a hotspot, check if the hit normal is too steep 
                            // (Hotspots override dot requirements)
                            if (Vector3.Dot (Result.End.Normal, Vector3.up) < minValidDot)
                            {
                                HitResult = NavigationSurfaceResultEnum.Invalid;
                            }
                            else
                            {
                                HitResult = NavigationSurfaceResultEnum.Valid;
                            }
                        }
                    }
                    else if ((Result.End.Object.layer & invalidLayers) != 0)
                    {
                        HitResult = NavigationSurfaceResultEnum.Invalid;
                    }
                    else
                    {
                        HitResult = NavigationSurfaceResultEnum.None;
                    }

                    // Use the step index to determine the length of the hit
                    for (int i = 0; i <= Result.RayStepIndex; i++)
                    {
                        if (i == Result.RayStepIndex)
                        {
                            Debug.DrawLine(Result.StartPoint + Vector3.up * 0.1f, Result.End.Point + Vector3.up * 0.1f, (HitResult != NavigationSurfaceResultEnum.None) ? Color.yellow : Color.cyan);
                            // Only add the distance between the start point and the hit
                            clearWorldLength += Vector3.Distance(Result.StartPoint, Result.End.Point);
                        }
                        else if (i < Result.RayStepIndex)
                        {
                            // Add the full length of the step to our total distance
                            clearWorldLength += rays[i].length;
                        }
                    }

                    // Clamp the end of the parabola to the result hit's point
                    parabolaMain.LineEndClamp = parabolaMain.GetNormalizedLengthFromWorldLength(clearWorldLength, lineCastResolution);

                }
                else
                {
                    parabolaMain.LineEndClamp = 1f;
                }

                // Set the line color
                for (int i = 0; i < parabolaMainRenderers.Length; i++)
                {
                    parabolaMainRenderers[i].LineColor = GetColor(HitResult);
                }

            } else {
                parabolaMain.enabled = false;
            }
        }
        
        [SerializeField]
        private float parabolaDistance = 1f;
        [SerializeField]
        private float parabolaDropDist = 1f;
        [SerializeField]
        [Range(5,100)]
        private int lineCastResolution = 25;

        [SerializeField]
        private Parabola parabolaMain;
        [SerializeField]
        private MRTK.UX.LineRenderer [] parabolaMainRenderers;
        /*[SerializeField]
        private Parabola parabolaBounce;
        [SerializeField]
        private MRTK.UX.LineRenderer [] parabolaBounceRenderers;*/
        [SerializeField]
        private float totalLineDistance;

        private DistorterGravity distorterGravity;
    }
}
