using System;
using TMPro;
using UnityEngine;

public class CodeRequest : MonoBehaviour {
  public TextMeshProUGUI Title;
  public TMP_InputField CodeInput;
  public TextMeshProUGUI Placeholder;

  bool visible = false;
  Action<string> callback;

  public void Show(string title, string placeholder, Action<string> cb) {
    CodeInput.SetTextWithoutNotify("");
    Title.text = title;
    Placeholder.text = placeholder;
    callback = cb;
    visible = true;
    gameObject.SetActive(true);
  }


  private void Update() {
    if (visible && Input.GetKeyDown(KeyCode.Return)) Complete();
  }

  public void TypeLetter(string l) {
    if (l == "{" && CodeInput.text.Length > 0) CodeInput.SetTextWithoutNotify(CodeInput.text[0..^1]);
    else if (l == "}") Complete();
    else if (CodeInput.text.Length < 16) CodeInput.SetTextWithoutNotify(CodeInput.text + l);
  }

  public void InputChanged() {
    if (CodeInput.text.Length > 16) CodeInput.SetTextWithoutNotify(CodeInput.text[0..16]);
  }

  public void Complete() {
    visible = false;
    gameObject.SetActive(false);
    callback(CodeInput.text);
  }
}