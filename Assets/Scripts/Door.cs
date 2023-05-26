using System;
using System.Collections;
using UnityEngine;

public class Door : Interactable {
  public Vector3 openVector = new(.064f, 0, 0);
  public Vector3 closeVector = new(1.090587f, 0, 0);
  bool animating = false;
  Vector3 dstPosition;
  Quaternion dstRotation;
  public Quaternion openRotation, closeRotation;
  public bool isOpen = false;
  public DoorType doorType;
  public Door Double;
  public GameObject[] ItemsHidden;
  public Cell OwnerCell;

  public void OpenDouble(bool open) {
    isOpen = open;
    if (doorType == DoorType.Rotating) dstRotation = open ? openRotation : closeRotation;
    else dstPosition = open ? openVector : closeVector;
    animating = true;
    bool showItems = (isOpen || Quest.GetCell == OwnerCell);
    foreach (var g in ItemsHidden) g.SetActive(showItems);
  }

  public bool Open() {
    isOpen = !isOpen;
    if (doorType == DoorType.Rotating) dstRotation = isOpen ? openRotation : closeRotation;
    else dstPosition = isOpen ? openVector : closeVector;
    animating = true;
    bool showItems = (isOpen || Quest.GetCell == OwnerCell);
    foreach (var g in ItemsHidden) g.SetActive(showItems);
    if (doorType == DoorType.Double && Double != null) Double.OpenDouble(isOpen);
    StartCoroutine(CloseAfterAWhile());
    return isOpen;
  }

  IEnumerator CloseAfterAWhile() {
    yield return new WaitForSeconds(10);
    if (isOpen) Open();
  }

  private void Start() {
    if (doorType == DoorType.Rotating) dstRotation = transform.rotation;
    else dstPosition = transform.position;
    foreach (var g in ItemsHidden) g.SetActive(isOpen);
  }
  private void Update() {
    if (animating) {
      if (doorType == DoorType.Rotating) {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, dstRotation, 8 * Time.deltaTime);
        if (Quaternion.Angle(transform.localRotation, dstRotation) < 1f) {
          transform.localRotation = dstRotation;
          animating = false;
        }
      }
      else {
        transform.localPosition = Vector3.Lerp(transform.localPosition, dstPosition, 8 * Time.deltaTime);
        if (Vector3.Distance(transform.localPosition, dstPosition) < .01f) {
          transform.localPosition = dstPosition;
          animating = false;
        }
      }
    }
  }

}

public enum DoorType { Sliding, Double, Rotating }