using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Room;
using static RoomGenerator;

public class FloorGenerator : MonoBehaviour
{
    public Range numberOfRoom = new(5, 10);
    public RoomGenerator roomGenerator;

    public List<Room> rooms = new();

    public void GenerateFloor()
    {
        GameObject floor = new GameObject("Floor");
        int numberOfRooms = Random.Range(numberOfRoom.minimum, numberOfRoom.maximum + 1);
        Room lastRoom = roomGenerator.GenerateRoom();
        lastRoom.transform.parent = floor.transform;
        // Generate bounding box for each part for the first room
        foreach (Vector2 partPos in lastRoom.partsPositions)
        {
            RectInt boundingBox = new();
            boundingBox.x = (int)(partPos.x * lastRoom.partSize.x);
            boundingBox.y = (int)(partPos.y * lastRoom.partSize.y);
            boundingBox.width = (int)(lastRoom.partSize.x);
            boundingBox.height = (int)(lastRoom.partSize.y);

            lastRoom.partSizeBoundingBox.Add(boundingBox);
        }

        Vector3 offset = Vector3.zero;

        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            print("room " + (i + 1));
            rooms.Add(lastRoom);

            Room room = null;
            List<DoorsDiff> possibleConnections = null;
            int randomIndex = -1;
            //Retry 15 times to find a room that can atleast connect to one other room
            for (int depth = 0; depth < 15; depth++)
            {
                randomIndex = UnityEngine.Random.Range(0, rooms.Count - 1);
                room = roomGenerator.GenerateRoom();
                possibleConnections = room.getPossibleConnections(rooms, randomIndex);
                if (possibleConnections.Count > 0)
                {
                    // set room parent to Floor
                    room.transform.parent = floor.transform;
                    break;
                }
                Destroy(room.gameObject);
                print("RETRY: " + depth + 1);
            }

            if (possibleConnections.Count > 0)
            {
                //Pick a random connection
                DoorsDiff randomConnection = possibleConnections[Random.Range(0, possibleConnections.Count - 1)];

                room.offset = new Vector2(randomConnection.offset.x, randomConnection.offset.y) + rooms[randomIndex].offset;
                room.partSizeBoundingBox = randomConnection.partSizeBoundingBox;

                //Remove doors from possible positions
                room.doorsPossiblePositions.Remove(randomConnection.doorPossiblePos);
                lastRoom.doorsPossiblePositions.Remove(randomConnection.otherRoomDoor);

                //Delete walls at door position
                Cell roomCell = room.GetCellAt(randomConnection.doorPossiblePos);
                if (roomCell != null && roomCell.gameObject != null)
                {
                    Destroy(roomCell.gameObject);
                }
                Cell lastRoomCell = lastRoom.GetCellAt(randomConnection.otherRoomDoor);
                if (lastRoomCell != null && lastRoomCell.gameObject != null)
                {
                    Destroy(lastRoomCell.gameObject);
                }

                //Instantiate the door
                GameObject doorInstance = Instantiate(PrefabManager.GetDoor(), randomConnection.doorPossiblePos, Quaternion.identity, room.transform);

                Door door = doorInstance.GetComponent<Door>();

                door.firstRoom = room;
                door.secondRoom = rooms[randomIndex];

                //
                if (room.GetCellAt(new Vector2(randomConnection.doorPossiblePos.x + 1, randomConnection.doorPossiblePos.y)) == null)
                {
                    door.direction = Direction.right;
                }
                else if (room.GetCellAt(new Vector2(randomConnection.doorPossiblePos.x - 1, randomConnection.doorPossiblePos.y)) == null)
                {
                    door.direction = Direction.left;
                }
                else if (room.GetCellAt(new Vector2(randomConnection.doorPossiblePos.x, randomConnection.doorPossiblePos.y + 1)) == null)
                {
                    door.direction = Direction.top;
                }
                else
                {
                    door.direction = Direction.bottom;
                }
                //Add door to room and other room
                room.doors.Add(door);
                lastRoom.doors.Add(door);

                //Move room by its offset
                room.transform.position += new Vector3(room.offset.x, room.offset.y, 0);

                lastRoom = room;
            }
            else
            {
                //SHOULD NOT HAPPEN ANYMORE
                Destroy(room.gameObject);
                throw new System.Exception("No compatible room found");
            }
        }

        rooms.Add(lastRoom);
    }

    public void RegenerateFloor()
    {
        DestroyImmediate(GameObject.Find("Floor"));
        rooms.Clear();
        GenerateFloor();
    }
}
