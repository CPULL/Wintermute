using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class Quest : MonoBehaviour {
  static Quest q;

  public Game game;
  public static bool Paused = false;
  public static int ChipCredits { get; private set; } = 0;
  public static int BankCredits = 0;
  public static float Vitals = .9f; // 0..1
  public static float HeartRate = 1f; // 0..4
  public static System.DateTime Day = new(2058, 11, 13, 9, 10, 0);


  public static string BAMAID = "";
  public static bool CreditChip = false;
  public static bool paidCheapHotel = false;
  public static bool BozoBankCustomer = false;
  public static bool apartmentKey = false;
  public static bool readDrugTrial = false;

  public static void AddCreditsToChip(int credits) {
    ChipCredits += credits;
    q.TextCredits.text = $"Credits: {ChipCredits}/{BankCredits}";
  }
  public static void AddCreditsToBank(int credits) {
    BankCredits += credits;
    q.TextCredits.text = $"Credits: {ChipCredits}/{BankCredits}";
  }

  /// <summary>
  /// From chip to Bank, use negative to move from bank to chip
  /// </summary>
  /// <param name="credits"></param>
  public static void MoveCredits(int credits) {
    ChipCredits -= credits;
    BankCredits += credits;
    q.TextCredits.text = $"Credits: {ChipCredits}/{BankCredits}";
  }


  private void Awake() {
    q = this;
  }

  void Start() {
    for (int i = 0; i < 10; i++) {
      BAMAID += Random.Range(0, 10);
    }
    BAMAID = "0000000000"; // FIXME
  }

  public static bool CheckCondition(Conditions cond, int val) {
    switch (cond) {
      case Conditions.Always: return true;
      case Conditions.Random: return Random.Range(0, val) == 0;
      case Conditions.CreditChip: return CreditChip;
      case Conditions.PaidCheapHotel: return paidCheapHotel;
      case Conditions.BozobankCustomer: return BozoBankCustomer;
      case Conditions.HasApartmentKey: return apartmentKey;
      case Conditions.HasEnoughMoney: return ChipCredits >= val;
      case Conditions.Vitals: return Quest.Vitals > val;
      case Conditions.HeartRate: return Quest.HeartRate > val;
      case Conditions.ReadDrugTrial: return Quest.readDrugTrial;
    }
    Debug.LogError($"Missing condition for {cond}");
    return false;
  }




  internal static void PerformAction(Dialogue topic) {
    PerformAction(topic.Action1, topic.Item1, topic.Quantity1);
    PerformAction(topic.Action2, topic.Item2, topic.Quantity2);
  }
  internal static void PerformAction(Interactable interact) {
    PerformAction(interact.ClickAction1, interact.ClickItem1, interact.ClickQuantity1);
    PerformAction(interact.ClickAction2, interact.ClickItem2, interact.ClickQuantity2);
  }

  internal static void PerformAction(QuestAction action, QuestItem item, int quantity) {
    switch (action) {
      case QuestAction.None: break;
      case QuestAction.AddItem:
        // FIXME add or remove somethign from player inventory
        break;
      case QuestAction.GiveMoney:
        AddCreditsToChip(quantity);
        break;
      case QuestAction.SetFlag:
        SetFlag(item, quantity);
        break;
      case QuestAction.StartCutScene:
        StartCutScene(item);
        break;
      case QuestAction.ChangeHealth:
        Vitals += quantity;
        break;
      case QuestAction.ChangeStress:
        HeartRate += quantity;
        break;
      case QuestAction.EnableItem:
        EnableItem(item, quantity != 0);
        break;
      case QuestAction.ShowForm:
        break;
    }
  }

  private static void EnableItem(QuestItem item, bool active) {
    switch (item) {
      case QuestItem.BozobankForm:
        q.BozobankForm.SetActive(active);
        break;
      default: break;
    }
  }

  private static void StartCutScene(QuestItem item) {
    switch (item) {
      case QuestItem.DoctorVisit:
        q.game.StartCutScene(q.cutScenes[0]);
        break;
      default:
        Debug.LogError("Asked a cutscene for item " + item);
        break;
    }
  }

  private static void SetFlag(QuestItem flag, int val) {
    switch (flag) {
      case QuestItem.None: break;
      case QuestItem.DoctorVisit: break;
      case QuestItem.CreditChip: CreditChip = val > 0; q.BankInterlinkOption.SetActive(CreditChip); break;
    }
  }

  internal static void CheckItemResult(QuestItem item, string res) {
    if (item == QuestItem.BozobankForm) {
      if (res == BAMAID) q.game.StartDialogue(q.BankEmployee, 7); // Good
      else q.game.StartDialogue(q.BankEmployee, 9); // Bad
    }
  }

  internal static Cell GetCell => q.game.cell;

  public TextMeshProUGUI TextCredits;
  public CutScene[] cutScenes;

  public GameObject BozobankForm;
  public Dialogues BankEmployee;
  public GameObject BankInterlinkOption;
}

[System.Serializable]
public class CutScene {
  public PlayableDirector timeline;
  public TimelineDialogues diags;
  public Vector3 startPosition; // new(-1.352f, 0, 12.594f) doctor visit
  public Transform actorHead;
}

public enum Conditions {
  Always, Random, CreditChip, PaidCheapHotel, BozobankCustomer, HasApartmentKey, HasEnoughMoney,
  Vitals, HeartRate, ReadDrugTrial
}

public enum QuestAction {
  None, AddItem, GiveMoney, SetFlag, StartCutScene, ChangeHealth, ChangeStress, EnableItem, ShowForm, 
}

public enum QuestItem {
  None,
  DoctorVisit,
  CreditChip,
  BozobankForm,
  PawnTicket,
  DeckUXB,

//  ApartmentKey,
//  BozoBank,
//  BasicDeck, // Use a better name
//  DowntowmPass,
}