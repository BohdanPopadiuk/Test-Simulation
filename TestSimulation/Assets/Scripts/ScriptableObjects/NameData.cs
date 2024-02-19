using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Data/Names")]
    public class NameData : ScriptableObject
    {
        [SerializeField] private string[] namePart;
        [SerializeField] private string[] namePart1;

        public string GenerateName()
        {
            string part = namePart[Random.Range(0, namePart.Length)];
            string part1 = namePart1[Random.Range(0, namePart1.Length)];
            return $"{part} {part1}";
        }
    }
}