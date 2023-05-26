using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialoguesWindow : EditorWindow {
  private List<DialogueNode> nodes;
  private List<DialogueConnection> connections;

  private GUIStyle nodeStyleNPC, nodeStylePla, selectedNodeStyleNPC, selectedNodeStylePla, inPointStyle, outPointStyle;
  private GUIStyle inspStyle;

  private DialogueConnectionPoint selectedInPoint;
  private DialogueConnectionPoint selectedOutPoint;
  
  [MenuItem("Window/Actor Dialogues")]
  private static void OpenWindow() {
    DialoguesWindow window = GetWindow<DialoguesWindow>();
    window.titleContent = new GUIContent("Dialogues Editor");
  }

  private void ChangeActor() {
    diag = null;
    diags = null;
    if (Selection.activeObject == null) return;
    if (Selection.activeObject is not GameObject go || !go.TryGetComponent(out Dialogues newdiags)) return;
    if (diags != newdiags) {
      diags = newdiags;
      if (nodes != null) nodes.Clear(); else nodes = new();
      if (diags.diags == null) return;
      foreach (var d in diags.diags) {
        nodes.Add(new DialogueNode(d, d.pos, 150, 75, 
          d.IsPlayer ? nodeStylePla : nodeStyleNPC, d.IsPlayer ? selectedNodeStylePla : selectedNodeStyleNPC, 
          inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
      }
      if (connections != null) connections.Clear(); else connections = new();
      foreach (var d in diags.diags) {
        AddNodeConnections(d);
      }
    }
  }

  private static Dialogues diags = null;
  Dialogue diag = null;

  private void OnDisable() {
    Selection.selectionChanged -= ChangeActor;

  }
  private void OnEnable() {
    Selection.selectionChanged += ChangeActor;

    nodeStyleNPC = new GUIStyle();
    nodeStyleNPC.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
    nodeStyleNPC.border = new RectOffset(12, 12, 12, 12);
    nodeStylePla = new GUIStyle();
    nodeStylePla.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
    nodeStylePla.border = new RectOffset(12, 12, 12, 12);

    selectedNodeStyleNPC = new GUIStyle();
    selectedNodeStyleNPC.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
    selectedNodeStyleNPC.border = new RectOffset(12, 12, 12, 12);
    selectedNodeStylePla = new GUIStyle();
    selectedNodeStylePla.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;
    selectedNodeStylePla.border = new RectOffset(12, 12, 12, 12);

    inPointStyle = new GUIStyle();
    inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
    inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
    inPointStyle.border = new RectOffset(4, 4, 12, 12);

    outPointStyle = new GUIStyle();
    outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
    outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
    outPointStyle.border = new RectOffset(4, 4, 12, 12);

    inspStyle = new GUIStyle();
    inspStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
    inspStyle.border = new RectOffset(12, 12, 12, 12);

    ChangeActor();
  }

  private void OnGUI() {
    if (diags == null) {
      GUILayout.Label("Please select an Actor");
      return;
    }
    DrawNodes();
    DrawConnections();
    DrawConnectionLine(Event.current);
    ProcessNodeEvents(Event.current);
    ProcessEvents(Event.current);

    DrawInspector();

    if (GUI.changed) Repaint();
  }

  private void DrawNodes() {
    if (nodes != null) {
      for (int i = 0; i < nodes.Count; i++) {
        nodes[i].Draw();
      }
    }
  }

  const int rh = 22;
  Rect inspRect = new(0, 0, 250, 300);
  Rect inspName = new(12, 8, 200, 16);
  Rect inspMsg = new(12, 8 + 1 * rh, 226, 4 * rh);
  Rect inspCond1 = new(12, 8 + 5 * rh, 176, 16);
  Rect inspCond2 = new(190, 8 + 5 * rh, 12, 16);
  Rect inspCond3 = new(206, 8 + 5 * rh, 32, 16);
  
  Rect inspOOL = new(12, 8 + 6 * rh, 80, 16);
  Rect inspOOV = new(100, 8 + 6 * rh, 100, 16);

  Rect inspAct1 = new(12, 8 + 8 * rh, 226, 16);
  Rect inspQItm1 = new(12, 8 + 9 * rh, 150, 16);
  Rect inspIQty1 = new(160, 8 + 9 * rh, 50, 16);
  Rect inspAct2 = new(12, 8 + 10 * rh, 226, 16);
  Rect inspQItm2 = new(12, 8 + 11 * rh, 150, 16);
  Rect inspIQty2 = new(160, 8 + 11 * rh, 50, 16);



  private void DrawInspector() {
    GUI.Box(inspRect, "", inspStyle);
    GUI.Label(inspName, $"{diags.gameObject.name} ({diags.Count})");

    if (diag == null) {
      GUI.Label(inspMsg, $"Select dialogue");
      return;
    }
    EditorGUI.BeginChangeCheck();
    string message = GUI.TextArea(inspMsg, diag.Message);
    Conditions cond = (Conditions)EditorGUI.EnumPopup(inspCond1, diag.Condition);
    bool res = EditorGUI.Toggle(inspCond2, diag.Result);
    int val = EditorGUI.IntField(inspCond3, diag.Val);

    GUI.Label(inspOOL, $"Only Once?");
    bool onlyOnce = EditorGUI.Toggle(inspOOV, diag.OnlyOnce);


    QuestAction action1 = (QuestAction)EditorGUI.EnumPopup(inspAct1, diag.Action1);
    QuestItem item1 = diag.Item1;
    int numItem1 = diag.Quantity1;
    if (action1 != QuestAction.None) {
      item1 = (QuestItem)EditorGUI.EnumPopup(inspQItm1, diag.Item1);
      numItem1 = EditorGUI.IntField(inspIQty1, diag.Quantity1);
    }
    QuestAction action2 = (QuestAction)EditorGUI.EnumPopup(inspAct2, diag.Action2);
    QuestItem item2 = diag.Item2;
    int numItem2 = diag.Quantity2;
    if (action2 != QuestAction.None) {
      item2 = (QuestItem)EditorGUI.EnumPopup(inspQItm2, diag.Item2);
      numItem2 = EditorGUI.IntField(inspIQty2, diag.Quantity2);
    }


    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(diags, "Updated Dialogue");
      diag.Message = message;
      diag.Condition = cond;
      diag.Result = res;
      diag.Val = val;
      diag.OnlyOnce = onlyOnce;

      diag.Action1 = action1;
      diag.Item1 = item1;
      diag.Quantity1 = numItem1;
      diag.Action2 = action2;
      diag.Item2 = item2;
      diag.Quantity2 = numItem2;
    }

    EditorUtility.SetDirty(diags.gameObject);
  }
  

  private void DrawConnections() {
    if (connections != null) {
      for (int i = 0; i < connections.Count; i++) {
        connections[i].Draw();
      }
    }
  }
  private void DrawConnectionLine(Event e) {
    if (selectedInPoint != null && selectedOutPoint == null) {
      Handles.DrawBezier(
          selectedInPoint.rect.center,
          e.mousePosition,
          selectedInPoint.rect.center + Vector2.left * 50f,
          e.mousePosition - Vector2.left * 50f,
          Color.white,
          null,
          2f
      );

      GUI.changed = true;
    }

    if (selectedOutPoint != null && selectedInPoint == null) {
      Handles.DrawBezier(
          selectedOutPoint.rect.center,
          e.mousePosition,
          selectedOutPoint.rect.center - Vector2.left * 50f,
          e.mousePosition + Vector2.left * 50f,
          Color.white,
          null,
          2f
      );

      GUI.changed = true;
    }
  }

  private void ProcessEvents(Event e) {
    switch (e.type) {
      case EventType.MouseDown:
        if (e.button == 0) {
          ClearConnectionSelection();
        }

        if (e.button == 1) {
          ProcessContextMenu(e.mousePosition);
        }
        break;
      case EventType.MouseDrag:
        if (e.button == 0) {
          OnDrag(e.delta);
        }
        break;
    }
  }

  private void OnDrag(Vector2 delta) {
    if (nodes != null) {
      for (int i = 0; i < nodes.Count; i++) {
        nodes[i].Drag(delta);
      }
    }

    GUI.changed = true;
  }

  private void ProcessNodeEvents(Event e) {
    if (nodes != null) {
      for (int i = nodes.Count - 1; i >= 0; i--) {
        var (guiChanged, seldiag) = nodes[i].ProcessEvents(e);

        if (guiChanged) GUI.changed = true;
        if (seldiag != null) diag = seldiag;
      }
    }
  }

  private void ProcessContextMenu(Vector2 mousePosition) {
    GenericMenu genericMenu = new();
    genericMenu.AddItem(new GUIContent("Add NPC Dialogue"), false, () => OnClickAddNode(mousePosition, true));
    genericMenu.AddItem(new GUIContent("Add Player Dialogue"), false, () => OnClickAddNode(mousePosition, false));
    genericMenu.ShowAsContext();
  }

  private void OnClickAddNode(Vector2 mousePosition, bool npc) {
    if (diags == null) return;
    nodes ??= new List<DialogueNode>();
    Dialogue d = diags.Add(!npc);
    GUIStyle nodeStyle = npc ? nodeStyleNPC : nodeStylePla;
    GUIStyle selectedNodeStyle = npc ? selectedNodeStyleNPC : selectedNodeStylePla;
    nodes.Add(new DialogueNode(d, mousePosition, 150, 75, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
  }

  private void OnClickRemoveNode(DialogueNode node) {
    if (connections != null) {
      List<DialogueConnection> connectionsToRemove = new();
      for (int i = 0; i < connections.Count; i++) {
        if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint) {
          connectionsToRemove.Add(connections[i]);
        }
      }
      for (int i = 0; i < connectionsToRemove.Count; i++) {
        connections.Remove(connectionsToRemove[i]);
      }
    }
    nodes.Remove(node);
    diags.Remove(node.diag);

    // We may need to clear the `next`
    foreach (var n in nodes) {
      if (n.diag.Next == null) continue;
      if (n.diag.Next.Contains(node.diag.ID)) n.diag.Next.Remove(node.diag.ID);
    }
  }

  private void OnClickInPoint(DialogueConnectionPoint inPoint) {
    selectedInPoint = inPoint;

    if (selectedOutPoint != null) {
      if (selectedOutPoint.node != selectedInPoint.node) {
        CreateConnection();
        ClearConnectionSelection();
      }
      else {
        ClearConnectionSelection();
      }
    }
  }

  private void OnClickOutPoint(DialogueConnectionPoint outPoint) {
    selectedOutPoint = outPoint;

    if (selectedInPoint != null) {
      if (selectedOutPoint.node != selectedInPoint.node) {
        CreateConnection();
        ClearConnectionSelection();
      }
      else {
        ClearConnectionSelection();
      }
    }
  }

  private void OnClickRemoveConnection(DialogueConnection connection) {
    connections.Remove(connection);

    var list = connection.outPoint.node.diag.Next;
    if (list == null) return;
    int val = connection.inPoint.node.diag.ID;
    if (list.Contains(val)) list.Remove(val);
  }

  private void CreateConnection() {
    connections ??= new List<DialogueConnection>();

    // Check if we can link
    Dialogue src = selectedOutPoint.node.diag;
    Dialogue dst = selectedInPoint.node.diag;

    if (src.IsPlayer && dst.IsPlayer) return;
//    if (!src.IsPlayer && !dst.IsPlayer) return;
    connections.Add(new DialogueConnection(selectedOutPoint, selectedInPoint, OnClickRemoveConnection, src, dst));
  }

  private void ClearConnectionSelection() {
    selectedInPoint = null;
    selectedOutPoint = null;
  }

  private void AddNodeConnections(Dialogue d) {
    // Find the src DialogueNode;
    DialogueNode srcdn = null;
    foreach (var dn in nodes) {
      if (dn.diag == d) {
        srcdn = dn;
        break;
      }
    }
    if (srcdn == null) {
      Debug.LogError("Cannot find Source DialogueNode for " + d);
      return;
    }
    if (d.Next == null) return;
    List<int> l = new(d.Next);
    if (l == null) return; // This is just for safety, but should never happen
    foreach (int id in l) {
      DialogueNode dstdn = null;
      foreach (var dn in nodes) {
        if (dn.diag.ID == id) {
          dstdn = dn;
          break;
        }
      }
      if (dstdn == null) {
        Debug.LogError("Cannot find Destination DialogueNode for " + d);
        continue;
      }
      connections.Add(new DialogueConnection(srcdn.outPoint, dstdn.inPoint, OnClickRemoveConnection, d, dstdn.diag));
    }
  }
}


public class DialogueNode {
  public Rect rect, NameRect, CondRect;
  public bool isDragged;
  public bool isSelected;
  public DialogueConnectionPoint inPoint;
  public DialogueConnectionPoint outPoint;
  public GUIStyle style;
  public GUIStyle defaultNodeStyle;
  public GUIStyle selectedNodeStyle;
  public Dialogue diag;

  public DialogueNode(Dialogue d, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<DialogueConnectionPoint> OnClickInPoint, Action<DialogueConnectionPoint> OnClickOutPoint, Action<DialogueNode> OnClickRemoveNode) {
    rect = new Rect(position.x, position.y, width, height);
    NameRect = new Rect(position.x + 10, position.y - 10, width - 30, height - 10);
    CondRect = new Rect(position.x + 10, position.y + 20, width - 30, height - 10);
    style = nodeStyle;
    inPoint = new DialogueConnectionPoint(this, DialogueConnectionPointType.In, inPointStyle, OnClickInPoint);
    outPoint = new DialogueConnectionPoint(this, DialogueConnectionPointType.Out, outPointStyle, OnClickOutPoint);
    defaultNodeStyle = nodeStyle;
    selectedNodeStyle = selectedStyle;
    OnRemoveNode = OnClickRemoveNode;
    diag = d;
  }

  public Action<DialogueNode> OnRemoveNode;

  public void Drag(Vector2 delta) {
    rect.position += delta;
    NameRect.position += delta;
    CondRect.position += delta;
    diag.pos = rect.position;
  }

  public void Draw() {
    GUI.Box(rect, "", style);
    GUI.Label(NameRect, diag.ToString());
    GUI.Label(CondRect, diag.GetCondition);

    inPoint?.Draw();
    outPoint?.Draw();
  }

  public (bool, Dialogue) ProcessEvents(Event e) {
    Dialogue ret = null;
    switch (e.type) {
      case EventType.MouseDown:
        if (e.button == 0) {
          if (rect.Contains(e.mousePosition)) {
            isDragged = true;
            GUI.changed = true;
            isSelected = true;
            style = selectedNodeStyle;
            ret = diag;
          }
          else {
            GUI.changed = true;
            isSelected = false;
            style = defaultNodeStyle;
          }
        }

        if (e.button == 1 && isSelected && rect.Contains(e.mousePosition)) {
          ProcessContextMenu();
          e.Use();
        }
        break;

      case EventType.MouseUp:
        isDragged = false;
        break;

      case EventType.MouseDrag:
        if (e.button == 0 && isDragged) {
          Drag(e.delta);
          e.Use();
          return (true, ret);
        }
        break;
    }
    return (false, ret);
  }

  private void ProcessContextMenu() {
    GenericMenu genericMenu = new();
    genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
    genericMenu.ShowAsContext();
  }

  private void OnClickRemoveNode() {
    OnRemoveNode?.Invoke(this);
  }
}


public enum DialogueConnectionPointType { In, Out }

public class DialogueConnectionPoint {
  public Rect rect;

  public DialogueConnectionPointType type;

  public DialogueNode node;

  public GUIStyle style;

  public Action<DialogueConnectionPoint> OnClickConnectionPoint;

  public DialogueConnectionPoint(DialogueNode node, DialogueConnectionPointType type, GUIStyle style, Action<DialogueConnectionPoint> OnClickConnectionPoint) {
    this.node = node;
    this.type = type;
    this.style = style;
    this.OnClickConnectionPoint = OnClickConnectionPoint;
    rect = new Rect(0, 0, 10f, 20f);
  }

  public void Draw() {
    rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

    switch (type) {
      case DialogueConnectionPointType.In:
        rect.x = node.rect.x - rect.width + 8f;
        break;

      case DialogueConnectionPointType.Out:
        rect.x = node.rect.x + node.rect.width - 8f;
        break;
    }

    if (GUI.Button(rect, "", style)) {
      OnClickConnectionPoint?.Invoke(this);
    }
  }
}

public class DialogueConnection {
  public DialogueConnectionPoint inPoint;
  public DialogueConnectionPoint outPoint;
  public Action<DialogueConnection> OnClickRemoveConnection;

  public DialogueConnection(DialogueConnectionPoint fromPoint, DialogueConnectionPoint toPoint, Action<DialogueConnection> clickRemove, Dialogue src, Dialogue dst) {
    outPoint = fromPoint;
    inPoint = toPoint;
    OnClickRemoveConnection = clickRemove;
    src.Next ??= new();
    if (!src.Next.Contains(dst.ID)) src.Next.Add(dst.ID);
  }

  public void Draw() {
    Vector2 ir = inPoint.rect.center;
    Vector2 or = outPoint.rect.center;
    if (or.y < ir.y - 30) ir.y -= 5;
    if (or.y > ir.y + 30) ir.y += 5;
    if (ir.y < or.y - 30) or.y -= 5;
    if (ir.y > or.y + 30) or.y += 5;

    Handles.DrawBezier(
        ir,
        or,
        ir + Vector2.left * 50f,
        or - Vector2.left * 50f,
        Color.white,
        null,
        2f
    );
    GUI.DrawTexture(new Rect(ir.x - 12, ir.y - 5, 16, 10), EditorGUIUtility.Load("HandlerArrow.png") as Texture2D, ScaleMode.StretchToFill);

    Handles.color = Color.red;
    if (Handles.Button((ir + or) * 0.5f, Quaternion.identity, 4, 8, Handles.CircleHandleCap)) {
      OnClickRemoveConnection?.Invoke(this);
    }
  }
}