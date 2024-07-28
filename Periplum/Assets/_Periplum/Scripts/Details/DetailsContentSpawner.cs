using Periplum;
using UnityEngine;

public class DetailsContentSpawner : MonoBehaviour
{
    public static DetailsContent detailsPrefab;

    private void Awake()
    {
        if (detailsPrefab == null)
            return;

        Instantiate(detailsPrefab, transform);
    }
}