using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    // TODO: Missing entries should return the Null type
    public class Dictionary : IPdfType<Dictionary>, IPdfType, IEnumerable<DictionaryEntry>
    {
        private readonly Hashtable _hashtable = new Hashtable(StringComparer.Ordinal);

        public IPdfType this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                return ((DictionaryEntry) _hashtable[name])?.Value ?? Null.Default;
            }
            set
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                var entry = new DictionaryEntry { Name = new Name(name), Value = value };
                _hashtable[name] = entry;
            }
        }

        public IPdfType this[Name name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                return ((DictionaryEntry) _hashtable[name.Value])?.Value ?? Null.Default;
            }
            set
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                var entry = new DictionaryEntry { Name = name, Value = value };
                _hashtable[name.Value] = entry;
            }
        }

        public T Get<T>(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.Length == 0) throw new ArgumentException(nameof(name));
            if (!_hashtable.ContainsKey(name)) throw new KeyNotFoundException($"The entry with name \"{name}\" could not be found.");

            var value = ((DictionaryEntry) _hashtable[name]).Value;
            if (value.Type != typeof(T)) throw new InvalidCastException($"Cannot cast entry value of type {value.Type.Name} to type {typeof(T).Name}.");
            return (T) value.Value;
        }

        public T Get<T>(Name name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return Get<T>(name.Value);
        }

        public void Add(string name, IPdfType value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var entry = new DictionaryEntry { Name = new Name(name), Value = value };
            _hashtable.Add(name, entry);
        }

        public void Add(Name name, IPdfType value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var entry = new DictionaryEntry { Name = name, Value = value };
            _hashtable.Add(name.Value, entry);
        }

        public void Remove(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _hashtable.Remove(name);
        }

        public void Remove(Name name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _hashtable.Remove(name.Value);
        }

        public void Clear()
        {
            _hashtable.Clear();
        }

        public bool Contains(string name)
        {
            if (name == null) return false;
            return _hashtable.ContainsKey(name);
        }

        public bool Contains(Name name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _hashtable.ContainsKey(name.Value);
        }

        public IEnumerator<DictionaryEntry> GetEnumerator()
        {
            return GetDictionaryEntries().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("<<");
            foreach (var entry in this) builder.Append(entry);      // TODO: These entries are coming out in reverse order. When items are added to the dictionary, they might be added at the front each time.
            builder.Append(">>");
            return builder.ToString();
        }

        public void Render(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<DictionaryEntry> GetDictionaryEntries()
        {
            var enumerator = _hashtable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return (DictionaryEntry)enumerator.Value;
            }
        }

        public Dictionary Value => this;
        public byte[] Data { get; }
        object IPdfType.Value { get; set; }
        Type IPdfType.Type { get; }

        //object IPdfType.Value => this;
        Dictionary IPdfType<Dictionary>.Value { get; set; }
        public ObjectState State { get; }
        public event EventHandler<ObjectState> StateChanged;
    }

    public class DictionaryEntry
    {
        public Name Name { get; internal set; }
        public IPdfType Value { get; internal set; }

        public override string ToString()
        {
            return $"{Name} {Value}";
        }
    }
}
