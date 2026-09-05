using UnityEngine;

namespace ZonelyVoxelEngine
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        public Transform target;
        public float distance = 5.5f;
        public float height = 1.45f;
        public float sensitivity = 2.1f;
        public float minPitch = -10f;
        public float maxPitch = 65f;

        public bool InputEnabled { get; set; } = true;

        float yaw = 180f;
        float pitch = 18f;

        void LateUpdate()
        {
            if (target == null) return;

            if (InputEnabled && Cursor.lockState == CursorLockMode.Locked)
            {
                yaw += Input.GetAxis("Mouse X") * sensitivity;
                pitch -= Input.GetAxis("Mouse Y") * sensitivity;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }

            Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
            Vector3 focus = target.position + Vector3.up * height;
            transform.position = focus + rot * new Vector3(0, 0, -distance);
            transform.LookAt(focus);
        }
    }
}
