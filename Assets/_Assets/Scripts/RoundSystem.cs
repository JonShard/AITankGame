using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundSystem : MonoBehaviour
{
    [SerializeField] private float roundTime = 120;
    [SerializeField] private List<int> scores;
    [SerializeField] private List<Tank> players;

    [SerializeField] private float timeLeft = 0; 
    // Start is called before the first frame update
    void Awake()
    {
        if (FindObjectsOfType<RoundSystem>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        scores = new List<int>();
        for (int i = 0; i < FindObjectsOfType<Tank>().Length; i++)
            scores.Add(0);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        players = new List<Tank>(FindObjectsOfType<Tank>());
        timeLeft = roundTime;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            Debug.Log("It's a draw, everybody looses!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        int playersAlive = 0;
        int winner = 0;
        foreach (Tank t in players)
        {
            if (t.alive)
            {
                playersAlive++;
                winner = players.IndexOf(t);
            }
        }
        if (playersAlive <= 1)
        {
            Debug.Log("Winner! " + players[winner].name);
            scores[winner]++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
