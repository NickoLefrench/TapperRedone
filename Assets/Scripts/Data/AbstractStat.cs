using UnityEngine;

namespace FMS.TapperRedone.Data
{
    public abstract class AbstractStat
    {
        [SerializeField]
        protected string Name;

        public readonly bool Lifetime;
        public string StatName => Name;

        protected AbstractStat(string name, bool lifetime)
        {
            Name = name;
            Lifetime = lifetime;
        }

        public abstract bool UpdateWithValue(object newValue);
        public abstract bool SetValueFromString(string newValue);
        public abstract string GetValueAsString();
    }
}
