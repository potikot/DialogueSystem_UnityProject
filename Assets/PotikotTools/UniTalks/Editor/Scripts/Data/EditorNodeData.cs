using MessagePack;
using UnityEngine;

namespace PotikotTools.UniTalks.Editor
{
    [MessagePackObject]
    public class EditorNodeData
    {
        [Key(0)]
        public Vector2 position;
    }
}
