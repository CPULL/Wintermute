using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(RigRenamer))]
public class RigRenamerEditor : Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if (GUILayout.Button("Rename")) {
      Debug.Log("rename");

      string oldName = ((RigRenamer)target).oldName;
      string newName = ((RigRenamer)target).newName;

      foreach (Transform t in ((RigRenamer)target).transform) {
        RenameRecursive(t, oldName, newName);
      }
    }
  }

  void RenameRecursive(Transform tr, string oldName, string newName) {
    tr.name = tr.name.Replace(oldName, newName);
    Debug.Log("Renamed: " + tr.name);
    foreach (Transform t in tr) {
      RenameRecursive(t, oldName, newName);
    }
  }
}
