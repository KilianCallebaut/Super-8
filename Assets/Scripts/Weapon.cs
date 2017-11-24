using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
	//shooting trigger/turnoff stuff etc.
	private bool shooting = false;
	private int volleysUntilStopShooting = -1;//-1 means don't stop

	//projectile stuff
	public GameObject projectileType = null;
	public float projectileSpawnDistanceFromCenter = 0.0f;
	public Vector3 center = new Vector3(0.0f,0.0f,0.0f);
	private float shotDirection = 0.0f;
	public int projectilesPerShot = 1;

	//accuracy
	public float optimalRangeHint = 100.0f;
	public int spreadMode = SPREAD_MODE_EVEN;
	public const int SPREAD_MODE_EVEN = 0;
	public const int SPREAD_MODE_RANDOM_EQUAL = 1;
	public const int SPREAD_MODE_RANDOM_NORMAL = 2;
	public float spread = 0.0f;

	//reload and ammo stuff
	public int ammoCount = -1;//-1 means infinite
	public float reloadDuration = 1.0f;
	public int volleysUntilReload = 10;//magazine size
	private bool outOfAmmo = false;

	//Volley timing and related information
	public float timeBetweenShots = 1.0f;
	public float timeBetweenVolleys = 1.0f;
	public float timeUntilFirstVolley = 1.0f;
	public int shotsPerVolley = 1;
	private int volleysFired = 0;
	private int shotsFiredThisVolley = 0;

	//concepts :
	//volley: a series of shots
	//shot: firing the weapon once
	//projectile: spawned when firing the weapon

	//each time you shoot the weapon you fire a volley
	//the volley is a series of evenly spaced shots
	//each shot launches a number of projectiles

	// Use this for initialization
	void Start () {

        projectileType = (GameObject) Resources.Load("Bullet");
    }

	public void startShooting() {
		shooting = true;
	}


	public void stopShooting() {
		shooting = false;
		volleysFired = 0;
		shotsFiredThisVolley = 0;
		volleysUntilStopShooting = -1;
		if (state != 4) {
			state = 0;
		}
	}

	public void setShooting(bool toggle) {
		if (toggle)
			startShooting ();
		else
			stopShooting ();
	}


	public void shootOnce() {
		volleysUntilStopShooting = 1;
		startShooting ();
	}
	public void shootXTimes(int x) {
		volleysUntilStopShooting = x;
		startShooting ();
	}

	public void setShootingDirection(float dir) {
		shotDirection = dir;
	}

	public void setShootingDirection(Vector3 dir) {
		float f = Vector3.Angle (new Vector3(1,0,0), dir - transform.position);
		Debug.Log ("" + dir + " becomes " + f);
		setShootingDirection (f * Mathf.Deg2Rad);
	}

	public void reload(int ammo) {
		if (ammoCount >= 0) {
			ammoCount += ammo;
			if (state == 4) {
				state = 0;
			}
			outOfAmmo = false;
		}
	}

	public bool isOutOfAmmo() {
		return outOfAmmo;
	}

    public bool isShooting()
    {
        return shooting;
    }

	public void spawnProjectiles(float dTime) {
		if(projectileType == null)return;
		for (int i = 0; i < projectilesPerShot; ++i) {
			//TODO: calculate spread
			float actualSpread = calculateSpread(i, projectilesPerShot);
			Vector3 direction = new Vector3 (Mathf.Cos (actualSpread+shotDirection), Mathf.Sin (actualSpread+shotDirection), 0.0f);
			GameObject g = Instantiate (projectileType, direction*projectileSpawnDistanceFromCenter+center+transform.localPosition, Quaternion.identity);
			AbstractProjectile a = g.GetComponent<AbstractProjectile> ();
			if (a != null) {
				a.initialUpdate (dTime, this.gameObject, direction);
			} else {
				Debug.Log ("Non-projectile projectile fired");
			}
		}
	}


	public float calculateSpread(int index, int max) {
		float ret = 0.0f;
		float mean = 0.0f;
		switch (spreadMode) {
		case SPREAD_MODE_EVEN:

				if (max > 1) {
					ret = spread / 2;
					float spreadPerIndex = spread / (float)(max - 1);
					ret = ret - spreadPerIndex * index;
				}
				break;
			case SPREAD_MODE_RANDOM_EQUAL:
				//generate random number between -spread/2 and spread/2
				ret = Random.value * spread - spread/2;
				break;
			case SPREAD_MODE_RANDOM_NORMAL:
				//use normal distribution with spread distribution
				float u1 = 1.0f-Random.value; //uniform(0,1] random doubles
				float u2 = 1.0f-Random.value;
				float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
				ret = mean + spread * randStdNormal; //random normal(mean,stdDev^2)
				break;
			default:
				break;
		}
		return ret;
	}

	int state = 0;
	float remainingTime;
	// Update is called once per frame
	void Update () {
		//implemented as a state machine
		if (shooting) {
			float dTime = UnityEngine.Time.deltaTime;
			dTime += remainingTime;
			bool stop = false;
			while (!stop) {
				switch (state) {
				case 0://waiting to start firing
					if (dTime >= timeUntilFirstVolley) {
						dTime -= timeUntilFirstVolley;
						state = 2;
					} else {
						stop = true;
					}
					break;
				case 1://wait to fire volley
					if (volleysFired >= volleysUntilReload) {
						state = 3;
						break;
					}
					if (volleysFired >= volleysUntilStopShooting && volleysUntilStopShooting >= 0) {
						stopShooting();
					}
					if (dTime >= timeBetweenVolleys) {
						dTime -= timeBetweenVolleys;
						state = 2;
					} else {
						stop = true;
					}
					break;	
				case 2://fire a volley
					while (dTime >= timeBetweenShots && shotsFiredThisVolley < shotsPerVolley) {
						spawnProjectiles (dTime);
						shotsFiredThisVolley += 1;
						dTime -= timeBetweenShots;
					}
					if (shotsFiredThisVolley < shotsPerVolley) {
						stop = true;
					} else {
						shotsFiredThisVolley = 0;
						volleysFired += 1;
						state = 1;
					}
					break;
				case 3://wait for reload
					if (ammoCount == 0) {
						state = 4;
					}
					if (dTime >= reloadDuration) {
						dTime -= reloadDuration;
						volleysFired = 0;
						if (ammoCount < 0) {
							ammoCount -= 1;
						}
						state = 1;
					} else {
						stop = true;
					}
					break;
				case 4://out of ammo
					stop = true;
					dTime = 0.0f;
					outOfAmmo = true;
					break;
				default:
					break;//throw exception here
				}
			}
			remainingTime = dTime;
		}
	}
}
