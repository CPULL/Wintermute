using System.Collections;
using TMPro;
using UnityEngine;

public class ComLink : MonoBehaviour {
  public RectTransform rt;
  public GameObject[] Sections;
  public string[] urls;
  public CodeRequest codeRequest;
  public TextMeshProUGUI MainMessage;
  public RectTransform InteractionPanel;
  public TextMeshProUGUI InteractionMsg;
  public TMP_InputField InteractionInput;


  void Show() {
    StartCoroutine(Appear());
  }


  IEnumerator Appear() {
    MainMessage.gameObject.SetActive(false);
    rt.localScale = Vector3.zero;
    yield return null;
    float time = 0;
    while (time < 1) {
      time += Time.deltaTime * 1.2f;
      rt.localScale = new(1, time, 1);
      yield return null;
    }
    rt.localScale = Vector3.one;
    codeRequest.Show("Comlink", "Enter your destination address...", NavigateTo);
  }

  private void NavigateTo(string url) {
    int section = -1;
    for (int i = 0; i < Sections.Length; i++) {
      if (urls[i] == url) {
        section = i;
        break;
      }
    }
    if (section == -1) {
      MainMessage.text = "Invalid Address!";
      MainMessage.gameObject.SetActive(true);
    }
  }
}
