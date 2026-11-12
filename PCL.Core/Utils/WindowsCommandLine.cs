using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCL.Core.Utils;

/// <summary>Quotes a single argument using the Windows C runtime command-line rules.</summary>
public static class WindowsCommandLine
{
    public static IReadOnlyList<string> ParseArguments(string commandLine)
    {
        ArgumentNullException.ThrowIfNull(commandLine);
        var arguments = new List<string>();
        var position = 0;
        while (position < commandLine.Length)
        {
            while (position < commandLine.Length && IsWhitespace(commandLine[position]))
                position++;
            if (position >= commandLine.Length)
                break;

            var argument = new StringBuilder();
            var inQuotes = false;
            var started = false;
            while (position < commandLine.Length &&
                   (inQuotes || !IsWhitespace(commandLine[position])))
            {
                var backslashes = 0;
                while (position < commandLine.Length && commandLine[position] == '\\')
                {
                    backslashes++;
                    position++;
                }

                if (position < commandLine.Length && commandLine[position] == '"')
                {
                    argument.Append('\\', backslashes / 2);
                    started = true;
                    if (backslashes % 2 != 0)
                    {
                        argument.Append('"');
                        position++;
                    }
                    else
                    {
                        position++;
                        if (inQuotes && position < commandLine.Length && commandLine[position] == '"')
                        {
                            argument.Append('"');
                            position++;
                        }
                        else
                        {
                            inQuotes = !inQuotes;
                        }
                    }
                }
                else
                {
                    argument.Append('\\', backslashes);
                    if (position < commandLine.Length && (inQuotes || !IsWhitespace(commandLine[position])))
                    {
                        argument.Append(commandLine[position++]);
                        started = true;
                    }
                    else if (backslashes > 0)
                    {
                        started = true;
                    }
                }
            }

            if (started)
                arguments.Add(argument.ToString());
            while (position < commandLine.Length && IsWhitespace(commandLine[position]))
                position++;
        }

        return arguments;
    }

    public static string QuoteArgument(string argument)
    {
        ArgumentNullException.ThrowIfNull(argument);
        var result = new StringBuilder(argument.Length + 2);
        result.Append('"');
        var backslashes = 0;
        foreach (var character in argument)
        {
            if (character == '\\')
            {
                backslashes++;
                continue;
            }

            if (character == '"')
            {
                result.Append('\\', backslashes * 2 + 1);
                result.Append('"');
            }
            else
            {
                result.Append('\\', backslashes);
                result.Append(character);
            }

            backslashes = 0;
        }

        result.Append('\\', backslashes * 2);
        result.Append('"');
        return result.ToString();
    }

    public static string FormatArguments(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        return string.Join(" ", arguments.Select(QuoteArgument));
    }

    private static bool IsWhitespace(char value) => value is ' ' or '\t';
}
