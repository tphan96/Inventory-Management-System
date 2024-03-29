﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Persistence.Entities
{
    public class Robot : Entity
    {
        private Int32 _energyConsumption;
        private Int32 _capacity; //max
        //private Int32 EnergyLeft; //current
        private Int32 _destinationID;
        private Pos _destination;
        private Boolean _underPod;

        public Int32 Potato { get; private set; }
        public Int32 EnergyConsumption { get { return _energyConsumption; } }
        public Int32 Capacity { get { return _capacity; } }
        public int EnergyLeft { get; set; }
        public Int32 DestinationID { get { return _destinationID; } set { } }

        public Boolean UnderPod { get { return _underPod; } set { } }

        public Pos Destination { get { return _destination; } set { } }
        //move robot to given position and change fuel
        public void Move(Pos other)
        {

            //  if (EnergyLeft <= Pos.Distance(other))
            //   return;
            //EnergyLeft += Pos.Distance(other);
            EnergyLeft--;
            //_energyConsumption += Pos.Distance(other);
            _energyConsumption++;
            Pos.X = other.X;
            Pos.Y = other.Y;
        }

        //move robot to given position and change fuel
        public void Rotate(Direction other)
        {
            if (this._dir != other)
            {
                EnergyLeft--;
                _energyConsumption++;
            }
            _dir = other;
        }
        //finds closest charger and returns true it can get to destination false otherwise  
        public Boolean EnoughCharge(IMSData IMSData)
        {
            Pos closestDockPos = new Pos();
            int shortestDistance = int.MaxValue;
            foreach (Dock dock in IMSData.EntityData.DockData) // iterate over all docks which is closer
            {
                if (shortestDistance > this.Pos.Distance(dock.Pos))
                {
                    shortestDistance = this.Pos.Distance(dock.Pos);
                    closestDockPos = dock.Pos;
                }
            }
            return this.EnergyLeft > shortestDistance;
        }

        public void Charge()
        {
            EnergyLeft = Capacity;
        }
        //public Int32 EnergyConsumption { get; }
        //public Int32 Capacity { get;}
        //public Int32 EnergyLeft { get; }
        //public Int32 DestinationID { get; }

        public Robot(int x, int y, Direction direction, int capacity, int energyLeft, int destinationID) : this(x, y, direction, capacity, energyLeft, destinationID, 0)
        {
        }

        public Robot(int x, int y, Direction direction, int capacity, int energyLeft, int destinationID, int energyConsumption) : base(x, y)
        {
            _type = EntityType.Robot;
            _capacity = capacity;
            _energyConsumption = energyConsumption;
            _dir = direction;
            EnergyLeft = energyLeft;
            _destinationID = destinationID;
            Random rnd = new Random();
            Potato = rnd.Next(100);
        }

        //public override bool Equals(object obj)
        //{
        //    var item = obj as Robot;
        //    if (item == null)
        //    {
        //        return false;
        //    }
        //    return Equals(obj as Robot);
        //}

        //public bool Equals(Robot other)
        //{
        //    return other != null && Potato == other.Potato;
        //           //Pos.X == other.Pos.X &&
        //           //Pos.Y == other.Pos.Y;
        //}

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(Pos.X, Pos.Y,Potato);
        //}

        //public static bool operator ==(Robot A, Robot B)
        //{
        //    if (A is null)
        //    {
        //        if (B is null)
        //        {
        //            // null == null = true.
        //            return true;
        //        }

        //        // Only the left side is null.
        //        return false;
        //    }
        //    // Equals handles the case of null on right side.
        //    return A.Equals(B);
        //}

        //public static bool operator !=(Robot A, Robot B) => !(A == B);

        public Robot(int x, int y) : this(x,y,Direction.UP,1,1,-1,0)
        {

        }

        public Robot(int x, int y, int capacity) : this(x,y,Direction.UP,capacity,capacity,-1)
        {

        }

        public Robot(int x, int y, int capacity, int id) : this(x, y, Direction.UP, capacity, capacity, id)
        {

        }

        public Robot(int x, int y, Direction direction, int capacity) : this(x, y, direction, capacity, capacity, -1)
        {

        }

        /*
        public Robot(int x, int y, Direction direction, int capacity, int energyLeft, int energyConsumption) : this(x, y, direction, capacity, energyLeft, -1, energyConsumption)
        {

        }
        */

        public Robot(Robot robot, Direction direction) : this(robot.Pos.X, robot.Pos.Y, direction, robot.Capacity, robot.EnergyLeft, robot.DestinationID, robot.EnergyConsumption)
        {

        }

    }
}
