using UnityEngine;
using MRTK.UX;

namespace MRDL.Controllers
{
    [RequireComponent(typeof(Bezeir))]
    [ExecuteInEditMode]
    public class GrabberSpline : MonoBehaviour
    {
        public enum GrabModeEnum
        {
            None,
            Drag,
            Rotate,
            Scale,
        }

        private void Update() {
            if (pointerOrigin == null || grabber == null)
                return;

            if (pointerOrigin.gameObject.activeSelf && grabber.gameObject.activeSelf) {
                bezeir.enabled = true;
                // Set the start and end positions
                float distanceToGrabber = Vector3.Distance(pointerOrigin.position, grabber.position);
                bezeir.FirstPoint = pointerOrigin.position;
                bezeir.LastPoint = grabber.position;
                // Set the mid point positions based on the orientation of the object
                // Start by getting a mid points at 1/4 & 3/4 the distance
                midPoint1 = pointerOrigin.position + (pointerOrigin.forward * distanceToGrabber * 0.25f);
                midPoint2 = Vector3.Lerp(midPoint1, grabber.position, 0.5f);
                // Now bend them away from the target based on the orientation of the pointer
                // TEMP just bend them away from the target a bit so we know it's working
                bezeir.SetPoint(1, midPoint1);
                bezeir.SetPoint(2, midPoint2);

                switch (grabMode) {
                    case GrabModeEnum.None:
                        dragCursor.enabled = false;
                        rotateCursor.enabled = false;
                        scaleCursor.enabled = false;
                        break;

                    case GrabModeEnum.Drag:
                        dragCursor.enabled = true;
                        rotateCursor.enabled = false;
                        scaleCursor.enabled = false;
                        dragCursor.transform.position = grabber.position;
                        dragCursor.transform.rotation = grabber.rotation;
                        break;

                    case GrabModeEnum.Rotate:
                        dragCursor.enabled = false;
                        rotateCursor.enabled = true;
                        scaleCursor.enabled = false;
                        rotateCursor.transform.position = grabber.position;
                        rotateCursor.transform.rotation = grabber.rotation;
                        break;

                    case GrabModeEnum.Scale:
                        dragCursor.enabled = false;
                        rotateCursor.enabled = false;
                        scaleCursor.enabled = true;
                        scaleCursor.transform.position = grabber.position;
                        scaleCursor.transform.rotation = grabber.rotation;
                        break;
                }

            } else {
                bezeir.enabled = false;
            }
        }

        [SerializeField]
        private Bezeir bezeir;

        [Header("Transforms")]
        [SerializeField]
        private Transform grabber; //TODO replace with actual grabber class
        [SerializeField]
        private Transform pointerOrigin;

        [Header ("Cursors")]
        [SerializeField]
        private GrabModeEnum grabMode = GrabModeEnum.Drag;
        [SerializeField]
        private Renderer dragCursor;
        [SerializeField]
        private Renderer rotateCursor;
        [SerializeField]
        private Renderer scaleCursor;

        private Vector3 midPoint1;
        private Vector3 midPoint2;
    }
}