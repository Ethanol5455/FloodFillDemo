using System;
using System.Collections;

namespace FloodFill
{
    class FloodFill
    {
        private static void FillFromPoint(int thisX, int thisY, int id, bool targetValue, bool[,] inputMap, ref Optional<int>[,] outputMap)
        {
            if (thisX < 0 || thisY < 0
                || thisX >= inputMap.GetLength(0) || thisY >= inputMap.GetLength(1)
                || outputMap[thisX, thisY].IsSome()
                || inputMap[thisX, thisY] != targetValue)
                return;
            outputMap[thisX, thisY].Set(id);

            FillFromPoint(thisX + 1, thisY, id, targetValue, inputMap, ref outputMap);
            FillFromPoint(thisX - 1, thisY, id, targetValue, inputMap, ref outputMap);
            FillFromPoint(thisX, thisY + 1, id, targetValue, inputMap, ref outputMap);
            FillFromPoint(thisX, thisY - 1, id, targetValue, inputMap, ref outputMap);
        }

        public static Optional<int>[,] SolveFloodFill(bool[,] inputMap)
        {
            Optional<int>[,] idMap = new Optional<int>[inputMap.GetLength(0), inputMap.GetLength(1)];
            int id = 0;

            for (int i = 0; i < inputMap.GetLength(0); i++)
            {
                for (int j = 0; j < inputMap.GetLength(0); j++)
                {
                    if (!idMap[i, j].HasValue)
                    {
                        FillFromPoint(i, j, id, inputMap[i, j], inputMap, ref idMap);
                        id++;
                    }
                }
            }


            // Console.WriteLine(id);
            // FillFromPoint((0, 0), ref id, inputMap, ref idMap);
            // Console.WriteLine(id);
            return idMap;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool[,] map = { { false, false }, { true, true } };
            Console.WriteLine($"{map[0, 0]}");
            Optional<int>[,] outputMap = FloodFill.SolveFloodFill(map);
            Console.WriteLine($"{outputMap[0, 0]} {outputMap[0, 1]}");
            Console.WriteLine($"{outputMap[1, 0]} {outputMap[1, 1]}");
        }
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

        public bool IsSome() => HasValue;

        public bool IsNone() => !HasValue;

        public T GetOrDefault(T defaultValue = default) => HasValue ? _value : defaultValue;

        public override string ToString() => HasValue ? $"Some({_value.ToString()})" : "None";
    }
}