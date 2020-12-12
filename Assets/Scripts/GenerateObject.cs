using UnityEngine;

/// <summary>
/// Generates objects in specific positions after starting the program.
/// </summary>
public class GenerateObject : MonoBehaviour
{
    // Human points
    private GameObject[] _humanPoints;
    // Vehicle points
    private GameObject[] _vehiclePoints;
    // Number of vehicles
    private int _vehiclesNum;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
        GeneratePeople();
        GenerateVehicles();
    }

    // Update is called once per frame
    private void Update()
    {
        // TODO: Manipulate human types in run-time
    }

    // Initializate parameters
    private void Init()
    {
        // Get human points
        _humanPoints = GameObject.FindGameObjectsWithTag("HumanPoint");
        // Get vehicle points
        _vehiclePoints = GameObject.FindGameObjectsWithTag("VehiclePoint");
        // Get number of vehicles
        _vehiclesNum = Resources.LoadAll<GameObject>("Vehicles/Prefabs").Length;
    }

    /// <summary>
    /// Generates some random people in selected positions.
    /// </summary>
    private void GeneratePeople()
    {
        // Search human points
        foreach (GameObject humanPoint in _humanPoints)
        {
            // Get some gender
            string gender = DrawGender();
            // Load prefab
            GameObject humanPrefab = Resources.Load<GameObject>("People/" + gender + "/" + gender);
            // Generate person
            GameObject human = GameObject.Instantiate<GameObject>(humanPrefab,
                humanPoint.transform.position, Quaternion.identity, humanPoint.transform.parent);
        }
    }

    public GameObject GenerateDriver(Transform parent, Transform manPoint, Transform womanPoint)
    {
        // Get some gender
        string gender = DrawGender();
        // Load prefab
        GameObject humanPrefab = Resources.Load<GameObject>("People/" + gender + "/" + gender);
        // Prepare game object
        GameObject driver;
        // Check gender
        if (gender.Equals("Man"))
            // Generate man
            driver = GameObject.Instantiate<GameObject>(humanPrefab,
                manPoint.position, manPoint.rotation, parent);
        else
            // Generate woman
            driver = GameObject.Instantiate<GameObject>(humanPrefab,
                womanPoint.position, womanPoint.rotation, parent);
        // return driver
        return driver;
    }

    /// <summary>
    /// Generates some random vehicles in selected positions.
    /// </summary>
    private void GenerateVehicles()
    {
        // Search vehicle points
        foreach (GameObject vehiclePoint in _vehiclePoints)
        {
            // Get some vehicle
            int num = DrawVehicle();
            // Load prefab
            GameObject vehiclePrefab = Resources.Load<GameObject>("Vehicles/Prefabs/Car0" + num);
            // Generate vehicle
            GameObject vehicle = GameObject.Instantiate<GameObject>(vehiclePrefab,
                vehiclePoint.transform.position, vehiclePoint.transform.rotation,
                vehiclePoint.transform.parent);
        }
    }

    /// <summary>
    /// Draws some gender (man or woman).
    /// </summary>
    private string DrawGender()
    {
        // Draw number
        int number = Random.Range(0, 2);
        // It is a man
        if (number.Equals(0))
            return "Man";
        // It is a woman
        return "Woman";
    }

    /// <summary>
    /// Draws some vehicle (car).
    /// </summary>
    private int DrawVehicle()
    {
        // Draw number
        int number = Random.Range(1, _vehiclesNum + 1);
        return number;
    }
}