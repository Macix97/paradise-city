using UnityEngine;

// Spawn human in selected place
public class SpawnHuman : MonoBehaviour
{
    // Selected spawn points
    private GameObject[] _spawnPoints;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
        SpawnPeople();
    }

    // Update is called once per frame
    private void Update()
    {
        // TODO: Manipulate human types in run-time
    }

    // Initializate parameters
    private void Init()
    {
        // Get spawn points
        _spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    // Spawn people in selected positions
    private void SpawnPeople()
    {
        // Search spawn points
        foreach (GameObject spawnPoint in _spawnPoints)
        {
            // Get some gender
            string gender = RandomGender();
            // Load prefab
            GameObject humanPrefab = Resources.Load<GameObject>("People/" + gender + "/" + gender);
            // Spawn person
            GameObject human = GameObject.Instantiate<GameObject>(humanPrefab,
                spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform.parent);
        }
    }

    // Random some gender (man or woman)
    public string RandomGender()
    {
        // Random number
        int number = Random.Range(0, 2);
        // It is a man
        if (number.Equals(0))
            return "Man";
        // It is a woman
        return "Woman";
    }
}