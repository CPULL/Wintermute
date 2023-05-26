using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(TimelineDialogues))]
public class TimelineDialoguesEditor : Editor {
  public SerializedProperty _Dialogues;
  ReorderableList _List;


  private void OnEnable() {
    _Dialogues = serializedObject.FindProperty("Dialogues");
    _List = new(serializedObject, _Dialogues, true, true, true, true) {
      drawElementCallback = DrawListItems,
      drawHeaderCallback = DrawHeader,
      elementHeight = 3.5f * EditorGUIUtility.singleLineHeight
    };
  }

  // Draws the elements on the list
  void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) {
    SerializedProperty element = _Dialogues.GetArrayElementAtIndex(index); // The element in the list

    /*
      NUM time  player  forcesize
      msg
    action item qty
     
     */
    float w = rect.width;
    float w3 = w * .3f - 40;
    float w6 = (w - 50) * .165f;
    float h = EditorGUIUtility.singleLineHeight;
    float hr = EditorGUIUtility.singleLineHeight * 1.1f;
    EditorGUI.LabelField(new Rect(rect.x, rect.y, 40, h), index.ToString());
    EditorGUIUtility.labelWidth = 40;
    EditorGUI.PropertyField(new Rect(rect.x + 40, rect.y, w3, h), element.FindPropertyRelative("Time"));
    EditorGUIUtility.labelWidth = 60;
    EditorGUI.PropertyField(new Rect(rect.x + 60 + w3, rect.y, w3, h), element.FindPropertyRelative("IsPlayer"));
    EditorGUIUtility.labelWidth = 100;
    EditorGUI.PropertyField(new Rect(rect.x + 40 + w3 * 2, rect.y, w3, h), element.FindPropertyRelative("ForceSide"));

    EditorGUIUtility.labelWidth = 60;
    EditorGUI.PropertyField(new Rect(rect.x, rect.y + hr, w, h), element.FindPropertyRelative("Message"));


    EditorGUIUtility.labelWidth = 50;
    EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2 * hr, w6, h), element.FindPropertyRelative("Action1"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUI.PropertyField(new Rect(rect.x + w6 + 10, rect.y + 2 * hr, w6, h), element.FindPropertyRelative("Item1"));
    EditorGUIUtility.labelWidth = 60;
    EditorGUI.PropertyField(new Rect(rect.x + 2 * w6 + 20, rect.y + 2 * hr, w6, h), element.FindPropertyRelative("Quantity1"));

    EditorGUIUtility.labelWidth = 50;
    EditorGUI.PropertyField(new Rect(rect.x + 3 * w6 + 30, rect.y + 2 * hr, w6, h), element.FindPropertyRelative("Action2"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUI.PropertyField(new Rect(rect.x + 4 * w6 + 40, rect.y + 2 * hr, w6, h), element.FindPropertyRelative("Item2"));
    EditorGUIUtility.labelWidth = 60;
    EditorGUI.PropertyField(new Rect(rect.x + 5 * w6 + 50, rect.y + 2 * hr, w6, h), element.FindPropertyRelative("Quantity2"));


  }

  void DrawHeader(Rect rect) {
    EditorGUI.LabelField(rect, "CutScene Dialogues");
  }


  public override void OnInspectorGUI() {
    serializedObject.Update();


    _List.DoLayoutList();

    serializedObject.ApplyModifiedProperties();
  }


}
