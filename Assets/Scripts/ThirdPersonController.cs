using UnityEngine;

namespace ZonelyVoxelEngine
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : MonoBehaviour
    {
        public Transform cameraTransform;
        public Transform visualRoot;

        public float walkSpeed = 4.2f;
        public float sprintSpeed = 6.6f;
        public float jumpHeight = 1.5f;
        public float gravity = -22f;
        public float turnSmooth = 14f;

        CharacterController cc;
        float verticalSpeed;

        public bool InputEnabled { get; set; } = true;

        void Awake()
        {
            cc = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (!InputEnabled || cameraTransform == null)
                return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
            Vector3 move = (camForward * v + camRight * h);
            if (move.sqrMagnitude > 1f) move.Normalize();

            float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

            if (cc.isGrounded && verticalSpeed < 0f)
                verticalSpeed = -2f;

            if (cc.isGrounded && Input.GetKeyDown(KeyCode.Space))
                verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalSpeed += gravity * Time.deltaTime;

            Vector3 velocity = move * speed;
            velocity.y = verticalSpeed;
            cc.Move(velocity * Time.deltaTime);

            if (move.sqrMagnitude > 0.01f && visualRoot != null)
            {
                Quaternion target = Quaternion.LookRotation(move, Vector3.up);
                visualRoot.rotation = Quaternion.Slerp(
                    visualRoot.rotation,
                    target,
                    1f - Mathf.Exp(-turnSmooth * Time.deltaTime)
                );
            }
        }
    }
}
