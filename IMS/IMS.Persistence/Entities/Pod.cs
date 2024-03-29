﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Persistence.Entities
{
    public class Pod : Entity
    {
        
        public Dictionary<Int32,Int32> Products{ get;private set; }
        public Boolean IsTake { get; set; }

        public Pod(int x, int y, Dictionary<Int32, Int32> products) : base(x, y)
        {
            _type = EntityType.Pod;
            Products = products;
            Pos = new Pos(x, y);
            IsTake = false;
        }

        public Pod(int x, int y) : this(x,y,new Dictionary<int, int>())
        {
        }
    }
}
