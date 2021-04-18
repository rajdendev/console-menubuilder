using System;
using MenuBuilder;

class Program {
    private static void Main(string[] args) {
        Menu.Display("main-menu");
    }

    [Menu("main-menu")]
    private static void MainMenu() {
        Menu.Align(Alignment.Center);
        Menu.Space(3);
        Menu.Button("PROFILE", null);
        Menu.Line(40);
        Menu.Text("Main Menu");
        Menu.Line(40);
        Menu.Align(Alignment.Left);
        Menu.Text(new string[] { "Why is this here", "No u", "Only for test purposes" });
        Menu.Line(40);
        Menu.Button("PLAY", () => {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("TESTING");
            Console.ReadLine();
        });
        Menu.Space();
        Menu.Space();
        Menu.Button("SETTINGS", () => Menu.Display("test-menu"));
        Menu.Text(new string[] { "Why is this here", "No u", "Only for test purposes" });
        Menu.Space();
        Menu.Align(Alignment.Right);
        Menu.Button("QUIT", null);
        Menu.Line(40);
        Menu.Text("Text or something");
        Menu.Space();
        Menu.Line(40);
    }

    [Menu("test-menu")]
    private static void TestMenu() {
        Menu.Space(3);
        Menu.Text("Main Menu");
        Menu.Line(40);
        Menu.Button("PLAY", null);
        Menu.Button("SETTINGS", null);
        Menu.Button("QUIT", null);
        Menu.Line(40);
    }
}
