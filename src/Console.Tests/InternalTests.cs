
namespace ConsoleTests
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using ObscureWare.Console;

    /// <summary>
    /// Most of this routines require instance of the console itself, so cannot be run as Unit test ;-(
    /// </summary>
    class InternalTests
    {
        public void PrintDefaultColors()
        {
            string fName = "default_setup.html";
            using (var colorHelper = new ConsoleManager())
            {
                PrintAllNamedColorsToHTML(colorHelper, fName);
            }
        }

        public void PrintCustomColors()
        {
            string fName = "custom_setup_01.html";
            using (var colorHelper = new ConsoleManager())
            {
                colorHelper.ReplaceConsoleColors(
                    new Tuple<ConsoleColor, Color>(ConsoleColor.DarkCyan, Color.Chocolate),
                    new Tuple<ConsoleColor, Color>(ConsoleColor.Blue, Color.DodgerBlue),
                    new Tuple<ConsoleColor, Color>(ConsoleColor.Yellow, Color.Gold),
                    new Tuple<ConsoleColor, Color>(ConsoleColor.DarkBlue, Color.MidnightBlue));

                PrintAllNamedColorsToHTML(colorHelper, fName);
            }
        }

        private static void PrintAllNamedColorsToHTML(ConsoleManager helper, string fName)
        {
            var props = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .Where(p => p.PropertyType == typeof(Color));

            var colorsVersionAtt = typeof(ConsoleManager).Assembly.GetCustomAttributes().FirstOrDefault(att => att is AssemblyFileVersionAttribute) as AssemblyFileVersionAttribute;
            string colorsVersion = colorsVersionAtt?.Version ?? "unknown";

            var dir = $"C:\\TestResults\\ConsoleColors\\{colorsVersion}\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var path = Path.Combine(dir, fName);

            using (var tw = new StreamWriter(path))
            {
                // TODO: print table with console colors

                tw.WriteLine("<html><body><table>");
                tw.WriteLine("<tr><th>ColorName</th><th>Sys Color</th><th>Console Color</th></tr>");

                foreach (var propertyInfo in props)
                {
                    tw.WriteLine("<tr>");

                    Color c = (Color)propertyInfo.GetValue(null);
                    ConsoleColor cc = helper.CloseColorFinder.FindClosestColor(c);

                    Color cCol = helper.CloseColorFinder.GetCurrentConsoleColor(cc);
                    var ccName = Enum.GetName(typeof(ConsoleColor), cc);

                    tw.WriteLine($"<td>{propertyInfo.Name}</td><td bgcolor=\"{c.ToRgbHex()}\">{c.Name}</td><td bgcolor=\"{cCol.ToRgbHex()}\">{ccName}</td>");
                    tw.WriteLine("</tr>");
                }

                tw.WriteLine("</table></body></html>");
                tw.Close();
            }
        }
    }

    
}
