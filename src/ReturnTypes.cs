namespace ReturnTypes
{
    struct Empty
    {
        public override readonly string ToString() => "()";
    }

    struct Result<T, E>
    {
        private T _value;
        public bool IsOk { get; private set; }
        private E _error;
        public readonly bool IsError
        {
            get
            {
                return !IsOk;
            }
        }

        public Result(T value)
        {
            SetOk(value);
        }

        public Result(E error)
        {
            SetErr(error);
        }

        public void SetOk(T value)
        {
            _value = value;
            IsOk = value != null;
        }

        public void SetErr(E error)
        {
            _error = error;
            IsOk = false;
        }

        public readonly T Value
        {
            get
            {
                if (!IsOk)
                    throw new InvalidOperationException("No value present");
                return _value;
            }
        }

        public E Error
        {
            get
            {
                if (!IsError)
                    throw new InvalidOperationException("No error present");
                return _error;
            }
        }

        public override readonly string ToString() => IsOk ? $"Ok({_value})" : $"Err({_error})";
    }

    struct Optional<T>
    {
        private T _value;
        public bool HasValue { get; private set; }

        public Optional()
        {
            HasValue = false;
        }

        public Optional(T value)
        {
            Set(value);
        }

        public void Set(T value)
        {
            _value = value;
            HasValue = value != null;
        }

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("No value present");
                return _value;
            }
        }

        public readonly bool IsSome() => HasValue;

        public readonly bool IsNone() => !HasValue;

        public readonly T GetOrDefault(T defaultValue = default) => HasValue ? _value : defaultValue;

        public override readonly string ToString() => HasValue ? $"Some({_value})" : "None";
    }
}