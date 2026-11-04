using UnityEngine;

// Black bullet fired by the handgun. On any hit the bullet disappears;
// if it hit a flying sphere, that sphere disappears too.

public class GunBullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BoardTrigger board = other.GetComponentInParent<BoardTrigger>();
        if (board == null) return;
        board.OnShot();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        FlyingSphere target = collision.collider.GetComponentInParent<FlyingSphere>();
        if (target != null) Destroy(target.gameObject);
        Destroy(gameObject);
    }
}
