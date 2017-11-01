using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.UX
{
    public class TeleportPad : MonoBehaviour
    {
        public void Update()
        {
            if (pointer.InteractionEnabled)
            {
                switch (pointer.HitResult)
                {
                    case NavigationSurfaceResultEnum.None:
                    default:
                        animator.SetBool("Disabled", true);
                        break;

                    case NavigationSurfaceResultEnum.Invalid:
                        animator.SetBool("Disabled", false);
                        animator.SetBool("Valid", false);
                        break;

                    case NavigationSurfaceResultEnum.HotSpot:
                        animator.SetBool("Disabled", false);
                        animator.SetBool("Valid", true);
                        break;

                    case NavigationSurfaceResultEnum.Valid:
                        animator.SetBool("Disabled", false);
                        animator.SetBool("Valid", true);
                        break;
                }
            }
            else
            {
                animator.SetBool("Disabled", true);
            }

            transform.position = pointer.NavigationTarget;
            transform.up = pointer.NavigatioNormal;
            // Point the arrow towards the target orientation
            arrowTransform.rotation = Quaternion.identity;
            arrowTransform.Rotate (0f, pointer.NavigationOrientation, 0f);
        }

        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform arrowTransform;
        [SerializeField]
        private NavigationPointer pointer;
    }
}