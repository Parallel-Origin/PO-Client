// /************************************************************
// *                                                           *
// *   Mobile Touch Camera                                     *
// *                                                           *
// *   Created 2015 by BitBender Games                         *
// *                                                           *
// *   bitbendergames@gmail.com                                *
// *                                                           *
// ************************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace BitBenderGames {

  [CustomEditor(typeof(MobileTouchCamera))]
  public class MobileTouchCameraEditor : Editor {

    public override void OnInspectorGUI() {

      MobileTouchCamera mobileTouchCamera = (MobileTouchCamera)target;

      DrawPropertyField("cameraAxes");
      DrawPropertyField("camZoomMin");
      DrawPropertyField("camZoomMax");
      DrawPropertyField("camOverzoomMargin");

      #region boundary
      SerializedProperty serializedPropertyBoundaryMin = serializedObject.FindProperty("boundaryMin");
      Vector2 vector2BoundaryMin = serializedPropertyBoundaryMin.vector2Value;

      SerializedProperty serializedPropertyBoundaryMax = serializedObject.FindProperty("boundaryMax");
      Vector2 vector2BoundaryMax = serializedPropertyBoundaryMax.vector2Value;

      const float sizeLabel = 100;
      const float sizeFloatInput = 70;
      EditorGUILayout.LabelField(new GUIContent("Boundary", "These values define the scrolling borders for the camera. The camera will not scroll further than defined here."), EditorStyles.boldLabel);

      EditorGUILayout.BeginHorizontal();
      GUILayout.Label("Top", GUILayout.Width(sizeLabel));
      GUILayout.FlexibleSpace();
      GUILayout.FlexibleSpace();
      vector2BoundaryMax.y = EditorGUILayout.FloatField(vector2BoundaryMax.y, GUILayout.Width(sizeFloatInput));
      GUILayout.FlexibleSpace();
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      GUILayout.Label("Left/Right", GUILayout.Width(sizeLabel));
      GUILayout.FlexibleSpace();
      vector2BoundaryMin.x = EditorGUILayout.FloatField(vector2BoundaryMin.x, GUILayout.Width(sizeFloatInput));
      GUILayout.FlexibleSpace();
      vector2BoundaryMax.x = EditorGUILayout.FloatField(vector2BoundaryMax.x, GUILayout.Width(sizeFloatInput));
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      GUILayout.Label("Bottom", GUILayout.Width(sizeLabel));
      GUILayout.FlexibleSpace();
      GUILayout.FlexibleSpace();
      vector2BoundaryMin.y = EditorGUILayout.FloatField(vector2BoundaryMin.y, GUILayout.Width(sizeFloatInput));
      GUILayout.FlexibleSpace();
      EditorGUILayout.EndHorizontal();

      serializedPropertyBoundaryMin.vector2Value = vector2BoundaryMin;
      serializedPropertyBoundaryMax.vector2Value = vector2BoundaryMax;
      #endregion

      DrawPropertyField("camFollowFactor");
      DrawPropertyField("autoScrollDamp");
      DrawPropertyField("groundLevelOffset");
      GUI.enabled = mobileTouchCamera.GetComponent<Camera>().orthographic == false;
      DrawPropertyField("perspectiveZoomMode");
      GUI.enabled = true;

      DrawPropertyField("OnPickItem");
      DrawPropertyField("OnPickItem2D");
      DrawPropertyField("OnPickItemDoubleClick");
      DrawPropertyField("OnPickItem2DDoubleClick");

      if (GUI.changed) {
        serializedObject.ApplyModifiedProperties();
      }
    }

    private void DrawPropertyField(string fieldName) {
      SerializedProperty serializedProperty = serializedObject.FindProperty(fieldName);
      EditorGUILayout.PropertyField(serializedProperty);
    }
  }
}
