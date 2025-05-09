using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisualEvents : MonoBehaviour
{
    private BaseEnemy baseEnemy;

    void Start(){
        baseEnemy = GetComponentInParent<BaseEnemy>();
    }

    public void CallShoot(){
        baseEnemy.Shooting(); 
    }

    public void CallDead(){
        baseEnemy.OnDeathAnimationFinished();
    }
}
