using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerInputInitializer : MonoBehaviour
{
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        PlayerInputManager lManager = GetComponent<PlayerInputManager>();
        GameObject lInputManagerPlayerPrefab = lManager.playerPrefab;
        int lPlayerCount = PlayerInput.all.Count;

        for (int i = 0; i < lManager.maxPlayerCount; i++)
        {
            if (lPlayerCount - 1 < i)
                PlayerInput.Instantiate(lInputManagerPlayerPrefab, i);
        }
    }
}