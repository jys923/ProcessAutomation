namespace SonoCap.MES.Models
{
    public class ThreadSafeCircularBuffer<T>
    {
        private readonly T[] _buffer;
        private readonly object _lock = new object();
        private readonly int _capacity;

        private int _head;    // 가장 오래된 요소 (읽기 시작 위치)
        private int _tail;    // 가장 최근 요소가 삽입될 위치 (쓰기 위치)
        private int _count;   // 현재 버퍼에 채워진 요소 수

        // 버퍼 크기를 10으로 고정합니다.
        public ThreadSafeCircularBuffer(int capacity = 10)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be positive.", nameof(capacity));
            }
            _capacity = capacity;
            _buffer = new T[_capacity];
        }

        /// <summary>
        /// 새로운 항목을 버퍼에 추가합니다. (가장 오래된 항목을 덮어씁니다.)
        /// 이미지 획득 스레드에서 호출됩니다.
        /// </summary>
        public void Put(T item)
        {
            lock (_lock)
            {
                _buffer[_tail] = item;
                _tail = (_tail + 1) % _capacity; // 다음 쓰기 위치로 이동 (순환)

                if (_count < _capacity)
                {
                    _count++;
                }
                else
                {
                    // 버퍼가 가득 찬 경우: Tail이 Head를 덮어썼으므로, 
                    // Head(가장 오래된 항목)도 Tail의 다음 위치로 순환 이동합니다.
                    _head = (_head + 1) % _capacity;
                }
            }
        }

        /// <summary>
        /// 현재 버퍼의 모든 항목을 순서대로 복사하여 배열로 반환합니다.
        /// 검사 요청 시 (C++로 전달) 호출됩니다.
        /// </summary>
        public T[] ToArrayInOrder()
        {
            lock (_lock)
            {
                if (_count == 0)
                {
                    return Array.Empty<T>();
                }

                T[] snapshot = new T[_count];

                // Head (가장 오래된 데이터)부터 순환 순서대로 읽기 시작
                for (int i = 0; i < _count; i++)
                {
                    int index = (_head + i) % _capacity;
                    snapshot[i] = _buffer[index];
                }
                return snapshot;
            }
        }

        public int Count
        {
            get { lock (_lock) { return _count; } }
        }
    }
}
