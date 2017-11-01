using HoloToolkit.Unity;
using MRTK.UX;
using UnityEngine;

namespace MRTK.UX
{
    [RequireComponent(typeof(Parabola))]
    [RequireComponent(typeof(DistorterGravity))]
    public class ParabolicPointer : NavigationPointer
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (parabolaMain == null)
                parabolaMain = gameObject.GetComponent<Parabola>();
        }

        public override void UpdatePointer() {

            // Set up our parabola
            Vector3 parabolaTarget = PointerOrigin + (PointerDirection * parabolaDistance) + (Vector3.down * parabolaDropDist);
            parabolaMain.FirstPoint = PointerOrigin;
            parabolaMain.LastPoint = parabolaTarget;

            base.UpdatePointer();
        }
        
        [Header("Parabola settings")]
        [SerializeField]
        private float parabolaDistance = 1f;
        [SerializeField]
        private float parabolaDropDist = 1f;
        [SerializeField]
        private Parabola parabolaMain;
    }
}
