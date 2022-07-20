using System.Collections.Generic;

namespace BTree.Core
{
    public class BehaviourTreeBlackboard
    {
        private readonly Dictionary<string, object> m_Databoard = new();

        public object this[string name]
        {
            get => m_Databoard[name];
            set => m_Databoard[name] = value;
        }

        public _Ty GetData<_Ty>(string name)
        {
            return (_Ty)this[name];
        }

        public bool TryGetData<_Ty>(string name, out _Ty data)
        {
            data = default;
            if (m_Databoard.TryGetValue(name, out var val))
                data = (_Ty)val;
            return data != null;
        }

        public _Ty GetDataOrDefault<_Ty>(string name, _Ty def)
        {
            _Ty data = def;
            if (m_Databoard.TryGetValue(name, out var val))
            {
                var as_data = (_Ty)val;
                if (as_data != null)
                    data = as_data;
            }
            return data;
        }
    }
}
