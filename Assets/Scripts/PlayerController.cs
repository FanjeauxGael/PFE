
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float mouseSensitivity = 7f;

    private PlayerMotor motor;

    private Animator animator;

    private void Start()
    {
        //récupere PlayerMotor 
        motor = GetComponent<PlayerMotor>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Calculer la vitesse du mouvement de notre joueur
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorizontal + moveVertical) * speed;

        // Jouer animation
        animator.SetFloat("ForwardVelocity", zMov);

        motor.Move(velocity);

        // Calcul la rotation du joueur en un Vector3
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivity;

        motor.Rotate(rotation);

        // Calcul la rotation de la caméra en un Vector3
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 cameraRotation = new Vector3(xRot, 0, 0) * mouseSensitivity;

        motor.RotateCamera(cameraRotation);
    }

}
