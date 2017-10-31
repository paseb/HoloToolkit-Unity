using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MRTK.UX
{
    public interface IPointerTarget
    {
        void OnPointerTarget(PhysicsPointer source);
    }

    [Serializable]
    public enum PointerSurfaceResultEnum
    {
        None,
        Valid,
        Invalid,
        HotSpot,
    }

    public abstract class PhysicsPointer : MonoBehaviour, IPointingSource
    {
        protected virtual void OnEnable()
        {
            FocusManager.Instance.RegisterPointer(this);
        }

        protected virtual void OnDisable()
        {
            FocusManager.Instance.UnregisterPointer(this);
        }

        #region input calls
        public void SetSelectPressed(bool pressed) {
            isSelectPressed = pressed;
        }
        #endregion

        #region IPointingSource implementation

        public RayStep[] Rays {
            get
            {
                if (lastRayRebuildFrame < Time.frameCount)
                {
                    lastRayRebuildFrame = Time.frameCount;
                    UpdateRays();
                }

                return rays;
            }
        }

        public virtual float? ExtentOverride { get { return null; } }

        public PointerResult Result {
            get
            {
                return pointerResult;
            }
            set
            {
                pointerResult = value;
                OnResultUpdated();
            }
        }

        public LayerMask[] PrioritizedLayerMasksOverride { get { return prioritizedLayerMasksOverride; } }

        [SerializeField]
        protected LayerMask[] prioritizedLayerMasksOverride = new LayerMask[1] { new LayerMask() };

        protected PointerResult pointerResult = new PointerResult();

        protected RayStep[] rays = new RayStep[1] { new RayStep(Vector3.zero, Vector3.forward) };

        #endregion

        /// The world origin of the targeting ray
        public Vector3 TargetOrigin {
            get { return raycastOrigin != null ? raycastOrigin.position : transform.position; }
        }
        /// The forward direction of the targeting ray
        public Vector3 TargetDirection {
            get { return raycastOrigin != null ? raycastOrigin.forward : transform.forward; }
        }
        /// The orientation of the focuser.
        public Quaternion TargetOrientation {
            get { return raycastOrigin != null ? raycastOrigin.rotation : transform.rotation; }
        }
        /// Returns true if the select button for this focuser is held down.
        public bool IsSelectPressed {
            get { return isSelectPressed; }
        }
        /// Return true if the focuser is ready to interact. (For example if the ready gesture is detected on a hololens hand.)
        public bool IsInteractionReady {
            get { return isInteractionReady; }
        }
        /// Return true if this focuser allow for interaction and should be used for the main focus list.
        public bool CanInteract {
            get { return canInteract; }
        }

        [Header("Colors")]
        [SerializeField]
        //[GradientDefault(GradientDefaultAttribute.ColorEnum.Blue, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient lineColorValid;
        [SerializeField]
        //[GradientDefault(GradientDefaultAttribute.ColorEnum.Red, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient lineColorInvalid;
        [SerializeField]
        //[GradientDefault(GradientDefaultAttribute.ColorEnum.Green, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient lineColorHotSpot;
        [SerializeField]
        //[GradientDefault(GradientDefaultAttribute.ColorEnum.Gray, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient lineColorNoTarget;

        [Header("Layers & Tags")]
        [SerializeField]
        [Tooltip("Layers that are considered 'valid'")]
        protected LayerMask validLayers = 1; // Default
        [SerializeField]
        [Tooltip("Layers that are considered 'invalid'")]
        protected LayerMask invalidLayers = 1 << 2; // Ignore raycast

        [Header("Focuser")]
        [SerializeField]
        [Tooltip("Returns true if the select button for this focuser is held down.")]
        protected bool isSelectPressed;
        [SerializeField]
        [Tooltip("Return true if the focuser is ready to interact. (For example if the ready gesture is detected on a hololens hand.)")]
        protected bool isInteractionReady;
        [SerializeField]
        [Tooltip("Return true if this focuser allow for interaction and should be used for the main focus list.")]
        protected bool canInteract;
        [SerializeField]
        private Transform raycastOrigin;

        /// The result of our hit
        public PointerSurfaceResultEnum HitResult { get; protected set; }

        public Gradient GetColor (PointerSurfaceResultEnum targetResult)
        {
            switch (targetResult)
            {
                case PointerSurfaceResultEnum.None:
                default:
                    return lineColorNoTarget;

                case PointerSurfaceResultEnum.Valid:
                    return lineColorValid;

                case PointerSurfaceResultEnum.Invalid:
                    return lineColorInvalid;

                case PointerSurfaceResultEnum.HotSpot:
                    return lineColorHotSpot;
            }
        }
               
        public virtual void UpdatePointer()
        {
            return;
        }

        protected virtual void OnResultUpdated()
        {

        }

        public virtual bool OwnsInput(BaseEventData eventData)
        {
            return false;
        }

        protected abstract void UpdateRays();

        private int lastRayRebuildFrame = 0;

        public static bool CheckForHotSpot(GameObject primeFocus, out NavigationHotSpot hotSpot)
        {
            hotSpot = null;

            if (primeFocus == null)
                return false;

            // First check the target directly
            hotSpot = primeFocus.GetComponent<NavigationHotSpot>();
            if (hotSpot == null) {
                // Then check the attached rigidbody, just in case
                Collider c = primeFocus.GetComponent<Collider>();
                if (c != null && c.attachedRigidbody != null) {
                    hotSpot = c.attachedRigidbody.GetComponent<NavigationHotSpot>();
                }
            }

            return hotSpot != null;
        }
        
        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(PhysicsPointer))]
        public class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}
