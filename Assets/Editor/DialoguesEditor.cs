using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Dialogues))]
public class DialoguesEditor : Editor {
  private SerializedProperty _Head, _Diags;
  ReorderableList list;

  private void OnEnable() {
    _Head = serializedObject.FindProperty("Head");
    _Diags = serializedObject.FindProperty("diags");
    list = new(serializedObject, _Diags, true, true, false, true) {
      drawElementCallback = DrawListItems, // Delegate to draw the elements on the list
      drawHeaderCallback = DrawHeader, // Skip this line if you set displayHeader to 'false' in your ReorderableList constructor.
      onReorderCallbackWithDetails = ReorderItem,
    };
  }


  private void ReorderItem(ReorderableList rolist, int oldIndex, int newIndex) {
    var list = (target as Dialogues).diags;
    var item = list[oldIndex];
    list.RemoveAt(oldIndex);
    list.Insert(newIndex, item);
  }


  // Draws the elements on the list
  void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) {
    SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

    int pos = 0;
    EditorGUI.PropertyField(
        new Rect(rect.x + pos, rect.y, 40, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("ID"),
        GUIContent.none
    );
    pos += 50;

    EditorGUI.PropertyField(
        new Rect(rect.x + pos, rect.y, 40, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("IsPlayer"),
        GUIContent.none
    );
    pos += 30;

    EditorGUI.PropertyField(
        new Rect(rect.x + pos, rect.y, rect.width - pos, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("Message"), GUIContent.none);
    pos += 270;
    serializedObject.ApplyModifiedProperties();
  }

  //Draws the header
  void DrawHeader(Rect rect) {
    EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, 100, EditorGUIUtility.singleLineHeight), "ID");
    EditorGUI.LabelField(new Rect(rect.x + 50, rect.y, 100, EditorGUIUtility.singleLineHeight), "IsPlayer");
    EditorGUI.LabelField(new Rect(rect.x + 150, rect.y, 100, EditorGUIUtility.singleLineHeight), "Message");
  }
  bool showDialogues = true;

  public override void OnInspectorGUI() {
    serializedObject.Update();
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.PropertyField(_Head);
    showDialogues = EditorGUILayout.Foldout(showDialogues, "Dialogues");
    if (showDialogues) {
      EditorGUIUtility.labelWidth = 90;
      list.DoLayoutList();
    }
  }
}

