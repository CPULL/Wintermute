using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Cell))]
public class CellEditor : Editor {
  private SerializedProperty _name, _isInterior, _possibleDirs, _ignoreBorder, _FOV;
  public SerializedProperty _HeightU, _HeightR, _HeightD, _HeightL;
  public SerializedProperty _AngleU, _AngleR, _AngleD, _AngleL;
  public SerializedProperty _DistU, _DistR, _DistD, _DistL;
  private SerializedProperty _ItemsU, _ItemsR, _ItemsD, _ItemsL;
  private SerializedProperty _AreasHidden, _Path, _ExitPath, _InitialDir, _ExitDir;
  ReorderableList listEnterPath, listExitPath;

  private SerializedProperty _StaticPosU, _StaticRotU;
  private SerializedProperty _StaticPosR, _StaticRotR;
  private SerializedProperty _StaticPosD, _StaticRotD;
  private SerializedProperty _StaticPosL, _StaticRotL;


  private void OnEnable() {
    _name = serializedObject.FindProperty("name");
    _isInterior = serializedObject.FindProperty("isInterior");
    _possibleDirs = serializedObject.FindProperty("possibleDirs");
    _ignoreBorder = serializedObject.FindProperty("ignoreBorder");
    _FOV = serializedObject.FindProperty("FOV");
    _HeightU = serializedObject.FindProperty("HeightU");
    _HeightR = serializedObject.FindProperty("HeightR");
    _HeightD = serializedObject.FindProperty("HeightD");
    _HeightL = serializedObject.FindProperty("HeightL");
    _AngleU = serializedObject.FindProperty("AngleU");
    _AngleR = serializedObject.FindProperty("AngleR");
    _AngleD = serializedObject.FindProperty("AngleD");
    _AngleL = serializedObject.FindProperty("AngleL");
    _DistU = serializedObject.FindProperty("DistU");
    _DistR = serializedObject.FindProperty("DistR");
    _DistD = serializedObject.FindProperty("DistD");
    _DistL = serializedObject.FindProperty("DistL");
    _ItemsU = serializedObject.FindProperty("ItemsU");
    _ItemsR = serializedObject.FindProperty("ItemsR");
    _ItemsD = serializedObject.FindProperty("ItemsD");
    _ItemsL = serializedObject.FindProperty("ItemsL");
    _AreasHidden = serializedObject.FindProperty("AreasHidden");
    _InitialDir = serializedObject.FindProperty("InitialDir");
    _ExitDir = serializedObject.FindProperty("ExitDir");
    _Path = serializedObject.FindProperty("Path");
    _ExitPath = serializedObject.FindProperty("ExitPath");
    listEnterPath = new(serializedObject, _Path, true, true, true, true) {
      drawElementCallback = DrawEnterListItems, // Delegate to draw the elements on the list
      drawHeaderCallback = DrawHeaderEnter // Skip this line if you set displayHeader to 'false' in your ReorderableList constructor.
    };
    listExitPath = new(serializedObject, _ExitPath, true, true, true, true) {
      drawElementCallback = DrawExitListItems, // Delegate to draw the elements on the list
      drawHeaderCallback = DrawHeaderExit // Skip this line if you set displayHeader to 'false' in your ReorderableList constructor.
    };

    _StaticPosU = serializedObject.FindProperty("StaticPosU");
    _StaticPosR = serializedObject.FindProperty("StaticPosR");
    _StaticPosD = serializedObject.FindProperty("StaticPosD");
    _StaticPosL = serializedObject.FindProperty("StaticPosL");
    _StaticRotU = serializedObject.FindProperty("StaticRotU");
    _StaticRotR = serializedObject.FindProperty("StaticRotR");
    _StaticRotD = serializedObject.FindProperty("StaticRotD");
    _StaticRotL = serializedObject.FindProperty("StaticRotL");
  }

