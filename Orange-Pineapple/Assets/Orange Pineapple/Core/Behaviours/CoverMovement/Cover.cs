using UnityEngine;
using System.Collections;
using System;

namespace Lockstep
{
    public class Cover : Lockstep.EnvironmentObject
    {
        /// <summary>
        /// The points of the cover forming consecutive line segmenets.
        /// </summary>
        [SerializeField]
        private Vector2d[] _points;

        public Vector2d[] Points { get { return _points; } }

        [SerializeField,HideInInspector]
        private long[] _pointDistances = new long[2];

        public long[] PointDistances { get { return _pointDistances; } }

        /// <summary>
        /// Describes the amount between the beginning and end of the cover that units start at when moving to this cover.
        /// </summary>
        [SerializeField, FixedNumber(false, true)]
        private long _degree = FixedMath.Half;

        public long Degree { get { return _degree; } }

        [SerializeField]
        private bool _looped;

        public bool Looped { get { return _looped; } }

        protected override void OnInitialize()
        {
            
        }

        public Vector2d GetDegreePoint()
        {
            return Cover.GetPoint(Degree, Looped, Points, PointDistances);
        }

        public void CalculatePointDistances()
        {
            int pointDistanceSize = Points.Length + (Looped ? 1 : 0);
            if (PointDistances.IsNull())
            {
                _pointDistances = new long[pointDistanceSize];
            } else if (PointDistances.Length != pointDistanceSize)
            {
                Array.Resize <long>(ref _pointDistances, pointDistanceSize);
            }

            Vector2d lastPoint = Points [0];
            for (int i = 1; i < Points.Length; i++)
            {
                Vector2d curPoint = Points [i];
                PointDistances [i] = (curPoint - lastPoint).Magnitude();
                lastPoint = curPoint;
            }
            if (Looped)
            {

            }
        }

        public static Vector2d GetPoint(long degree, bool looped, Vector2d[] points, long[] pointDistances)
        {
            //TODO: Degree guards
            long totalMag = 0;
            for (int i = pointDistances.Length - 1; i >= 0; i--)
                totalMag += pointDistances [i];

            long degreeMag = totalMag.Mul(degree);
            long distanceAcc = 0;
            for (int i = 0;; i++)
            {
                if (i < points.Length)
                {
                    long curDistance = pointDistances [i];
                    long nextDistanceAcc = distanceAcc + curDistance;
                    if (nextDistanceAcc == degreeMag)
                    {
                        return points [i + 1];
                    }
                    if (nextDistanceAcc > degreeMag)
                    {
                        long fraction = (degreeMag - distanceAcc).Div(curDistance);
                        return points [i].Lerped(points [i + 1], fraction);
                    }
                    distanceAcc = nextDistanceAcc;
                }
                else if (looped){
                    long finaldist = pointDistances[i];
                    long fraction = (degreeMag - distanceAcc) / finaldist;
                    return points[i].Lerped (points[0], fraction);
                    break;
                }
                else {
                    break;
                }
            }
            return Vector2d.zero;
        }
    }
}
