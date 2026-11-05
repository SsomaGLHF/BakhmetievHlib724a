"""
Допоміжний модуль stats_utils.py
Функції для статистичної обробки показів «розумного будинку»
та виводу результатів у консоль.
"""
import statistics
from typing import Dict, Optional, Sequence

# Тип даних одного показника: {часова мітка: значення}
Series = Dict[str, float]


def get_average(values: Sequence[float]) -> Optional[float]:
    """Повертає середнє значення списку (округлене до 2 знаків) або None."""
    return round(statistics.mean(values), 2) if values else None


def get_min(values: Sequence[float]) -> Optional[float]:
    """Повертає мінімальне значення списку або None, якщо список порожній."""
    return min(values) if values else None


def get_max(values: Sequence[float]) -> Optional[float]:
    """Повертає максимальне значення списку або None, якщо список порожній."""
    return max(values) if values else None


def get_median(values: Sequence[float]) -> Optional[float]:
    """Повертає медіану списку (округлену до 2 знаків) або None."""
    return round(statistics.median(values), 2) if values else None


def detect_jumps(values: Sequence[float],
                 timestamps: Sequence[str],
                 threshold: float) -> list[str]:
    """
    Виявляє різкі перепади між сусідніми значеннями.

    :param values: список значень показника
    :param timestamps: список часових міток (така ж довжина, як і values)
    :param threshold: порогове значення (перепад строго більший за нього)
    :return: список часових міток, у яких відбувся перепад
    """
    jumps: list[str] = []
    for i in range(1, len(values)):
        if abs(values[i] - values[i - 1]) > threshold:
            jumps.append(timestamps[i])
    return jumps


def print_table(parameter_name: str, data_dict: Series,
                markdown: bool = False) -> None:
    """
    Виводить у консоль таблицю з показниками певного параметра.

    :param parameter_name: назва показника (заголовок другого стовпця)
    :param data_dict: словник {часова мітка: значення}
    :param markdown: True — вивід у розмітці markdown, False — псевдографіка
    """
    print(f"\nПоказники для параметра: {parameter_name}")
    w1 = max(len("Timestamp"), *(len(t) for t in data_dict)) if data_dict else 9
    w2 = max(len(parameter_name), *(len(str(v)) for v in data_dict.values())) \
        if data_dict else len(parameter_name)

    if markdown:
        print(f"| {'Timestamp':<{w1}} | {parameter_name:^{w2}} |")
        print(f"| {'-' * w1} | :{'-' * (w2 - 2)}: |")
        for timestamp, value in data_dict.items():
            print(f"| {timestamp:<{w1}} | {value:^{w2}} |")
        return

    line = f"+-{'-' * w1}-+-{'-' * w2}-+"
    print(line)
    print(f"| {'Timestamp':<{w1}} | {parameter_name:<{w2}} |")
    print(line)
    for timestamp, value in data_dict.items():
        print(f"| {timestamp:<{w1}} | {value:<{w2}} |")
    print(line)


def print_results(parameter_name: str, results: dict) -> None:
    """
    Виводить у консоль таблицю з розрахованими характеристиками показника.

    :param parameter_name: назва показника
    :param results: словник {назва характеристики: значення}
    """
    print(f"\nРезультати обчислень для параметра: {parameter_name}")
    w1 = max([len("Характеристика")] + [len(k) for k in results])
    w2 = max([len("Значення")] + [len(str(v)) for v in results.values()])
    line = f"+-{'-' * w1}-+-{'-' * w2}-+"
    print(line)
    print(f"| {'Характеристика':<{w1}} | {'Значення':<{w2}} |")
    print(line)
    for name, value in results.items():
        print(f"| {name:<{w1}} | {str(value):<{w2}} |")
    print(line)
