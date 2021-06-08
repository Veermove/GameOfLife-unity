using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GM
{
    public class GameMenger : MonoBehaviour
    {
        Camera MainCamera;
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

        // Initializes mainMatirx, fills with false
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

        void UpdatePlane(bool changePixel, Vector3 mousePos)
        {
            Rules temp = new Rules();
            mainMatrix = temp.updateMatrix(mainMatrix, sizeX, sizeY);
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

            // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition) + ", -> " + Input.mousePosition);
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

        // sets cell pointed by Vector3 to Alive in mainMatrix
        // Vector3 mouse should be given by with offset +size/2
        public void paintPixel(Vector3 mouse)
        {

            // int xPos = (int)Math.Ceiling(mouse.x + sizeX/2);
            // int yPos = (int)Math.Ceiling(mouse.y + sizeY/2);
            int xPos = (int)(mouse.x + sizeX/2);
            int yPos = (int)(mouse.y + sizeY/2);
            Debug.Log(xPos + "   xD " + yPos);
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
