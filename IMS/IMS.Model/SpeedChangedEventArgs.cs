﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model
{
    public class SpeedChangedEventArgs:EventArgs
    {
        /// <summary>
        /// Ido lekérdezése.
        /// </summary>
        public Int32 Speed { get; private set; }

        /// <summary>
        /// Ido eltelesenek eseményargumentuma
        /// </summary>
        /// <param name="player"></param>
        public SpeedChangedEventArgs(Int32 speed) { Speed = speed; }
    }
}
