﻿using System;
using System.Collections.Generic;
using System.Text;

namespace inventory_management.ViewModel.Fields
{
    public abstract class GameField : ViewModelBase
    {
        private String _color;
        public String Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Oszlop lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Sor lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 Y { get; set; }

    }
}
