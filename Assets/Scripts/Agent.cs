using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    [SerializeField]
    private float speed;

    private Vector3 destination;

    // Use this for initialization
    void Start() {
        moveToRandomDestination();
    }

    // Update is called once per frame
    void Update() {
        moveToDirection();
    }

    private void moveToDirection()
    {
        if (transform.position != destination) {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        } else
        {
            moveToRandomDestination();
        }

    }

    void moveToRandomDestination()
    {
        int index = Random.Range(0, ((int) LevelManager.Instance.MaxXTiles) * ((int) LevelManager.Instance.MaxYTiles));

        GameObject tile = (GameObject) LevelManager.Instance.Tiles.GetValue(index);
        destination = tile.transform.position;
    }

}
