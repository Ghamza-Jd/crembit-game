using UnityEngine;

/// <summary>
/// This controller keeps the GameObject (which is the background music in this case),
/// alive when loading another scene.
/// </summary>
public class GameMusicPlayer : MonoBehaviour
{
    private static GameMusicPlayer _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
