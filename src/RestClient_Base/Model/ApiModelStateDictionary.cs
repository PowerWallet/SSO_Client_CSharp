using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FinApps.SSO.RestClient_Base.Model
{
    [Serializable]
    public class ApiModelStateDictionary : IDictionary<string, ApiModelState>
    {

        private readonly Dictionary<string, ApiModelState> _innerDictionary = new Dictionary<string, ApiModelState>(StringComparer.OrdinalIgnoreCase);

        public ApiModelStateDictionary()
        {
        }

        public ApiModelStateDictionary(ApiModelStateDictionary dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            foreach (var entry in dictionary)
            {
                _innerDictionary.Add(entry.Key, entry.Value);
            }
        }

        public int Count
        {
            get
            {
                return _innerDictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<string, ApiModelState>)_innerDictionary).IsReadOnly;
            }
        }

        public bool IsValid
        {
            get
            {
                return Values.All(modelState => modelState.Errors.Count == 0);
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return _innerDictionary.Keys;
            }
        }

        public ApiModelState this[string key]
        {
            get
            {
                ApiModelState value;
                _innerDictionary.TryGetValue(key, out value);
                return value;
            }
            set
            {
                _innerDictionary[key] = value;
            }
        }

        public ICollection<ApiModelState> Values
        {
            get
            {
                return _innerDictionary.Values;
            }
        }

        public void Add(KeyValuePair<string, ApiModelState> item)
        {
            ((IDictionary<string, ApiModelState>)_innerDictionary).Add(item);
        }

        public void Add(string key, ApiModelState value)
        {
            _innerDictionary.Add(key, value);
        }

        public void AddModelError(string key, Exception exception)
        {
            GetModelStateForKey(key).Errors.Add(exception);
        }

        public void AddModelError(string key, string errorMessage)
        {
            GetModelStateForKey(key).Errors.Add(errorMessage);
        }

        public void Clear()
        {
            _innerDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, ApiModelState> item)
        {
            return ((IDictionary<string, ApiModelState>)_innerDictionary).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _innerDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, ApiModelState>[] array, int arrayIndex)
        {
            ((IDictionary<string, ApiModelState>)_innerDictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, ApiModelState>> GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        private ApiModelState GetModelStateForKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            ApiModelState apiModelState;
            if (!TryGetValue(key, out apiModelState))
            {
                apiModelState = new ApiModelState();
                this[key] = apiModelState;
            }

            return apiModelState;
        }

        public void Merge(ApiModelStateDictionary dictionary)
        {
            if (dictionary == null)
            {
                return;
            }

            foreach (var entry in dictionary)
            {
                this[entry.Key] = entry.Value;
            }
        }

        public bool Remove(KeyValuePair<string, ApiModelState> item)
        {
            return ((IDictionary<string, ApiModelState>)_innerDictionary).Remove(item);
        }

        public bool Remove(string key)
        {
            return _innerDictionary.Remove(key);
        }

        public void SetModelValue(string key, ValueProviderResult value)
        {
            GetModelStateForKey(key).Value = value;
        }

        public bool TryGetValue(string key, out ApiModelState value)
        {
            return _innerDictionary.TryGetValue(key, out value);
        }

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_innerDictionary).GetEnumerator();
        }
        #endregion

    }
}