﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Data.Entities
{
    public class SettingEntity : BaseEntity
    {
        public bool MaintenenceMode { get; set; }
    }
}
