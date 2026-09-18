def add(a, b):
    return a + b


def subtract(a, b):
    return a - b


def multiply(a, b):
    return a * b


def divide(a, b):
    if b == 0:
        return "Ошибка: деление на ноль"
    return a / b


print("Простой калькулятор")
print("Доступные операции: +, -, *, /")

while True:
    try:
        num1 = float(input("Введите первое число: "))
        op = input("Введите операцию (+, -, *, /): ")
        num2 = float(input("Введите второе число: "))

        if op == '+':
            result = add(num1, num2)
        elif op == '-':
            result = subtract(num1, num2)
        elif op == '*':
            result = multiply(num1, num2)
        elif op == '/':
            result = divide(num1, num2)
        else:
            print("Некорректная операция")
            continue

        print(f"Результат: {result}")

    except ValueError:
        print("Ошибка: введите число")

    again = input("Хотите продолжить? (y/n): ")
    if again.lower() != 'y':
        print("До свидания!")
        break
