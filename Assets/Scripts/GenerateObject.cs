using UnityEngine;

// Generate objects in specific positions after starting program
public class GenerateObject : MonoBehaviour
{
    // Number of vehicles
    private int _vehiclesNum;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    // Initializate parameters
    private void Init()
    {
        // Get number of vehicles
        _vehiclesNum = Resources.LoadAll<GameObject>("Vehicles/Prefabs").Length;
        // Get human points
        GameObject[] humanPoints = GameObject.FindGameObjectsWithTag("HumanPoint");
        // Get vehicle points
        GameObject[] vehiclePoints = GameObject.FindGameObjectsWithTag("VehiclePoint");
        // Get static vehicle points
        GameObject[] staticVehiclePoints = GameObject.FindGameObjectsWithTag("StaticVehiclePoint");
        // Search human points
        foreach (GameObject humanPoint in humanPoints)
        {
            // Get some gender
            string gender = DrawGender();
            // Load prefab
            GameObject humanPrefab = Resources.Load<GameObject>("People/" + gender + "/" + gender);
            // Generate person
            GameObject human = GameObject.Instantiate<GameObject>(humanPrefab,
                humanPoint.transform.position, Quaternion.identity, humanPoint.transform.parent);
        }
        // Search vehicle points
        foreach (GameObject vehiclePoint in vehiclePoints)
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
        // Search static vehicle points
        foreach (GameObject staticVehiclePoint in staticVehiclePoints)
        {
            // Get some vehicle
            int num = DrawVehicle();
            // Load prefab
            GameObject vehiclePrefab = Resources.Load<GameObject>("Vehicles/Prefabs/Car0" + num);
            // Generate vehicle
            GameObject vehicle = GameObject.Instantiate<GameObject>(vehiclePrefab,
                staticVehiclePoint.transform.position, staticVehiclePoint.transform.rotation,
                staticVehiclePoint.transform.parent);
        }
    }

    // Generate some driver in car
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

    // Draw some gender (man or woman)
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

    // Draw some vehicle (car)
    private int DrawVehicle()
    {
        // Draw number
        int number = Random.Range(1, _vehiclesNum + 1);
        return number;
    }
}