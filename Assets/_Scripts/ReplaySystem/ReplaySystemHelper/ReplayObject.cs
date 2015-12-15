using UnityEngine;
using System.Collections;

namespace TinyReplay
{
    // add to the target will be recorded.
    public class ReplayObject : MonoBehaviour
    {
        public ReplayEntityType entityType = ReplayEntityType.None;
        public int entityIndex = 0;
    }
}