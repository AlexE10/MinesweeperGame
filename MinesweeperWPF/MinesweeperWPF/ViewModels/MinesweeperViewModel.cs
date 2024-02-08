using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperWPF.ViewModels
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using MinesweeperWPF.Models;

    public class MinesweeperViewModel : INotifyPropertyChanged
    {
        private GameState _currentGameState;

        public GameState CurrentGameState
        {
            get => _currentGameState;
            set
            {
                _currentGameState = value;
                OnPropertyChanged(nameof(CurrentGameState));
            }
        }

        public ICommand CellClickCommand { get; private set; }

        public int BoardRows => CurrentGameState?.Board?.Cells?.Count ?? 0;

        public int BoardColumns => CurrentGameState?.Board?.Cells?.FirstOrDefault()?.Count ?? 0;


        public IEnumerable<CellViewModel> FlattenedCells
        {
            get
            {
                return CurrentGameState?.Board?.Cells
                    .SelectMany((row, rowIndex) => row.Select((cell, columnIndex) =>
                        new CellViewModel(cell, new CellPosition(rowIndex, columnIndex))))
                    .ToList();
            }
        }

        public ICommand ResetGameCommand { get; private set; }

        private int _selectedDifficulty;

        public int SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                if (_selectedDifficulty != value)
                {
                    _selectedDifficulty = value;
                    OnPropertyChanged(nameof(SelectedDifficulty));
                    InitializeGame();
                }
            }
        }

        public ICommand FlagCellCommand { get; private set; }

        private int _remainingFlags;
    //    private readonly Dictionary<int, int> _difficultyToFlagCount = new Dictionary<int, int>
    //{
    //    { 0, 10 },  // Easy
    //    { 1, 40 },  // Medium
    //    { 2, 99 }   // Hard
    //};

        public int RemainingFlags
        {
            get => _remainingFlags;
            set
            {
                _remainingFlags = value;
                OnPropertyChanged(nameof(RemainingFlags));
            }
        }

        public MinesweeperViewModel()
        {
            SelectedDifficulty = 1; 
            InitializeGame();
            CellClickCommand = new RelayCommand(CellClick);
            ResetGameCommand = new RelayCommand(ResetGame);
            FlagCellCommand = new RelayCommand(FlagCell);
        }
        private void FlagCell(object parameter)
        {
            if (parameter is CellPosition position)
            {
                var oldFlaggedState = CurrentGameState.Board.Cells[position.X][position.Y].IsFlagged;
                CurrentGameState = GameLogic.ToggleFlagOnCell(CurrentGameState, position.X, position.Y);
                
                RemainingFlags = CurrentGameState.AvailableFlags;

                OnPropertyChanged(nameof(CurrentGameState));
                OnPropertyChanged(nameof(FlattenedCells));
                OnPropertyChanged(nameof(RemainingFlags));
            }
        }
        private void ResetGame(object parameter)
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            
            CurrentGameState = GameLogic.CreateInitialState(SelectedDifficulty);
            RemainingFlags = CurrentGameState.AvailableFlags;
            OnPropertyChanged(nameof(FlattenedCells));
            OnPropertyChanged(nameof(BoardRows));
            OnPropertyChanged(nameof(BoardColumns));
        }

        private void CellClick(object parameter)
        {
            if (parameter is CellPosition position)
            {
                CurrentGameState = GameLogic.RevealCell(CurrentGameState, position.X, position.Y);
                RemainingFlags = CurrentGameState.AvailableFlags;
                OnPropertyChanged(nameof(RemainingFlags));

                if (CurrentGameState.GameOver || CurrentGameState.GameWon)
                {
                    RemainingFlags = 0;
                    CurrentGameState = GameLogic.RevealAllBombs(CurrentGameState);
                    OnPropertyChanged(nameof(CurrentGameState));
                    OnPropertyChanged(nameof(FlattenedCells));
                    OnPropertyChanged(nameof(RemainingFlags));
                    string message = CurrentGameState.GameWon ? "You won!" : "Game over!";
                    MessageBox.Show(message, "Game Over");
                }
                else
                {

                    OnPropertyChanged(nameof(CurrentGameState));
                    OnPropertyChanged(nameof(FlattenedCells));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CellPosition
    {
        public int X { get; }
        public int Y { get; }

        public CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

}
