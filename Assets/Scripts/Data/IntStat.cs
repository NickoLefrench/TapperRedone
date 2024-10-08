using System;

using UnityEngine;

namespace FMS.TapperRedone.Data
{
    public class IntStat : AbstractStat
    {
        public static Func<int, int, int> OverrideFunc = (curValue, newValue) => newValue;
        public static Func<int, int, int> AddFunc = (curValue, newValue) => curValue + newValue;
        public static Func<int, int, int> MinFunc = (curValue, newValue) => Mathf.Min(curValue, newValue);
        public static Func<int, int, int> MaxFunc = (curValue, newValue) => Mathf.Max(curValue, newValue);

        [SerializeField]
        private int _value;
        private readonly Func<int, int, int> _updateFunc;

        public IntStat(string name, bool lifetime, Func<int, int, int> updateFunc, int defaultValue = 0) : base(name, lifetime)
        {
            _updateFunc = updateFunc;
            _value = defaultValue;
        }

        public int Value => _value;

        public override string GetValueAsString()
        {
            return Value.ToString();
        }

        public override bool SetValueFromString(string newValue)
        {
            return int.TryParse(newValue, out _value);
        }

        public override bool UpdateWithValue(object newValue)
        {
            if (newValue is int v)
            {
                _value = _updateFunc(_value, v);
                return true;
            }
            return false;
        }
    }
}
