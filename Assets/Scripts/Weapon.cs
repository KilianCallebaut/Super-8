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
    private Vector3 shotDirection;
	public int projectilesPerShot = 1;

	//accuracy
	public float optimalRangeHint = 100.0f;
	public int spreadMode = SPREAD_MODE_EVEN;
	public const int SPREAD_MODE_EVEN = 0;
	public const int SPREAD_MODE_RANDOM_EQUAL = 1;
	public const int SPREAD_MODE_RANDOM_NORMAL = 2;
	public float spread = 0.0f;//spread angle in radians

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
	public int shotsFiredThisVolley = 0;

	//concepts :
	//volley: a series of shots
	//shot: firing the weapon once
	//projectile: spawned when firing the weapon

	//each time you shoot the weapon you fire a volley
	//the volley is a series of evenly spaced shots
	//each shot launches a number of projectiles

	// Use this for initialization
	void Start () {

        if(projectileType == null)
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



	public void setShootingDirection(Vector3 dir) {
        shotDirection = (dir - transform.position).normalized;
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
            //Vector3 direction = new Vector3 (Mathf.Cos (actualSpread+shotDirection), Mathf.Sin (actualSpread+shotDirection), 0.0f);
            
            Vector3 direction = Quaternion.Euler(0.0f, 0.0f, actualSpread)* shotDirection  ;

			GameObject g = Instantiate (projectileType, direction*projectileSpawnDistanceFromCenter+center+transform.position, Quaternion.identity);
            
            AbstractProjectile a = g.GetComponent<AbstractProjectile> ();
            Agent ag = gameObject.GetComponentInParent<Agent>();

			if (a != null) {
				a.initialUpdate ( ag, direction);
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

	//******************************************************
	//pre-defined functions to generate specific weapons here

	public void beShadowPistol() {
		shotsPerVolley = 1;
		volleysUntilReload = 9;
		timeUntilFirstVolley = 0;
		timeBetweenShots = 0;
		timeBetweenVolleys = 0.8f;
		spread = 10;
		reloadDuration = 3.0f;
		spreadMode = SPREAD_MODE_RANDOM_EQUAL;
		projectilesPerShot = 1;
		optimalRangeHint = 200 / (spread*Mathf.Rad2Deg);
		projectileType = Resources.Load ("PistolBullet") as GameObject;
	}

	public void beHeavyMinigun() {
		shotsPerVolley = 30;
		volleysUntilReload = 10;
		timeUntilFirstVolley = 5;
		timeBetweenShots = 0.1f;
		timeBetweenVolleys = 2;
		spread = 10;
		reloadDuration = 10.0f;
		spreadMode = SPREAD_MODE_RANDOM_NORMAL;
		projectilesPerShot = 1;
		optimalRangeHint = 200 / (spread*Mathf.Rad2Deg);
		projectileType = Resources.Load ("MGBullet") as GameObject;
	}

	public void beAssaultShotgun() {
		shotsPerVolley = 2;
		volleysUntilReload = 3;
		timeUntilFirstVolley = 3;
		timeBetweenShots = 0.5f;
		timeBetweenVolleys = 2;
		spread = 45;
		reloadDuration = 5.0f;
		spreadMode = SPREAD_MODE_EVEN;
		projectilesPerShot = 5;
		optimalRangeHint = 200 / (spread*Mathf.Rad2Deg);
		projectileType = Resources.Load ("SGBullet") as GameObject;
	}

	public void beSoldierRifle() {
		shotsPerVolley = 8;
		volleysUntilReload = 3;
		timeUntilFirstVolley = 2;
		timeBetweenShots = 0.1f;
		timeBetweenVolleys = 1.0f;
		spread = 0;
		reloadDuration = 5.0f;
		spreadMode = SPREAD_MODE_RANDOM_NORMAL;
		projectilesPerShot = 1;
		optimalRangeHint = 200 / (spread*Mathf.Rad2Deg);
		projectileType = Resources.Load ("MGBullet") as GameObject;
	}

	public void beSniperRifle() {
		shotsPerVolley = 1;
		volleysUntilReload = 3;
		timeUntilFirstVolley = 3;
		timeBetweenShots = 0;
		timeBetweenVolleys = 3;
		spread = 1;
		reloadDuration = 10.0f;
		spreadMode = SPREAD_MODE_RANDOM_NORMAL;
		projectilesPerShot = 1;
		optimalRangeHint = 200 / (spread*Mathf.Rad2Deg);
		projectileType = Resources.Load ("SniperBullet") as GameObject;
	}

	public void shapeAfterRole(AgentBehaviour s) {
		//TODO: load ammunition type somewhere
		if (s is AssaultRole) {
			beAssaultShotgun ();
		} else if (s is HeavyRole) {
			beHeavyMinigun ();
		}
	}
}
