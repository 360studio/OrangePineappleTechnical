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
        SerializedProperty Degree;
        public void GenerateProperties (SerializedObject so) {
            Points = so.FindProperty("_points");
            Degree = so.FindProperty("_degree");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GenerateProperties (serializedObject);


        }
        public void OnSceneGUI()
        {
            Cover cover = (Cover)target;

            //Direct first
            Vector3[] draw = new Vector3[cover.Points.Length + (cover.Looped ? 1 : 0)];
            float height = cover.transform.position.y;
            for (int i = 0; i < cover.Points.Length; i++) {
                cover.Points[i] = new Vector2d(
                    Handles.FreeMoveHandle (cover.Points[i].ToVector3(height),Quaternion.identity,.5f,Vector3.zero,Handles.SphereCap)
                );
                draw[i] = cover.Points[i].ToVector3(height);
            }
            if (cover.Looped)
                draw[cover.Points.Length] = cover.Points[0].ToVector3(height);
            Handles.DrawPolyLine (draw);

            Handles.FreeMoveHandle (cover.GetDegreePoint ().ToVector3(height),Quaternion.identity,1f,Vector3.zero,Handles.DotCap);

            //SP after
            SerializedObject so = new SerializedObject(target);
            this.GenerateProperties(so);
        }
    }
}