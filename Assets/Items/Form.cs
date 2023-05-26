using System;
using UnityEngine;

public class Form : Interactable {
  public string Title;
  public string Message;
  public CodeRequest CodeRequest;
  public QuestItem Item;

  internal void Show() {
    CodeRequest.Show(Title, Message, (string res) => {
      Quest.CheckItemResult(Item, res);
    });
  }
}
