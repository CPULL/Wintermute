public class PANTerminal : Interactable {
  public PAN PANPanel;


  internal bool Interact() {
    if (PANPanel != null) PANPanel.Activate();
    return true;
  }
}
