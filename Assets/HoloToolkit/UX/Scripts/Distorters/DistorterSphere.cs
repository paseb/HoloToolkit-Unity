using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.UX
{
    public class DistorterSphere : Distorter
    {
        public Vector3 SphereCenter
        {
            get
            {
                return transform.TransformPoint(sphereCenter);
            }
            set
            {
                sphereCenter = transform.InverseTransformPoint(value);
            }
        }

        [SerializeField]
        private Vector3 sphereCenter;
        [SerializeField]
        private float radius = 2f;

        public override Vector3 DistortPoint(Vector3 point, float strength)
        {
            Vector3 direction = (point - SphereCenter).normalized;
            return Vector3.Lerp(point, SphereCenter + (direction * radius), strength);
        }

        public override Vector3 DistortScale(Vector3 point, float strength)
        {
            return Vector3.one;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(SphereCenter, radius);
        }
    }
}