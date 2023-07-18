using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class Door : MonoBehaviour
{

    public Room firstRoom;

    public RoomGenerator.Direction direction;

    public Room secondRoom;

    private bool isChangingRoom = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !isChangingRoom)
        {
            isChangingRoom = true;
            Debug.Log("Player enter door");
            if (other.GetComponent<Player>().currentRoom == firstRoom)
            {
                Debug.Log("Player enter door from first room");
                PlayerController.Instance.currentRoom = secondRoom;
                foreach (Door door in secondRoom.doors)
                {
                    if (door == this)
                    {
                        Vector2 mov = RoomGenerator.MoveInDirection(new Vector2(), (RoomGenerator.Direction)(((int)door.direction)));
                        other.transform.position = secondRoom.transform.position + this.transform.position + new Vector3(mov.x, mov.y, 0);
                        //other.transform.position = RoomGenerator.MoveInDirection(secondRoom.transform.position, door.direction) + secondRoom.offset;
                    }
                }
            }
            else if (other.GetComponent<Player>().currentRoom == secondRoom)
            {
                Debug.Log("Player enter door from second room");
                Debug.Log(other.GetComponent<Player>().currentRoom);
                PlayerController.Instance.currentRoom = firstRoom;
                Debug.Log(other.GetComponent<Player>().currentRoom);
                foreach (Door door in firstRoom.doors)
                {
                    if (door == this)
                    {
                        Vector2 mov = RoomGenerator.MoveInDirection(new Vector2(), (RoomGenerator.Direction)(((int)door.direction + 2) % 4));
                        other.transform.position = secondRoom.transform.position + this.transform.position + new Vector3(mov.x, mov.y, 0);
                    }
                }
            }
            else
            {
                Debug.Log("Player enter door from unknown room");
                Debug.Log(other.GetComponent<Player>().currentRoom);
            }
            isChangingRoom = false;
        }
    }
}
