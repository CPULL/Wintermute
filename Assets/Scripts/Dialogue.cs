using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {
  [HideInInspector] public Vector2 pos; // Used by the editor window
  [HideInInspector] public bool used = false; // Probably to be saved in a savegame
  public int ID;
  public bool IsPlayer = false;
  public float Time;
  public BallonSide ForceSide; 

  public string Message;

  public Conditions Condition;
  public bool Result = true;
  public int Val;

  public bool OnlyOnce = false;

  public QuestAction Action1;
  public QuestItem Item1;
  public int Quantity1;
  public QuestAction Action2;
  public QuestItem Item2;
  public int Quantity2;

  public List<int> Next;


  public override string ToString() {
    if (string.IsNullOrWhiteSpace(Message)) return "<empty>";
    int pos1 = Message.IndexOf('.');
    int pos2 = Message.IndexOf('/');
    if (pos1 == -1 && pos2 == -1) return Message;
    if (pos1 == -1) return Message[0..pos2];
    if (pos2 == -1) return Message[0..pos1];
    if (pos1 < pos2) return Message[0..pos1]; else return Message[0..pos2];
  }

  public string GetCondition => Condition switch {
    Conditions.Always => Result ? "Always" : "Never",
    Conditions.Random => $"Random(0,{Val}) {(Result ? "== 0" : "!= 0")} ",
    Conditions.CreditChip => $"{(Result ? "Has" : "No")} BankCard",
    Conditions.PaidCheapHotel => $"{(Result ? "" : "Not ")}Paid CheapHotel",
    Conditions.BozobankCustomer => $"Is {(Result ? "" : "Not ")}BozoBank Customer",
    Conditions.HasApartmentKey => $"{(Result ? "Has" : "No")} Apartment Key",
    Conditions.HasEnoughMoney => $"Credits {(Result ? ">" : "<=")} {Val}",
    Conditions.Vitals => $"Health {(Result ? "=>" : "<")} {Val}",
    Conditions.HeartRate => $"Hearth {(Result ? "=>" : "<")} {Val}",
    Conditions.ReadDrugTrial => $"{(Result ? "Read" : "Not read")} DrugTrail",
    _ => "UNKNOWN!!!"
  };
}

public enum BallonSide {
  Dont, L, R
}