using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractProjectileCollidable : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}

	public float hitChanceMultiplier = 1.0f;//maybe disregard, maybe lower etc.

	//dist is a measure of distance traveled
	//time is a measure of lifetime of projectile
	//frac is a measure of (distance/lifetime) divided by (total_distance/total_lifetime) (pick best option)
	public abstract bool bulletCollides(float dist, float time, float frac);//returning false here will make the projectile assume it was a miss and disappear
	public abstract void receiveDamage(int damage);//receive X points of damage

	// Update is called once per frame
	void Update()
	{

	}
}