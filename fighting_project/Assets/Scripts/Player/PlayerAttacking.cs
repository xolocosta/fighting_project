using UnityEngine;

public class PlayerAttacking : MonoBehaviour
{
    private Player _player_attack;

    private void Start()
    {
        _player_attack = GetComponentInParent<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            if (collision.TryGetComponent<Enemy>(out var enemy))
            {
                bool canHit = Mathf.Abs(enemy.Y_Pos - _player_attack.Y_Pos) <= 1.5f;
                if (canHit)
                {
                    if (collision.TryGetComponent(out IDamageable dam))
                        dam.TakeDamage();

                    Debug.Log($"{Time.time}, hit gameObject {collision.gameObject.name}");
                }
                else
                {
                    Debug.Log($"{Time.time}, Player Y_Pos: {_player_attack.Y_Pos} || Enemy Y_Pos: {enemy.Y_Pos}");
                }
            }
        }
    }
}
