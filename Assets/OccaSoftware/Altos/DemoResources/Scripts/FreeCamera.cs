using UnityEngine;


namespace OccaSoftware.Altos.Demo
{
    /// <summary>
    /// A Free Camera controller.
    /// <br></br>
    /// <br></br>
    /// <b>Controls</b>
    ///     <br><b>WASD, QE:</b> Camera translation.</br>
    ///     <br><b>Mouse:</b> Camera rotation.</br>
    /// </summary>
    public class FreeCamera : MonoBehaviour
    {
        [SerializeField]
        private float speed = 75;

        [SerializeField]
        private float sensitivity = 1;

        [SerializeField, Range(1, 5)]
        private float maxSpeedPickup = 3;

        private float hAcc = 0;
        private float fAcc = 0;
        private float vAcc = 0;


        void Start()
        {
            LockAndHideCursor();
        }

        void Update()
        {
            Translate();
            Rotate();
        }


        private void LockAndHideCursor()
		{
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }


        private void Translate()
        {
            float h = 0;
            float f = 0;
            float v = 0;

            if (Input.GetKey(KeyCode.A))
                h -= 1;
            if (Input.GetKey(KeyCode.D))
                h += 1;

            if (Input.GetKey(KeyCode.S))
                f -= 1;
            if (Input.GetKey(KeyCode.W))
                f += 1;

            if (Input.GetKey(KeyCode.Q))
                v -= 1;
            if (Input.GetKey(KeyCode.E))
                v += 1;

            hAcc += h * Time.deltaTime;
            fAcc += f * Time.deltaTime;
            vAcc += v * Time.deltaTime;

            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                hAcc = 0;

            if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
                fAcc = 0;

            if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
                vAcc = 0;

            hAcc = Mathf.Clamp(hAcc, -maxSpeedPickup, maxSpeedPickup);
            fAcc = Mathf.Clamp(fAcc, -maxSpeedPickup, maxSpeedPickup);
            vAcc = Mathf.Clamp(vAcc, -maxSpeedPickup, maxSpeedPickup);

            Vector3 s = new Vector3(hAcc, vAcc, fAcc) * speed;
            transform.Translate(s * Time.deltaTime, Space.Self);
        }

        private void Rotate()
        {
			if (Input.GetMouseButton(1))
			{
                float x = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
                float y = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity;
                transform.localEulerAngles = new Vector3(y, x, 0);
            }
        }
    }

}
