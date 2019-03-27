using Avalonia.Controls.Shapes;
using GameLib;
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
        private ObservableCollection<Models.Piece> _pieces;
        private ObservableCollection<Player> _players;
        private ObservableCollection<Field> _fields;
        private GameModel model;

        public ObservableCollection<Models.Piece> Pieces
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

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }

        public MainWindowViewModel()
        {
            Players = new ObservableCollection<Player>();
            Fields = new ObservableCollection<Field>();
            Pieces = new ObservableCollection<Models.Piece>();
            model = new GameModel(this);
        }
    }
}
