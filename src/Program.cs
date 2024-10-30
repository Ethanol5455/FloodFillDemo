using System;
using System.Collections;
using System.Numerics;
using System.Text;
using ReturnTypes;

namespace FloodFill
{
    class FloodFill
    {
        private static void FillFromPointRecursive(int thisX, int thisY, int id, bool targetValue, bool[,] inputMap, ref Optional<int>[,] outputMap)
        {
            if (thisX < 0 || thisY < 0
                || thisX >= inputMap.GetLength(0) || thisY >= inputMap.GetLength(1)
                || outputMap[thisX, thisY].IsSome()
                || inputMap[thisX, thisY] != targetValue)
                return;
            outputMap[thisX, thisY].Set(id);

            FillFromPointRecursive(thisX + 1, thisY, id, targetValue, inputMap, ref outputMap);
            FillFromPointRecursive(thisX - 1, thisY, id, targetValue, inputMap, ref outputMap);
            FillFromPointRecursive(thisX, thisY + 1, id, targetValue, inputMap, ref outputMap);
            FillFromPointRecursive(thisX, thisY - 1, id, targetValue, inputMap, ref outputMap);
        }

        public static int[,] SolveFloodFill(bool[,] inputMap)
        {
            Optional<int>[,] idMapOptional = new Optional<int>[inputMap.GetLength(0), inputMap.GetLength(1)];
            int id = 0;

            for (int i = 0; i < inputMap.GetLength(0); i++)
            {
                for (int j = 0; j < inputMap.GetLength(1); j++)
                {
                    if (!idMapOptional[i, j].HasValue)
                    {
                        FillFromPointRecursive(i, j, id, inputMap[i, j], inputMap, ref idMapOptional);
                        id++;
                    }
                }
            }

            int[,] idMap = new int[inputMap.GetLength(0), inputMap.GetLength(1)];

            for (int i = 0; i < inputMap.GetLength(0); i++)
            {
                for (int j = 0; j < inputMap.GetLength(1); j++)
                {
                    idMap[i, j] = idMapOptional[i, j].Value;
                }
            }

            return idMap;
        }

        public static Result<bool[,], IOException> ReadMapFromFile(string inPath)
        {
            List<bool[]> map = [];
            Optional<char> firstChar = new();
            Optional<char> secondChar = new();
            var lines = File.ReadLines(inPath);
            foreach (string line in lines)
            {
                if (map.Count > 0 && line.Length != map[0].Length)
                    return new Result<bool[,], IOException>(new IOException("Line lengths are not equal"));
                bool[] lineValues = new bool[line.Length];
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (firstChar.IsSome())
                    {
                        if (secondChar.IsSome())
                        {
                            if (c != secondChar.Value && c != firstChar.Value)
                            {
                                return new Result<bool[,], IOException>(new IOException($"More than two distinct characters in input file!\nFirst char is {firstChar}\nSecond char is {secondChar}\nThird char is \"{c}\")"));
                            }
                        }
                        else if (c != firstChar.Value)
                        {
                            secondChar.Set(c);
                        }
                    }
                    else
                    {
                        firstChar.Set(c);
                    }

                    lineValues[i] = c == firstChar.Value;
                }
                map.Add(lineValues);
            }

            bool[,] arrayMap = new bool[map.Count, map[0].Length];

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    arrayMap[i, j] = map[i][j];
                }
            }

            return new Result<bool[,], IOException>(arrayMap);
        }

        public static Result<Empty, Exception> WriteMapToFile(string outPath, int[,] outputMap)
        {
            try
            {
                using StreamWriter file = new(outPath);
                for (int i = 0; i < outputMap.GetLength(0); i++)
                {
                    var builder = new StringBuilder();
                    for (int j = 0; j < outputMap.GetLength(1); j++)
                    {
                        builder.Append(outputMap[i, j]);
                        if (j != outputMap.GetLength(1) - 1)
                            builder.Append(' ');
                    }
                    if (i != outputMap.GetLength(0) - 1)
                        builder.AppendLine();
                    file.Write(builder.ToString());
                }

                return new Result<Empty, Exception>(new Empty());
            }
            catch (Exception e)
            {
                return new Result<Empty, Exception>(e);
            }

        }
    }

    class Program
    {
        static int Main()
        {
            var readResult = FloodFill.ReadMapFromFile("demo_files/zero_one.ffin");
            if (readResult.IsError)
            {
                Console.Error.WriteLine($"Could not read input map with error {readResult.Error}");
                return -1;
            }
            int[,] outputMap = FloodFill.SolveFloodFill(readResult.Value);
            var writeResult = FloodFill.WriteMapToFile("demo_files/zero_one.ffout", outputMap);
            if (writeResult.IsError)
            {
                Console.Error.WriteLine($"Could not write output map with error {readResult.Error}");
                return -1;
            }

            return 0;
        }
    }

}