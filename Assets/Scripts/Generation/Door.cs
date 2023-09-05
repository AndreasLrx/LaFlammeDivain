using UnityEngine;

public class Door : MonoBehaviour
{

    public Room firstRoom;

    public RoomGenerator.Direction direction;

    public Room secondRoom;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            Room destRoom;


            if (!player || player.currentRoom.remainingEnemies > 0)
                return;
            if (player.currentRoom == firstRoom)
                destRoom = secondRoom;
            else if (player.currentRoom == secondRoom)
                destRoom = firstRoom;
            else
                return;

            RoomGenerator.Direction direction = this.direction;
            if (destRoom.GetCellAt(RoomGenerator.MoveInDirection(transform.position - destRoom.transform.position, this.direction)) == null)
                direction = RoomGenerator.OppositeDirection(direction);
            Vector2 mov = RoomGenerator.MoveInDirection(new Vector2(), direction);
            GameManager.Instance.ChangeRoom(destRoom, transform.position + new Vector3(mov.x, mov.y, 0));
        }
    }
}
