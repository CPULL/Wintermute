using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
  public Camera cam;
  public Camera camView;
  public static Dir ViewDir { get; private set; }
  static Dir viewDirToSet;

  public Transform Player, PlayerHead;
  public Player player;
  public Animator anim;
  public Cell cell;
  public Balloon balloon, thinking;

  public Toggle DialogueToggle;
  public Toggle InventoryToggle;

  public Image HeartBeat;

  void Start() {
    walkPosition = Player.position;
    ViewDir = Dir.L;
    cell.HideBack();
  }

  void Update() {
    HeartBeat.material.SetFloat("_Rate", Quest.HeartRate);
    HeartBeat.material.SetFloat("_Peak", Quest.Vitals * .76f + .24f);

    if (!overUI && !interacting) Clicks();

    TimeManagement();
    Movement();

    if (ViewDir != viewDirToSet) {
      ViewDir = viewDirToSet;
      Direction.sprite = ViewDir switch {
        Dir.U => DirectionSprites[0],
        Dir.R => DirectionSprites[1],
        Dir.D => DirectionSprites[2],
        Dir.L => DirectionSprites[3],
        _ => DirectionSprites[0],
      };
      cameraFollow.SetCamera(cell);
    }


    if (thinking.gameObject.activeSelf) {
      float change = Input.mouseScrollDelta.y;
      if (change > 0) ChangeQuestion(true);
      else if (change < 0) ChangeQuestion(false);
    }


  }


  public Sun sun;
  int day = 0;
  float dayTime = 0.3819444444444444f; // 9:10
  const float SecsPerDay = 1 / 120f;
  int hour = -1, mins = -1;
  void TimeManagement() {
    if (!Quest.Paused) {
      dayTime += Time.deltaTime * SecsPerDay;
      if (dayTime > 1) dayTime -= 1;
      sun.UpdateDayTime(dayTime);
      int m = (int)(dayTime * 1440);
      int h = m / 60;
      m %= 60;
      m /= 10;
      if (h != hour && h == 0) {
        day++;
        Quest.Day = Quest.Day.Add(System.TimeSpan.FromDays(1));
      }
      if (h != hour || m != mins) {
        hour = h;
        mins = m;
        DayTime.text = $"{Quest.Day:yy/MM/dd}   {hour:d2}:{(mins * 10):d2}";
      }
    }
  }


  public Material PixelArtVFX;
  public LayerMask GroundMask;
  public LayerMask ClickMask;

  public Transform dbgsp; // FIXME

  Vector3 walkPosition;


  public Transform[] Cubes;
  bool walking = false, pathNotYetAvailable = true;
  static Interactable lastOver = null;

  void Clicks() {
    Ray ray = camView.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit, 50, ClickMask)) {
      int layer = hit.collider.gameObject.layer;
      if (layer == 8 /* Interact */) {
        if (hit.collider.TryGetComponent(out Interactable interact)) {
          lastOver = interact;
          interact.GoOver(true);
        }
      }
      else if (lastOver != null) {
        lastOver.GoOver(false);
        lastOver = null;
      }

      if (Input.GetMouseButtonDown(0)) {
        if (layer == 11 /* Wall blocks */) {
          return;
        }
        else if (layer == 31 /* Ground */ && conversation == null) {
          StartWalk(hit.point, false);
        }
        else if (layer == 6 /* Actor */ && conversation == null) {
          if (hit.collider.TryGetComponent(out Dialogues diags)) {
            if (DialogueToggle.isOn) ShowQuestions(diags);
            else ShowActorDialogue(diags.Head, diags);
          }
        }
        else if (layer == 7 /* Sign */ && conversation == null && hit.collider.TryGetComponent(out Sign sign)) {
          balloon.Show(sign.Message);
        }
        else if (layer == 8 /* Interact */) {
          if (hit.collider.TryGetComponent(out PANTerminal pan)) {
            interacting = pan.Interact();
            StartWalk(hit.point, true);
          }
          else if (hit.collider.TryGetComponent(out Door door)) {
            if (door.Open()) StartWalk(hit.point, true);
            if (door.ClickAction1 != QuestAction.None) Quest.PerformAction(door);
          }
          else if (hit.collider.TryGetComponent(out Form form)) {
            form.Show();
          }
          else if (hit.collider.TryGetComponent(out Interactable inter)) {
            StartWalk(hit.point, true);
            if (inter.ClickAction1 != QuestAction.None) Quest.PerformAction(inter);
          }
        }
        //        else Debug.Log("unhandled hit: " + hit.collider.gameObject.name);
      }
    }
  }
  void Movement() {
    Dbg.text = $"{cell.name}";
    if (cutScenePlaying) return;

    // Player
    if (walking) {
      if (pathNotYetAvailable) {
        if (player.agent.hasPath) pathNotYetAvailable = false;
      }
      else {
        float dist = (Player.position - walkPosition).magnitude;
        if (dist > .05f && player.agent.hasPath) {
          if (dist < .2f) anim.SetInteger("Status", 0);
          float speed = Mathf.Clamp(dist, 0.75f, 1f);
          anim.speed = speed;
          if (
            Physics.Raycast(Player.position + Vector3.up, -Vector3.up, out RaycastHit hit, 10, GroundMask) &&
            hit.collider.TryGetComponent(out Cell c) &&
            c != cell) {
            cell.ShowAll();
            if (!c.isInterior && cell.isInterior) ViewDir = cell.ExitDir;
            cell = c;
            ViewDir = cell.SetDir(ViewDir, Player.forward);
            cell.HideBack();
            cameraFollow.SetCell(cell);
          }
        }
        else {
          anim.speed = 1;
          walking = false;
          if (startingCutScene) PostStartCutScene();
          else anim.SetInteger("Status", 0);
        }
      }
    }
  }


  void StartWalk(Vector3 point, bool resetHeight) {
    walkPosition = point;
    if (resetHeight) walkPosition.y = 0;
    dbgsp.position = walkPosition;
    player.agent.SetDestination(walkPosition);
    pathNotYetAvailable = true;
    walking = true;
    anim.SetBool("Left", Random.Range(0, 2) == 0);
    anim.SetInteger("Status", 1);
  }

  public bool overUI = false;
  static bool interacting = false;

  public void OverUI(bool over) {
    overUI = over;
  }

  public void TurnCamera(bool clockwise) {
    viewDirToSet = cell.Rotate(ViewDir, clockwise);
  }
  internal static void SetDir(Dir dir) {
    viewDirToSet = dir;
  }


  Dialogue lastDiag = null;
  private void ShowActorDialogue(Transform pos, Dialogues diags) {
    Dialogue diag = diags.GetDialogue(lastDiag);
    if (diag != null) balloon.Show(diag.Message, pos);
    lastDiag = diag;
  }

  int currentPlayerDialogue = -1;
  List<Dialogue> playerDiags = null;
  Dialogues conversation;
  Dialogue topic;
  void ShowQuestions(Dialogues diags) {
    playerDiags = diags.GetPlayerDialogues();
    if ((playerDiags?.Count ?? 0) > 0) {
      topic = playerDiags[0];
      thinking.Show(topic.Message);
      currentPlayerDialogue = 0;
      conversation = diags;
      balloon.Hide();
    }
    else {
      balloon.Show("I have nothing to say");
      balloon.Hide();
    }
  }

  public void ChangeQuestion(bool next) {
    if (playerDiags == null) return;
    if (next) {
      currentPlayerDialogue++;
      if (currentPlayerDialogue >= playerDiags.Count) currentPlayerDialogue = 0;
    }
    else {
      currentPlayerDialogue--;
      if (currentPlayerDialogue < 0) currentPlayerDialogue = playerDiags.Count - 1;
    }
    topic = playerDiags[currentPlayerDialogue];
    thinking.Show(topic.Message);
    balloon.Hide();
  }

  public void SayQuestion() {
    if (topic != null && (topic.Action1 != QuestAction.None || topic.Action2 != QuestAction.None)) {
      Quest.PerformAction(topic);
    }

    if (thinking.gameObject.activeSelf) {
      thinking.Hide();
      balloon.Show(topic.Message);
    }
    else if (conversation != null) {
      if (topic.IsPlayer) {
        topic = conversation.GetNextDialogue(topic);
        if (topic != null) {
          balloon.Show(topic.Message, conversation.Head);
        }
        else {
          conversation = null;
          balloon.Hide();
          thinking.Hide();
          DialogueToggle.SetIsOnWithoutNotify(false);
        }
      }
      else {
        var l = conversation.GetNextDialogues(topic);
        if (l == null || l.Count == 0) {
          conversation = null;
          balloon.Hide();
          thinking.Hide();
          DialogueToggle.SetIsOnWithoutNotify(false);
        }
        else if (l[0].IsPlayer) {
          currentPlayerDialogue = 0;
          topic = l[0];
          playerDiags = l;
          thinking.Show(topic.Message);
          balloon.Hide();
        }
        else {
          topic = l[0];
          balloon.Show(topic.Message, conversation.Head);
        }
      }
    }

    else if (currentCutScene != null) {
      for (int i = 0; i < currentCutScene.diags.Dialogues.Length; i++) {
        if (currentCutScene.diags.Dialogues[i].Time > currentCutScene.timeline.time) {
          currentCutScene.timeline.time = currentCutScene.diags.Dialogues[i].Time - .25f;
          break;
        }
      }
    }
    else {
      balloon.Hide();
    }
  }

  public void StartDialogue(Dialogues diags, int id) {
    topic = diags.GetTopic(id);
    if (topic == null) return;
    conversation = diags;
    if (topic.IsPlayer) balloon.Show(topic.Message);
    else balloon.Show(topic.Message, diags.Head);
  }



  public void CloseDialogues() {
    thinking.Hide();
    topic = null;
    playerDiags = null;
    conversation = null;
  }

  internal static void StopInteracting() {
    interacting = false;
    if (lastOver != null) {
      lastOver.GoOver(false);
      lastOver = null;
    }
  }


  public void TimelineMessage(int msg) {
    topic = currentCutScene.diags.Dialogues[msg];
    if (topic.IsPlayer) balloon.Show(topic.Message);
    else balloon.Show(topic.Message, currentCutScene.actorHead);
  }

  public void TimelineSignalReceiver(int action) {
    if (action == 0) { // Stop
      currentCutScene.timeline.Stop(); // Test if we stop here, and if the messages will actually spkip to the next one in the timeline
      cutScenePlaying = false;
      StartWalk(cutSceneOriginalPos, false);
      currentCutScene = null;
    }
  }

  CutScene currentCutScene;
  bool cutScenePlaying = false;
  Vector3 cutSceneOriginalPos;
  bool startingCutScene = false;
  public void StartCutScene(CutScene cutScene) {
    startingCutScene = true;
    currentCutScene = cutScene;
    cutScene.diags = cutScene.timeline.GetComponent<TimelineDialogues>();
    StartWalk(cutScene.startPosition, false);
  }
  void PostStartCutScene() {
    cutScenePlaying = true;
    startingCutScene = false;
    currentCutScene.timeline.Play();
    cutSceneOriginalPos = player.transform.position;
  }

  public TextMeshProUGUI DayTime;
  public TextMeshProUGUI Dbg;


  public CameraFollow cameraFollow;
  public Sprite[] DirectionSprites;
  public Image Direction;
}
