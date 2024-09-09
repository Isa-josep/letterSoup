using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Debug {
    private static readonly string resetColor = "\u001b[0m";
    private static readonly string blueColor = "\u001b[34m";
    private static readonly string greenColor = "\u001b[32m";
    private static readonly string purpleColor = "\u001b[35m";

    public static void debug(object arg, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
        var argName = GetVariableName(arg, filePath, lineNumber);
        Log(arg, argName, lineNumber);
    }

    private static void Log(object arg, string argName, int lineNumber) {
        Console.Write($"{greenColor}[{lineNumber}] {blueColor}{argName}: {resetColor}");
        Print(arg);
        Console.WriteLine();
    }

    private static void Print(object obj) {
        if (obj == null) {
            Console.Write("null ");
            return;
        }

        var type = obj.GetType();

        if (obj is string) {
            Console.Write($"{purpleColor}{obj}{resetColor}, ");
        } else if (!type.IsEnumerableType()) {
            Console.Write($"{purpleColor}{obj}{resetColor}, ");
        } else {
            PrintEnumerable(obj as IEnumerable);
        }
    }

    private static void PrintEnumerable(IEnumerable enumerable) {
        Console.Write("[");

        var isFirst = true;
        foreach (var item in enumerable) {
            if (!isFirst)
                Console.Write(", ");
            Console.Write($"{purpleColor}{item}{resetColor}");
            isFirst = false;
        }

        Console.Write("]");
    }

    private static bool IsEnumerableType(this Type type) {
        return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }

    private static string GetVariableName(object arg, string filePath, int lineNumber) {
        if (arg is string) {
            return string.Empty;
        }

        string[] lines = System.IO.File.ReadAllLines(filePath);
        string line = lines[lineNumber - 1];
        string[] parts = line.Split(new[] { "debug(" }, StringSplitOptions.None);
        if (parts.Length > 1) {
            string variablePart = parts[1];
            int endIndex = variablePart.IndexOf(')');
            if (endIndex != -1) {
                return variablePart.Substring(0, endIndex).Trim();
            }
        }
        return "unknown";
    }
}