  // Draws the elements on the list
  void DrawEnterListItems(Rect rect, int index, bool isActive, bool isFocused) {
    SerializedProperty element = listEnterPath.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

    int pos = 0;
    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 30, EditorGUIUtility.singleLineHeight), index.ToString()); pos += 30;
    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 30, EditorGUIUtility.singleLineHeight), "Pos"); pos += 30;
    EditorGUI.PropertyField(
        new Rect(rect.x + pos, rect.y, 200, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("pos"),
        GUIContent.none
    ); pos += 200;


    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 10, EditorGUIUtility.singleLineHeight), " "); pos += 10;
    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 30, EditorGUIUtility.singleLineHeight), "Rot"); pos += 30;
    EditorGUI.PropertyField(
        new Rect(rect.x + pos, rect.y, 200, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("rot"),
        GUIContent.none
    ); pos += 200;

    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 10, EditorGUIUtility.singleLineHeight), " "); pos += 10;
    if (GUI.Button(new Rect(rect.x + pos, rect.y, 60, EditorGUIUtility.singleLineHeight), "Set")) {
      Transform cam = Camera.main.transform.parent;
      element.FindPropertyRelative("pos").vector3Value = cam.position;
      element.FindPropertyRelative("rot").quaternionValue = cam.rotation;
    }; pos += 60;
    if (GUI.Button(new Rect(rect.x + pos, rect.y, 40, EditorGUIUtility.singleLineHeight), "Go")) {
      Camera.main.transform.parent.SetPositionAndRotation(element.FindPropertyRelative("pos").vector3Value, element.FindPropertyRelative("rot").quaternionValue);
      SceneView.lastActiveSceneView.pivot = element.FindPropertyRelative("pos").vector3Value;
      SceneView.lastActiveSceneView.rotation = element.FindPropertyRelative("rot").quaternionValue;
    };
  }
  void DrawExitListItems(Rect rect, int index, bool isActive, bool isFocused) {
    SerializedProperty element = listExitPath.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

    int pos = 0;
    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 30, EditorGUIUtility.singleLineHeight), index.ToString()); pos += 30;
    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 30, EditorGUIUtility.singleLineHeight), "Pos"); pos += 30;
    EditorGUI.PropertyField(
        new Rect(rect.x + pos, rect.y, 200, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("pos"),
        GUIContent.none
    ); pos += 200;


    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 10, EditorGUIUtility.singleLineHeight), " "); pos += 10;
    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 30, EditorGUIUtility.singleLineHeight), "Rot"); pos += 30;
    EditorGUI.PropertyField(
        new Rect(rect.x + pos, rect.y, 200, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("rot"),
        GUIContent.none
    ); pos += 200;

    EditorGUI.LabelField(new Rect(rect.x + pos, rect.y, 10, EditorGUIUtility.singleLineHeight), " "); pos += 10;
    if (GUI.Button(new Rect(rect.x + pos, rect.y, 60, EditorGUIUtility.singleLineHeight), "Set")) {
      Transform cam = Camera.main.transform.parent;
      element.FindPropertyRelative("pos").vector3Value = cam.position;
      element.FindPropertyRelative("rot").quaternionValue = cam.rotation;
    }; pos += 60;
    if (GUI.Button(new Rect(rect.x + pos, rect.y, 40, EditorGUIUtility.singleLineHeight), "Go")) {
      Camera.main.transform.parent.SetPositionAndRotation(element.FindPropertyRelative("pos").vector3Value, element.FindPropertyRelative("rot").quaternionValue);
      SceneView.lastActiveSceneView.pivot = element.FindPropertyRelative("pos").vector3Value;
      SceneView.lastActiveSceneView.rotation = element.FindPropertyRelative("rot").quaternionValue;
    };
  }

  //Draws the header
  void DrawHeaderEnter(Rect rect) {
    EditorGUI.LabelField(rect, "Enter Camera Path");
  }
  void DrawHeaderExit(Rect rect) {
    EditorGUI.LabelField(rect, "Exit Camera Path");
  }

  bool showCellBounds, showPath, showInteriorCamera, showHidden;

  public override void OnInspectorGUI() {
    serializedObject.Update();

    PossibleDir pd = ((Cell)target).possibleDirs;

    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.PropertyField(_name, GUILayout.Width(140));

    EditorGUIUtility.labelWidth = 60;
    EditorGUILayout.PropertyField(_isInterior, GUILayout.Width(80));

    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.PropertyField(_FOV, GUILayout.Width(100));

    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 80;
    EditorGUILayout.PropertyField(_possibleDirs, GUILayout.Width(150));

    EditorGUIUtility.labelWidth = 60;
    EditorGUILayout.PropertyField(_InitialDir, GUILayout.Width(100));

    EditorGUIUtility.labelWidth = 50;
    EditorGUILayout.PropertyField(_ExitDir, GUILayout.Width(90));

    EditorGUIUtility.labelWidth = 80;
    EditorGUILayout.PropertyField(_ignoreBorder, GUILayout.Width(150));
    EditorGUILayout.EndHorizontal();
    EditorGUIUtility.labelWidth = 12;

    #region CellBounds

    showCellBounds = EditorGUILayout.Foldout(showCellBounds, "Cell Bounds");
    if (showCellBounds) {
      if (pd.HasFlag(PossibleDir.D)) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
        EditorGUILayout.PropertyField(_HeightD, new GUIContent("H"), GUILayout.Width(50));
        EditorGUILayout.PropertyField(_AngleD, new GUIContent("A"), GUILayout.Width(50));
        EditorGUILayout.PropertyField(_DistD, new GUIContent("D"), GUILayout.Width(50));
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(" ", GUILayout.Width(180)); // ----------------
        if (GUILayout.Button("Set", GUILayout.Width(40))) Set(Dir.D);
        if (GUILayout.Button("Look ↓", GUILayout.Width(70))) Go(Dir.D);
        GUILayout.Label(" ", GUILayout.Width(170)); // ----------------
        GUILayout.EndHorizontal();
      }

      GUILayout.BeginHorizontal();
      if (pd.HasFlag(PossibleDir.R)) {
        EditorGUILayout.PropertyField(_HeightR, new GUIContent("H"), GUILayout.Width(50));
        EditorGUILayout.PropertyField(_AngleR, new GUIContent("A"), GUILayout.Width(50));
        EditorGUILayout.PropertyField(_DistR, new GUIContent("D"), GUILayout.Width(50));
      }
      else
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
      GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
      if (pd.HasFlag(PossibleDir.L)) {
        EditorGUILayout.PropertyField(_HeightL, new GUIContent("H"), GUILayout.Width(50));
        EditorGUILayout.PropertyField(_AngleL, new GUIContent("A"), GUILayout.Width(50));
        EditorGUILayout.PropertyField(_DistL, new GUIContent("D"), GUILayout.Width(50));
      }
      else
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label(" ", GUILayout.Width(30)); // ----------------
      if (pd.HasFlag(PossibleDir.R)) {
        if (GUILayout.Button("Set", GUILayout.Width(40))) Set(Dir.R);
        if (GUILayout.Button("Look →", GUILayout.Width(70))) Go(Dir.R);
      }
      else
        GUILayout.Label(" ", GUILayout.Width(100)); // ----------------
      GUILayout.Label(" ", GUILayout.Width(190)); // ----------------
      if (pd.HasFlag(PossibleDir.L)) {
        if (GUILayout.Button("Set", GUILayout.Width(40))) Set(Dir.L);
        if (GUILayout.Button("Look ←", GUILayout.Width(70))) Go(Dir.L);
      }
      GUILayout.EndHorizontal();

      if (pd.HasFlag(PossibleDir.U)) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
        EditorGUILayout.PropertyField(_HeightU, new GUIContent("H"), GUILayout.Width(50));
        EditorGUILayout.PropertyField(_AngleU, new GUIContent("A"), GUILayout.Width(50));
        EditorGUILayout.PropertyField(_DistU, new GUIContent("D"), GUILayout.Width(50));
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(" ", GUILayout.Width(180)); // ----------------
        if (GUILayout.Button("Set", GUILayout.Width(40))) Set(Dir.U);
        if (GUILayout.Button("Look ↑", GUILayout.Width(70))) Go(Dir.U);
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
        GUILayout.EndHorizontal();
      }
    }
    #endregion CellBounds

    if (_isInterior.boolValue) {
      showPath = EditorGUILayout.Foldout(showPath, "Enter and Exit paths");
      if (showPath) {
        EditorGUIUtility.labelWidth = 90;
        listEnterPath.DoLayoutList();
        listExitPath.DoLayoutList();
      }

      showInteriorCamera = EditorGUILayout.Foldout(showInteriorCamera, "Interior camera");
      if (showInteriorCamera) {

        EditorGUIUtility.labelWidth = 10;
        GUILayout.BeginHorizontal();
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
        EditorGUILayout.PropertyField(_StaticPosD, new GUIContent("↓ P"), GUILayout.Width(200));
        if (GUILayout.Button("Set", GUILayout.Width(40))) SetS(Dir.D);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
        EditorGUILayout.PropertyField(_StaticRotD, new GUIContent("  R"), GUILayout.Width(200));
        if (GUILayout.Button("Go", GUILayout.Width(40))) GoS(Dir.D);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_StaticPosR, new GUIContent("→ P"), GUILayout.Width(200));
        if (GUILayout.Button("Set", GUILayout.Width(40))) SetS(Dir.R);
        GUILayout.Label(" ", GUILayout.Width(50)); // ----------------
        if (GUILayout.Button("Set", GUILayout.Width(40))) SetS(Dir.L);
        EditorGUILayout.PropertyField(_StaticPosL, new GUIContent("← P"), GUILayout.Width(200));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_StaticRotR, new GUIContent("  R"), GUILayout.Width(200));
        if (GUILayout.Button("Go", GUILayout.Width(40))) GoS(Dir.R);
        GUILayout.Label(" ", GUILayout.Width(50)); // ----------------
        if (GUILayout.Button("Go", GUILayout.Width(40))) GoS(Dir.L);
        EditorGUILayout.PropertyField(_StaticRotL, new GUIContent("  R"), GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
        EditorGUILayout.PropertyField(_StaticPosU, new GUIContent("↑ P"), GUILayout.Width(200));
        if (GUILayout.Button("Set", GUILayout.Width(40))) SetS(Dir.U);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(" ", GUILayout.Width(150)); // ----------------
        EditorGUILayout.PropertyField(_StaticRotU, new GUIContent("  R"), GUILayout.Width(200));
        if (GUILayout.Button("Go", GUILayout.Width(40))) GoS(Dir.U);
        GUILayout.EndHorizontal();
      }
    }


    showHidden = EditorGUILayout.Foldout(showHidden, "Hidden objects");
    if (showHidden) {
      if (pd.HasFlag(PossibleDir.U)) EditorGUILayout.PropertyField(_ItemsU, new GUIContent("Items Not Visible ↑"), null);
      if (pd.HasFlag(PossibleDir.L)) EditorGUILayout.PropertyField(_ItemsR, new GUIContent("Items Not Visible →"), null);
      if (pd.HasFlag(PossibleDir.D)) EditorGUILayout.PropertyField(_ItemsD, new GUIContent("Items Not Visible ↓"), null);
      if (pd.HasFlag(PossibleDir.R)) EditorGUILayout.PropertyField(_ItemsL, new GUIContent("Items Not Visible ←"), null);

      EditorGUILayout.PropertyField(_AreasHidden, new GUIContent("Areas Hidden"), null);

    }
    serializedObject.ApplyModifiedProperties();
  }

  void Go(Dir dir) {
    Cell cell = (Cell)target;
    Transform cam = Camera.main.transform.parent;
    Vector3 pos = cell.transform.position;
    var (height, angleX, angleY, _, fov) = cell.GetVals(dir);
    pos.y = height;
    if (dir == Dir.U) pos.z -= cell.transform.localScale.z * .5f;
    if (dir == Dir.D) pos.z += cell.transform.localScale.z * .5f;
    if (dir == Dir.R) pos.x -= cell.transform.localScale.x * .5f;
    if (dir == Dir.L) pos.x += cell.transform.localScale.x * .5f;

    Vector3 rot = cam.rotation.eulerAngles;
    rot.x = angleX;
    rot.y = angleY;
    cam.SetPositionAndRotation(pos, Quaternion.Euler(rot));
    Camera.main.fieldOfView = fov;
    Camera.main.transform.parent.GetComponent<Camera>().fieldOfView = fov;
  }

  void Set(Dir dir) {
    Transform cam = Camera.main.transform.parent;
    Vector3 pos = cam.position;
    Vector3 rot = cam.rotation.eulerAngles;
    Cell cell = (Cell)target;
    switch (dir) {
      case Dir.U: cell.HeightU = pos.y; break;
      case Dir.L: cell.HeightL = pos.y; break;
      case Dir.D: cell.HeightD = pos.y; break;
      case Dir.R: cell.HeightR = pos.y; break;
    };
    switch (dir) {
      case Dir.U: cell.AngleU = rot.x; break;
      case Dir.L: cell.AngleL = rot.x; break;
      case Dir.D: cell.AngleD = rot.x; break;
      case Dir.R: cell.AngleR = rot.x; break;
    };
  }


  void SetS(Dir dir) {
    Transform cam = Camera.main.transform.parent;
    Vector3 pos = cam.position;
    Quaternion rot = cam.rotation;
    Cell cell = (Cell)target;
    switch (dir) {
      case Dir.U: cell.StaticPosU = pos; break;
      case Dir.L: cell.StaticPosL = pos; break;
      case Dir.D: cell.StaticPosD = pos; break;
      case Dir.R: cell.StaticPosR = pos; break;
    };
    switch (dir) {
      case Dir.U: cell.StaticRotU = rot; break;
      case Dir.L: cell.StaticRotL = rot; break;
      case Dir.D: cell.StaticRotD = rot; break;
      case Dir.R: cell.StaticRotR = rot; break;
    };
  }

  void GoS(Dir dir) {
    Transform cam = Camera.main.transform.parent;
    Cell cell = (Cell)target;
    switch (dir) {
      case Dir.U: cam.position = cell.StaticPosU; break;
      case Dir.L: cam.position = cell.StaticPosL; break;
      case Dir.D: cam.position = cell.StaticPosD; break;
      case Dir.R: cam.position = cell.StaticPosR; break;
    };
    switch (dir) {
      case Dir.U: cam.rotation = cell.StaticRotU; break;
      case Dir.L: cam.rotation = cell.StaticRotL; break;
      case Dir.D: cam.rotation = cell.StaticRotD; break;
      case Dir.R: cam.rotation = cell.StaticRotR; break;
    };
    Camera.main.fieldOfView = cell.FOV;
    Camera.main.transform.parent.GetComponent<Camera>().fieldOfView = cell.FOV;
  }


}
