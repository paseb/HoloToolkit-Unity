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
    
    public abstract class PhysicsPointer : MonoBehaviour, IPointingSource
    {
        protected virtual void OnEnable()
        {
            FocusManager.Instance.RegisterPointer(this);
        }

        protected virtual void OnDisable()
        {
            if (FocusManager.Instance != null)
            {
                FocusManager.Instance.UnregisterPointer(this);
            }
        }

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

        public virtual bool InteractionEnabled
        {
            get { return interactionEnabled; }
            set { interactionEnabled = value; }
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
        public virtual Vector3 PointerOrigin
        {
            get { return raycastOrigin != null ? raycastOrigin.position : transform.position; }
        }

        /// The forward direction of the targeting ray
        public virtual Vector3 PointerDirection
        {
            get { return raycastOrigin != null ? raycastOrigin.forward : transform.forward; }
        }

        public virtual float PointerOrientation
        {
            get
            {
                return pointerOrientation + (raycastOrigin != null ? raycastOrigin.eulerAngles.y : transform.eulerAngles.y);
            }
            set
            {
                pointerOrientation = value;
            }
        }

        [SerializeField]
        protected float pointerOrientation = 0f;
        [SerializeField]
        private bool interactionEnabled = false;
 
        [Header("Layers & Tags")]
        [SerializeField]
        [Tooltip("Layers that are considered 'valid'")]
        protected LayerMask validLayers = 1; // Default
        [SerializeField]
        [Tooltip("Layers that are considered 'invalid'")]
        protected LayerMask invalidLayers = 1 << 2; // Ignore raycast
        [Tooltip("Source transform for raycast origin - leave null to use default transform")]
        [SerializeField]
        protected Transform raycastOrigin;
              
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
       
        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(PhysicsPointer))]
        public class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}
