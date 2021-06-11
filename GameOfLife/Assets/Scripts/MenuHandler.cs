using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{

    public int targetFramesPerSecond;
    public int animationSpeed = 2;
    public int animationProgress = 5;

    // Map objects
    GameObject mapObject;
    SpriteRenderer mapRenderer;

    // Dictionary with 4 gliders
    Dictionary<int, bool[,]> glidersReference;

    // Minimum distance from previously generated glider
    public int distanceFromGliders = 15;

    // checks if disappearing animation is started
    bool animationStarted = false;

    // Number of initially generated gliders in the background
    public int numberOfGliders = 5;


    // Dimensions
    public int sizeX = 100; // X size of plane
    public int sizeY = 100; // Y size of plane

    // Colors of map
    public Color DeadColor;

    public Color AliveColor;

    // Matrix of dead/alive cells
    public bool[,] mainMatrix;
    public bool[,] background;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFramesPerSecond;
        mainMatrix = new bool[sizeX, sizeY];
        background = new bool[sizeX, sizeY];
        feelGlidersDictionary();
        for (int xi = 0; xi < sizeX; xi++)
        {
            for (int yi = 0; yi < sizeY; yi++)
            {
                background[xi, yi] = false;
            }
        }
        randomGlidersFill(numberOfGliders);
        string text = System.IO.File.ReadAllText(@"./PlayExitSetting.txt");
        char[] textChar = text.ToCharArray();
        int x = 0;
        int y = 0;
        int i = 0;
        while(true)
        {
            if (textChar[i] == 'E')
            {
                break;
            }
            if (textChar[i] == 'z')
            {
                mainMatrix[x % sizeX, y % sizeY] = false;
            }
            else
            {
                mainMatrix[x % sizeX, y % sizeY] = true;
            }
            i++;
            y++;
            if(y % sizeX == 0)
            {
                x++;
            }
        }
        createPlane();
    }

    // Update is called once per frame
    void Update()
    {
        GM.Rules temp = new GM.Rules();
        background = temp.updateMatrix(background, sizeX, sizeY);
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.x += sizeX/2;
            mouse.y += sizeY/2;
            if(mouse.x > 13 && mouse.x < 54 && mouse.y < 80 && mouse.y > 66)
            {
                animationStarted = true;
                mainMatrix = mergeMatrices(mainMatrix, background);
                StartCoroutine(RulesOfLife());
            }
            else if (mouse.x > 13 && mouse.x < 54 && mouse.y < 64 && mouse.y > 51)
            {
                Debug.Log("exit");
                Application.Quit();
            }
        }
        if (!animationStarted)
        {
            updatePlane(mergeMatrices(mainMatrix, background));
        }
        else
        {
            updatePlane(mainMatrix);
        }
    }

    //update onscreen plane
    void updatePlane(bool[,] matrix)
    {
        Texture2D txt = new Texture2D(sizeX, sizeY);
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if(!matrix[x, y])
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

    // Coroutin responsible for handling evolutions after pressing MB1, disappearing animation and loading next level
    IEnumerator RulesOfLife()
    {
        int frames = 0;
        int animationY = sizeY - 1 - animationProgress;
        GM.Rules temp = new GM.Rules();
        while (true)
        {
            mainMatrix = temp.updateMatrix(mainMatrix, sizeX, sizeY);
            frames++;
            if (frames % animationSpeed == 0)
            {
                if (animationY <= 0)
                {
                    break;
                }
                for (int y = sizeY - 1; y > animationY; y--)
                {
                    for(int x = 0; x < sizeX; x++)
                    {
                        mainMatrix[x, y] = false;
                    }
                }
                animationY -= animationProgress;

            }
            yield return null;
        }
        bool [,] teporal = new bool[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                teporal[x,y] = false;
            }
        }
        updatePlane(teporal);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Creates and updates BackgroundMatrix, by putting given amount of gliders
    void randomGlidersFill(int gliders)
    {
        List<Vector2> posCheck = new List<Vector2>();
        System.Random rand = new System.Random();
        Vector2 prev = Vector2.zero;
        for (int i = 0; i < gliders; i++)
        {
            while(true)
            {
                int x = rand.Next(0, 100);
                int y = rand.Next(0, 100);
                Vector2 temp = new Vector2(x, y);
                if (!posCheck.Contains(temp) && Vector2.Distance(temp, prev) > distanceFromGliders)
                {
                    posCheck.Add(temp);
                    drawGlider(x, y, rand.Next(0, 4));
                    prev = temp;
                    break;
                }
            }
        }
    }

    // Gets glider from dictionary by glider and updates BackgroundMatrix by putting said glider at given position
    void drawGlider(int x, int y, int gliderNum)
    {
        bool[,] tempGlider;
        glidersReference.TryGetValue(gliderNum, out tempGlider);
        int gliderX = 0;
        int gliderY = 0;
        for (int xDev = -1; xDev < 2; xDev++)
        {
            for (int yDev = -1; yDev < 2; yDev++)
            {
                background[(x + xDev + sizeX) % sizeX, (y + yDev + sizeY) % sizeY] = tempGlider[gliderX, gliderY];
                gliderY++;
            }
            gliderX++;
            gliderY = 0;
        }
    }

    // Initializes dictionary and fills it with four gliders headed in four directions
    void feelGlidersDictionary()
    {
        glidersReference = new Dictionary<int, bool[,]>();
        bool[,] temp1 = {{false, true, false}, {false, false, true}, {true, true, true}};
        glidersReference.Add(0, temp1);
        bool[,] temp2 = {{false, true, false}, {true, false, false}, {true, true, true}};
        glidersReference.Add(1, temp2);
        bool[,] temp3 = {{true, true, true}, {false, false, true}, {false, true, false}};
        glidersReference.Add(2, temp3);
        bool[,] temp4 = {{true, true, true}, {true, false, false}, {false, true, false}};
        glidersReference.Add(3, temp4);
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

    // Mergin two matricies, if one of the matrices is true at some X and Y, set addres X and Y to be true in returned matrix
    bool[,] mergeMatrices(bool[,] temp1, bool[,] temp2)
    {
        bool[,] retVal = new bool[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                retVal[x, y] = temp1[x,y] || temp2[x,y];
            }
        }
        return retVal;
    }
}
