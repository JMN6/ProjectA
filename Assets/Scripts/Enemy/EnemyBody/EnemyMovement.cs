using UnityEngine;

public class EnemyMovement : Movement
{
    private bool _isThereVehicle;
    private Rigidbody2D _vehicle;

    private void FixedUpdate()
    {
        if(_isThereVehicle == true)
            ModifyVelocityAC2Vehicle();
    }

    private void ModifyVelocityAC2Vehicle()
    {
        Vector2 newVel = rigid.linearVelocity;

        // ÁÂ¿ì
        newVel.x = Mathf.Max(newVel.x, _vehicle.linearVelocityX);

        // »óÇÏ
        newVel.y = Mathf.Max(newVel.y, _vehicle.linearVelocityY);

        SetVelocity(newVel);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody != null && 
            collision.contacts.Length > 0 &&
            collision.GetContact(0).normal.y > 0.7f &&
            collision.rigidbody.TryGetComponent<MovingPlatforms>(out var p))
        {
            _vehicle = collision.rigidbody;
            _isThereVehicle = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.rigidbody == _vehicle)
        {
            _vehicle = null;
            _isThereVehicle = false;
        }
    }
}
