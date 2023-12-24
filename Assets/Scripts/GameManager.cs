using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject playerPrefab;
    public float cubeSize = 1f;

    private GameObject[,,] cubes;

    void Start()
    {
        GenerateCubeGrid();
    }

    void GenerateCubeGrid()
    {
        // Instantiate player prefab
        GameObject poo = Instantiate(playerPrefab, new Vector3(-4f, 0f, 7f), Quaternion.Euler(90, 0, -90));

        cubes = new GameObject[10, 10, 10];

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int z = 0; z < 10; z++)
                {
                    // Instantiate cubes and position them to form a grid
                    GameObject cube = Instantiate(cubePrefab, new Vector3(x * cubeSize, y * cubeSize, z * cubeSize), Quaternion.identity);
                    cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
                    cube.tag = "cube"; // Set tag for easy identification
                    cubes[x, y, z] = cube;
                }
            }
        }


    }
}
