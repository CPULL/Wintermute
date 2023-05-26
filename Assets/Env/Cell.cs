using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
  public new string name;
  public bool isInterior = false;
  public PossibleDir possibleDirs = PossibleDir.All;
  public PossibleDir ignoreBorder = PossibleDir.All;

  public float HeightU, HeightR, HeightD, HeightL;
  public float AngleU, AngleR, AngleD, AngleL;
  public float DistU, DistR, DistD, DistL;

  public Dir InitialDir, ExitDir;
  public List<CameraPath> Path, ExitPath;

  public GameObject[] ItemsU, ItemsR, ItemsD, ItemsL;
  public GameObject[] AreasHidden;

  public Vector3 StaticPosU, StaticPosR, StaticPosD, StaticPosL;
  public Quaternion StaticRotU, StaticRotR, StaticRotD, StaticRotL;
  public float FOV = 60;

  public void HideBack() {
    bool hn = Game.ViewDir != Dir.U;
    bool he = Game.ViewDir != Dir.L;
    bool hs = Game.ViewDir != Dir.D;
    bool hw = Game.ViewDir != Dir.R;
    foreach (var g in ItemsU) g.SetActive(hn);
    foreach (var g in ItemsL) g.SetActive(he);
    foreach (var g in ItemsD) g.SetActive(hs);
    foreach (var g in ItemsR) g.SetActive(hw);
  }

  public Dir Rotate(Dir d, bool clockwise) {
    if (clockwise) {
      if (d == Dir.U) {
        if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
        else if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
        else if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
        else return Dir.U;
      }
      else if (d == Dir.R) {
        if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
        else if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
        else if (possibleDirs.HasFlag(PossibleDir.U)) return Dir.U;
        else return Dir.R;
      }
      else if (d == Dir.D) {
        if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
        else if (possibleDirs.HasFlag(PossibleDir.U)) return Dir.U;
        else if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
        else return Dir.D;
      }
      else if (d == Dir.L) {
        if (possibleDirs.HasFlag(PossibleDir.U)) return Dir.U;
        else if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
        else if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
        else return Dir.L;
      }
    }
    else {
      if (d == Dir.U) {
        if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
        else if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
        else if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
        else return Dir.U;
      }
      else if (d == Dir.R) {
        if (possibleDirs.HasFlag(PossibleDir.U)) return Dir.U;
        else if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
        else if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
        else return Dir.R;
      }
      else if (d == Dir.D) {
        if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
        else if (possibleDirs.HasFlag(PossibleDir.U)) return Dir.U;
        else if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
        else return Dir.D;
      }
      else if (d == Dir.L) {
        if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
        else if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
        else if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
        else return Dir.L;
      }
    }
    return d;
  }

  internal CameraPath GetStaticPath() {
    return InitialDir switch {
      Dir.U => new() { pos = StaticPosU, rot = StaticRotU },
      Dir.L => new() { pos = StaticPosL, rot = StaticRotL },
      Dir.D => new() { pos = StaticPosD, rot = StaticRotD },
      Dir.R => new() { pos = StaticPosR, rot = StaticRotR },
      _ => new(),
    };
  }

  internal Vector3 GetStaticPosition() {
    if (Game.ViewDir == Dir.U) {
      if (possibleDirs.HasFlag(PossibleDir.U)) return StaticPosU;
      if (possibleDirs.HasFlag(PossibleDir.L)) return StaticPosL;
      if (possibleDirs.HasFlag(PossibleDir.D)) return StaticPosD;
      if (possibleDirs.HasFlag(PossibleDir.R)) return StaticPosR;
    }
    if (Game.ViewDir == Dir.L) {
      if (possibleDirs.HasFlag(PossibleDir.L)) return StaticPosL;
      if (possibleDirs.HasFlag(PossibleDir.D)) return StaticPosD;
      if (possibleDirs.HasFlag(PossibleDir.R)) return StaticPosR;
      if (possibleDirs.HasFlag(PossibleDir.U)) return StaticPosU;
    }
    if (Game.ViewDir == Dir.D) {
      if (possibleDirs.HasFlag(PossibleDir.D)) return StaticPosD;
      if (possibleDirs.HasFlag(PossibleDir.R)) return StaticPosR;
      if (possibleDirs.HasFlag(PossibleDir.U)) return StaticPosU;
      if (possibleDirs.HasFlag(PossibleDir.L)) return StaticPosL;
    }
    if (Game.ViewDir == Dir.R) {
      if (possibleDirs.HasFlag(PossibleDir.R)) return StaticPosR;
      if (possibleDirs.HasFlag(PossibleDir.U)) return StaticPosU;
      if (possibleDirs.HasFlag(PossibleDir.L)) return StaticPosL;
      if (possibleDirs.HasFlag(PossibleDir.D)) return StaticPosD;
    }
    return Vector3.zero;
  }
  internal Quaternion GetStaticRotation() {
    if (Game.ViewDir == Dir.U) {
      if (possibleDirs.HasFlag(PossibleDir.U)) return StaticRotU;
      if (possibleDirs.HasFlag(PossibleDir.L)) return StaticRotL;
      if (possibleDirs.HasFlag(PossibleDir.D)) return StaticRotD;
      if (possibleDirs.HasFlag(PossibleDir.R)) return StaticRotR;
    }
    if (Game.ViewDir == Dir.L) {
      if (possibleDirs.HasFlag(PossibleDir.L)) return StaticRotL;
      if (possibleDirs.HasFlag(PossibleDir.D)) return StaticRotD;
      if (possibleDirs.HasFlag(PossibleDir.R)) return StaticRotR;
      if (possibleDirs.HasFlag(PossibleDir.U)) return StaticRotU;
    }
    if (Game.ViewDir == Dir.D) {
      if (possibleDirs.HasFlag(PossibleDir.D)) return StaticRotD;
      if (possibleDirs.HasFlag(PossibleDir.R)) return StaticRotR;
      if (possibleDirs.HasFlag(PossibleDir.U)) return StaticRotU;
      if (possibleDirs.HasFlag(PossibleDir.L)) return StaticRotL;
    }
    if (Game.ViewDir == Dir.R) {
      if (possibleDirs.HasFlag(PossibleDir.R)) return StaticRotR;
      if (possibleDirs.HasFlag(PossibleDir.U)) return StaticRotU;
      if (possibleDirs.HasFlag(PossibleDir.L)) return StaticRotL;
      if (possibleDirs.HasFlag(PossibleDir.D)) return StaticRotD;
    }
    return Quaternion.identity;
  }

  public (float height, float angleX, float angleY, float dist, float fow) GetVals(Dir viewDir) {
    if (viewDir == Dir.U && possibleDirs.HasFlag(PossibleDir.U)) return new(HeightU, AngleU, 0, DistU, FOV);
    if (viewDir == Dir.L && possibleDirs.HasFlag(PossibleDir.L)) return new(HeightL, AngleL, -90, DistL, FOV);
    if (viewDir == Dir.D && possibleDirs.HasFlag(PossibleDir.D)) return new(HeightD, AngleD, 180, DistD, FOV);
    if (viewDir == Dir.R && possibleDirs.HasFlag(PossibleDir.R)) return new(HeightR, AngleR, 90, DistR, FOV);

    return InitialDir switch {
      Dir.U => new(HeightU, AngleU, 0, DistU, FOV),
      Dir.L => new(HeightL, AngleL, -90, DistL, FOV),
      Dir.D => new(HeightD, AngleD, 180, DistD, FOV),
      Dir.R => new(HeightR, AngleR, 90, DistR, FOV),
      _ => new(0, 0, 0, 0, 0)
    };
  }


  internal Dir SetDir(Dir viewDir, Vector3 fwd) {
    float angle = Mathf.Atan2(fwd.x, fwd.z) * Mathf.Rad2Deg; if (angle < 0) angle += 360;
    Dir d;
    if (angle > 315 || angle < 45) d = Dir.U;
    else if (angle >= 45 && angle < 135) d = Dir.L;
    else if (angle >= 135 && angle < 225) d = Dir.D;
    else d = Dir.R;

    if (viewDir == Dir.U) {
      if (possibleDirs.HasFlag(PossibleDir.U)) return Dir.U;
      if (possibleDirs.HasFlag(PossibleDir.D) && d == Dir.D) return Dir.D;
      if (possibleDirs.HasFlag(PossibleDir.R) && d == Dir.R) return Dir.R;
      if (possibleDirs.HasFlag(PossibleDir.L) && d == Dir.L) return Dir.L;
      if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
      if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
      if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
    }
    if (viewDir == Dir.L) {
      if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
      if (possibleDirs.HasFlag(PossibleDir.R) && d == Dir.R) return Dir.R;
      if (possibleDirs.HasFlag(PossibleDir.U) && d == Dir.U) return Dir.U;
      if (possibleDirs.HasFlag(PossibleDir.D) && d == Dir.D) return Dir.D;
      if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
      if (possibleDirs.HasFlag(PossibleDir.U)) return Dir.U;
      if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
    }
    if (viewDir == Dir.R) {
      if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
      if (possibleDirs.HasFlag(PossibleDir.L) && d == Dir.L) return Dir.L;
      if (possibleDirs.HasFlag(PossibleDir.U) && d == Dir.U) return Dir.U;
      if (possibleDirs.HasFlag(PossibleDir.D) && d == Dir.D) return Dir.D;
      if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
      if (possibleDirs.HasFlag(PossibleDir.U)) return Dir.U;
      if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
    }
    if (viewDir == Dir.D) {
      if (possibleDirs.HasFlag(PossibleDir.D)) return Dir.D;
      if (possibleDirs.HasFlag(PossibleDir.U) && d == Dir.U) return Dir.U;
      if (possibleDirs.HasFlag(PossibleDir.L) && d == Dir.L) return Dir.L;
      if (possibleDirs.HasFlag(PossibleDir.R) && d == Dir.R) return Dir.R;
      if (possibleDirs.HasFlag(PossibleDir.L)) return Dir.L;
      if (possibleDirs.HasFlag(PossibleDir.R)) return Dir.R;
      if (possibleDirs.HasFlag(PossibleDir.U)) return Dir.U;
    }
    return viewDir;
  }

  internal void ShowAll() {
    foreach (GameObject g in ItemsU) g.SetActive(true);
    foreach (GameObject g in ItemsR) g.SetActive(true);
    foreach (GameObject g in ItemsD) g.SetActive(true);
    foreach (GameObject g in ItemsL) g.SetActive(true);
  }



}

public enum Dir { U = 0, L = 1, D = 2, R = 3, };

[System.Flags]
public enum PossibleDir { U = 1, R = 2, D = 4, L = 8,     All = 15 }



[System.Serializable]
public class CameraPath {
  public Vector3 pos;
  public Quaternion rot;


  public override string ToString() {
    return $"{pos.x:f1},{pos.y:f1},{pos.z:f1}   {rot.eulerAngles.x:f0},{rot.eulerAngles.y:f0}";
  }
}

