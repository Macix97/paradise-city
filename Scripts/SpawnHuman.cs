using UnityEngine;

// Spawn human in selected place
public class SpawnHuman : MonoBehaviour
{
    // Selected spawn points
    public GameObject[] SpawnPoints;

    // Start is called before the first frame update
    private void Start()
    {
        SpawnPerson();
    }

    // Update is called once per frame
    private void Update()
    {
        // TODO: Manipulate human types in run-time
    }

    // Spawn person in selected position
    private void SpawnPerson()
    {
        // Load prefab
        GameObject humanPrefab = Resources.Load<GameObject>("People/Man/Man");
        foreach (GameObject spawnPoint in SpawnPoints)
        {
            // Spawn person
            GameObject human = GameObject.Instantiate<GameObject>(humanPrefab,
                spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform.parent);
        }
    }
}