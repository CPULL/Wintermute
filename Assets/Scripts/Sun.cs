using UnityEngine;

public class Sun : MonoBehaviour {
  [SerializeField] private Light DirLight;
  [SerializeField] Quaternion Midnigth, Morning, Midday, Evening;
  public void UpdateDayTime(float dayTime) {
    Quaternion rotation;
    if (dayTime < .25f) rotation = Quaternion.Slerp(Midnigth, Morning, dayTime * 4);
    else if (dayTime < .5f) rotation = Quaternion.Slerp(Morning, Midday, (dayTime - .25f) * 4);
    else if (dayTime < .75f) rotation = Quaternion.Slerp(Midday, Evening, (dayTime - .5f) * 4);
    else rotation = Quaternion.Slerp(Evening, Midnigth, (dayTime - .75f) * 4);
    transform.rotation = rotation;

    float intensity = Mathf.Sign((dayTime + .25f) * Mathf.PI * 2);
    DirLight.intensity = intensity;
    DirLight.colorTemperature = 2000 + intensity * 2000;
  }
}
