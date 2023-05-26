using UnityEngine;

public class RigRenamer : MonoBehaviour {
  public string oldName;
  public string newName;


  private void Start() {
    Debug.LogError($"Remove RigRenamer from object {gameObject.name}", gameObject);
  }
}

