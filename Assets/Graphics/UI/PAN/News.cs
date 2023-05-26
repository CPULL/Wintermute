using TMPro;
using UnityEngine;

public class News : MonoBehaviour {
  public Animator anim;
  public TextMeshProUGUI Title;
  public NewsStatus Status = NewsStatus.Available;

  internal void MakeVisible() {
    gameObject.SetActive(Status == NewsStatus.Available || Status == NewsStatus.Read);
  }

  internal void Show() {
    anim.Play("News Show");
    Title.color = Color.white;
    Title.fontStyle = FontStyles.Normal;
    Status = NewsStatus.Read;
  }
  internal void Hide() {
    anim.Play("News Hide");
  }

  public enum NewsStatus { Read, Deleted, Available, Hidden };
}

