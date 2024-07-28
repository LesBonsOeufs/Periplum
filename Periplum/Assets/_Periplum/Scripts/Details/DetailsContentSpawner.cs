using Periplum;
using UnityEngine;

public class DetailsContentSpawner : MonoBehaviour
{
    public static DetailsContent detailsPrefab;

    private void Awake()
    {
        Instantiate(detailsPrefab, transform);
    }
}