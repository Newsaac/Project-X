using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] EnemyStats stats;

    protected int hp;
    protected FloatingStatusBar healthBar;

    protected void Awake() {
        healthBar = GetComponentInChildren<FloatingStatusBar>();
    }

    protected void Start() {
        hp = stats.maxHp;
        healthBar.UpdateValue(hp, stats.maxHp);
    }

    protected abstract void Attack();
    protected abstract void Idle();
    protected abstract void TrackPlayer();

    public virtual void TakeDamage(int value) {
        hp -= value;
        healthBar.UpdateValue(hp, stats.maxHp);
        if (hp <= 0) {
            gameManager.EnemyKilled();
            Destroy(this.gameObject);
        }
    }
}
