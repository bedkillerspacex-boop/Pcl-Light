using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCL.Core.Utils;

namespace PCL.Core.Test.Utils;

[TestClass]
public sealed class WindowsCommandLineTest
{
    [TestMethod]
    public void QuotesWhitespaceAndEscapesTrailingBackslashes()
    {
        Assert.AreEqual("\"C:\\Program Files\\Java\\bin\\javaw.exe\"",
            WindowsCommandLine.QuoteArgument(@"C:\Program Files\Java\bin\javaw.exe"));
        Assert.AreEqual("\"C:\\path with space\\\\\"",
            WindowsCommandLine.QuoteArgument(@"C:\path with space\"));
        Assert.AreEqual("\"a\\\"b\"",
            WindowsCommandLine.QuoteArgument("a\"b"));
        Assert.AreEqual("\"\"", WindowsCommandLine.QuoteArgument(string.Empty));
    }

    [TestMethod]
    public void FormattedArgumentsRoundTripThroughWindowsParser()
    {
        var expected = new[]
        {
            @"C:\Program Files\Java\bin\javaw.exe",
            @"C:\path with a trailing slash\",
            "an embedded \" quote",
            string.Empty,
            "plain"
        };
        var commandLine = "probe.exe " + WindowsCommandLine.FormatArguments(expected);
        var actual = ParseWithWindows(commandLine);

        CollectionAssert.AreEqual(expected, actual);
    }

    private static string[] ParseWithWindows(string commandLine)
    {
        var argumentsPointer = CommandLineToArgvW(commandLine, out var count);
        Assert.AreNotEqual(IntPtr.Zero, argumentsPointer);
        try
        {
            var result = new List<string>(count - 1);
            for (var index = 1; index < count; index++)
            {
                var argumentPointer = Marshal.ReadIntPtr(argumentsPointer, index * IntPtr.Size);
                result.Add(Marshal.PtrToStringUni(argumentPointer)!);
            }

            return result.ToArray();
        }
        finally
        {
            LocalFree(argumentsPointer);
        }
    }

    [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr CommandLineToArgvW(string commandLine, out int argumentCount);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LocalFree(IntPtr handle);
}
