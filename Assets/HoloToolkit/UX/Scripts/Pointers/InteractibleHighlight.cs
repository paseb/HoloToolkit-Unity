using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRDL.Interaction
{
    public class InteractibleHighlight : MonoBehaviour
    {
        [Flags]
        public enum MatStyleEnum
        {
            None,
            Highlight,
            Outline,
            Cutout
        }

        public enum FadeStyleEnum
        {
            None,
            Smooth,
        }

        private enum StateEnum
        {
            MaterialsRemoved,
            MaterialsAdded,
        }

        public bool HasFocus {
            get {
                return hasFocus;
            }
            set {
                if (value != hasFocus) {
                    hasFocus = value;
                    Refresh();
                }
            }
        }

        public MatStyleEnum Style {
            set {
                if (matStyle != value) {
                    matStyle = value;
                    Refresh();
                }
            }
        }

        public Renderer[] TargetRenderers {
            set {
                if (targetRenderers != value) {
                    targetRenderers = value;
                    Refresh();
                }
            }
        }

        public string HighlightColorProp = "_Color";

        public string OutlineColorProp = "_Color";

        public void FocusEnter() {
            HasFocus = true;
        }
        
        public void FocusExit() {
            HasFocus = false;
        }

        [SerializeField]
        private Color highlightColor;
        [SerializeField]
        private Color outlineColor;        
        [SerializeField]
        private Renderer[] targetRenderers;
        [SerializeField]
        private Material highlightMat;
        [SerializeField]
        private Material outlineMat;
        [SerializeField]
        private MatStyleEnum matStyle = MatStyleEnum.Highlight;
        [SerializeField]
        private FadeStyleEnum fadeStyle = FadeStyleEnum.None;
        [SerializeField]
        private bool hasFocus = false;

        private StateEnum focusState = StateEnum.MaterialsRemoved;

        private void Refresh() {
            if (hasFocus) {
                AddFocusMats();
            } else {
                RemoveFocusMats();
            }
        }

        private void AddFocusMats() {
            if (focusState == StateEnum.MaterialsAdded)
                return;

            if (materialsBeforeFocus == null) {
                materialsBeforeFocus = new Dictionary<Renderer, List<Material>>();
            }

            for (int i = 0; i < targetRenderers.Length; i++) {
                List<Material> preFocusMaterials = null;
                if (!materialsBeforeFocus.TryGetValue(targetRenderers[i], out preFocusMaterials)) {
                    preFocusMaterials = new List<Material>();
                    materialsBeforeFocus.Add(targetRenderers[i], preFocusMaterials);
                } else {
                    preFocusMaterials.Clear();
                }
                preFocusMaterials.AddRange(targetRenderers[i].sharedMaterials);
            }

            if ((matStyle & MatStyleEnum.Highlight) != 0) {
                AddMatToRenderers(targetRenderers, highlightMat, HighlightColorProp, highlightColor);
            }
            if ((matStyle & MatStyleEnum.Outline) != 0) {
                AddMatToRenderers(targetRenderers, outlineMat, OutlineColorProp, outlineColor);
            }
            focusState = StateEnum.MaterialsAdded;
        }

        private void RemoveFocusMats() {
            if (materialsBeforeFocus == null)
                return;

            foreach (KeyValuePair<Renderer, List<Material>> preFocusMats in materialsBeforeFocus) {
                preFocusMats.Key.sharedMaterials = preFocusMats.Value.ToArray();
            }
            materialsBeforeFocus.Clear();
            focusState = StateEnum.MaterialsRemoved;
        }

        private static Material AddMatToRenderers(Renderer[] renderers, Material mat, string propName, Color color) {
            mat.SetColor(propName, color);
            for (int i = 0; i < renderers.Length; i++) {
                if (renderers[i] != null) {
                    List<Material> currentMaterials = new List<Material>(renderers[i].sharedMaterials);
                    if (!currentMaterials.Contains(mat)) {
                        currentMaterials.Add(mat);
                        renderers[i].sharedMaterials = currentMaterials.ToArray();
                    }
                }
            }
            return mat;
        }

        private static void RemoveMatFromRenderers(Renderer[] renderers, List<Material> mats) {
            for (int i = 0; i < mats.Count; i++) {
                RemoveMatFromRenderers(renderers, mats[i]);
            }
        }

        private static void RemoveMatFromRenderers(Renderer[] renderers, Material mat) {
            if (mat == null) {
                return;
            }

            for (int i = 0; i < renderers.Length; i++) {
                if (renderers[i] != null) {
                    List<Material> currentMaterials = new List<Material>(renderers[i].sharedMaterials);
                    //use the name because it may be instanced
                    for (int j = currentMaterials.Count - 1; j >= 0; j--) {
                        Material currentMaterial = currentMaterials[j];
                        if (currentMaterial != null && currentMaterial.name == mat.name) {
                            currentMaterials.RemoveAt(j);
                        }
                    }
                    currentMaterials.Remove(mat);
                    renderers[i].sharedMaterials = currentMaterials.ToArray();
                }
            }
        }

        private Dictionary<Renderer, List<Material>> materialsBeforeFocus;
        private float mDetectionIntensity = 0f;
        private bool mFadingOutDetection = false;
    }
}