using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyStats stats;

    protected int hp;
    protected FloatingStatusBar healthBar;
    protected GameManager gameManager;

    protected void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
