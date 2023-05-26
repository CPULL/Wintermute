using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Balloon : MonoBehaviour {
  public Camera cam;
  public TextMeshProUGUI Text;
  public RectTransform CanvasRT;
  public RectTransform Img;
  public RectTransform ImgExtra;
  public RectTransform BackRT;
  public RectTransform ButtonRT;
  public Image Under;
  public RectTransform rt;
  Transform actor;
  Vector3 scaleL = Vector3.one;
  Vector3 scaleR = new(-1, 1, 1);
  public float lineH = 72;
  public bool IsThinking;
  public Transform PlayerHead;
  public Game game;

  Color npcCol = new Color32(58, 131, 241, 255);
  Color plaCol = new Color32(58, 241, 131, 255);
  bool prevRight = false;

  void Update() {
    if (actor == null) return;

    Vector2 pos = RectTransformUtility.WorldToScreenPoint(cam, actor.position + Vector3.up * .05f);
    pos.x -= cam.pixelWidth * .5f;
    pos.y *= 1.2f;
    pos.y -= cam.pixelHeight * .5f;

    if (pos.x < -950) pos.x = -950;
    if (pos.x > 950) pos.x = 950;
    rt.anchoredPosition = pos;

    bool right = (pos.x >= -500 && pos.x < 0) || (pos.x >= 500 && pos.x < 1000);
    if (forcedSide == BallonSide.L) right = false;
    else if (forcedSide == BallonSide.R) right = true;

    if (prevRight != right) {
      prevRight = right;
      if (right) {
        Img.localScale = scaleR;
        ImgExtra.localScale = scaleR;
      }
      else {
        Img.localScale = scaleL;
        ImgExtra.localScale = scaleL;
      }
    }


    Vector2 size = Text.GetPreferredValues(Text.text);
    int lines = Text.textInfo.lineCount;
    if (IsThinking) { lines += 1; if (lines < 3) lines = 3; }
    size.y = lines * lineH + 10;
    size.x = 1920;
    BackRT.sizeDelta = size;
    size.x = IsThinking ? 1860 : 1920;
    ButtonRT.sizeDelta = size;
  }

  public void Show(string msg, Transform a = null, BallonSide forcedSide = BallonSide.Dont) {
    if (a == null) {
      actor = PlayerHead;
      Under.color = plaCol;
      Text.color = plaCol;
    }
    else {
      actor = a;
      Under.color = npcCol;
      Text.color = npcCol;
    }
    Text.text = "";
    Update();
    gameObject.SetActive(true);
    if (prev != null) StopCoroutine(prev);
    prev = StartCoroutine(ShowMsg(msg));
    this.forcedSide = forcedSide;
  }
  Coroutine prev = null;
  private BallonSide forcedSide;

  public bool WasPlayer() {
    return actor == PlayerHead;
  }

  readonly WaitForSeconds oneSec = new(1);
  readonly WaitForSeconds zeroFiveSec = new(.05f);
  readonly WaitForSeconds zeroTwentyFiveSec = new(.025f);

  IEnumerator ShowMsg(string msg) {
    string txt = "";
    bool bold = false, italic = false;
    foreach (char c in msg) {
      if (c == '\\') txt += "<br>";
      else if (c == '*') {
        bold = !bold;
        if (bold) txt += "<b>"; else txt += "</b>";
      }
      else if (c == '_') {
        italic = !italic;
        if (italic) txt += "<i>"; else txt += "</i>";
      }
      else txt += c;
      Text.text = txt;
      if (!IsThinking) {
        if (c == ' ' || c == ',' || c == '\n' || c == '\\') yield return zeroFiveSec;
        else if (c == '.' || c == '!' || c == '?') yield return oneSec;
        else yield return zeroTwentyFiveSec;
      }
    }
    if (!IsThinking) {
      yield return new WaitForSeconds(200 + 10 + msg.Length * .1f);
      gameObject.SetActive(false);
      prev = null;
      game.SayQuestion();
    }
  }

  internal void Hide() {
    if (prev != null) {
      StopCoroutine(prev);
      prev = null;
    }
    gameObject.SetActive(false);
    forcedSide = BallonSide.Dont;
  }

}
