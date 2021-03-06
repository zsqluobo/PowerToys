﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Microsoft.PowerToys.Settings.UI.Lib
{
    public static class SettingsUtils
    {
        private const string DefaultFileName = "settings.json";
        private const string DefaultModuleName = "";

        public static void DeleteSettings(string powertoy, string fileName = DefaultFileName)
        {
            File.Delete(GetSettingsPath(powertoy, fileName));
        }

        public static bool SettingsFolderExists(string powertoy)
        {
            return Directory.Exists(Path.Combine(LocalApplicationDataFolder(), $"Microsoft\\PowerToys\\{powertoy}"));
        }

        public static void CreateSettingsFolder(string powertoy)
        {
            Directory.CreateDirectory(Path.Combine(LocalApplicationDataFolder(), $"Microsoft\\PowerToys\\{powertoy}"));
        }

        /// <summary>
        /// Get path to the json settings file.
        /// </summary>
        /// <returns>string path.</returns>
        public static string GetSettingsPath(string powertoy, string fileName = DefaultFileName)
        {
            if (string.IsNullOrWhiteSpace(powertoy))
            {
                return Path.Combine(
                    LocalApplicationDataFolder(),
                    $"Microsoft\\PowerToys\\{fileName}");
            }

            return Path.Combine(
                LocalApplicationDataFolder(),
                $"Microsoft\\PowerToys\\{powertoy}\\{fileName}");
        }

        public static bool SettingsExists(string powertoy = DefaultModuleName, string fileName = DefaultFileName)
        {
            return File.Exists(GetSettingsPath(powertoy, fileName));
        }

        /// <summary>
        /// Get a Deserialized object of the json settings string.
        /// </summary>
        /// <returns>Deserialized json settings object.</returns>
        public static T GetSettings<T>(string powertoy = DefaultModuleName, string fileName = DefaultFileName)
        {
            // Adding Trim('\0') to overcome possible NTFS file corruption.
            // Look at issue https://github.com/microsoft/PowerToys/issues/6413 you'll see the file has a large sum of \0 to fill up a 4096 byte buffer for writing to disk
            // This, while not totally ideal, does work around the problem by trimming the end.
            // The file itself did write the content correctly but something is off with the actual end of the file, hence the 0x00 bug
            var jsonSettingsString = File.ReadAllText(GetSettingsPath(powertoy, fileName)).Trim('\0');

            return JsonSerializer.Deserialize<T>(jsonSettingsString);
        }

        // Save settings to a json file.
        public static void SaveSettings(string jsonSettings, string powertoy = DefaultModuleName, string fileName = DefaultFileName)
        {
            try
            {
                if (jsonSettings != null)
                {
                    if (!SettingsFolderExists(powertoy))
                    {
                        CreateSettingsFolder(powertoy);
                    }

                    File.WriteAllText(GetSettingsPath(powertoy, fileName), jsonSettings);
                }
            }
            catch
            {
            }
        }

        public static string LocalApplicationDataFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }
}
