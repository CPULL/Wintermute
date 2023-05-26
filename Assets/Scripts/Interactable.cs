using UnityEngine;

public class Interactable : MonoBehaviour {
  public GameObject Overlay;
  public QuestAction ClickAction1;
  public QuestItem ClickItem1;
  public int ClickQuantity1;
  public QuestAction ClickAction2;
  public QuestItem ClickItem2;
  public int ClickQuantity2;

  public void GoOver(bool over) {
    if (Overlay == null) Debug.LogError(gameObject.name);
    Overlay.SetActive(over);
  }
}
