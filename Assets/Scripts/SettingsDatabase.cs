using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public static class SettingsDatabase
{
    // Game configuration file name
    public static readonly string GameConfigName = "/game_config";
    // Menu configuration file name
    public static readonly string MenuConfigName = "/menu_config";
    // File format
    public static readonly string DatFormat = ".dat";
    // List of settings names
    public static readonly string Disabled = "Disabled";
    public static readonly string Low = "Low";
    public static readonly string Medium = "Medium";
    public static readonly string High = "High";
    // Drawing distance
    public static readonly float NearDist = 300f;
    public static readonly float NormalDist = 900f;
    public static readonly float FarDist = 1500f;
    // Grass density
    public static readonly float DisabledGrass = 0f;
    public static readonly float PoorGrass = 0.25f;
    public static readonly float GreatGrass = 1f;
    // Types of configuration files
    public enum ConfigType
    {
        Menu,
        Game
    };
    // Types of loading results
    public enum LoadResult
    {
        Success,
        Error,
        NoFile
    }
    // Menu configuration structure
    [Serializable]
    public struct MenuConfiguration
    {
        //--- Main menu options ---//

        public float MenuSoundsVolume { get; set; }
        public float MenuMusicVolume { get; set; }
    };
    // Game configuration structure
    [Serializable]
    public struct GameConfiguration
    {
        // Post-processing profile
        public int ProfileIndex { get; set; }
        // Quality level
        public int QualityLevel { get; set; }
        // Day duration
        public float DayDuration { get; set; }
        // Sounds volume
        public float SoundsVolume { get; set; }
        // Check if post-processing is active
        public bool IsPostProcess { get; set; }
        // Grass density
        public float GrassDensity { get; set; }
        // Drawing distance
        public float DrawingDist { get; set; }

        //--- Labels ---//

        // Quality label
        public string QualityLabel { get; set; }
        // Post-processing label
        public string PostProcessLabel { get; set; }
        // Pedestrians label
        public string PedestriansLabel { get; set; }
        // Static vehicles label
        public string StaticVehiclesLabel { get; set; }
        // Vehicles label
        public string VehiclesLabel { get; set; }
        // Grass label
        public string GrassLabel { get; set; }
        // Distance label
        public string DistLabel { get; set; }

        //--- Buttons states (+) ---//

        // Quality increasing button state
        public bool QualityInBtnState { get; set; }
        // Post-processing increasing button state
        public bool PostInProcessBtnState { get; set; }
        // Pedestrians increasing button state
        public bool PedestriansInBtnState { get; set; }
        // Static vehicles increasing button state
        public bool StaticVehiclesInBtnState { get; set; }
        // Vehicles increasing button state
        public bool VehiclesInBtnState { get; set; }
        // Grass increasing button state
        public bool GrassInBtnState { get; set; }
        // Distance increasing button state
        public bool DistInBtnState { get; set; }

        //--- Buttons states (-) ---//

        // Quality decreasing button state
        public bool QualityDeBtnState { get; set; }
        // Post-processing decreasing button state
        public bool PostDeProcessBtnState { get; set; }
        // Pedestrians decreasing button state
        public bool PedestriansDeBtnState { get; set; }
        // Static vehicles decreasing button state
        public bool StaticVehiclesDeBtnState { get; set; }
        // Vehicles decreasing button state
        public bool VehiclesDeBtnState { get; set; }
        // Grass decreasing button state
        public bool GrassDeBtnState { get; set; }
        // Distance decreasing button state
        public bool DistDeBtnState { get; set; }

        //--- Keys ---//

        public KeyCode MoveForward { get; set; }
        public KeyCode MoveBack { get; set; }
        public KeyCode MoveLeft { get; set; }
        public KeyCode MoveRight { get; set; }
        public KeyCode Climb { get; set; }
        public KeyCode Drop { get; set; }
        public KeyCode MoveFaster { get; set; }
        public KeyCode MoveSlower { get; set; }
    };
    // Game configuration structure
    public static GameConfiguration GameConfig;
    // Menu configuration structure
    public static MenuConfiguration MenuConfig;

    // Set default simulation settings (first start, damaged configuration file)
    public static void SetDefaultGameSettings()
    {
        // Set proper settings
        GameConfig.ProfileIndex = Resources.LoadAll<PostProcessProfile>("Post Processing").Length - 1;
        GameConfig.QualityLevel = QualitySettings.names.Length - 1;
        GameConfig.DayDuration = 1440f;
        GameConfig.SoundsVolume = 1f;
        GameConfig.IsPostProcess = true;
        GameConfig.GrassDensity = GreatGrass;
        GameConfig.DrawingDist = FarDist;
        // Labels
        GameConfig.QualityLabel = GameConfig.PostProcessLabel = GameConfig.PedestriansLabel =
            GameConfig.StaticVehiclesLabel = GameConfig.VehiclesLabel = GameConfig.GrassLabel =
            GameConfig.DistLabel = High;
        // Increasing buttons (disabled)
        GameConfig.QualityInBtnState = GameConfig.PostInProcessBtnState = GameConfig.PedestriansInBtnState =
            GameConfig.StaticVehiclesInBtnState = GameConfig.VehiclesInBtnState = GameConfig.GrassInBtnState =
            GameConfig.DistInBtnState = false;
        // Decreasing buttons (enabled)
        GameConfig.QualityDeBtnState = GameConfig.PostDeProcessBtnState = GameConfig.PedestriansDeBtnState =
            GameConfig.StaticVehiclesDeBtnState = GameConfig.VehiclesDeBtnState = GameConfig.GrassDeBtnState =
            GameConfig.DistDeBtnState = true;
        // Set keys
        GameConfig.MoveForward = KeyCode.W;
        GameConfig.MoveBack = KeyCode.S;
        GameConfig.MoveLeft = KeyCode.A;
        GameConfig.MoveRight = KeyCode.D;
        GameConfig.Climb = KeyCode.Q;
        GameConfig.Drop = KeyCode.E;
        GameConfig.MoveFaster = KeyCode.LeftShift;
        GameConfig.MoveSlower = KeyCode.LeftControl;
    }

    // Set default menu settings (first start, damaged configuration file)
    public static void SetDefaultMenuSettings()
    {
        // Set proper settings
        MenuConfig.MenuSoundsVolume = MenuConfig.MenuMusicVolume = 1f;
    }

    // Copy parameters from variables to configuration structure when simulation is running
    public static void CopyGameToConfig(GameSettingsManager settingsManager,
        CameraMovement cameraMovement)
    {
        // Basic parameters
        GameConfig.QualityLevel = QualitySettings.GetQualityLevel();
        GameConfig.ProfileIndex = settingsManager.CurProfile;
        GameConfig.DayDuration = settingsManager.TimeSld.value;
        GameConfig.SoundsVolume = settingsManager.AudioSld.value;
        GameConfig.IsPostProcess = settingsManager.IsPostProcess;
        GameConfig.GrassDensity = settingsManager.Terrain.detailObjectDensity;
        GameConfig.DrawingDist = Camera.main.farClipPlane;
        // Labels
        GameConfig.QualityLabel = QualitySettings.names[QualitySettings.GetQualityLevel()];
        GameConfig.PostProcessLabel = settingsManager.CurProcessText.text;
        GameConfig.PedestriansLabel = settingsManager.CurPedestriansText.text;
        GameConfig.StaticVehiclesLabel = settingsManager.CurStaticVehiclesText.text;
        GameConfig.VehiclesLabel = settingsManager.CurVehiclesText.text;
        GameConfig.GrassLabel = settingsManager.CurGrassText.text;
        GameConfig.DistLabel = settingsManager.CurDistText.text;
        // Buttons states (+)
        GameConfig.QualityInBtnState = settingsManager.IncreaseQualityBtn.interactable;
        GameConfig.PostInProcessBtnState = settingsManager.IncreaseProcessBtn.interactable;
        GameConfig.PedestriansInBtnState = settingsManager.IncreasePedestriansBtn.interactable;
        GameConfig.StaticVehiclesInBtnState = settingsManager.IncreaseStaticVehiclesBtn.interactable;
        GameConfig.VehiclesInBtnState = settingsManager.IncreaseVehiclesBtn.interactable;
        GameConfig.GrassInBtnState = settingsManager.IncreaseGrassBtn.interactable;
        GameConfig.DistInBtnState = settingsManager.IncreaseDistBtn.interactable;
        // Buttons states (-)
        GameConfig.QualityDeBtnState = settingsManager.DecreaseQualityBtn.interactable;
        GameConfig.PostDeProcessBtnState = settingsManager.DecreaseProcessBtn.interactable;
        GameConfig.PedestriansDeBtnState = settingsManager.DecreasePedestriansBtn.interactable;
        GameConfig.StaticVehiclesDeBtnState = settingsManager.DecreaseStaticVehiclesBtn.interactable;
        GameConfig.VehiclesDeBtnState = settingsManager.DecreaseVehiclesBtn.interactable;
        GameConfig.GrassDeBtnState = settingsManager.DecreaseGrassBtn.interactable;
        GameConfig.DistDeBtnState = settingsManager.DecreaseDistBtn.interactable;
        // Keys
        GameConfig.MoveForward = cameraMovement.MoveForward;
        GameConfig.MoveBack = cameraMovement.MoveBack;
        GameConfig.MoveLeft = cameraMovement.MoveLeft;
        GameConfig.MoveRight = cameraMovement.MoveRight;
        GameConfig.Climb = cameraMovement.Climb;
        GameConfig.Drop = cameraMovement.Drop;
        GameConfig.MoveFaster = cameraMovement.MoveFaster;
        GameConfig.MoveSlower = cameraMovement.MoveSlower;
    }

    // Copy parameters from variables to configuration structure when main menu is active
    public static void CopyMenuToConfig(MenuSettingsManager settingsManager)
    {
        // Sliders
        MenuConfig.MenuSoundsVolume = settingsManager.SoundsSld.value;
        MenuConfig.MenuMusicVolume = settingsManager.MusicSld.value;
    }

    // Write parameters from configuration structure before start simulation
    public static void SetGameFromConfig(ref GameSettingsManager settingsManager,
        ref CameraMovement cameraMovement)
    {
        // Set quality level
        QualitySettings.SetQualityLevel(GameConfig.QualityLevel);
        // Set current post-processing profile
        settingsManager.CurProfile = GameConfig.ProfileIndex;
        settingsManager.Volume.profile = settingsManager.Profiles[settingsManager.CurProfile];
        // Set post-processing
        settingsManager.IsPostProcess = settingsManager.Layer.enabled =
            settingsManager.PostProcessCheck.enabled = GameConfig.IsPostProcess;
        // Set day duration
        settingsManager.DayAndNightCycle.SecondsInAFullDay = settingsManager.TimeSld.value =
            GameConfig.DayDuration;
        // Set audio volume (slider only)
        settingsManager.AudioSld.value = GameConfig.SoundsVolume;
        // Set proper labels
        settingsManager.CurQualityText.text =
            QualitySettings.names[QualitySettings.GetQualityLevel()];
        settingsManager.CurProcessText.text =
            settingsManager.Profiles[settingsManager.CurProfile].name;
        settingsManager.CurPedestriansText.text = GameConfig.PedestriansLabel;
        settingsManager.CurStaticVehiclesText.text = GameConfig.StaticVehiclesLabel;
        settingsManager.CurVehiclesText.text = GameConfig.VehiclesLabel;
        settingsManager.CurGrassText.text = GameConfig.GrassLabel;
        settingsManager.CurDistText.text = GameConfig.DistLabel;
        // Set buttons states
        settingsManager.IncreaseQualityBtn.interactable = GameConfig.QualityInBtnState;
        settingsManager.DecreaseQualityBtn.interactable = GameConfig.QualityDeBtnState;
        settingsManager.IncreaseProcessBtn.interactable = GameConfig.PostInProcessBtnState;
        settingsManager.DecreaseProcessBtn.interactable = GameConfig.PostDeProcessBtnState;
        settingsManager.IncreasePedestriansBtn.interactable = GameConfig.PedestriansInBtnState;
        settingsManager.DecreasePedestriansBtn.interactable = GameConfig.PedestriansDeBtnState;
        settingsManager.IncreaseStaticVehiclesBtn.interactable = GameConfig.StaticVehiclesInBtnState;
        settingsManager.DecreaseStaticVehiclesBtn.interactable = GameConfig.StaticVehiclesDeBtnState;
        settingsManager.IncreaseVehiclesBtn.interactable = GameConfig.VehiclesInBtnState;
        settingsManager.DecreaseVehiclesBtn.interactable = GameConfig.VehiclesDeBtnState;
        settingsManager.IncreaseGrassBtn.interactable = GameConfig.GrassInBtnState;
        settingsManager.DecreaseGrassBtn.interactable = GameConfig.GrassDeBtnState;
        settingsManager.IncreaseDistBtn.interactable = GameConfig.DistInBtnState;
        settingsManager.DecreaseDistBtn.interactable = GameConfig.DistDeBtnState;
        // Set post-processing panel visibility
        settingsManager.PostProcessPanel.gameObject.SetActive(settingsManager.IsPostProcess);
        // Set grass density
        settingsManager.Terrain.detailObjectDensity = GameConfig.GrassDensity;
        settingsManager.Terrain.Flush();
        // Set drawing distance
        Camera.main.farClipPlane = RenderSettings.fogEndDistance = GameConfig.DrawingDist;
        // Set labels for keys
        settingsManager.MoveForwardText.text = GameConfig.MoveForward.ToString();
        settingsManager.MoveBackText.text = GameConfig.MoveBack.ToString();
        settingsManager.MoveLeftText.text = GameConfig.MoveLeft.ToString();
        settingsManager.MoveRightText.text = GameConfig.MoveRight.ToString();
        settingsManager.ClimbText.text = GameConfig.Climb.ToString();
        settingsManager.DropText.text = GameConfig.Drop.ToString();
        settingsManager.MoveFasterText.text = GameConfig.MoveFaster.ToString();
        settingsManager.MoveSlowerText.text = GameConfig.MoveSlower.ToString();
        // Set keys
        cameraMovement.MoveForward = GameConfig.MoveForward;
        cameraMovement.MoveBack = GameConfig.MoveBack;
        cameraMovement.MoveLeft = GameConfig.MoveLeft;
        cameraMovement.MoveRight = GameConfig.MoveRight;
        cameraMovement.Climb = GameConfig.Climb;
        cameraMovement.Drop = GameConfig.Drop;
        cameraMovement.MoveFaster = GameConfig.MoveFaster;
        cameraMovement.MoveSlower = GameConfig.MoveSlower;
        // Clear configuration variable
        GameConfig = new GameConfiguration();
    }

    // Write parameters from configuration structure before start program
    public static void SetMenuFromConfig(ref MenuSettingsManager settingsManager)
    {
        // Set sounds volume
        settingsManager.SoundsSld.value = MenuConfig.MenuSoundsVolume;
        // Set music volume
        settingsManager.MusicSld.value = MenuConfig.MenuMusicVolume;
        // Clear configuration variable
        MenuConfig = new MenuConfiguration();
    }

    // Try save settings to configuration file
    public static bool TrySaveConfig(string path, string fileName, ConfigType type)
    {
        // Check operation result
        bool isSucceed;
        // Create new binary formater
        BinaryFormatter binary = new BinaryFormatter();
        // Check if configuration file already exists
        if (File.Exists(path + fileName + DatFormat))
            // Delete configuration file
            File.Delete(path + fileName + DatFormat);
        // Create configuration file
        FileStream configFile = File.Create(path + fileName + DatFormat);
        // Try save data
        try
        {
            // It is menu configuration file
            if (type.Equals(ConfigType.Menu))
                // Save menu configuration to file
                binary.Serialize(configFile, MenuConfig);
            // It is game configuration file
            if (type.Equals(ConfigType.Game))
                // Save game configuration to file
                binary.Serialize(configFile, GameConfig);
            // Operation succeeded
            isSucceed = true;
        }
        catch
        {
            // Operation failed
            isSucceed = false;
        }
        // End action
        finally
        {
            // Close file
            configFile.Close();
            // It is menu configuration file
            if (type.Equals(ConfigType.Menu))
                // Clear configuration variable
                MenuConfig = new MenuConfiguration();
            // It is game configuration file
            if (type.Equals(ConfigType.Game))
                // Clear configuration variable
                GameConfig = new GameConfiguration();
        }
        // Return operation result
        return isSucceed;
    }

    // Try load settings from configuration file
    public static LoadResult TryLoadConfig(string path, string fileName, ConfigType type)
    {
        // Check operation result
        LoadResult result;
        // Create new binary formater
        BinaryFormatter binary = new BinaryFormatter();
        // Check if file exists
        if (!File.Exists(path + fileName + DatFormat))
            // No file
            return LoadResult.NoFile;
        // Open configuration file
        FileStream configFile = File.Open(path + fileName + DatFormat, FileMode.Open);
        // Try load data
        try
        {
            // It is menu configuration file
            if (type.Equals(ConfigType.Menu))
            {
                // Initialize variables
                MenuConfig = new MenuConfiguration();
                // Move menu data from file to variable
                MenuConfig = (MenuConfiguration)binary.Deserialize(configFile);
            }
            // It is game configuration file
            if (type.Equals(ConfigType.Game))
            {
                // Initialize variables
                GameConfig = new GameConfiguration();
                // Move game data from file to variable
                GameConfig = (GameConfiguration)binary.Deserialize(configFile);
            }
            // Operation succeeded
            result = LoadResult.Success;
        }
        // Catch exception
        catch
        {
            // Operation failed
            result = LoadResult.Error;
        }
        // End action
        finally
        {
            // Close file
            configFile.Close();
        }
        // Return operation result
        return result;
    }
}