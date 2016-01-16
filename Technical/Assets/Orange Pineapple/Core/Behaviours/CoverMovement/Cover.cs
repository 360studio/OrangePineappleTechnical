using UnityEngine;
using System.Collections;
using System;

namespace Lockstep
{
    [RequireComponent(typeof(LSBody))]
    public class Cover : Lockstep.EnvironmentObject
    {
        /// <summary>
        /// The points of the cover forming consecutive line segmenets.
        /// </summary>
        [SerializeField]
        private Vector2d[] _points = new Vector2d[2];

        public Vector2d[] Points { get { return _points; } }

        [SerializeField]
        private bool _looped;
        public bool Looped { get { return _looped && Points.Length > 2; } }

        private long[] _pointDistances;

        public long[] PointDistances { get { return _pointDistances; } }

        private Vector2d[] _axes;

        public Vector2d[] Axes { get { return _axes; } }

        private long[] _cachedPointMags;
        public long[] CachedPointMags {get {return _cachedPointMags;}}

        public ushort ID {get;set;}

        internal uint Version {get; set;}

        public LSBody Body {get; private set;}

        public long TotalLength
        {
            get;
            private set;
        }
        private bool inited = false;
        protected override void OnLateInitialize()
        {
            Body = this.GetComponent<LSBody> ();
            inited = true;
            Generate();

            CoverManager.Assimilate(this);

        }

        Vector2d Offset {
            get {
                if (inited) {
                    return Body.Position;
                }
                else {
                    return Vector2d.zero;
                }
            }
        }

        public void Generate()
        {
            this.CalculateDistancesAndAxes();
        }


        public long GetDegreeSpeed (long nonTimescaledSpeed) {
            return nonTimescaledSpeed.Div(this.TotalLength) / LockstepManager.FrameRate;
        }

        private void CalculateDistancesAndAxes()
        {
            if (Points.Length == 1) {

                return;
            }
            else if (Points.Length <= 0) {
                
                return;
            }
            int connectionCount = Points.Length + (Looped ? 0 : -1);

            EnsureSize (ref _pointDistances, connectionCount);
            EnsureSize (ref _axes, connectionCount);
            EnsureSize (ref _cachedPointMags, connectionCount);
            long totalLength = 0;
            Vector2d lastPoint = Points [0];
            for (int i = 1; i < Points.Length; i++)
            {
                Vector2d curPoint = Points [i];
                Vector2d delta = curPoint - lastPoint;
                long dist;
                delta.Normalize(out dist);
                PointDistances [i - 1] = dist;
                _axes [i - 1] = delta;
                _cachedPointMags[i - 1] = totalLength;

                totalLength += dist;

                lastPoint = curPoint;
            }
            if (Looped)
            {
                Vector2d delta = _points [0] - lastPoint;
                long dist;
                delta.Normalize(out dist);
                PointDistances [_points.Length - 1] = dist;
                _axes [_points.Length - 1] = delta;
                _cachedPointMags[_points.Length - 1] = totalLength;

                totalLength += dist;

            }

            TotalLength = totalLength;
        }
        private void EnsureSize <T> (ref T[] array, int size) {
            if (size < 0) size = 0;
            if (array.IsNull() || array.Length != size) {
                array = new T[size];
            }
        }
        public long NormalizeDegree (long degree) {
            if (Looped) {
                return degree.Mod(FixedMath.One);
            }
            if (degree < 0) degree = 0;
            if (degree > FixedMath.One) degree = FixedMath.One;
            return degree;
        }
        public long GetClosestDegree(Vector2d target)
        {
            if (Points.Length == 1) {
                return 0;
            }
            else if (Points.Length <= 0) {

                return 0;
            }
            target -= Offset;
            long closestDegree = 0;
            long closestDist = long.MaxValue;
            for (int i = 0; i < _axes.Length; i++)
            {
                Vector2d axis = _axes [i];
                Vector2d axisOffset = _points [i];
                long min = 0;
                long max = (_points [i + 1 < _points.Length ? i + 1 : 0] - axisOffset).Dot(axis);
                long projection = (target - axisOffset).Dot(axis);
                if (projection < min)
                    projection = min;
                if (projection > max)
                    projection = max;
                if (projection >= min && projection <= max)
                {
                    Vector2d point = axis * projection + axisOffset;
                    long dist = point.Distance(target);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestDegree = _cachedPointMags[i]+ projection;
                    }
                }
                else {
                }
            }
            closestDegree = closestDegree.Div(TotalLength);
            return closestDegree;
        }
            
        public Vector2d GetDegreePoint(long degree)
        {
            if (Points.Length == 1) {
                return Points[0];
                ;
            }
            else if (Points.Length <= 0) {
                return Vector2d.zero;
                ;
            }
            if (Looped) {
                degree = degree.Mod(FixedMath.One);
            }
            else {
            if (degree < 0) degree = 0;
            if (degree > FixedMath.One) degree = FixedMath.One;
            }
            //TODO: Degree guards

            long degreeMag = TotalLength.Mul(degree);
            long distanceAcc = 0;
            for (int i = 0; i < PointDistances.Length; i++)
            {

                long curDistance = PointDistances [i];
                long nextDistanceAcc = distanceAcc + curDistance;
                if (nextDistanceAcc >= degreeMag)
                {
                    long fraction = (degreeMag - distanceAcc).Div(curDistance);
                    long index1 = i;
                    long index2 = i + 1;
                    if (index2 >= Points.Length)
                        index2 = 0;
                    Vector2d ret = Points [index1].Lerped(Points [index2], fraction);
                    return ret + Offset;
                }
                distanceAcc = nextDistanceAcc;

            }
            throw new System.Exception();
        }
    }
}
