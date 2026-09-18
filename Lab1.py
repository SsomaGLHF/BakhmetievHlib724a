"""
Простой консольный калькулятор.
Поддерживает: +, -, *, / (и // и % как бонус).
"""
 
 
def calculate(a: float, op: str, b: float) -> float:
    if op == "+":
        return a + b
    elif op == "-":
        return a - b
    elif op == "*":
        return a * b
    elif op == "/":
        if b == 0:
            raise ZeroDivisionError("Деление на ноль недопустимо")
        return a / b
    elif op == "//":
        if b == 0:
            raise ZeroDivisionError("Деление на ноль недопустимо")
        return a // b
    elif op == "%":
        if b == 0:
            raise ZeroDivisionError("Деление на ноль недопустимо")
        return a % b
    else:
        raise ValueError(f"Неизвестная операция: {op}")
 
 
def get_number(prompt: str) -> float:
    while True:
        try:
            return float(input(prompt))
        except ValueError:
            print("Ошибка: введите число (например, 3.14 или -7).")
 
 
def get_operator() -> str:
    valid_ops = {"+", "-", "*", "/", "//", "%"}
    while True:
        op = input(f"Операция {sorted(valid_ops)}: ").strip()
        if op in valid_ops:
            return op
        print("Ошибка: недопустимая операция. Попробуйте снова.")
 
 
def main():
    print("=== Калькулятор ===")
    print("Для выхода нажмите Ctrl+C или введите 'q' на вводе числа.\n")
 
    while True:
        try:
            a = get_number("Первое число: ")
            op = get_operator()
            b = get_number("Второе число: ")
 
            result = calculate(a, op, b)
            print(f"Результат: {a} {op} {b} = {result}\n")
 
        except ZeroDivisionError as e:
            print(f"Ошибка: {e}\n")
        except ValueError as e:
            print(f"Ошибка: {e}\n")
        except KeyboardInterrupt:
            print("\nВыход из калькулятора. До встречи!")
            break
 
        again = input("Ещё одно вычисление? (да/нет): ").strip().lower()
        if again not in ("да", "yes", "y", "д"):
            print("До встречи!")
            break
 
 
if __name__ == "__main__":
    main()
