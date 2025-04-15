using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    [System.Serializable]
    public class GeneratorSettingsContainer : MonoBehaviour
    {
        public GridGeneratorSettings Gridsettings = new GridGeneratorSettings();
        public RingGeneratorSettings Ringsettings = new RingGeneratorSettings();
        public CircleGridGeneratorSettings Circlesettings = new CircleGridGeneratorSettings();

        [SerializeReference]
        public GeneratorSettings _selectedGenerator;
        public int SelectedIndex = 0;

        public void SetSelectedGenerator(GeneratorSettings generator)
        {
            _selectedGenerator = generator;
        }

        public GeneratorSettings GetSelectedGenerator()
        {
            return _selectedGenerator;
        }
    }
}
