using UnityEngine;

public class CyberNode : MonoBehaviour {
  public string Name;
  public Color MainColor;
  // FIXME passwords, contents, etc.

  MeshRenderer mr;
  Material mat;
  void Start() {
    mr = GetComponent<MeshRenderer>();
    mat = mr.material;
    mr.material = mat;
    col = new Color(Random.Range(.1f, 2), Random.Range(.1f, 2), Random.Range(.1f, 2), 1);
    col = Color.Lerp(MainColor, col, .5f);
    colDst = col;
    mat.SetColor("_LinesColor", col);
    speed = new(Random.Range(-.02f, .02f), Random.Range(-.02f, .02f));
    speedDst = speed;
  }

  float time = 0;
  float dstTime = 0;
  Quaternion rotation = Quaternion.identity;
  Vector2 speedDst, speed;
  Color col, colDst;

  void FixedUpdate() {
    time += Time.fixedDeltaTime;
    if (time > dstTime) {
      time = 0;
      dstTime = Random.Range(1, 5f);
      rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
      speedDst = new(Random.Range(-.02f, .02f), Random.Range(-.02f, .02f));
      colDst = new Color(Random.Range(.1f, 2), Random.Range(.1f, 2), Random.Range(.1f, 2), 1);
      colDst = Color.Lerp(MainColor, col, .5f);
    }
    float step = time / dstTime;
    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, step);
    col = Color.Lerp(col, colDst, step);
    mat.SetColor("_LinesColor", col);
    speed = Vector2.Lerp(speed, speedDst, step);
    mat.SetVector("_Background_Speed", speed);

  }
}
