using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace MRTK.UX
{
    public interface INavigationHotSpot
    {
        Vector3 Position { get; }
        Vector3 Normal { get; }
        bool IsActive { get; }
        bool OverrideTargetOrientation { get; }
        float TargetOrientation { get; }
    }

    [RequireComponent(typeof(InteractibleHighlight))]
    public class NavigationHotSpot : MonoBehaviour, IPointerSpecificFocusable, ISelectHandler, INavigationHotSpot
    {
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        public Vector3 Normal
        {
            get
            {
                return transform.up;
            }
        }

        public bool IsActive
        {
            get
            {
                return isActiveAndEnabled;
            }
        }

        public bool OverrideTargetOrientation
        {
            get
            {
                return overrideOrientation;
            }
        }

        public float TargetOrientation
        {
            get
            {
                return transform.eulerAngles.y;
            }
        }

        public void OnEnable()
        {
            GetComponent<InteractibleHighlight>().enabled = true; ;
        }

        public void OnDisable()
        {
            GetComponent<InteractibleHighlight>().enabled = false;
        }

        public void OnFocusEnter(PointerSpecificEventData eventData)
        {
            if (!IsActive)
                return;

            GetComponent<InteractibleHighlight>().HasFocus = true;
        }

        public void OnFocusExit(PointerSpecificEventData eventData)
        {
            if (!IsActive)
                return;

            GetComponent<InteractibleHighlight>().HasFocus = false;
        }

        public void OnSelectPressedAmountChanged(SelectPressedEventData eventData)
        {

        }

        [SerializeField]
        private bool overrideOrientation = false;

        public void OnDrawGizmos()
        {
            Gizmos.color = IsActive ? Color.green : Color.red;
            Gizmos.DrawLine(Position + (Vector3.up * 0.1f), Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f));
            Gizmos.DrawSphere(Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f), 0.01f);
        }
    }
}