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

        // Generate first room
        Room lastRoom = roomGenerator.GenerateRoom();
        lastRoom.ComputePartSizeBoundingBox();
        lastRoom.transform.parent = floor.transform;
        rooms.Add(lastRoom);


        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            Room room = null;
            List<DoorsDiff> possibleConnections = null;
            Room matchingRoom = null;
            List<int> roomIndexes = new(rooms.Count);
            for (int j = 0; j < rooms.Count; j++)
                roomIndexes.Add(j);

            // Generate a room and try to place it up to four times on the floor
            for (int depth = 0; depth < 4; depth++)
            {
                List<int> roomIndexesCopy = new(roomIndexes);
                room = roomGenerator.GenerateRoom();

                // Try every room for a matching door
                while (roomIndexesCopy.Count > 0)
                {
                    int idxIndex = Random.Range(0, roomIndexesCopy.Count - 1);
                    matchingRoom = rooms[roomIndexesCopy[idxIndex]];

                    possibleConnections = room.getPossibleConnections(rooms, matchingRoom);
                    roomIndexesCopy.RemoveAt(idxIndex);
                    if (possibleConnections.Count > 0)
                        break;
                }
                if (possibleConnections.Count > 0)
                    break;
                Destroy(room.gameObject);
            }

            // Skip this room
            if (possibleConnections.Count == 0)
            {
                Destroy(room.gameObject);
                throw new System.Exception("No compatible room found");
            }


            //Pick a random connection
            DoorsDiff randomConnection = possibleConnections[Random.Range(0, possibleConnections.Count - 1)];

            room.offset = new Vector2(randomConnection.offset.x, randomConnection.offset.y) + matchingRoom.offset;
            room.partSizeBoundingBox = randomConnection.partSizeBoundingBox;

            //Remove doors from possible positions
            room.doorsPossiblePositions.Remove(randomConnection.doorPossiblePos);
            matchingRoom.doorsPossiblePositions.Remove(randomConnection.otherRoomDoor);

            //Delete walls at door position
            Cell roomCell = room.GetCellAt(randomConnection.doorPossiblePos);
            if (roomCell != null && roomCell.gameObject != null)
                Destroy(roomCell.gameObject);
            Cell lastRoomCell = matchingRoom.GetCellAt(randomConnection.otherRoomDoor);
            if (lastRoomCell != null && lastRoomCell.gameObject != null)
                Destroy(lastRoomCell.gameObject);

            //Instantiate the door
            Door door = Instantiate(PrefabManager.GetDoor(), randomConnection.doorPossiblePos, Quaternion.identity, room.transform).GetComponent<Door>();

            door.firstRoom = room;
            door.secondRoom = matchingRoom;
            door.direction = (Direction)room.getFreeDirectionFromDoor(randomConnection.doorPossiblePos);
            //Add door to room and other room
            room.doors.Add(door);
            matchingRoom.doors.Add(door);

            //Move room by its offset
            room.transform.parent = floor.transform;
            room.transform.position += new Vector3(room.offset.x, room.offset.y, 0);

            rooms.Add(room);
        }
        GameManager.Instance.ChangeRoom(rooms[0]);
    }

    public void RegenerateFloor()
    {
        DestroyImmediate(GameObject.Find("Floor"));
        rooms.Clear();
        GenerateFloor();
    }
}
