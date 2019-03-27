using Avalonia;
using Avalonia.Controls;
using System.Collections.Generic;

namespace ProjectGameGUI.Controls
{
    public class ExpandedGrid : Grid
    {
        private int _rowsCount;
        private int _columnsCount;

        public static readonly AvaloniaProperty<int> RowsCountDeclaration =
            AvaloniaProperty.RegisterDirect<ExpandedGrid, int>(nameof(RowsCount), eg => eg.RowsCount, (eg, value) => eg.RowsCount = value);
        public static readonly AvaloniaProperty<int> ColumnsCountDeclaration = 
            AvaloniaProperty.RegisterDirect<ExpandedGrid, int>(nameof(ColumnsCount), eg => eg.ColumnsCount, (eg, value) => eg.ColumnsCount = value);
        public static readonly AvaloniaProperty<IEnumerable<IControl>> ItemsDeclaration =
            AvaloniaProperty.RegisterDirect<ExpandedGrid, IEnumerable<IControl>>(nameof(Items), eg => eg.Items, (eg, value) => eg.Items = value);
        private int rowsCount
        {
            get => _rowsCount;
            set
            {
                _rowsCount = value;
                RowDefinitions.Clear();
                for (int i = 0; i < _rowsCount; ++i)
                {
                    RowDefinitions.Add(new RowDefinition());
                }
            }
        }
        private int columnsCount
        {
            get => _columnsCount;
            set
            {
                _columnsCount = value;
                ColumnDefinitions.Clear();
                for (int i = 0; i < _columnsCount; ++i)
                {
                    ColumnDefinitions.Add(new ColumnDefinition());
                }
            }
        }

        public int RowsCount
        {
            get { return rowsCount; }
            set { rowsCount = value; }
        }

        public int ColumnsCount
        {
            get { return columnsCount; }
            set { columnsCount = value; }
        }

        public IEnumerable<IControl> Items
        {
            get { return Children; }
            set { Children.Clear(); if(value!=null) Children.AddRange(value); }
        }

        public ExpandedGrid() : base()
        {
        }
    }
}