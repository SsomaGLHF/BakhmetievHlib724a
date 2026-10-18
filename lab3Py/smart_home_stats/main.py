"""
Лабораторна робота №3. Робота з файлами користувацьких даних.
Статистична обробка показів «розумного будинку»:
читання data.csv -> читання налаштувань (INI/TOML) -> обчислення -> results.json
"""
import argparse
import configparser
import csv
import json
import sys
import tomllib
from typing import Dict, List

import stats_utils as su

# Порогові значення для виявлення різких перепадів
THRESHOLDS: Dict[str, float] = {
    "temperature": 7,      # °C
    "pressure": 5000,      # Па
    "humidity": 20,        # %
}

PARAMETERS = ("temperature", "humidity", "pressure")
ALLOWED_STATS = ("average", "min", "max", "median", "jumps")


class DataFormatError(Exception):
    """Помилка формату вхідних даних або налаштувань."""


def read_csv(filename: str) -> Dict[str, Dict[str, float]]:
    """Зчитує дані з CSV і повертає словник {показник: {мітка: значення}}."""
    data: Dict[str, Dict[str, float]] = {p: {} for p in PARAMETERS}
    with open(filename, "r", encoding="utf-8", newline="") as file:
        reader = csv.DictReader(file)
        # Перевірка наявності потрібних стовпців
        fields = reader.fieldnames or []
        missing = [c for c in ("timestamp", *PARAMETERS) if c not in fields]
        if missing:
            raise DataFormatError(
                f"У файлі '{filename}' відсутні стовпці: {', '.join(missing)}")
        for line_no, row in enumerate(reader, start=2):
            try:
                timestamp = row["timestamp"].strip()
                if not timestamp:
                    raise ValueError("порожня часова мітка")
                for p in PARAMETERS:
                    data[p][timestamp] = float(row[p])
            except (ValueError, TypeError, AttributeError) as err:
                raise DataFormatError(
                    f"Некоректний рядок {line_no} у файлі '{filename}': {err}"
                ) from err
    if not data["temperature"]:
        raise DataFormatError(f"Файл '{filename}' не містить вимірювань")
    return data


def _split_stats(raw) -> List[str]:
    """Перетворює значення stats (рядок або список) на список назв."""
    if isinstance(raw, str):
        raw = raw.split(",")
    return [s.strip().lower() for s in raw if str(s).strip()]


def read_config(filename: str) -> Dict[str, List[str]]:
    """
    Зчитує файл налаштувань (INI або TOML — за розширенням) і повертає
    словник {показник: [список характеристик]}.
    """
    config: Dict[str, List[str]] = {}
    if filename.lower().endswith(".toml"):
        with open(filename, "rb") as file:
            raw = tomllib.load(file)
        for section, values in raw.items():
            config[section] = _split_stats(values.get("stats", []))
    else:
        parser = configparser.ConfigParser()
        if not parser.read(filename, encoding="utf-8"):
            raise FileNotFoundError(filename)
        for section in parser.sections():
            config[section] = _split_stats(parser.get(section, "stats", fallback=""))

    # Перевірка коректності налаштувань
    for section, stats in config.items():
        if section not in PARAMETERS:
            print(f"Увага: невідомий показник '{section}' у налаштуваннях — пропущено")
        for s in stats:
            if s not in ALLOWED_STATS:
                raise DataFormatError(
                    f"Невідома характеристика '{s}' для показника '{section}'")
    return config


def calculate(parameter: str, series: Dict[str, float],
              stats: List[str], thresholds: Dict[str, float]) -> dict:
    """Обчислює вказані характеристики одного показника."""
    timestamps = list(series.keys())
    values = list(series.values())
    functions = {
        "average": lambda: su.get_average(values),
        "min": lambda: su.get_min(values),
        "max": lambda: su.get_max(values),
        "median": lambda: su.get_median(values),
        "jumps": lambda: su.detect_jumps(values, timestamps, thresholds[parameter]),
    }
    return {name: functions[name]() for name in stats}


def parse_args() -> argparse.Namespace:
    """Розбір параметрів командного рядка."""
    p = argparse.ArgumentParser(
        description="Статистична обробка показів «розумного будинку»",
        add_help=False)  # -h використовується для порога вологості
    p.add_argument("--help", action="help", help="показати цю довідку")
    p.add_argument("-d", "--data", default="data.csv", help="файл з даними (CSV)")
    p.add_argument("-c", "--config", default="config.ini",
                   help="файл налаштувань (.ini або .toml)")
    p.add_argument("-o", "--output", default="results.json", help="файл результатів")
    p.add_argument("-t", type=float, metavar="TEMP",
                   help="поріг перепаду температури, °C (за замовчуванням 7)")
    p.add_argument("-h", type=float, metavar="HUM", dest="hum",
                   help="поріг перепаду вологості, %% (за замовчуванням 20)")
    p.add_argument("-p", type=float, metavar="PRESS",
                   help="поріг перепаду тиску, Па (за замовчуванням 5000)")
    return p.parse_args()


def main() -> int:
    # Щоб коректно виводилась кирилиця в консолі Windows
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    args = parse_args()
    thresholds = dict(THRESHOLDS)
    if args.t is not None:
        thresholds["temperature"] = args.t
    if args.hum is not None:
        thresholds["humidity"] = args.hum
    if args.p is not None:
        thresholds["pressure"] = args.p

    try:
        # 1. Зчитування даних
        data = read_csv(args.data)
        # 2. Зчитування налаштувань
        config = read_config(args.config)
    except FileNotFoundError as err:
        print(f"Помилка: файл не знайдено: {err.filename or err}")
        return 1
    except (DataFormatError, tomllib.TOMLDecodeError,
            configparser.Error, PermissionError) as err:
        print(f"Помилка: {err}")
        return 1

    # 3. Обробка кожного показника, вказаного у налаштуваннях
    results: Dict[str, dict] = {}
    for parameter in PARAMETERS:
        if parameter not in config:
            print(f"\nПоказник '{parameter}' відсутній у налаштуваннях — пропущено")
            continue
        su.print_table(parameter, data[parameter])
        results[parameter] = calculate(parameter, data[parameter],
                                       config[parameter], thresholds)
        su.print_results(parameter, results[parameter])

    # 4. Збереження результатів у JSON
    try:
        with open(args.output, "w", encoding="utf-8") as file:
            json.dump(results, file, ensure_ascii=False, indent=2)
    except OSError as err:
        print(f"Помилка запису файлу '{args.output}': {err}")
        return 1
    print(f"\nРезультати збережено у файлі {args.output}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
