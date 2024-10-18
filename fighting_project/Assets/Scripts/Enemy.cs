using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float Y_Pos { get => _y_pos; }
    private float _y_pos;

    [SerializeField] private GameObject _basePosition;
    [SerializeField] private Animator _animator;
    void Start()
    {
        
    }

    void Update()
    {
        _y_pos = _basePosition.transform.position.y;
    }

    public void TakeDamage()
    {
        StartCoroutine(WaitForAnimation());
    }
    private IEnumerator WaitForAnimation()
    {
        _animator.Play("Hurt");
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);
    }
}
