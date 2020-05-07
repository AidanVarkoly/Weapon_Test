using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum GameStage
{
    gracePeriod,
    roundInProgress,
    roundOver
}

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public UIManager uim;

    public static bool DebugMode = true;
    public GameStage GameFlow;
    public List<GameObject> Spawners;
    public GameObject Player;
    public List<GameObject> Enemy;
    public AudioSource Siren;


    public float GracePeriodLength = 30f;
    float GracePeriodTime;

    bool firstWave = false;
    public int EnenmyLimit = 10; //Amount of Enemies that can be spawned in at a time
    public int EnenmyInRound = 20; //Amount of Enemies that will be spawned during the round
    public static int EnenmiesLeft; //Amount of Enemies that still have to be killed
    public int EnenmiesLeftToSpawn; //Amount of Enemies that still have to be spawned
    public int framesBetweenEnemies = 240; //Number of frames in between enemy spawns
    public int EnemySpawnFrames; //Number of frames before the next enemy can spawn
    public int EnemiesInFirstSpawn = 5; //Number of enemies in the first spawn
    public bool PlayerSpotted = false; //Determine whether the player has been spotted
    static List<GameObject> enemies;
    public LightController[] lightController;

    // Start is called before the first frame update
    void Start()
    {
        GameFlow = GameStage.gracePeriod;
        EnenmiesLeft = EnenmyInRound;
        EnenmiesLeftToSpawn = EnenmyInRound;
        enemies = new List<GameObject>(EnenmyLimit);
        GracePeriodTime = GracePeriodLength;
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameFlow)
        {
            case GameStage.gracePeriod:
                if (!uim.roundInfoPanel.activeInHierarchy && (!uim.inspecting && !uim.SkillMenuActive))
                {
                    uim.roundInfoPanel.SetActive(true);
                }
                GracePeriodTime -= Time.deltaTime;
                uim.updateRoundCountdown(GracePeriodTime);

                if (GracePeriodTime <= 0f)
                {
                    uim.updateRoundEnemies(EnenmiesLeft);
                    firstWave = true;
                    GameFlow = GameStage.roundInProgress;
                }
                break;
            case GameStage.roundInProgress:
                if (EnenmiesLeft == 0) GameFlow = GameStage.roundOver;

                if (firstWave)
                {
                    for (int i = 0; i < EnemiesInFirstSpawn; i++)
                    {
                        SpawnEnemy();
                    }
                    EnemySpawnFrames = framesBetweenEnemies;
                }
                else if (EnenmiesLeft > 0 && enemies.Count < EnenmyLimit)
                {
                    if (EnenmiesLeftToSpawn != 0)
                    {
                        if (EnemySpawnFrames != 0)
                        {
                            EnemySpawnFrames--;
                        }
                        else
                        {
                            SpawnEnemy();
                            EnemySpawnFrames = framesBetweenEnemies;
                        }
                    }
                }

                if (firstWave) firstWave = false;
                break;
            case GameStage.roundOver:
                uim.updateRoundResult("You Win!");
                StartCoroutine("LoadScene");
                break;
        }
    }

    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Main Menu");
    }

    GameObject FindBestSpawner()
    {
        GameObject bestSpawner = null;
        float bestSpawnerDistanceFromPlayer = 0;
        foreach(GameObject go in Spawners)
        {
            float distance = Vector3.Distance(go.transform.position, Player.transform.position);
            if (bestSpawner == null)
            {
                if(distance > 5f)
                {
                    bestSpawnerDistanceFromPlayer = distance;
                    bestSpawner = go;
                }
            }
            else if (distance < bestSpawnerDistanceFromPlayer && distance > 5f)
            {
                if (!go.GetComponent<Spawner>().recentlyUsed)
                {
                    go.GetComponent<Spawner>().recentlyUsed = true;
                    bestSpawner.GetComponent<Spawner>().recentlyUsed = false;
                    bestSpawnerDistanceFromPlayer = distance;
                    bestSpawner = go;
                }
            }
        }

        return bestSpawner;
    }

    void SpawnEnemy()
    {
        int whichEnemy = (Random.Range(0, Enemy.Count));
        GameObject spawnPoint = FindBestSpawner();
        GameObject tempEnemy;

        Vector3 temp = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        if (whichEnemy == 0)
        {
            tempEnemy = Instantiate<GameObject>(Enemy[0], temp, Quaternion.identity);
        }
        else if (whichEnemy == 1)
        {
            tempEnemy = Instantiate<GameObject>(Enemy[1], temp, Quaternion.identity);
        }
        else
        {
            tempEnemy = Instantiate<GameObject>(Enemy[2], temp, Quaternion.identity);
        }
        //tempEnemy.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        //tempEnemy.GetComponent<Enemy>().Ground();
        enemies.Add(tempEnemy);
        EnenmiesLeftToSpawn--;
    }

    public void RemoveEnemy(GameObject enemyToBeRemoved)
    {
        enemies.Remove(enemyToBeRemoved);
        Destroy(enemyToBeRemoved);
        EnenmiesLeft--;
        uim.updateRoundEnemies(EnenmiesLeft);
        if (EnenmiesLeft == 0)
        {
            foreach (GameObject go in enemies)
            {
                Destroy(go);//need to limit spawns when getting close to end
            }
        }
    }

    public void CodeRed()
    {
        foreach (LightController light in lightController)
        {
            LightController.LockDown = true;
        }
    }

    public bool getIfSeen()
    {
        return PlayerSpotted;
    }

    public void setIfSeen(bool s)
    {
        PlayerSpotted = s;
        if(s)
        {
            CodeRed();
            Siren.Play();
        }
    }
}


