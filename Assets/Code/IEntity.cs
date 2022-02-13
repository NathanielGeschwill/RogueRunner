using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IEntity : MonoBehaviour
{
    protected int health = 1;
    protected int maxHealth = 1;
    protected int damage = 0;

    public static event EventHandler<int> OnDeath;

    //public delegate void Hit(int damage);
    public static event EventHandler<int> OnHit;
    //protected GameObject thisGameObject;
    //public string ID;

    //protected List<string> tagsThatHitMe;
    protected List<string> tagsICanHit;

    private void Start()
    {
        //ID = gameObject.name + Time.time;
    }

    protected virtual void OnEnable()
    {
        IEntity.OnHit += LoseHealth;
        IEntity.OnDeath += ResolveDeath;
    }

    protected virtual void OnDisable()
    {
        IEntity.OnHit -= LoseHealth;
        IEntity.OnDeath -= ResolveDeath;
    }

    protected virtual void ResolveDeath(object sender, int senderID)
    {
        print("RESOLVING");
        if(senderID == gameObject.GetInstanceID())
        {
            print("DESTORYING " + senderID);
            Destroy(gameObject);
        }
    }

    protected void KillMe()
    {
        Destroy(gameObject);
    }

    protected virtual void LoseHealth(object hitObject, int amount)
    {
        //print(((GameObject)hitObject).name);
        //print(gameObject.name);
        //print("COMPAING " + ((GameObject)hitObject).GetInstanceID() + " AND " + gameObject.GetInstanceID());
        if (((GameObject)hitObject).GetInstanceID() == gameObject.GetInstanceID())
        {
            print("FOUND MATCH" + gameObject.name);
            health -= amount;

            if (health <= 0)
            {
                //print("INVOKING DEATH");
                OnDeath?.Invoke(gameObject, gameObject.GetInstanceID());
            }
        }
    }

    protected virtual bool GainHealth(object sender, int amount)
    {
        if(health + 1 <= maxHealth)
        {
            health += amount;
            return true;
        }
        return false;
    }

    protected void InvokeHit(GameObject g, int damage)
    {
        OnHit?.Invoke(g, damage);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //print("TRIGGERED");
        foreach(string s in tagsICanHit)
        {
            if (other.gameObject.CompareTag(s) && damage > 0)
            {
                //print("GOING INVOKE " + other.gameObject);
                OnHit?.Invoke(other.gameObject, damage);
                break;
            }
            else
            {
                print("didn't find");
            }
        }
    }

    protected void FireOnHit(GameObject gameObj)
    {
        OnHit?.Invoke(gameObj, damage);
    }
}
