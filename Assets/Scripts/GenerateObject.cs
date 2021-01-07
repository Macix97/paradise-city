using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates the objects in the specific positions after starting the program.
/// </summary>
public class GenerateObject : MonoBehaviour
{
    // Number of vehicles
    private int _vehiclesNum;

    // Awake is called when the script instance is being loaded
    private void Awake()
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
        // Temporary list for people
        List<HumanBehavior> peopleList = new List<HumanBehavior>();
        // Temporary list for vehicles
        List<VehicleBehavior> vehiclesList = new List<VehicleBehavior>();
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
            // Add script to list
            peopleList.Add(human.GetComponent<HumanBehavior>());
        }
        // Convert list to array
        HumanBehavior[] humanBehaviors = peopleList.ToArray();
        // Search people
        foreach (HumanBehavior humanBehavior in humanBehaviors)
            // Prepare people
            humanBehavior.PreparePeople();
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
            // Add script to list
            vehiclesList.Add(vehicle.GetComponent<VehicleBehavior>());
        }
        // Convert list to array
        VehicleBehavior[] vehicleBehaviors = vehiclesList.ToArray();
        // Search vehicles
        foreach (VehicleBehavior vehicleBehavior in vehicleBehaviors)
        {
            // Prepare vehicles
            vehicleBehavior.PrepareVehicles();
            vehicleBehavior.PrepareDriver();
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

    /// <summary>
    /// Generates the driver in the mobile car.
    /// </summary>
    /// <param name="parent">A transform that represents a vehicle.</param>
    /// <param name="manPoint">A transform that represents position for a male driver.</param>
    /// <param name="womanPoint">A transform that represents position for a female driver.</param>
    /// <returns>
    /// The object that represents the driver.
    /// </returns>
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
    /// Draws the gender of the person (male or female).
    /// </summary>
    /// <returns>
    /// The label that represents the gender.
    /// </returns>
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
    /// Draws some type of the vehicle.
    /// </summary>
    /// <returns>
    /// The number that represents the type of the vehicle.
    /// </returns>
    private int DrawVehicle()
    {
        // Draw number
        return Random.Range(1, _vehiclesNum + 1);
    }
}