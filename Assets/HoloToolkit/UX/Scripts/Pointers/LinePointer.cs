using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

namespace MRTK.UX
{
    [RequireComponent(typeof(Line))]
    public class LinePointer : NavigationPointer {

        protected override void OnEnable()
        {
            base.OnEnable();

            if (renderers == null || renderers.Length == 0)
                renderers = gameObject.GetComponentsInChildren<MRTK.UX.LineRenderer>();

            if (line == null)
                line = gameObject.GetComponent<Line>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (line != null)
                line.enabled = false;
        }

        protected override void UpdateRays()
        {
            rays[0] = new RayStep(PointerOrigin, PointerOrigin + PointerDirection * FocusManager.Instance.GetPointingExtent(this));
        }

        public override void UpdatePointer() {

            base.UpdatePointer();

            HitResult = NavigationSurfaceResultEnum.None;

            if (InteractionEnabled)
            {
                line.enabled = true;
                line.FirstPoint = PointerOrigin;
                line.LastPoint = PointerOrigin + PointerDirection * FocusManager.Instance.GetPointingExtent(this);

                if (Result.End.Object != null)
                {
                    line.LastPoint = Result.End.Point;
                    // Prime focus is on a valid layer
                    if (((1 << Result.End.Object.layer) & validLayers.value) != 0)
                    {
                        HitResult = NavigationSurfaceResultEnum.Valid;
                        // Check our focuser hit for pointer results
                        INavigationHotSpot hotSpot = null;
                        if (NavigationPointer.CheckForHotSpot(Result.End.Object, out hotSpot) && hotSpot.IsActive)
                        {
                            HitResult = NavigationSurfaceResultEnum.HotSpot;
                            // If we've hit a hotspot, set the end point to the hotspot
                            line.LastPoint = hotSpot.Position;
                        }
                    }
                    else if (((1 << Result.End.Object.layer) & invalidLayers.value) != 0)
                    {
                        // Prime focus is on an invalid layer
                        HitResult = NavigationSurfaceResultEnum.Invalid;
                    }
                    else
                    {
                        // Prime focus has no value at all
                        HitResult = NavigationSurfaceResultEnum.None;
                    }

                    // Set the line color
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        renderers[i].LineColor = GetColor(HitResult);
                    }

                }

                // Set the line color
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].LineColor = GetColor(HitResult);
                }
            }
            else
            {
                line.enabled = false;
            }
        }

        [Header("Line properties")]
        [SerializeField]
        private Line line;
        [SerializeField]
        protected MRTK.UX.LineRenderer[] renderers;

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(LinePointer))]
        public new class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}
