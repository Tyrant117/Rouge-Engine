using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Map.Creation
{
    public interface IMapCreationStrategy<T> where T : IMap
    {
        /// <summary>
        /// Creates a new IMap of the specified type
        /// </summary>
        T CreateMap();
    }
}