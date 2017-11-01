using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MRTK.UX
{
    [Serializable]
    public enum NavigationSurfaceResultEnum
    {
        None,
        Valid,
        Invalid,
        HotSpot,
    }

    public abstract class NavigationPointer : PhysicsPointer
    {
        /// The position of the navigation target
        public virtual Vector3 NavigationTarget
        {
            get
            {
                if (HitResult == NavigationSurfaceResultEnum.HotSpot)
                    return targetHotSpot.Position;
                else
                    return Result.End.Point;
            }
        }

        /// The normal of the navigation target
        public virtual Vector3 NavigatioNormal
        {
            get
            {
                if (HitResult == NavigationSurfaceResultEnum.HotSpot)
                    return targetHotSpot.Normal;
                else
                    return Result.End.Normal;
            }
        }

        /// The Y rotation of the target in world-space degrees.
        public virtual float NavigationOrientation
        {
            get
            {
                if (HitResult == NavigationSurfaceResultEnum.HotSpot && targetHotSpot.OverrideTargetOrientation)
                    return targetHotSpot.TargetOrientation;
                else
                    return targetOrientation + (raycastOrigin != null ? raycastOrigin.eulerAngles.y : transform.eulerAngles.y);
            }
            set { targetOrientation = value; }
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

        [SerializeField]
        protected float minValidDot = 0.2f;

        /// The result of our hit
        public NavigationSurfaceResultEnum HitResult { get; protected set; }

        public Gradient GetColor(NavigationSurfaceResultEnum targetResult)
        {
            switch (targetResult)
            {
                case NavigationSurfaceResultEnum.None:
                default:
                    return lineColorNoTarget;

                case NavigationSurfaceResultEnum.Valid:
                    return lineColorValid;

                case NavigationSurfaceResultEnum.Invalid:
                    return lineColorInvalid;

                case NavigationSurfaceResultEnum.HotSpot:
                    return lineColorHotSpot;
            }
        }

        [SerializeField]
        private float targetOrientation = 0f;

        protected INavigationHotSpot targetHotSpot;

        public static bool CheckForHotSpot(GameObject primeFocus, out INavigationHotSpot hotSpot)
        {
            hotSpot = null;

            if (primeFocus == null)
                return false;

            // First check the target directly
            hotSpot = primeFocus.GetComponent(typeof(INavigationHotSpot)) as INavigationHotSpot;
            if (hotSpot == null)
            {
                // Then check the attached rigidbody, just in case
                Collider c = primeFocus.GetComponent<Collider>();
                if (c != null && c.attachedRigidbody != null)
                {
                    hotSpot = c.attachedRigidbody.GetComponent(typeof(INavigationHotSpot)) as INavigationHotSpot;
                }
            }

            return hotSpot != null;
        }
    }
}