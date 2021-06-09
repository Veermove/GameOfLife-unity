using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GM
{
    public class GameMenger : MonoBehaviour
    {
        public int targetFramesPerSecond;

        // Map objects
        GameObject mapObject;
        SpriteRenderer mapRenderer;


        // Dimensions
        public int sizeX = 100; // X size of plane
        public int sizeY = 100; // Y size of plane
        Rect rectangle;


        // Colors of map
        public Color DeadColor;

        public Color AliveColor;

        // Matrix of dead/alive cells
        public bool[,] mainMatrix;

        bool wasPaused = false;


        // Start is called before the first frame update
        void Start()
        {
            rectangle = new Rect(0, 0, sizeX, sizeY);
            Application.targetFrameRate = targetFramesPerSecond;
            createMatrix();
            createPlane();
        }

        // Initializes randomized mainMatirx
        public void createMatrix()
        {
            var Rand = new System.Random();
            mainMatrix = new bool[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(Rand.Next(101) >= 50)
                    {
                        mainMatrix[x, y] = false;
                    }
                    else
                    {
                         mainMatrix[x, y] = true;
                    }


                }
            }
        }

        // Initializes all-false mainMatirx
        public void clearMatrix()
        {
            mainMatrix = new bool[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    mainMatrix[x, y] = false;
                }
            }
        }

        // Initializes gameObjects, shows onscreen first frame
        public void createPlane()
        {
            mapObject = new GameObject("Map");
            mapRenderer = mapObject.AddComponent<SpriteRenderer>();
            Texture2D txt = new Texture2D(sizeX, sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(mainMatrix[x, y])
                    {
                        txt.SetPixel(x, y, AliveColor);
                    }
                    else
                    {
                        txt.SetPixel(x, y, DeadColor);
                    }
                }
            }
            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, sizeX, sizeY);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }

        // Updates onscreen plane, calculates next generation of Dead/Alive cells
        // if changePixel = true -> paintPixel(Vector3 mousePos) will be called
        void UpdatePlane(bool changePixel, Vector3 mousePos)
        {
            //calculte next generatrion
            Rules temp = new Rules();
            mainMatrix = temp.updateMatrix(mainMatrix, sizeX, sizeY);

            //paint given pixel
            if (changePixel)
            {
                paintPixel(mousePos);
            }

            //update onscreen plane
            Texture2D txt = new Texture2D(sizeX, sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(!mainMatrix[x, y])
                    {
                        txt.SetPixel(x, y, DeadColor);
                    }
                    else
                    {
                        txt.SetPixel(x, y, AliveColor);
                    }

                }
            }
            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, sizeX, sizeY);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }


        // Updates onscreen plane WHILE PathPainting cells, does NOT calculate next generation od Dead/Alie cells
        void UpdatePlane()
        {
            //update onscreen plane
            Texture2D txt = new Texture2D(sizeX, sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(!mainMatrix[x, y])
                    {
                        txt.SetPixel(x, y, DeadColor);
                    }
                    else
                    {
                        txt.SetPixel(x, y, AliveColor);
                    }

                }
            }
            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, sizeX, sizeY);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }

        // Updates onscreen plane WHILE IN-GAME PAUSED, does NOT calculate next generation od Dead/Alie cells
        // if changePixel = true -> paintPixel(Vector3 mousePos) will be called
        void UpdatePausedPlane(bool changePixel, Vector3 mousePos)
        {
            if (changePixel)
            {
                paintPixel(mousePos);
            }
            Texture2D txt = new Texture2D(sizeX, sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(!mainMatrix[x, y])
                    {
                        txt.SetPixel(x, y, DeadColor);
                    }
                    else
                    {
                        txt.SetPixel(x, y, AliveColor);
                    }

                }
            }
            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, sizeX, sizeY);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }

        // Update is called once per frame
        void Update()
        {
            bool pixelPiant = false;
            Vector3 mouse = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.R))
            {
                createMatrix();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if(wasPaused)
                {
                    wasPaused = false;
                }
                else
                {
                    wasPaused = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                clearMatrix();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (mouse.x < sizeX/2 && mouse.x > -sizeX/2
                    && mouse.y < sizeY/2 && mouse.y > -sizeY/2)
                {
                    pixelPiant = true;
                }
            } else if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(BrushPixel());
            }
            if(!wasPaused)
            {
                UpdatePlane(pixelPiant, mouse);
            }
            else
            {
                UpdatePausedPlane(pixelPiant, mouse);
            }
        }

        IEnumerator BrushPixel()
        {
            int prevX = 0;
            int prevY = 0;
            bool firstIteratrion = true;
            while (true)
            {
                IntVector3 currentPos = new IntVector3(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                // Debug.Log(currentPos.x + ", " + currentPos.y);
                int xPos = currentPos.x + sizeX/2;
                int yPos = currentPos.y + sizeY/2;

                if (xPos >= 0 && xPos < sizeX && yPos >= 0 && yPos < sizeY)
                {
                    if (firstIteratrion)
                    {
                        firstIteratrion = false;
                        prevX = xPos;
                        prevY = yPos;
                    }

                    mainMatrix[xPos, yPos] = true;
                    Queue<IntVector3> pointsAlong = AproximatePoints(xPos, yPos, prevX, prevY);
                    while(pointsAlong.Count != 0)
                    {
                        IntVector3 temp = pointsAlong.Dequeue();
                        if(temp.x >= 0 && temp.x < sizeX &&  temp.y >= 0 &&  temp.y < sizeY)
                        {
                            mainMatrix[temp.x, temp.y] = true;
                        }
                    }
                    UpdatePlane();
                    prevX = xPos;
                    prevY = yPos;
                }
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    break;
                }
                yield return null;
            }
        }

        // Aproximates points between along edge
        public Queue<IntVector3> AproximatePoints (int curX, int curY, int prevX, int prevY)
        {
            Queue<IntVector3> additional = new Queue<IntVector3>();
            if(curX == prevX && curY == prevY)
            {
                return additional;
            }
            int distance = (int)Math.Sqrt( (double)((curX - prevX) * (curX - prevX))  +  (double)((curY - prevY) * (curY - prevY)) );
            int count = distance;
            int d = distance / count;
            double angle = Math.Atan2(curX - prevX, curY - prevY);
            for (int i = 0; i < count; i++)
            {
                additional.Enqueue(new IntVector3((int)Math.Floor(curX + i * d * Math.Cos(angle)), (int)Math.Floor(curY + i * d * Math.Cos(angle)), 0));
            }
            return additional;
        }

        // Hi im trying to paint "pixels". I created Conway's game of life in Unity and i want to add a feature where you press mouse button and it's pressed you "paint" - set cells alive. So my idea was:
        // In Update() if mouse button is pressed Start Coroutine
        // In Corutine you have loop that sets cell pointed by Input.mousePosition to Alive-state then waits for end of frame. Loop, and by that Coroutine ends when that mouse button is released.
        // My problem is that if you move mouse rapidly created line will not be continous, becaouse inputs form mouse from two frames will be different (far apart).
        // Since you can take Input.mousePosition only once per frame i tried aproximating this by storing mousePosition from previous frame and calculating all points that lie on Edge betweem CurrentMousePosition and PreviousMousePosition
        // However i was not happy with the resault.
        // My Question is: is there a better way to prevent this un-contnous line than letting it be and then fixing it? And if not is there a better way to aproximate points that lie on Edge?

        // sets cell pointed by Vector3 to Alive in mainMatrix
        // Vector3 mouse should be given with offset +size/2
        public void paintPixel(Vector3 mouse)
        {
            int xPos = (int)(mouse.x + sizeX/2);
            int yPos = (int)(mouse.y + sizeY/2);
            if (xPos >= 0 && xPos < sizeX && yPos >= 0 && yPos < sizeY)
            {
                if (mainMatrix[xPos, yPos])
                {
                    mainMatrix[xPos, yPos] = false;
                }
                else
                {
                    mainMatrix[xPos, yPos] = true;
                }

            }
        }
    }

}
