using System.Collections.Generic;
using UnityEngine;

public class Dialogues : MonoBehaviour {
  public Transform Head;
  [SerializeReference] public List<Dialogue> diags;
  [SerializeField] private int lastID = 1;

  public int Count => diags == null ? 0 : diags.Count;

  public Dialogue Add(bool player) {
    diags ??= new();

    int maxLast = 0;
    foreach (var d in diags)
      if (d.ID > maxLast) maxLast = d.ID;
    lastID = maxLast + 1;
    Dialogue diag = new() { IsPlayer = player, ID = lastID };
    diags.Add(diag);
    return diag;
  }

  internal Dialogue GetDialogue(Dialogue lastDiag) {
    Dialogue diag = null;
    int tries = 3;
    while (tries > 0 && diag == null) {
      tries--;
      foreach (Dialogue d in diags) {
        if (d == lastDiag) continue;
        if (Quest.CheckCondition(d.Condition, d.Val) == d.Result) {
          diag = d;
          break;
        }
      }
      lastDiag = null;
    }
    return diag;
  }

  internal List<Dialogue> GetPlayerDialogues() {
    List<Dialogue> list = new();
    foreach (var d in diags) {
      if (!d.IsPlayer) continue;
      bool alone = true;
      foreach (var src in diags) {
        if (src.Next?.Contains(d.ID) ?? false) {
          alone = false;
          break;
        }
      }
      if (alone && Quest.CheckCondition(d.Condition, d.Val) == d.Result) {
        list.Add(d);
      }
    }
    return list;
  }
  internal List<Dialogue> GetNextDialogues(Dialogue from) {
    List<Dialogue> list = new();
    foreach (var d in diags) {
      if (from.Next?.Contains(d.ID) ?? false) {
        if (Quest.CheckCondition(d.Condition, d.Val) == d.Result) {
          list.Add(d);
        }
      }
    }
    return list;
  }

  internal Dialogue GetNextDialogue(Dialogue from) {
    foreach (var d in diags) {
      if ((from.Next?.Contains(d.ID) ?? false) && (!d.OnlyOnce || !d.used)) {
        if (Quest.CheckCondition(d.Condition, d.Val) == d.Result) return d;
      }
    }
    return null;
  }

  internal Dialogue GetTopic(int id) {
    foreach (var d in diags) {
      if (d.ID == id) return d;
    }
    Debug.LogError($"Cannot find dialogue with id {id} on Dialogues for {gameObject.name}");
    return null;
  }

  public void Remove(Dialogue diag) {
    diags.Remove(diag);
  }
}





