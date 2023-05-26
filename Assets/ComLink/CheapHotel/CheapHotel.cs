using UnityEngine;

public class CheapHotel : MonoBehaviour {
  public GameObject Main, Rooms, RoomService, Balance;
  public GameObject PaymentInput, EditBalanceButton;


  public void ShowMain() {
    Main.SetActive(true);
    Rooms.SetActive(false);
    RoomService.SetActive(false);
    Balance.SetActive(false);
  }
  public void ShowRooms() {
    Main.SetActive(false);
    Rooms.SetActive(true);
    RoomService.SetActive(false);
    Balance.SetActive(false);
  }
  public void ShowRoomService() {
    Main.SetActive(false);
    Rooms.SetActive(false);
    RoomService.SetActive(true);
    Balance.SetActive(false);
  }
  public void ShowBalance() {
    Main.SetActive(false);
    Rooms.SetActive(false);
    RoomService.SetActive(false);
    Balance.SetActive(true);
  }

}
