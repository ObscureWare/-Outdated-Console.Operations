# Console.Operations
Color control and System.Console base operations wrapper.

*![PayPal](https://github.com/ObscureWare/Console.Operations/blob/master/doc/pp64.png) If you find this library useful please consider [donating](https://www.paypal.me/SebastianGruchacz) to support my development.*

*![Nuget](https://github.com/ObscureWare/Console.Operations/blob/master/doc/nugetlogo.png) You can find Nuget [here](https://www.nuget.org/packages/ObscureWare.Console.Operations/)*

Or install from Nuget commandline:

>Install-Package ObscureWare.Console.Operations

#Demo Samples
For all demo examples console has been configured as follows:
```csharp
ConsoleController controller = new ConsoleController();
controller.ReplaceConsoleColors(
    new Tuple<ConsoleColor, Color>(ConsoleColor.DarkCyan, Color.Chocolate),
    new Tuple<ConsoleColor, Color>(ConsoleColor.Blue, Color.DodgerBlue),
    new Tuple<ConsoleColor, Color>(ConsoleColor.Yellow, Color.Gold),
    new Tuple<ConsoleColor, Color>(ConsoleColor.DarkBlue, Color.MidnightBlue));

IConsole console = new SystemConsole(controller, isFullScreen: false);
ConsoleOperations ops = new ConsoleOperations(console);
```

1. Adapting text colors to console capabilities
    ```csharp
    private static void PrintColorsMessages(IConsole console)
    {
        console.WriteText(0, 0, "test message", Color.Red, Color.Black);
        console.WriteText(0, 1, "test message 2", Color.Cyan, Color.YellowGreen);
        console.WriteText(0, 2, "test message 3d ds sfsdfsad ", Color.Orange, Color.Plum);
        console.WriteText(0, 3, "test messaf sdf s sfsdfsad ", Color.DarkOliveGreen, Color.Silver);
        console.WriteText(0, 4, "tsd fsfsd fds fsd f fa fas fad ", Color.AliceBlue, Color.PaleVioletRed);
        console.WriteText(0, 5, "tsd fsfsd fds fsd f fa fas fad ", Color.Blue, Color.CadetBlue);
        console.WriteText(0, 6, "tsd fsdfsdfsd fds fa fas fad ", Color.Maroon, Color.ForestGreen);

        // lol: http://stackoverflow.com/questions/3811973/why-is-darkgray-lighter-than-gray
        console.WriteText(0, 10, "test message", Color.Gray, Color.Black);
        console.WriteText(0, 11, "test message 2", Color.DarkGray, Color.Black);
        console.WriteText(0, 12, "test message 3d ds sfsdfsad ", Color.DimGray, Color.Black);
        console.WriteText(0, 20, "", Color.Gray, Color.Black); // reset

        Console.ReadLine();
        console.Clear();
    }
    ```
    ![](https://github.com/ObscureWare/Console.Operations/blob/master/demo/demo_1.png)

2. Printing all named colors using console colors
    ```csharp
    private static void PrintAllNamedColors(ConsoleController controller, IConsole console)
    {
        var props =
            typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(Color));
        foreach (var propertyInfo in props)
        {
            Color c = (Color) propertyInfo.GetValue(null);
            ConsoleColor cc = controller.CloseColorFinder.FindClosestColor(c);
            Console.ForegroundColor = cc;
            Console.WriteLine("{0,-25} {1,-18} #{2,-8:X}", propertyInfo.Name, Enum.GetName(typeof(ConsoleColor), cc),
                c.ToArgb());
        }

        Console.ReadLine();
        console.Clear();
    }
    ```

    ![](https://github.com/ObscureWare/Console.Operations/blob/master/demo/demo_2.png)

3. Printing text in frames (plain box or box with frame)
    ```csharp
    private static void PrintFrames(ConsoleOperations ops, IConsole console)
    {
        var text1Colors = new ConsoleFontColor(Color.Gold, Color.Black);
        var text2Colors = new ConsoleFontColor(Color.Brown, Color.Black);
        var text3Colors = new ConsoleFontColor(Color.Black, Color.Silver);
        var frame2Colors = new ConsoleFontColor(Color.Silver, Color.Black);
        var solidFrameTextColors = new ConsoleFontColor(Color.Red, Color.Yellow);
        var solidFrameColors = new ConsoleFontColor(Color.Yellow, Color.Black);
        FrameStyle block3Frame = new FrameStyle(frame2Colors, text3Colors, @"┌─┐││└─┘", '░');
        FrameStyle doubleFrame = new FrameStyle(frame2Colors, text3Colors, @"╔═╗║║╚═╝", '▒');
        FrameStyle solidFrame = new FrameStyle(solidFrameColors, solidFrameTextColors, @"▄▄▄██▀▀▀", '▓');

        ops.WriteTextBox(5, 5, 50, 7,
            @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis. Maecenas malesuada elit lectus felis, malesuada ultricies. Curabitur et ligula. Ut molestie a, ultricies porta urna. Vestibulum commodo volutpat a, convallis ac, laoreet enim. Phasellus fermentum in, dolor.",
            text1Colors);
        ops.WriteTextBox(25, 15, 80, 37,
            @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis. Maecenas malesuada elit lectus felis, malesuada ultricies. Curabitur et ligula. Ut molestie a, ultricies porta urna. Vestibulum commodo volutpat a, convallis ac, laoreet enim. Phasellus fermentum in, dolor. Pellentesque facilisis. Nulla imperdiet sit amet magna. Vestibulum dapibus, mauris nec malesuada fames ac turpis velit, rhoncus eu, luctus et interdum adipiscing wisi. Aliquam erat ac ipsum. Integer aliquam purus. Quisque lorem tortor fringilla sed, vestibulum id, eleifend justo vel bibendum sapien massa ac turpis faucibus orci luctus non, consectetuer lobortis quis, varius in, purus.",
            text2Colors);

        ops.WriteTextBox(new Rectangle(26, 26, 60, 20),
            @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis. Maecenas malesuada elit lectus felis, malesuada ultricies. Curabitur et ligula. Ut molestie a, ultricies porta urna. Vestibulum commodo volutpat a, convallis ac, laoreet enim. Phasellus fermentum in, dolor. Pellentesque facilisis. Nulla imperdiet sit amet magna. Vestibulum dapibus, mauris nec malesuada fames ac turpis velit, rhoncus eu, luctus et interdum adipiscing wisi. Aliquam erat ac ipsum. Integer aliquam purus. Quisque lorem tortor fringilla sed, vestibulum id, eleifend justo vel bibendum sapien massa ac turpis faucibus orci luctus non, consectetuer lobortis quis, varius in, purus.",
            block3Frame);
        ops.WriteTextBox(new Rectangle(8, 10, 30, 7),
            @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis.",
            doubleFrame);
        ops.WriteTextBox(new Rectangle(10, 20, 25, 7),
            @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis.",
            solidFrame);

        console.WriteText(0, 20, "", Color.Gray, Color.Black); // reset

        Console.ReadLine();
        console.Clear();
    }
    ```

    ![](https://github.com/ObscureWare/Console.Operations/blob/master/demo/demo_3.png)

4. Printing tables
    *... comming soon ...*
