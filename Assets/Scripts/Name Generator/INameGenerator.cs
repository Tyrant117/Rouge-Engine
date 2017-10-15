using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike.NameGen
{
    public interface INameGenerator
    {
        void Parse(string filename);

        string Generate(string name, bool allocate);

        string GenerateCustom(string name, string rule, bool allocate);

        List<INameGenerator> GetSets();

        void Destroy();
    }
}