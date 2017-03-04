﻿using EddiDataDefinitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Utilities;

namespace EddiShipMonitor
{
    /// <summary>Storage for ship and shipyard information</summary>
    public class ShipMonitorConfiguration
    {
        public Ship ship{ get; set; }
        public ObservableCollection<Ship> shipyard{ get; set; }

        [JsonIgnore]
        private string dataPath;

        public ShipMonitorConfiguration()
        {
            shipyard = new ObservableCollection<Ship>();
        }

        /// <summary>
        /// Obtain ships configuration from a file.  If the file name is not supplied the the default
        /// path of Constants.Data_DIR\shipmonitor.json is used
        /// </summary>
        public static ShipMonitorConfiguration FromFile(string filename=null)
        {
            if (filename == null)
            {
                filename = Constants.DATA_DIR + @"\shipmonitor.json";
            }

            ShipMonitorConfiguration configuration = new ShipMonitorConfiguration();
            if (File.Exists(filename))
            {
                try
                {
                    configuration = JsonConvert.DeserializeObject<ShipMonitorConfiguration>(File.ReadAllText(filename));
                }
                catch (Exception ex)
                {
                    Logging.Debug("Failed to read ship configuration", ex);
                }
            }
            else
            {
                // Used to be in a separate 'ships' file so try that to allow migration
                string oldFilename = Constants.DATA_DIR + @"\ships.json";
                if (File.Exists(oldFilename))
                {
                    try
                    {
                        Dictionary<string, ObservableCollection<Ship>> oldShipsConfiguration = JsonConvert.DeserializeObject<Dictionary<string, ObservableCollection<Ship>>>(File.ReadAllText(oldFilename));
                        // At this point the old file is confirmed to have been there - migrate it
                        // There was a bug that caused null entries to be written to the ships configuration; remove these if present
                        ObservableCollection<Ship> oldShips = new ObservableCollection<Ship>(oldShipsConfiguration["ships"].Where(x => x.role != null));
                        configuration.shipyard = oldShips;
                        File.Delete(oldFilename);
                        configuration.ToFile();
                    }
                    catch
                    {
                        // Ther was a problem parsing the old file
                    }
                }
            }
            if (configuration == null)
            {
                configuration = new ShipMonitorConfiguration();
            }

            configuration.dataPath = filename;
            return configuration;
        }

        /// <summary>
        /// Write configuration to a file.  If the filename is not supplied then the path used
        /// when reading in the configuration will be used, or the default path of 
        /// Constants.Data_DIR\shipmonitor.json will be used
        /// </summary>
        public void ToFile(string filename=null)
        {
            if (filename == null)
            {
                filename = dataPath;
            }
            if (filename == null)
            {
                filename = Constants.DATA_DIR + @"\shipmonitor.json";
            }

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
    }
}
