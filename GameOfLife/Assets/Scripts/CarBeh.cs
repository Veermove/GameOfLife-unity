using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GM;

using System.IO;
using System.Threading.Tasks;


public class CarBeh : MonoBehaviour
{
    // Information if car is already created
    bool isCarCreated = false;

    // Vector representing car position
    Vector2 carPos;

    // Car color
    public Color CarColor;

    // Dictionary of unitVectors on a grid-based plane:
    // UpperLeft, Upper, UpperRight, Left, Right, LowerLeft, Lower, LowerRight
    Dictionary<int, Vector2> UnitVectorTable;

    // How many pixels/cells in front of a car are checked for collisions
    public int searchAhead = 3;

    // matrix of Dead/Alive cells. Represents entire plane
    bool[,] view;

    // Sizes of original plane
    int sizeX;
    int sizeY;

    // Object of mainGame; used for communication between scripts
    GameMenger mainGame;

    // Start is called before the first frame update
    void Start()
    {
        // "Connecting" to GameMenger.cs script
        mainGame = GameObject.Find("GameMenager").GetComponent<GameMenger>();
        Application.targetFrameRate = mainGame.targetFramesPerSecond;

        // Setting sizes
        sizeX = mainGame.sizeX;
        sizeY = mainGame.sizeY;

        // Loading unit vectors into dictionary
        UnitVectorTable = new Dictionary<int, Vector2>();
        UnitVectorTable.Add(0, new Vector2(-1, -1));
        UnitVectorTable.Add(1, new Vector2(-1, 0));
        UnitVectorTable.Add(2, new Vector2(-1, 1));
        UnitVectorTable.Add(3, new Vector2(0, -1));
        UnitVectorTable.Add(4, new Vector2(0, 1));
        UnitVectorTable.Add(5, new Vector2(1, -1));
        UnitVectorTable.Add(6, new Vector2(1, 0));
        UnitVectorTable.Add(7, new Vector2(1, 1));
    }

    // Update is called once per frame
    void Update()
    {
        // Update "view" matrix
        view = mainGame.getMatrix();

        // If there is no car upon pressing "O" and other conditions are met car is created:
        // Other conditions: Mouse is positioned inside plane
        if (Input.GetKeyDown(KeyCode.O) && !isCarCreated)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mouse.x < sizeX/2 && mouse.x > -sizeX/2
                && mouse.y < sizeY/2 && mouse.y > -sizeY/2)
            {
                // translating from World point to Matrix addres
                carPos = new Vector2();
                carPos.x = mouse.x + sizeX/2;
                carPos.y = mouse.y + sizeY/2;
                isCarCreated = true;
                StartCoroutine(Pathfinder());
            }
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            char[] retVal = new char[sizeX * sizeY];
            int iterator = 0;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (view[x, y])
                    {
                        retVal[iterator] = 'a';
                    }
                    else
                    {
                        retVal[iterator] = 'z';
                    }
                    iterator++;
                }
            }
            string ret = new string(retVal);
            string[] lines = new string[2];
            lines[0] = ret;
            lines[1] = "";
            File.WriteAllLines("hello.txt", lines);
        }
    }

    // Corutine that "drives the car". Finds path, and sends carPosition to GameMenger for painting everyFrame
    // Also responsible for stopping the car and destroying it upon pressing P - Park
    IEnumerator Pathfinder()
    {

        Vector2 nextMove = new Vector2();
        Vector2 Delta = new Vector2(1, 0);

        while (true)
        {
            // Drive randomly, at evry frame there is 95% chance that car will NOT make a turn
            if (changeDirection(95))
            {
                Delta = randUnitVector(Delta);
            }
            nextMove = carPos + Delta;

            // Check for Collisions ahead
            bool wasCollision = checkForCollision(nextMove, Delta);

            // if Collision was found change direction
            if (wasCollision)
            {
                Vector2 newDirection = Vector2.zero;
                for (int i = 0; i < 8; i++)
                {
                    // Make sure that new direction will not be a collision
                    UnitVectorTable.TryGetValue(i, out newDirection);
                    if (!checkForCollision(carPos + newDirection, newDirection))
                    {
                        break;
                    }
                }
                // if there is no way out/ car is sorrouned from all sides throw Exception
                // Unless plane is completly full - this shouldn't happen
                if (newDirection == Vector2.zero)
                {
                    throw new ArgumentException();
                }
                Delta = newDirection;
                nextMove = carPos + Delta;
            }

            // Send information to GameMenger for painting
            mainGame.DrawCar(normalize(nextMove), CarColor);
            carPos = nextMove;

            // If "P" is pressed - stop and park the car
            if (Input.GetKeyDown(KeyCode.P))
            {
                resetAll();
                break;
            }
            yield return null;
        }
    }

    // resets state of a car, allowing for creation of a new one
    void resetAll()
    {
        isCarCreated = false;
    }


    // Checks if Vector2 PlannedMove would collide with cell that is alive
    // Then checks all cells in range of "searchAhead"
    // If any cell is around a "live" cell it counts as a Collision
    // CheckedCell(t) = plannedMove + Delta * t
    bool checkForCollision(Vector2 plannedMove, Vector2 Delta)
    {
        for (int i = 0; i < searchAhead; i++)
        {
            Vector2 temp = normalize(plannedMove);
            int wallsAround = calculateAliveAround(temp);
            if (wallsAround > 0)
            {
                return true;
            }
            temp += Delta;
        }
        return false;
    }


    // Given positional vector returns number of alive cells around of point given by Vector2
    int calculateAliveAround(Vector2 planned)
    {
        int x = (int)planned.x;
        int y = (int)planned.y;
        int counter = 0;
        for (int xDev = -1; xDev < 2; xDev++)
        {
            for (int yDev = -1; yDev < 2; yDev++)
            {
                 if (view[(sizeX + xDev + x) % sizeX, (sizeY + yDev + y) % sizeY])
                {
                    counter++;
                }
            }
        }
        return counter;
    }

    // Generates Vector2 of Length = 1 in semi-random direction
    // Generated vector cannot be equal to given vector, nor can it be negation of given vector
    // That ensures that returned vector is not the same as current vector and that car will not randomly turn 180deg at once
    Vector2 randUnitVector(Vector2 UnitVector)
    {
        Vector2 forbidden = UnitVector;
        forbidden.x = -forbidden.x;
        forbidden.y = -forbidden.y;
        System.Random rand = new System.Random();
        Vector2 retVal = Vector2.zero;
        while(true)
        {
            UnitVectorTable.TryGetValue(rand.Next(0, 8), out retVal);
            if (retVal != forbidden && retVal != UnitVector)
            {
                break;
            }
        }
        return retVal;
    }

    // returns false givenArgument/100 % of the time
    // if user passes percent = 27, there is 27% that method will return false
    bool changeDirection(int percent)
    {
        System.Random rand = new System.Random();
        return rand.Next(0, 100) > percent;
    }


    // "Normalizes" vector, turns it into vector that can be used as matrix Addres
    // operation (q + sizeQ) % sizeQ ensures that there won't negative addreses
    Vector2 normalize(Vector2 given)
    {
        given.x = (int)((given.x + sizeX) % sizeX);
        given.y = (int)((given.y + sizeY) % sizeY);
        return given;
    }
}
