using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PAN : MonoBehaviour {
  public RectTransform[] LinesOption;
  public GameObject Options;
  public GameObject FirstUse;
  public TextMeshProUGUI FirstUseTxt;
  public GameObject NewsBox;
  public GameObject DeleteButton;
  public GridLayoutGroup NewsGrid;
  public GameObject UserInfo;
  public GameObject BankPanel;
  public TextMeshProUGUI UserVal, BAMAVal, ConnVal, CredsVal;
  


  int lineOver = -1, currentLine = -1;
  float lineTime = 0;
  readonly string[] firstUseTxt = {
    "\r\n","\r\n",
    "Welcome"," to"," the"," Public"," Access"," Network"," (","<b>PAN</b>",")",
    "\r\n","\r\n",
    "If"," you"," are"," here"," you"," probably"," cannot"," afford"," Internet",".",
    "\r\n","\r\n","\r\n",
    "No"," worries",","," PAN"," will"," help"," you"," in"," checking"," the"," city"," news"," and"," find"," the"," best"," locations"," around",".",
    "\r\n",
    "These"," features"," cost"," no"," money",".",
    "\r\n","\r\n","\r\n",
    "You"," can"," also"," connect"," to"," your"," personal"," device"," or"," brain"," implant",","," in"," this"," case"," you"," will"," be"," billed",".",
    "\r\n\r\n\r\n\r\n",
    "                                  ","<size=60><i>","To"," exit"," click"," on"," the"," top-right"," button","</i></size>",
    "\r\n        "
  };

  internal void Activate() {
    gameObject.SetActive(true);
    DeleteButton.SetActive(false);
    Options.SetActive(true);
    FirstUse.SetActive(false);
    NewsBox.SetActive(false);
  }

  private void Update() {
    if (lineOver != -1) {
      if (currentLine != lineOver && currentLine != -1) LinesOption[currentLine].sizeDelta = new Vector2(0, 4);
      currentLine = lineOver;
      if (lineTime < 1) {
        LinesOption[currentLine].sizeDelta = new Vector2(2200 * lineTime, 4);
        lineTime += Time.deltaTime * 5;
      }
    }
    else if (currentLine != -1) {
      LinesOption[currentLine].sizeDelta = new Vector2(0, 4);
      currentLine = -1;
    }
  }

  public void GoOver(int line) {
    lineTime = 0;
    lineOver = line;
  }
  public void GoOut() {
    lineTime = 0;
    lineOver = -1;
  }

  public void Exit() {
    if (Options.activeSelf) {
      gameObject.SetActive(false);
      DeleteButton.SetActive(false);
      Game.StopInteracting();
    }
    else if (FirstUse.activeSelf) {
      FirstUse.SetActive(false);
      Options.SetActive(true);
      DeleteButton.SetActive(false);
    }
    else if (UserInfo.activeSelf) {
      UserInfo.SetActive(false);
      Options.SetActive(true);
      DeleteButton.SetActive(false);
    }
    else if (news != null) {
      DeleteButton.SetActive(false);
      news.Hide();
      news = null;
      for (int i = 0; i < NewsGrid.transform.childCount; i++) { // FIXME use an array, do nto rely on transform components
        NewsGrid.transform.GetChild(i).GetComponent<News>().MakeVisible();
      }
      NewsGrid.cellSize = new Vector2(720, 800);
    }
    else if (NewsBox.activeSelf) {
      NewsBox.SetActive(false);
      Options.SetActive(true);
    }
    
  }

  #region FirstUse
  public void ShowFirstUse() {
    StartCoroutine(ShowFirstUseCoroutine());
  }

  IEnumerator ShowFirstUseCoroutine() {
    FirstUseTxt.text = "";
    FirstUse.SetActive(true);
    Options.SetActive(false);
    yield return null;
    foreach (string word in firstUseTxt) {
      FirstUseTxt.text += word;
      if (word == ".") yield return new WaitForSeconds(1f);
      if (word == ",") yield return new WaitForSeconds(.1f);
      else yield return new WaitForSeconds(.03f);
    }
  }
  #endregion #region FirstUse


  #region News
  public void ShowNewsBox() {
    NewsBox.SetActive(true);
    Options.SetActive(false);
    NewsGrid.cellSize = new Vector2(720, 800);
  }
  News news = null;
  public void ShowNews(int index) {
    NewsGrid.cellSize = new Vector2(3600, 1600);
    for (int i = 0; i < NewsGrid.transform.childCount; i++) { // Keep in a list, not as child transform
      NewsGrid.transform.GetChild(i).gameObject.SetActive(i == index);
    }
    news = NewsGrid.transform.GetChild(index).GetComponent<News>();
    news.Show();
    if (index == 3) Quest.readDrugTrial = true;
    DeleteButton.SetActive(true);
  }

  public void DeleteNews() {
    if (news == null) return;
    news.Status = News.NewsStatus.Deleted;
    news.Hide();
    news = null;
    for (int i = 0; i < NewsGrid.transform.childCount; i++) { // FIXME use an array, do nto rely on transform components
      NewsGrid.transform.GetChild(i).GetComponent<News>().MakeVisible();
    }
    NewsGrid.cellSize = new Vector2(720, 800);

  }


  #endregion News

  #region UserInfo

  public void ShowUserInfo() {
    UserInfo.SetActive(true);
    Options.SetActive(false);
    // FIXME we should check if we connect using another user, like Molly or Flatline or Maelcum
    BAMAVal.text = Quest.BAMAID;
    ConnVal.text = "None"; // We should check if we have a deck or an implant, show the name of the item
    CredsVal.text = Quest.ChipCredits.ToString();
  }

  #endregion UserInfo


  #region Bank

  public TextMeshProUGUI BankBama;
  public TextMeshProUGUI BankCredits;
  public TextMeshProUGUI CardCredits;
  public TextMeshProUGUI CardHistory;
  public GameObject TransferPanel;
  public TMP_InputField TransferAmount;

  public void ShowBank() {
    BankPanel.SetActive(true);
    Options.SetActive(false);
    TransferPanel.SetActive(false);
    // FIXME we should check if we connect using another user, like Molly or Flatline or Maelcum
    BankBama.text = $"BAMA: {Quest.BAMAID}";
    CardCredits.text = $"Chip: {Quest.ChipCredits}";
    BankCredits.text = $"Bank: {Quest.BankCredits}";
  }

  bool toChip = true;
  public void TransferCreditsB2C() {
    TransferPanel.SetActive(true);
    TransferAmount.SetTextWithoutNotify("0");
    toChip = true;
  }
  public void TransferCreditsC2B() {
    TransferPanel.SetActive(true);
    TransferAmount.SetTextWithoutNotify("0");
    toChip = false;
  }
  public void TransferBackspace() {
    if (TransferAmount.text.Length < 2) TransferAmount.SetTextWithoutNotify("0");
    else TransferAmount.SetTextWithoutNotify(TransferAmount.text[0..^1]);
  }
  public void TransferComplete() {
    if (int.TryParse(TransferAmount.text, out int amount)) {
      if (toChip) {
        if (Quest.BankCredits < amount) amount = Quest.BankCredits;
        Quest.MoveCredits(-amount);
      }
      else {
        if (Quest.ChipCredits < amount) amount = Quest.ChipCredits;
        Quest.MoveCredits(amount);
      }
      TransferPanel.SetActive(false);
      CardCredits.text = $"Chip: {Quest.ChipCredits}";
      BankCredits.text = $"Bank: {Quest.BankCredits}";
    }
  }
  public void TransferCancel() {
    TransferPanel.SetActive(false);
  }
  public void TransferTypeNumber(int number) {
    string txt = TransferAmount.text;
    if (txt == "0") txt = "";
    TransferAmount.SetTextWithoutNotify(txt + number);
  }
  public void TransferKeyPressed() {
    string amount = "";
    foreach (char c in TransferAmount.text) {
      if (c >= '0' && c <= '9') amount += c;
    }
    if (amount != TransferAmount.text) TransferAmount.SetTextWithoutNotify(amount);
  }

  #endregion Bank



}
