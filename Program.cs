using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices;

namespace MultiProgram
{
    class Program
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int MAXIMIZE = 3;

        public static int LiczbaPowtorzen = 0;
        public static int BledyParzyste = 0;
        public static int BledyNieparzyste = 0;
        public static string Sentence;
        public static int Nazwa = 0;
        public static ConsoleKey Klucz;

        public static bool Question()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Chcesz zagrać ponownie? (T/N)");
                Console.ResetColor();
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.T)
                {
                    return true;
                }
                else if (key.Key == ConsoleKey.N)
                {
                    return false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Wybrałeś zły przycisk! Wybierz poprawnie. Wciśnij dowolny przycisk.");
                    Console.ResetColor();
                    Console.ReadKey(true);
                }
            }
        }

        public static string Zdania()
        {
            LiczbaPowtorzen++;
            Console.Clear();
            string[] zdania = File.ReadAllLines("../../../zdania.txt"); //należy zmienić ścieżkę przy Publikowaniu do pliku .exe (aktualna ścieżka to główny folder projektu)
            Random random = new Random();
            return zdania[random.Next(0, zdania.Length)];
        }

        public static string Losowe()
        {
            LiczbaPowtorzen++;
            Console.Clear();
            Random random = new Random();
            Random random2 = new Random();
            const string znaki = "AĄBCĆDEĘFGHIJKLŁMNŃOÓPQRSŚTUVWXYZŹŻaąbcćdeęfghijklłmnńoóprsśtuvwxyzźż0123456789[];',./-=_+{}\\|:<>'\"?!@#$%^&*() ";
            return new string(Enumerable.Repeat(znaki, random2.Next(5,20)).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void Input()
        {
            bool poprawnyWybor = false;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Wybierz wersję trudności:");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("================================================");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1 - Zdania");
                Console.WriteLine("2 - Losowe znaki");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("================================================");
                Console.ResetColor();
                string length = null;
                int bledy = 0;
                var trudnosc = Console.ReadKey(true).Key;
                if (trudnosc == ConsoleKey.D1)
                {
                    Sentence = Zdania();
                    poprawnyWybor = true;
                }
                else if (trudnosc == ConsoleKey.D2)
                {
                    Sentence = Losowe();
                    poprawnyWybor = true;
                }
                else
                {
                    Console.WriteLine("Wybrano błędną opcję.  Wciśnij dowolny przycisk i spróbuj ponownie.");
                    Sentence = "";
                    Console.ReadKey();
                }
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Napisz:");
                Console.Write($"{Sentence}");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\nWybierz \"Esc\" aby przerwać wpisywanie zdania");
                Console.ResetColor();
                int cursorPosition = Console.CursorTop;
                Stopwatch sw = new Stopwatch();
                for (int i = 0; i < Sentence.Length; i++)
                {
                    var EscapeKey = Console.ReadKey(true);
                    Klucz = EscapeKey.Key;
                    if (i == 0) { sw.Start(); }
                    if (EscapeKey.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    else
                    {
                        if (EscapeKey.KeyChar == Sentence[i])
                        {
                            length += EscapeKey.KeyChar;
                            Console.Write(EscapeKey.KeyChar);
                            if (i == Sentence.Length) { sw.Stop(); }
                        }
                        else
                        {
                            i--;
                            bledy++;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("\nBLAD");
                            Console.ResetColor();
                            Thread.Sleep(500);
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write("     ");
                            Console.SetCursorPosition(0, cursorPosition);
                            Console.Write(length);
                        }
                    }
                }
                if (Klucz != ConsoleKey.Escape)
                {
                    Console.WriteLine($"\nLiczba błędów: {bledy}");
                    if (LiczbaPowtorzen != 1 && Klucz != ConsoleKey.Escape)
                    {
                        if (LiczbaPowtorzen % 2 == 0)
                        {
                            BledyParzyste = bledy;
                            if (BledyParzyste - BledyNieparzyste < 0)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                Console.WriteLine($"Obecna liczba błędów jest o {BledyNieparzyste - BledyParzyste} mniejsza niż przy poprzednim podejściu");
                                Console.ResetColor();
                            }
                            else if (BledyParzyste - BledyNieparzyste > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                Console.WriteLine($"Obecna liczba błędów jest o {BledyParzyste - BledyNieparzyste} większa niż przy poprzednim podejściu");
                                Console.ResetColor();
                            }
                            else if (BledyNieparzyste == BledyParzyste)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Obecna ilość błędów jest taka sama jak za poprzednim razem");
                                Console.ResetColor();
                            }
                            BledyNieparzyste = 0;
                        }
                        else
                        {
                            BledyNieparzyste = bledy;
                            if (BledyNieparzyste - BledyParzyste < 0)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                Console.WriteLine($"Obecna liczba błędów jest o {BledyParzyste - BledyNieparzyste} mniejsza niż przy poprzednim podejściu");
                                Console.ResetColor();
                            }
                            else if (BledyNieparzyste - BledyParzyste > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                Console.WriteLine($"Obecna liczba błędów jest o {BledyNieparzyste - BledyParzyste} większa niż przy poprzednim podejściu");
                                Console.ResetColor();
                            }
                            else if (BledyParzyste == BledyNieparzyste)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Obecna ilość błędów jest taka sama jak za poprzednim razem");
                                Console.ResetColor();
                            }
                            BledyParzyste = 0;
                        }
                    }
                    else
                    {
                        if (LiczbaPowtorzen % 2 == 0)
                        {
                            BledyParzyste = bledy;
                            BledyNieparzyste = 0;
                        }
                        else
                        {
                            BledyNieparzyste = bledy;
                            BledyParzyste = 0;
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Czas: {sw.Elapsed}");
                    Console.ResetColor();
                    Console.WriteLine("Wciśnij enter");
                    Console.ReadLine();
                }
            } while (!poprawnyWybor);
        }

        public static void KeyboardTyper()
        {

            Console.Title = "Keyboard Typer";
            bool isGameOn;

            do
            {
                Input();
                isGameOn = Question();
            }
            while (isGameOn);

        }

        public static void MorseEmiter()
        {
            Console.Title = "Morse";
            char[] alphabet =
            {
                'a',
                'b',
                'c',
                'd',
                'e',
                'f',
                'g',
                'h',
                'i',
                'j',
                'k',
                'l',
                'm',
                'n',
                'o',
                'p',
                'q',
                'r',
                's',
                't',
                'u',
                'v',
                'w',
                'x',
                'y',
                'z',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
                '0'
            };
            string[] morse =
            {
                "10",
                "0111",
                "0101",
                "011",
                "1",
                "1101",
                "001",
                "1111",
                "11",
                "1000",
                "010",
                "1011",
                "00",
                "01",
                "000",
                "1001",
                "0010",
                "101",
                "111",
                "0",
                "110",
                "1110",
                "100",
                "0110",
                "0100",
                "0011",
                "10000",
                "11000",
                "11100",
                "11110",
                "11111",
                "01111",
                "00111",
                "00011",
                "00001",
                "00000"
            };
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Dźwięk inicjujący");
            Console.ResetColor();
            Console.Beep(2000, 300);
            Console.Beep(2000, 300);
            Console.Beep(2000, 300);
            Console.Beep(2000, 800);
            Console.Beep(2000, 300);
            Console.Beep(2000, 300);
            Console.Beep(2000, 300);
            Console.Beep(2000, 800);
            Console.Beep(2000, 300);
            Console.Beep(2000, 300);
            Console.Beep(2000, 300);
            Console.Beep(2000, 800);
            Console.Clear();
            bool dzialaj = true;
            while (dzialaj)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Podaj tekst do zamienienia w alfabet Morse'a:");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                string text = Console.ReadLine();
                Console.ResetColor();
                text = text.ToLower();
                for (int z = 0; z < text.Length; z++)
                {
                    char character = text[z];
                    if (alphabet.Contains(character))
                    {
                        string szukanySygnal = morse[Array.IndexOf(alphabet, character)];
                        for (int i = 0; i < szukanySygnal.Length; i++)
                        {
                            if (szukanySygnal[i] == '1')
                            {
                                Console.Beep(2000, 300);
                            }
                            if (szukanySygnal[i] == '0')
                            {
                                Console.Beep(2000, 800);
                            }
                        }
                    }
                    else
                    {
                        if (character != ' ')
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Niestety program nie obsługuje znaku '{character}'");
                            Console.ResetColor();
                        }
                    }
                    Thread.Sleep(800);
                }
                bool ok2 = true;
                while (ok2) //wychodzi jeśli t lub n
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Chcesz kontynuować?(T/N)");
                    Console.ResetColor();
                    var tLubNKey = Console.ReadKey().Key;
                    if (tLubNKey == ConsoleKey.T)
                    {
                        Console.Clear();
                        ok2 = false;
                    }
                    else if (tLubNKey == ConsoleKey.N)
                    {
                        dzialaj = false;
                        ok2 = false;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wybrałeś zły przycisk! Wybierz poprawną opcję.");
                        Console.ResetColor();
                        Thread.Sleep(800);
                        Console.Clear();
                    }
                }
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Dźwięk zamykający");
            Console.ResetColor();
            Console.Beep(2000, 300);
            Console.Beep(2000, 300);
            Console.Beep(2000, 300);
            Console.Beep(2000, 800);
            Console.Beep(2000, 300);
            Console.Beep(2000, 800);
            Console.Clear();
        }

        public static void Startup()
        {
            while (true)
            {
                if (Nazwa > 0)
                {
                    Console.Clear();
                }
                Nazwa++;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Wybierz aplikację, którą chcesz uruchomić:");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("================================================");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1 -\tKeyboard Typer");
                Console.WriteLine("2 -\tMorse Code Emiter");
                Console.WriteLine("0 -\tWyjście z programu");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("================================================");
                Console.ResetColor();
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        KeyboardTyper();
                        break;
                    case ConsoleKey.D2:
                        MorseEmiter();
                        break;
                    case ConsoleKey.D0:
                        Console.Clear();
                        return;
                    case ConsoleKey.Escape:
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wybrałeś złą opcję! Wybierz poprawnie.");
                        Console.ResetColor();
                        Thread.Sleep(700);
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "";
            ShowWindow(ThisConsole, MAXIMIZE);
            Startup();
        }
    }
}
