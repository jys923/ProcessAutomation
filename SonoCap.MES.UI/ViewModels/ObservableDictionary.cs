using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SonoCap.MES.UI.ViewModels
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyPropertyChanged
        where TValue : INotifyPropertyChanged
    {
        private Dictionary<TKey, TValue> _internalDictionary = new Dictionary<TKey, TValue>();

        // PropertyChanged 이벤트는 속성이 변경될 때 발생합니다.
        public event PropertyChangedEventHandler? PropertyChanged;

        // 인덱서는 키에 대한 값을 가져오거나 설정합니다.
        // 값이 설정될 때마다 PropertyChanged 이벤트를 발생시킵니다.
        public TValue this[TKey key]
        {
            get => _internalDictionary[key];
            set
            {
                if (_internalDictionary.ContainsKey(key))
                {
                    // 기존 항목의 PropertyChanged 이벤트 구독 취소
                    _internalDictionary[key].PropertyChanged -= Item_PropertyChanged;
                }

                _internalDictionary[key] = value;

                // 새 항목의 PropertyChanged 이벤트 구독
                _internalDictionary[key].PropertyChanged += Item_PropertyChanged;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            }
        }

        public ICollection<TKey> Keys => _internalDictionary.Keys;

        public ICollection<TValue> Values => _internalDictionary.Values;

        public int Count => _internalDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            _internalDictionary.Add(key, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }

        public bool ContainsKey(TKey key) => _internalDictionary.ContainsKey(key);

        public bool Remove(TKey key)
        {
            if (_internalDictionary.Remove(key))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value) => _internalDictionary.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            _internalDictionary.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => _internalDictionary.Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _internalDictionary.ToArray().CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _internalDictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 항목이 변경되었음을 알림
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }
}
