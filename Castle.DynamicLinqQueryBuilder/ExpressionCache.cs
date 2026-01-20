using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Castle.DynamicLinqQueryBuilder
{
    /// <summary>
    /// A thread-safe LRU (Least Recently Used) cache with bounded size.
    /// When the cache exceeds maxSize, the least recently accessed items are evicted.
    /// </summary>
    /// <typeparam name="TValue">The type of cached values.</typeparam>
    internal sealed class ExpressionCache<TValue> where TValue : class
    {
        private readonly int _maxSize;
        private readonly ConcurrentDictionary<string, LinkedListNode<CacheEntry>> _cache;
        private readonly LinkedList<CacheEntry> _lruList;
        private readonly object _lruLock = new();
        private int _currentSize;

        private sealed class CacheEntry
        {
            public string Key { get; }
            public TValue Value { get; }

            public CacheEntry(string key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        /// <summary>
        /// Creates a new expression cache with the specified maximum size.
        /// </summary>
        /// <param name="maxSize">Maximum number of entries to cache before eviction.</param>
        public ExpressionCache(int maxSize)
        {
            if (maxSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxSize), "Max size must be greater than 0.");
            
            _maxSize = maxSize;
            _cache = new ConcurrentDictionary<string, LinkedListNode<CacheEntry>>(StringComparer.Ordinal);
            _lruList = new LinkedList<CacheEntry>();
            _currentSize = 0;
        }

        /// <summary>
        /// Gets the current number of items in the cache.
        /// </summary>
        public int Count => _currentSize;

        /// <summary>
        /// Tries to get a cached value by key.
        /// If found, the item is moved to the front of the LRU list.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value if found.</param>
        /// <returns>True if the value was found, false otherwise.</returns>
        public bool TryGet(string key, out TValue? value)
        {
            if (_cache.TryGetValue(key, out var node))
            {
                // Move to front (most recently used)
                lock (_lruLock)
                {
                    // Node might have been removed between TryGetValue and lock acquisition
                    if (node.List != null)
                    {
                        _lruList.Remove(node);
                        _lruList.AddFirst(node);
                    }
                }
                value = node.Value.Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Adds or updates a cached value.
        /// If the cache exceeds maxSize, the least recently used items are evicted.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        public void Set(string key, TValue value)
        {
            var entry = new CacheEntry(key, value);
            var newNode = new LinkedListNode<CacheEntry>(entry);

            // Try to add the new node
            if (_cache.TryAdd(key, newNode))
            {
                lock (_lruLock)
                {
                    _lruList.AddFirst(newNode);
                    Interlocked.Increment(ref _currentSize);

                    // Evict if over capacity
                    while (_currentSize > _maxSize && _lruList.Last != null)
                    {
                        var lastNode = _lruList.Last;
                        _lruList.RemoveLast();
                        _cache.TryRemove(lastNode.Value.Key, out _);
                        Interlocked.Decrement(ref _currentSize);
                    }
                }
            }
            else
            {
                // Key already exists - update it
                lock (_lruLock)
                {
                    if (_cache.TryGetValue(key, out var existingNode))
                    {
                        // Remove old node from list
                        if (existingNode.List != null)
                        {
                            _lruList.Remove(existingNode);
                        }
                        
                        // Update dictionary and add new node to front
                        _cache[key] = newNode;
                        _lruList.AddFirst(newNode);
                    }
                }
            }
        }

        /// <summary>
        /// Gets an existing value or adds a new one if the key doesn't exist.
        /// Thread-safe and ensures the factory is called at most once per key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="valueFactory">Factory function to create the value if not found.</param>
        /// <returns>The cached or newly created value.</returns>
        public TValue GetOrAdd(string key, Func<string, TValue> valueFactory)
        {
            if (TryGet(key, out var existing) && existing != null)
            {
                return existing;
            }

            var value = valueFactory(key);
            Set(key, value);
            return value;
        }

        /// <summary>
        /// Clears all entries from the cache.
        /// </summary>
        public void Clear()
        {
            lock (_lruLock)
            {
                _cache.Clear();
                _lruList.Clear();
                Interlocked.Exchange(ref _currentSize, 0);
            }
        }
    }
}
