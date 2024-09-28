using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class Program
{
    static char[,] matrix;
    static Hashtable wordTable = new Hashtable();
    static Random random = new Random();
    static List<string> words = new List<string>();
    static List<string> hints = new List<string>();
    static int currentHint = 0;
    static List<string> foundWords = new List<string>(); // Lista para almacenar las palabras encontradas
    static List<(int row, int col, int direction, string word)> placedWords = new List<(int, int, int, string)>(); // Lista de palabras colocadas y su posición

    static void Main(string[] args)
    {
        Console.WriteLine("Iniciando el programa...");

        Menu();
        int choice = Convert.ToInt32(Console.ReadLine());

        if (choice == 1)
        {
            Easy();
        }
        else if (choice == 2)
        {
            Medium();
        }
        else if (choice == 3)
        {
            Hard();
        }
        else
        {
            Console.WriteLine("Invalid choice");
        }

        // Mostrar la sopa de letras
        ShowMatrix();

        // Ciclo para verificar las palabras ingresadas y cambiar pistas
        ConsoleKey key;
        do
        {
            ShowHint();
            Console.Write("Enter the word: ");
            string userInput = Console.ReadLine().ToUpper();

            Console.Clear(); // Limpiar la consola después de cada palabra ingresada

            CheckWord(userInput);

            key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.RightArrow)
            {
                ShowNextHint();
            }

            ShowWordsInHashtable(); // Mostrar palabras en Hashtable después de cada intento

        } while (key != ConsoleKey.Escape);
    }

    static public void Menu()
    {
        Console.WriteLine("Choose a difficulty: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("1. Easy");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("2. Medium");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("3. Hard");
        Console.ResetColor();
    }

    static public void Easy()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Easy mode selected");
        Console.ResetColor();
        Mtx(12, 12); // Aumentamos el tamaño de la matriz
        ReadFile("Easy.txt", 5);
        FillMatrix();
        FillWithRandomLetters();
    }

    static public void Medium()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Medium mode selected");
        Console.ResetColor();
        Mtx(15, 15); // Aumentamos el tamaño de la matriz
        ReadFile("Medium.txt", 10);
        FillMatrix();
        FillWithRandomLetters();
    }

    static public void Hard()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Hard mode selected");
        Console.ResetColor();
        Mtx(20, 20); // Aumentamos el tamaño de la matriz
        ReadFile("Hard.txt", 15);
        FillMatrix();
        FillWithRandomLetters();
    }

    static public void Mtx(int n, int m)
    {
        matrix = new char[n, m];
        Console.WriteLine($"Inicializando la matriz de {n} x {m}...");

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                matrix[i, j] = ' '; // Inicialmente dejamos todos los espacios vacíos
            }
        }

        Console.WriteLine("Matriz inicializada con espacios vacíos.");
    }

    static public void ReadFile(string file, int n)
    {
        Console.WriteLine($"Leyendo archivo {file}...");

        if (!File.Exists(file))
        {
            Console.WriteLine($"Error: El archivo {file} no se encuentra.");
            return;
        }

        words.Clear();
        hints.Clear();

        try
        {
            string[] lines = File.ReadAllLines(file);

            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 2)
                {
                    string word = parts[0].Trim().ToUpper();
                    string hint = parts[1].Trim();

                    words.Add(word);
                    hints.Add(hint);
                    wordTable.Add(word, hint); // Agregar la palabra y pista al Hashtable
                    Console.WriteLine($"Palabra agregada: {word} con pista: {hint}");
                }
                else
                {
                    Console.WriteLine($"Formato incorrecto en la línea: {line}");
                }
            }

            if (words.Count < n)
            {
                Console.WriteLine($"El archivo no tiene suficientes palabras. Se usarán todas las disponibles ({words.Count}).");
                n = words.Count;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al leer el archivo: {e.Message}");
        }
    }

    static public void FillMatrix()
    {
        Console.WriteLine("Comenzando a llenar la matriz con las palabras...");

        foreach (string word in words)
        {
            bool placed = false;
            int tries = 0;

            while (!placed && tries < 100) // Limitar a 100 intentos para evitar ciclos infinitos
            {
                int direction = random.Next(0, 3); // 0: horizontal, 1: vertical, 2: diagonal
                int row = random.Next(0, matrix.GetLength(0));
                int col = random.Next(0, matrix.GetLength(1));

                placed = TryPlaceWord(word, row, col, direction);
                tries++;

                if (placed)
                {
                    Console.WriteLine($"Palabra '{word}' colocada en la matriz.");
                    placedWords.Add((row, col, direction, word)); // Guardar la ubicación de la palabra
                }
                else
                {
                    Console.WriteLine($"No se pudo colocar la palabra '{word}' en la posición ({row},{col}) en el intento {tries}");
                }
            }

            if (!placed)
            {
                Console.WriteLine($"Error: No se pudo colocar la palabra '{word}' después de {tries} intentos.");
            }
        }

        ShowMatrix();
    }

    static public void FillWithRandomLetters()
    {
        Console.WriteLine("Rellenando la matriz con letras aleatorias...");

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (matrix[i, j] == ' ')
                {
                    matrix[i, j] = (char)random.Next('A', 'Z' + 1); // Rellenar espacios vacíos con letras aleatorias
                }
            }
        }

        Console.WriteLine("Matriz rellenada con letras aleatorias.");
        ShowMatrix();
    }

    static public bool TryPlaceWord(string word, int row, int col, int direction)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

        // Verificar si la palabra cabe en la matriz dependiendo de la dirección
        if (direction == 0 && col + word.Length > m) return false;  // Horizontal
        if (direction == 1 && row + word.Length > n) return false;  // Vertical
        if (direction == 2 && (row + word.Length > n || col + word.Length > m)) return false;  // Diagonal

        // Verificar si hay espacio suficiente sin sobrescribir letras no coincidentes
        for (int i = 0; i < word.Length; i++)
        {
            char currentChar = word[i];
            if (direction == 0 && matrix[row, col + i] != ' ' && matrix[row, col + i] != currentChar) return false;
            if (direction == 1 && matrix[row + i, col] != ' ' && matrix[row + i, col] != currentChar) return false;
            if (direction == 2 && matrix[row + i, col + i] != ' ' && matrix[row + i, col + i] != currentChar) return false;
        }

        // Colocar la palabra
        for (int i = 0; i < word.Length; i++)
        {
            if (direction == 0) matrix[row, col + i] = word[i];  // Horizontal
            if (direction == 1) matrix[row + i, col] = word[i];  // Vertical
            if (direction == 2) matrix[row + i, col + i] = word[i];  // Diagonal
        }

        return true;
    }

    static public void ShowMatrix()
    {
        Console.WriteLine("\nSopa de Letras:");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                bool isPartOfFoundWord = false;

                foreach (var (row, col, direction, word) in placedWords)
                {
                    if (foundWords.Contains(word))
                    {
                        if (direction == 0 && row == i && j >= col && j < col + word.Length) isPartOfFoundWord = true;  // Horizontal
                        if (direction == 1 && col == j && i >= row && i < row + word.Length) isPartOfFoundWord = true;  // Vertical
                        if (direction == 2 && i - row == j - col && i >= row && i < row + word.Length) isPartOfFoundWord = true;  // Diagonal
                    }
                }

                if (isPartOfFoundWord)
                {
                    Console.ForegroundColor = ConsoleColor.Green;  // Color verde para la palabra encontrada
                }

                Console.Write(matrix[i, j] + " ");
                Console.ResetColor();  // Restablecer color
            }
            Console.WriteLine();
        }
    }

    static public void ShowHint()
    {
        if (currentHint < hints.Count)
        {
            Console.WriteLine("Hint: " + hints[currentHint]);
        }
        else
        {
            Console.WriteLine("No hay más pistas.");
        }
    }

    static public void ShowNextHint()
    {
        currentHint = (currentHint + 1) % hints.Count;
        ShowHint();
    }

    static public void CheckWord(string word)
    {
        if (words.Contains(word.ToUpper()))
        {
            foundWords.Add(word.ToUpper()); // Agregar palabra encontrada a la lista
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Correct!");
            Console.ResetColor();
            ShowMatrix();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Incorrect!");
            Console.ResetColor();
        }
    }

    static public void ShowWordsInHashtable()
    {
        Console.WriteLine("\nPalabras en Hashtable:");
        foreach (DictionaryEntry entry in wordTable)
        {
            Console.WriteLine($"Palabra: {entry.Key}, Pista: {entry.Value}");
        }
    }
}
