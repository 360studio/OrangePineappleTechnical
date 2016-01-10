using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

namespace Lockstep
{
    [CustomEditor(typeof(Cover))]
    public class EditorCover : Editor
    {
        SerializedProperty Points;

        public void GenerateProperties(SerializedObject so)
        {
            //Points = so.FindProperty("_points");
        }

        public override void OnInspectorGUI()
        {
            Cover cover = (Cover)target;
            cover.Generate();

            base.OnInspectorGUI();
            GenerateProperties(serializedObject);

        }
        public void OnSceneGUI()
        {
            Cover cover = (Cover)target;
            Transform transform = cover.gameObject.GetComponent<LSBody>().PositionalTransform;
            Vector3 mainPos = transform.position;
            cover.Generate();
            Handles.color = Color.white;
            //Direct first
            Vector3[] draw = new Vector3[cover.Points.Length + (cover.Looped ? 1 : 0)];
            float height = cover.transform.position.y;
            for (int i = 0; i < cover.Points.Length; i++)
            {
                Vector3 drawPos = cover.Points[i].ToVector3() + mainPos;
                cover.Points [i] = new Vector2d(
                    Handles.FreeMoveHandle(drawPos, Quaternion.identity, .5f, Vector3.zero, Handles.SphereCap)
                - mainPos);
                draw [i] = drawPos;
                Handles.Label(draw [i], "P: " + i);
            }
            if (cover.Looped)
                draw [cover.Points.Length] = cover.Points [0].ToVector3() + mainPos;
            Handles.DrawPolyLine(draw);

            /*
            for (int i = 0; i < cover.EntranceDegrees.Length; i++)
            {
                long degree = cover.EntranceDegrees[i];
                Vector3 drawPos = cover.GetDegreePoint(degree).ToVector3() + mainPos;
                Vector3 newDrawPos = Handles.FreeMoveHandle(drawPos, Quaternion.identity, .5f, Vector3.zero * .1f, Handles.CubeCap);
                if (drawPos.V3SqrDistance(newDrawPos) > .1f) {
                    newDrawPos -= mainPos;
                    Vector2d newPos = new Vector2d(newDrawPos);
                    degree = cover.GetClosestDegree(newPos);
                    degree = (cover.NormalizeDegree(degree));
                    cover.EntranceDegrees[i] = degree;
                    Handles.Label(drawPos, "H: " + i);
                }
            }
            */
            //SP after
            SerializedObject so = new SerializedObject(target);
            this.GenerateProperties(so);
            so.ApplyModifiedProperties();
        }

    }
}