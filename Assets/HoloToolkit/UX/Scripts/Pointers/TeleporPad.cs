using System.Collections.Generic;
using UnityEngine;

namespace MRTK.UX
{
    public class TeleporPad : MonoBehaviour {
        protected void OnEnable() {
            if (Application.isPlaying && (padMaterials == null || padMaterials.Length == 0)) {
                List<Material> padMaterialsList = new List<Material>();
                Renderer[] renderers = targetTransform.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers) {
                    padMaterialsList.Add(r.material);
                }
                padMaterials = padMaterialsList.ToArray();
            }
        }

        protected void Update() {
            if (pointer == null)
                return;

            if (pointer.IsSelectPressed) {
                /*if (pointer.Hit) {
                    targetTransform.gameObject.SetActive(true);
                    targetTransform.position = pointer.Position;
                    targetTransform.up = pointer.Normal;
                    targetTransform.Rotate(0f, pointer.TargetOrientation.eulerAngles.y, 0f);
                }*/

            } else {
                targetTransform.gameObject.SetActive(false);
            }

            float offset = Mathf.Repeat(colorOffset, 1f);
            if (animateColorOffset) {
                offset = Mathf.Repeat(colorOffset + (Time.unscaledTime * animationSpeed), 1f);
            }
            for (int i = 0; i < padMaterials.Length; i++) {
                padMaterials[i].color = pointer.GetColor(pointer.HitResult).Evaluate(offset);
            }
        }

        [SerializeField]
        private PhysicsPointer pointer;
        [SerializeField]
        private Transform targetTransform;
        [SerializeField]
        [Range(0f,1f)]
        private float colorOffset = 0f;
        [SerializeField]
        private bool animateColorOffset = true;
        [SerializeField]
        private float animationSpeed = 0.5f;

        private Material[] padMaterials;
    }
}