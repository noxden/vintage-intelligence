using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int cubeAmount;
    [SerializeField] private int rangeMin;
    [SerializeField] private int rangeMax;
    [SerializeField] private List<Material> materials;

    private void Start()
    {
        for (int i = 0; i < cubeAmount; i++)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        GameObject go = Instantiate(prefab, RandomPosition(), Quaternion.identity, this.transform);
        SelectRandomMaterial(go);
    }

    private Vector3 RandomPosition()
    {
        Vector3 randomPos = new Vector3(Random.Range(-rangeMax, rangeMax), Random.Range(-rangeMax, rangeMax), Random.Range(-rangeMax, rangeMax));

        if (randomPos.magnitude <= rangeMin)
            return RandomPosition();

        return randomPos;
    }

    private void SelectRandomMaterial(GameObject _go)
    {
        int randomValue = Random.Range(1, materials.Count + 1);
        _go.GetComponent<MeshRenderer>().material = materials[randomValue - 1];
    }
}