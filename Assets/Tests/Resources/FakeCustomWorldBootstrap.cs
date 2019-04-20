using CustomWorldBoostrapInternal;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class FakeCustomWorldBootstrap : ICustomWorldBootstrap
    {

        public FakeCustomWorldBootstrap(
            List<CustomWorldBootstrap.WorldOption> worldOptions = null,
            bool createDefaultWorld = true,
            string defaultWorldName = "")
        {
            if (worldOptions != null)
            {
                WorldOptions.AddRange(worldOptions);
            }

            CreateDefaultWorld = createDefaultWorld;
            DefaultWorldName = defaultWorldName;
        }
        public List<Type> Alterations = new List<Type>();
        public bool PostInitializeRan = false;

        public bool CreateDefaultWorld { get; set; }
        public string DefaultWorldName { get; set; }

        public List<CustomWorldBootstrap.WorldOption> WorldOptions { get; } = new List<CustomWorldBootstrap.WorldOption>();

        public List<Type> PostInitialize(List<Type> systems)
        {
            PostInitializeRan = true;
            if (systems != null)
            {
                systems.AddRange(Alterations);
            }

            return systems;
        }
    }
}