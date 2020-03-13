using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLevelWithRooms : GridLevel
{
    Stack<Room> unplacedRooms;
    float CHANCE_OF_ROOM = 0.9f;
    int iteration = 0;
    bool includeUnreachables = false;

    public GridLevelWithRooms(int width, int height) : base(width, height)
    {
        // populate the rooms stack
        unplacedRooms = new Stack<Room>();
        int numRooms = 20;
        for (int i=0; i < numRooms; i++)
        {
            Room room = new Room();
            room.width = (int)Random.Range(3f, 4.99f);
            room.height = (int)Random.Range(3f, 4.99f);
            unplacedRooms.Push(room);
        }

        if (includeUnreachables)
        {
            int numCells = width * height;
            int numUnreachable = (int)(numCells * 0.05f);
            for (int i = 0; i< numUnreachable; i++)
            {
                int x = (int)Random.Range(0f, width-1);
                int y = (int)Random.Range(0f, height-1);
                cells[x, y].inMaze = true;
            }
        }
    }

    bool canPlaceRoom(Room room, int x, int y)
    {
        bool inBounds = (x >= 0) && (x < (mWidth - room.width)) && (y >= 0) && (y < (mHeight - room.height));
        if (!inBounds)
        {
            return false;
        }

        for (int rx = x; rx < x + room.width; rx++)
        {
            for (int ry = y; ry < y + room.height; ry++)
            {
                if (cells[rx,ry].inMaze)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void addRoom(Room room, Location location)
    {
        for (int x = location.x; x < location.x + room.width; x++)
        {
            for (int y = location.y; y < location.y + room.height; y++)
            {
                cells[x, y].inMaze = true;
                // we are using connections to determine where walls
                // are drawn, so set all connections in the room
                // directions are listed in this order: +x, +y, -y, -x
                if (x != location.x + room.width - 1)
                {
                    cells[x, y].directions[0] = true;
                    cells[x + 1, y].directions[3] = true;
                }
                if (y != location.y + room.height - 1)
                {
                    cells[x, y].directions[1] = true;
                    cells[x, y + 1].directions[2] = true;
                }
            }
        }
    }

    public override Location makeConnection(Location location)
    {
        iteration++;

        // try to fit a room
        if (unplacedRooms.Count > 0 && iteration > 5 && (Random.Range(0f, 1.0f) < CHANCE_OF_ROOM))
        {
            int x = location.x;
            int y = location.y;

            // choose a room and work out its origin
            //Room room = unplacedRooms.Pop();
            Room room = unplacedRooms.Peek();
            Vector3 v = NEIGHBORS[(int)Random.Range(0f, 3.99f)];
            // make it easier to compare with Millington p. 707
            int dx = (int)v.x;
            int dy = (int)v.y;
            int dirn = (int)v.z;

            int nx = x + dx;
            int ny = y + dy;
            if (dx < 0)
            {
                nx -= (room.width-1);
            }
            if (dy < 0)
            {
                ny -= (room.height-1);
            }

            if (canPlaceRoom(room, nx, ny))
            {
                unplacedRooms.Pop();
                // fill the room
                addRoom(room, new Location(nx, ny));

                // perform the connection
                cells[x, y].directions[dirn] = true;
                cells[x + dx, y + dy].directions[3 - dirn] = true;
                //Debug.Log("connected " + x + "," + y + " to " + (x + dx) + "," + (y + dy));

                // return nothing if rooms aren't part of the main
                // maze. otherwise you might return the room exit.
                // NOTE: returning null here can result in very short "mazes" if we fit a room early
                return null;
            }
        }

        // otherwise go through the neighbors as before
        return base.makeConnection(location);
    }
}

public class Room
{
    public int width;
    public int height;
}
