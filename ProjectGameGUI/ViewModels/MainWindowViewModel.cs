using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ProjectGameGUI.Models;
using ReactiveUI;

namespace ProjectGameGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<Piece> _pieces;
        private ObservableCollection<Player> _players;
        private ObservableCollection<Field> _fields;
        private GameModel model;
        private int _windowWidth;
        private int _windowHeight;
        private int _boardWidth;
        private int _boardHeight;

        public ObservableCollection<Piece> Pieces
        {
            get => _pieces;
            set
            {
                _pieces = value;
                this.RaisePropertyChanged(nameof(Pieces));
            }
        }

        public ObservableCollection<Player> Players { get => _players; set { _players = value; this.RaisePropertyChanged(nameof(Players)); } }
        public ObservableCollection<Field> Fields { get => _fields; set { _fields = value; this.RaisePropertyChanged(nameof(Fields)); } }

        public int WindowWidth { get => _windowWidth; set { _windowWidth = value; this.RaisePropertyChanged(nameof(WindowWidth)); } }
        public int WindowHeight { get => _windowHeight; set { _windowHeight = value; this.RaisePropertyChanged(nameof(WindowHeight)); } }

        public int BoardWidth { get => _boardWidth; set { _boardWidth = value; this.RaisePropertyChanged(nameof(BoardWidth)); } }
        public int BoardHeight { get => _boardHeight; set { _boardHeight = value; this.RaisePropertyChanged(nameof(BoardHeight)); } }

        public MainWindowViewModel()
        {
            Players = new ObservableCollection<Player>();
            Fields = new ObservableCollection<Field>();
            Pieces = new ObservableCollection<Piece>();
            model = new GameModel(this);
            WindowHeight = 100;
            WindowWidth = 100;
        }

        public void InitializeGame()
        {
            model.InitializeGame();

        }
    }
}
