using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] int hp = 10;

    protected abstract void Attack();
    protected abstract void Idle();
    protected abstract void TrackPlayer();

    public virtual void TakeDamage(int value) {
        hp -= value;
        if (hp < 0) {
            gameManager.EnemyKilled();
            Destroy(this.gameObject);
        }
    }
}
