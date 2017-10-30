//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;

namespace MRTK.UX
{
    public interface IDistorter
    {
        Vector3 DistortPoint(Vector3 point, float strength);
        Vector3 DistortScale(Vector3 point, float strength);
        int DistortOrder { get; set; }
    }

    public abstract class Distorter : MonoBehaviour, IComparable<Distorter>
    {
        public int CompareTo(Distorter other)
        {
            if (other == null)
                return 0;

            return DistortOrder.CompareTo(other.DistortOrder);
        }

        public abstract Vector3 DistortPoint(Vector3 point, float strength);

        public abstract Vector3 DistortScale(Vector3 point, float strength);

        public int DistortOrder
        {
            get { return distortOrder; }
            set { distortOrder = value; }
        }

        [SerializeField]
        protected int distortOrder = 0;
    }
}