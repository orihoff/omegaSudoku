# HoffSudoku Solver

Hey, welcome to HoffSudoku, a generic, high-performance Sudoku solver built in C#. Whether you’re intrest in classic 9×9 puzzles or pushing your limits with up to 25×25 boards, this project is engineered to deliver fast and efficient results.

## Sudoku Rules

At its core, Sudoku is a logic puzzle played on an N×N grid divided into subgrids (boxes) of size √N×√N. The mission? Fill every row, column, and box with numbers 1 through N without repeats. I decided that empty cells are represented by the digit 0.

## About the Project

HoffSudoku is built as a generic board game solver, currently focused on Sudoku through Console UI. Its modular architecture, object-oriented design, and strict commitment to SOLID principles make it incredibly easy to extend—whether you're looking to add new puzzles or explore advanced solving techniques.

## How It Solves Sudoku

### Bitwise Backtracking

The heart of the solver is a bitwise backtracking algorithm. Each cell’s possible values are stored as bits in an integer, enabling very fast checks and updates. This low-level bit manipulation improve performance, especially when exploring complex board configurations.

### Human Solving Tactics

For easier puzzles, HoffSudoku employs classic human strategies:

- **Naked Singles:** When a cell has only one possible candidate, it gets filled immediately.
- **Hidden Singles:** If a candidate appears just once in a row, column, or box, it’s placed in that cell right away.
  (I should note that Hidden Single is only called for boards larger than 9 x 9)
  

These tactics are applied repeatedly in each iteration to simplify the puzzle before attempting backtracking.

## Optimizations

- **Bitwise Masking:** Uses integer arrays to represent the state of rows, columns, and boxes—allowing rapid determination of available numbers.
- **Degree Heuristic:** Selects the next cell by choosing the one with the fewest candidates and the highest impact on its neighbors, effectively reducing the search space.
- **ValidateHallCondition:** Added to quickly detect unsolvable board configurations using Hall's condition.

## User Interface

The Console UI is straightforward and user-friendly:

- **Manual Input:** Enter puzzles directly via the console.
- **File I/O:** Load and save puzzles to text files.
- **Robust Error Handling:** Clear, detailed messages guide you through any input errors.

## Getting Started

- **Clone the Repository:** Use your favorite Git client to clone the project.
- **Open the Solution:** Open Visual Studio 2019 or 2022.
- **Build & Run:** Compile and run the project directly in Visual Studio.

## Testing

A comprehensive suite of unit tests (using MSTest) covers both board validation and the solving algorithms for puzzles of various difficulty levels.

- Open Visual Studio’s Test Explorer.
- Run all tests to ensure everything is functioning perfectly.

## Future Enhancements

On the horizon:

- Integrate additional human solving techniques.
- Expand support to other board game puzzles.
- Develop a GUI for a more interactive, user-friendly experience.

Happy Sudoku solving!
