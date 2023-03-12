namespace Lab3;

public class Program
{
    
    public static void Main()
    { 
        var inputString = Console.ReadLine() ?? string.Empty;
        {
            // Обрахування точності виводу результату.
            // Якщо дано ціле число, то буде забезпечено мінімальну точність
            // у 5 знаків після крапки 
            var numberParts = inputString.Split(".");
            var precision = 0;
            if (numberParts.Length > 1)
            {
                var fractionalPart = numberParts[1];
                precision = fractionalPart.Length;
                if (fractionalPart.EndsWith(";"))
                {
                    precision--;
                }
            }
            
            var isValidNumber = Analyzer.GetValue(inputString, out var result);
            Console.WriteLine("Чи є шіснадцядковим числом з плаваючою точкою: " + isValidNumber);
            Console.WriteLine("Результат: " + Converter.ConvertDecimalToHex(result, precision));
        }
    }
    
} 

// Клас, що представляє аналізатор відповідності ланцюжка
// граматиці шіснадцяткового числа з плаваючою комою
internal static class Analyzer
{
    // Перерахування станів автомату
    private enum States { S, A, B, C, D, E, F, G, H, I, J, K }
    
    private static readonly Dictionary <char, int> Digit2Value = new()
    {
        {'0', 0},
        {'1', 1},
        {'2', 2},
        {'3', 3},
        {'4', 4},
        {'5', 5},
        {'6', 6},
        {'7', 7},
        {'8', 8},
        {'9', 9},
        {'a', 10},
        {'b', 11},
        {'c', 12},
        {'d', 13},
        {'e', 14},
        {'f', 15},
    };
    
    public static bool GetValue(string s, out double value)
    {
        value = 0; // Значення числа
        
        var sign = 1; // Знак числа
        var f = 0.0625; // Значення порядку дробної частини числа
        
        var position = 0; // Поточний номер позиції в рядку
        var state = States.S;

        var powerSign = 1; // знак значення степеню
        var powerValue = 0; // значення степеню
        
        while (state != States.F && state != States.E && s.Length > position)
        {
            var symbol = s[position]; // Символ, що аналізується
            switch (state)
            {
                case States.S:
                {
                    switch (symbol)
                    {
                        case '+':
                        {
                            state = States.A;
                            break;
                        }
                        case '-':
                        {
                            state = States.A;
                            sign = -1;
                            break;
                        }
                        case '0':
                        {
                            state = States.C;
                            break;
                        }
                        case '1': case '2': case '3': case '4': case '5':
                        case '6': case '7': case '8': case '9': case 'a':
                        case 'b': case 'c': case 'd': case 'e': case 'f':
                        {
                            state = States.B;
                            value = value * 16 + Digit2Value[symbol];
                            break;
                        }
                        default:
                        {
                            // Помилка! Очікується знак або цифра.
                            state = States.E;
                            break;
                        }
                    }
                    break;
                }
                case States.A:
                {
                    switch (symbol)
                    {
                        case '0':
                        {
                            state = States.C;
                            break;
                        }
                        case '1': case '2': case '3': case '4': case '5':
                        case '6': case '7': case '8': case '9': case 'a':
                        case 'b': case 'c': case 'd': case 'e': case 'f':
                        {
                            state = States.B;
                            value = value * 16 + Digit2Value[symbol];
                            break;
                        }
                        default:
                        {
                            // Помилка в цілій частині числа!
                            state = States.E;
                            break;
                        }
                    }

                    break;
                }
                case States.B:
                {
                    switch (symbol)
                    {
                        case ';':
                        {
                            state = States.F;
                            break;
                        }
                        case '.':
                        {
                            state = States.D;
                            break;
                        }
                        case 'p':
                        {
                            state = States.H;
                            break;
                        }
                        case '1': case '2': case '3': case '4': case '5':
                        case '6': case '7': case '8': case '9': case 'a':
                        case 'b': case 'c': case 'd': case 'e': case 'f':
                        {
                            state = States.B;
                            value = value * 16 + Digit2Value[symbol];
                            break;
                        }
                        default:
                        {
                            // Помилка! Очікується ".", "p", ";" або цифра
                            state = States.E;
                            break;
                        }
                    }
                    break;
                }
                case States.C:
                {
                    switch (symbol)
                    {
                        case ';':
                        {
                            state = States.F;
                            break;
                        }
                        case '.':
                        {
                            state = States.D;
                            break;
                        }
                        default:
                        {
                            // Помилка! Очікується ";" або "."
                            state = States.E;
                            break;
                        }
                    }
                    break;
                }
                case States.D:
                {
                    switch (symbol)
                    {
                        case '0': case '1': case '2': case '3': case '4':
                        case '5': case '6': case '7': case '8': case '9': 
                        case 'a': case 'b': case 'c': case 'd': case 'e':
                        case 'f':
                        {
                            state = States.G;
                            value += f * Digit2Value[symbol];
                            f /= 16;
                            break;
                        }
                        default:
                        {
                            // Помилка! Очікується цифра дробової частини
                            state = States.E;
                            break;
                        }
                    }
                    break;
                }
                case States.G:
                {
                    switch (symbol)
                    {
                        case ';':
                        {
                            state = States.F;
                            break;
                        }
                        case 'p':
                        {
                            state = States.H;
                            break;
                        }
                        case '0': case '1': case '2': case '3': case '4':
                        case '5': case '6': case '7': case '8': case '9': 
                        case 'a': case 'b': case 'c': case 'd': case 'e':
                        case 'f':
                        {
                            state = States.G;
                            value += f * Digit2Value[symbol];
                            f /= 16;
                            break;
                        }
                        default:
                        {
                            // Помилка! Очікується "." або ";"
                            state = States.E;
                            break;
                        }
                    }
                    break;
                }
                case States.H:
                {
                    switch (symbol)
                    {
                        case '+':
                        {
                            state = States.I;
                            break;
                        }
                        case '-':
                        {
                            state = States.I;
                            powerSign = -1;
                            break;
                        }
                        case '0':
                        {
                            state = States.K;
                            break;
                        }
                        case '1': case '2': case '3': case '4': case '5':
                        case '6': case '7': case '8': case '9': 
                        {
                            state = States.J;
                            powerValue = powerValue * 10 + Digit2Value[symbol];
                            break;
                        }
                        default:
                        {
                            // Помилка! Очікується знак або цифра.
                            state = States.E;
                            break;
                        }
                    }
                    break;
                }
                case States.I:
                {
                    switch (symbol)
                    {
                        case '0':
                        {
                            state = States.K;
                            break;
                        }
                        case '1': case '2': case '3': case '4': case '5':
                        case '6': case '7': case '8': case '9': 
                        {
                            state = States.J;
                            powerValue = powerValue * 10 + Digit2Value[symbol];
                            f /= 10;
                            break;
                        }
                        default:
                        {
                            // Помилка!
                            state = States.E;
                            break;
                        }
                    }
                    break;
                }
                case States.J:
                {
                    switch (symbol)
                    {
                        case ';':
                        {
                            state = States.F;
                            break;
                        }
                        case '0': case '1': case '2': case '3': case '4': 
                        case '5': case '6': case '7': case '8': case '9': 
                        {
                            state = States.J;
                            powerValue = powerValue * 10 + Digit2Value[symbol];
                            break;
                        }
                        default:
                        {
                            // Помилка!
                            state = States.E;
                            break;
                        }
                    }
                    break;
                }
                case States.K:
                {
                    state = symbol switch
                    {
                        ';' => States.F,
                        _ => States.E
                    };
                    break;
                }
            }

            position++;
        }

        value *= sign;
        powerValue *= powerSign;
        value *= Math.Pow(2, powerValue);
        return state == States.F;
    }
}

