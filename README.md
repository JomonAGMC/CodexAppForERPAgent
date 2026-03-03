# Lightweight .NET Data Manipulation App

This repository now includes a lightweight .NET console application designed for **large-scale CSV data manipulation** using a streaming approach (line-by-line processing, low memory usage).

## Project

- Path: `src/LightweightDataManipulator`
- Framework: `.NET 8`

## Features

- Stream reads large CSV files (does not load whole file into memory)
- Aggregates numeric values by group key
- Async file I/O for throughput
- Minimal dependencies for lightweight runtime

## Usage

```bash
dotnet run --project src/LightweightDataManipulator -- \
  --input data/input.csv \
  --output data/output.csv \
  --group-by Region \
  --sum Revenue
```

## Output format

The output CSV contains:

- group-by column
- aggregated total of the requested numeric column

Example:

```csv
Region,RevenueTotal
East,1234.56
West,9876.54
```
