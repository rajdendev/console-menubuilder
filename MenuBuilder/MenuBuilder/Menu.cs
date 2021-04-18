using System;
using System.Reflection;
using System.Collections.Generic;

namespace MenuBuilder {
    public class Menu {
        private static bool registered = false;
        public static bool Registered {
            get => registered;
        }

        private readonly static Dictionary<string, Module> menus = new Dictionary<string, Module>();
        public static Dictionary<string, Module> Menus {
            get => menus;
        }

        private static Module menu = new Module();

        private static Alignment alignment = Alignment.Left;

        public static void Display(string id) {
            if (!registered) { Register(); }
            if (!menus.ContainsKey(id.ToLower())) { return; }

            id = id.ToLower();
            int buttonIndex = 0;
            bool enabled = true;

            while (enabled) {
                Console.Clear();
                Console.CursorVisible = false;

                int i = 0;
                foreach (IModule module in menus[id].modules) {
                    if (module is ButtonModule) {
                        if (buttonIndex == i) {
                            Console.ForegroundColor = menus[id].buttons[i].hoverColor;
                        }
                        else {
                            Console.ForegroundColor = menus[id].modules[i].Color;
                        }

                        i++;
                    }
                    else {
                        Console.ForegroundColor = menus[id].modules[i].Color;
                    }

                    module.Output();
                }

                enabled = Input(id, ref buttonIndex);
                buttonIndex = buttonIndex < 0 ? menus[id].buttons.Count - 1 : buttonIndex > menus[id].buttons.Count - 1 ? 0 : buttonIndex;
            }
        }

        private static bool Input(string id, ref int buttonIndex) {
            Console.ForegroundColor = ConsoleColor.Black;

            ConsoleKey key = Console.ReadKey().Key;
            if ((int)key >= 49 && (int)key <= 57) {
                if (!((int)key - 49 > menus[id].buttons.Count - 1)) {
                    buttonIndex = (int)key - 49;
                }

                return true;
            }

            switch (key) {
                // UP HOTKEYS
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                case ConsoleKey.NumPad8:
                    buttonIndex--;
                    return true;

                // DOWN HOTKEYS
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                case ConsoleKey.NumPad2:
                    buttonIndex++;
                    return true;

                // INTERACT HOTKEYS
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                case ConsoleKey.NumPad0:
                    menus[id].buttons[buttonIndex].func?.Invoke();
                    return true;

                // RETURN HOTKEYS
                case ConsoleKey.Escape:
                case ConsoleKey.Backspace:
                    return false;

                default: return true;
            }
        }

        public static void Text(string text) {
            TextModule textModule = new TextModule {
                text = text,
                alignment = Menu.alignment,
            };

            menu.modules.Add(textModule);
        }

        public static void Text(string[] text) {
            foreach (string str in text) {
                TextModule textModule = new TextModule {
                    text = str,
                    alignment = Menu.alignment,
                };

                menu.modules.Add(textModule);
            }
        }

        public static void Button(string text, Action func) {
            ButtonModule buttonModule = new ButtonModule();
            buttonModule.textModule.text = text;
            buttonModule.textModule.alignment = Menu.alignment;
            buttonModule.func = func;

            menu.buttons.Add(buttonModule);
            menu.modules.Add(buttonModule);
        }

        public static void Line(int length) {
            string line = "";
            for (int i = 0; i < length; i++) {
                line += "-";
            }

            TextModule textModule = new TextModule {
                text = line,
                alignment = Menu.alignment,
            };

            menu.modules.Add(textModule);
        }

        public static void Space(int rows = 1) {
            string space = "";
            for (int i = 0; i < rows - 1; i++) {
                space += "\n";
            }

            TextModule textModule = new TextModule {
                text = space,
                alignment = Menu.alignment,
            };

            menu.modules.Add(textModule);
        }

        public static void Align(Alignment alignment) {
            Menu.alignment = alignment;
        }

        private static void Print(string text, Alignment alignment) {
            switch (alignment) {
                case Alignment.Left: Console.SetCursorPosition(0, Console.CursorTop); break;
                case Alignment.Center: Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop); break;
                case Alignment.Right: Console.SetCursorPosition(Console.WindowWidth - text.Length - 1, Console.CursorTop); break;
                default: break;
            }

            foreach (char letter in text) {
                Console.Write(letter);
            }

            Console.WriteLine();
        }

        private static void Register() {
            BindingFlags methodFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
                foreach (MethodInfo method in type.GetMethods(methodFlags)) {
                    if (!(Attribute.GetCustomAttribute(method, typeof(MenuAttribute)) is MenuAttribute attribute)) { continue; }
                    if (menus.ContainsKey(attribute.ID)) { continue; }
                    if (method.GetParameters().Length != 0) { continue; }

                    menu = new Module {
                        modules = new List<IModule>(),
                        buttons = new List<ButtonModule>()
                    };

                    alignment = Alignment.Left;
                    Action func = (Action)Delegate.CreateDelegate(typeof(Action), method);
                    func?.Invoke();

                    menus.Add(attribute.ID, menu);
                }
            }

            registered = true;
        }

        public class TextModule : IModule {
            public string text = string.Empty;
            public ConsoleColor color = ConsoleColor.White;
            public Alignment alignment = Alignment.Left;

            public ConsoleColor Color {
                get {
                    return color;
                }
            }

            public void Output() {
                Print(text, alignment);
            }
        }

        public class ButtonModule : IModule {
            public TextModule textModule = new TextModule();
            public Action func = null;
            public ConsoleColor color = ConsoleColor.White;
            public ConsoleColor hoverColor = ConsoleColor.DarkYellow;

            public ConsoleColor Color {
                get {
                    return color;
                }
            }

            public void Output() {
                Print(textModule.text, textModule.alignment);
            }
        }

        public struct Module {
            public List<IModule> modules;
            public List<ButtonModule> buttons;

            public Module(List<IModule> modules, List<ButtonModule> buttons) {
                this.modules = modules;
                this.buttons = buttons;
            }
        }

        public interface IModule {
            void Output();
            ConsoleColor Color { get; }
        }
    }

    public enum Alignment { Left, Center, Right }
}