// Клас конвертер із десяткової у шістнадцяткову систему числення 
internal static class Converter
{
    // Словник значень в десятковій системі до цифр шіснядцяткової системи
    private static readonly Dictionary <int, char> Value2Digit = new()
    {
        {10, 'a'},
        {11, 'b'},
        {12, 'c'},
        {13, 'd'},
        {14, 'e'},
        {15, 'f'}
    };
    
    // Метод, що переводить число з десяткової системи числення до шіснадцяткової
    // з можливістю задати точність, мінімальна точність 5 символів. 
    public static string ConvertDecimalToHex(double decimalNumber, int precision)
    {
        var hexResult = "";
        
        // Враховуємо знак числа
        if (decimalNumber < 0)
        {
            hexResult += "-";
            decimalNumber *= -1;
        }

        // Конвертації цілої частини
        var remainders = new List<string>();
        var integerDecimalPart = (int) decimalNumber;
        while (integerDecimalPart > 0)
        {
            var reminderI = integerDecimalPart % 16;
            remainders.Add(reminderI < 10 ? reminderI.ToString() : Value2Digit[reminderI].ToString());
            integerDecimalPart /= 16;
        }

        if (remainders.Count == 0)
        {
            hexResult += "0";
        }
        else
        {
            remainders.Reverse();
            hexResult += string.Join("", remainders);
        }
        
        // Конвертація дробової частини
        hexResult += ".";
        var fractionalPartHexDigits = new List<string>();
        var fractionalPart = decimalNumber - (int)decimalNumber;
        precision = Math.Max(5, precision);
        while (precision > 0)
        {
            fractionalPart *= 16;
            var integerPart = (int)fractionalPart;
            fractionalPartHexDigits.Add(integerPart < 10 ? integerPart.ToString() : Value2Digit[integerPart].ToString());
            fractionalPart -= integerPart;
            precision--;
        }
        
        var fractionalPartHexDigitsSet = new HashSet<string>(fractionalPartHexDigits);
        if (fractionalPartHexDigits.Count == 0 || (fractionalPartHexDigitsSet.Count == 1 && fractionalPartHexDigitsSet.Contains("0")))
        {
            hexResult += "0";
        }
        else
        {
            hexResult += string.Join("", fractionalPartHexDigits);
        }
        
        return hexResult;
    }

}

