using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
  public Transform target;
  public LayerMask GroundMask;
  public Cell cell = null;
  public Cell dstCell = null;
  Quaternion rotation;
  Camera cam, subCam;


  float cHeight = 4;
  float cAngleX = 4;
  float cAngleY = 4;
  float cDist = 4;
  float cFOV = 4;
  bool isInterior = false;
  float extraAngle = 0;
  Dir lastDir;
  Vector3 cSize, cTL, cBR;

  void CalculateCellValues(Cell c) {
    if (cell == c && lastDir == Game.ViewDir) return;

    cell = c;
    isInterior = cell.isInterior;
    (cHeight, cAngleX, cAngleY, cDist, cFOV) = cell.GetVals(Game.ViewDir);
    rotation = Quaternion.Euler(extraAngle + cAngleX, cAngleY, 0);
    lastDir = Game.ViewDir;

    cSize = cell.transform.localScale;
    cSize.y = 0;
    cSize *= .5f;
    cTL = cell.transform.position + cSize;
    cBR = cell.transform.position - cSize;
  }

  public TextMeshProUGUI Dbg;

  private void Start() {
    cam = GetComponent<Camera>();
    subCam = transform.GetChild(0).GetComponent<Camera>();
    var (_, rx, ry, _, fov) = cell.GetVals(Game.ViewDir);
    rotation = Quaternion.Euler(rx, ry, 0);
    transform.rotation = rotation;
    cam.fieldOfView = fov;
    subCam.fieldOfView = fov;
  }

  void Update() {
    if (dstCell != null) FollowPath();
    else if (migratePosition > 0) MigrateCamera();
    else if (!isInterior) MoveCamera();
  }


  void MoveCamera() {
    if (extraAngle != 0) {
      rotation = Quaternion.Euler(extraAngle + cAngleX, cAngleY, 0);
    }

    Vector3 tpos = target.position;
    Vector3 cpos = transform.position;
    switch (Game.ViewDir) {
      case Dir.U: cpos.x = tpos.x; cpos.z = tpos.z - cDist; break;
      case Dir.D: cpos.x = tpos.x; cpos.z = tpos.z + cDist; break;
      case Dir.L: cpos.x = tpos.x + cDist; cpos.z = tpos.z; break;
      case Dir.R: cpos.x = tpos.x - cDist; cpos.z = tpos.z; break;
    }
    cpos.y = cHeight;

    if (cpos.x < cBR.x) { cpos.x = cBR.x; }
    if (cpos.x > cTL.x) { cpos.x = cTL.x; }
    if (cpos.z < cBR.z) { cpos.z = cBR.z; }
    if (cpos.z > cTL.z) { cpos.z = cTL.z; }

    
    transform.SetPositionAndRotation(
      Vector3.Lerp(transform.position, cpos, Time.deltaTime * 2), 
      Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5));

    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, cFOV, Time.deltaTime * 8);
    subCam.fieldOfView = cam.fieldOfView;


    Vector3 midPos = cam.WorldToViewportPoint(target.position + Vector3.up);

    if (midPos.y < .2f) {
      if (extraAngle + cAngleX < 75) extraAngle += 45 * Time.deltaTime;
    }
    else if (midPos.y > .21f && extraAngle > 0) {
      extraAngle -= 15 * Time.deltaTime;
      if (extraAngle < 0) {
        extraAngle = 0;
        rotation = Quaternion.Euler(cAngleX, cAngleY, 0);
      }
    }
  }


  float migratePosition = 0;
  Vector3 migrationDestP;
  Quaternion migrationDestQ;

  void MigrateCamera() {
    transform.SetPositionAndRotation(
      Vector3.Lerp(transform.position, migrationDestP, Time.deltaTime * 2),
      Quaternion.Slerp(transform.rotation, migrationDestQ, Time.deltaTime * 10));
    migratePosition -= Time.deltaTime;
    if (migratePosition <= 0) {
      migratePosition = 0;
      transform.SetPositionAndRotation(migrationDestP, migrationDestQ);
    }
  }

  void FollowPath() {
    pathtime += Time.deltaTime * cellWithPath.Path.Count * .65f;
    if (pathtime >= 1) {
      pathNode += 2;
      pathtime = 0;
      if (pathNode >= path.Length - 2) { // Completed
        CameraPath cp = path[path.Length-1];
        cam.transform.SetPositionAndRotation(cp.pos, cp.rot);
        CalculateCellValues(dstCell);
        cam.fieldOfView = cFOV;
        subCam.fieldOfView = cFOV;
        dstCell = null;
        Game.SetDir(pathFinalDir);
        if (cell.isInterior) {
          cam.transform.SetPositionAndRotation(cell.GetStaticPosition(), cell.GetStaticRotation());
        }
      }
    }
    else { // Move along a bezier curve passign for all control points
      CameraPath cp1 = path[pathNode + 0];
      CameraPath cp2 = path[pathNode + 1];
      CameraPath cp3 = path[pathNode + 2];
      float t = pathtime;
      float tm = 1 - pathtime;
      Vector3 middle = (cp2.pos - .25f * cp1.pos - .25f * cp3.pos) * 2;
      Vector3 pos = tm * tm * cp1.pos + 2 * tm * t * middle + t * t * cp3.pos;
      Quaternion rot = Quaternion.Slerp(cp1.rot, cp3.rot, pathtime);
      cam.transform.SetPositionAndRotation(pos, rot);
    }

  }
  float pathtime = 0;
  int pathNode = 0;
  int pathCount = 0;
  Cell cellWithPath = null;
  Dir pathFinalDir;

  CameraPath[] path;
  public void SetCell(Cell c) {
    if (c.isInterior) { // Inside (we always use paths when moving inside)
      if (c.Path.Count > 0) {
        dstCell = c;
        pathtime = 0;
        pathNode = 0;
        cellWithPath = c;
        pathCount = cellWithPath.Path.Count;
        path = new CameraPath[pathCount + 2];
        path[0] = new() { pos = cam.transform.position, rot = cam.transform.rotation };
        for (int i = 0; i < pathCount; i++) {
          path[i + 1] = cellWithPath.Path[i];
        }
        path[pathCount + 1] = c.GetStaticPath();
        pathFinalDir = cellWithPath.InitialDir;
      }
      else {
        migratePosition = 1;
        migrationDestP = c.GetStaticPosition();
        migrationDestQ = c.GetStaticRotation();
        CalculateCellValues(c);
      }
    }
    else if (!c.isInterior && cell.isInterior) { // Going out
      if (cell.ExitPath.Count > 0) {
        dstCell = c;
        pathtime = 0;
        pathNode = 0;
        cellWithPath = cell;
        pathCount = cellWithPath.ExitPath.Count;
        path = new CameraPath[pathCount + 1];
        path[0] = new() { pos = cam.transform.position, rot = cam.transform.rotation };
        for (int i = 0; i < pathCount; i++) {
          path[i + 1] = cellWithPath.ExitPath[i];
        }
        pathCount--;
        pathFinalDir = cellWithPath.ExitDir;
      }
      else {
        CalculateCellValues(c);
      }
    }
    else {
      CalculateCellValues(c);
    }
  }

  internal void SetCamera(Cell c) {
    CalculateCellValues(c);
    rotation = Quaternion.Euler(cAngleX, cAngleY, 0);
    if (isInterior) {
      transform.SetPositionAndRotation(c.GetStaticPosition(), c.GetStaticRotation());
    }
  }
}

