using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
using IMS.Model;
using IMS.Persistence;
using System;
using System.Diagnostics;
using IMS.Persistence.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMS.Model.Simulation;
using System.Linq;

namespace IMS.Test
{
    [TestClass]
    public class UnitTest1
    {
        #region Fields
        private IMSModel _model;
        private IMSData _table;
        private IMSDataAccess _dataAccess;
        #endregion

        #region Initialization
        [TestInitialize]
        public void Initialize()
        {
            
            _dataAccess = new IMSDataAccess();
            _model = new IMSModel(_dataAccess);

        }
        #endregion
        [TestMethod]
        public void CreateEmptyTable()
        {
            _model.NewSimulation();
            Assert.AreEqual(12, _model.SizeX);
            Assert.AreEqual(12, _model.SizeY);
            for (Int32 i = 0; i < _model.SizeX; ++i)
            {
                for (Int32 j = 0; j < _model.SizeY; ++j)
                {
                    Assert.AreEqual(EntityType.Empty, _model[i, j].Type);
                }
            }
        }

        [TestMethod]
        public void StartStepsTest()
        {
            _model.NewSimulation();

            Assert.AreEqual(_model.Steps, 0);
        }

        [TestMethod]
        public void StartEnergyTest()
        {
            _model.NewSimulation();

            Assert.AreEqual(_model.AllEnergy, 0);
        }

        [TestMethod]
        public void StartSpeedTest()
        {
            _model.NewSimulation();

            Assert.AreEqual(1, _model.Speed);
        }

        [TestMethod]
        public void TimeTest()
        {
            _model.NewSimulation();
            Assert.AreEqual(_model.Time, 0);

            _model.AdvanceTime();
            Assert.AreEqual(_model.Time, 1);
        }

        [TestMethod]
        public void SimulationTest()
        {
            _model.Simulation();
            //TODO
        }

        //[TestMethod]
        //public async Task GameModelLoadTest()
        //{
        //    // kezd�nk egy �j j�t�kot
        //    _model.NewSimulation();

        //    // majd bet�lt�nk egy j�t�kot
        //    await _model.LoadSimulationAsync(String.Empty);

        //    // ellen�rizz�k, hogy megh�vt�k-e a Load m�veletet a megadott param�terrel
        //    //_mock.Verify(dataAccess => dataAccess.LoadSimulationAsync(String.Empty), Times.Once());
        //}


        [TestMethod]
        public void SetSpeedTestSpeedUp()
        {
            int startSpeed = _model.Speed;
            _model.setSpeed(1);
            Assert.AreEqual(startSpeed + 1, _model.Speed);
        }
        

        [TestMethod]
        public void SetSpeedTestSpeedDown()
        {
            int startSpeed = _model.Speed;

            _model.setSpeed(-1);
            Assert.AreEqual(startSpeed - 1, _model.Speed);
        }
        #region Simulation
        public Dictionary<Robot, List<Pos>> CreatTestTable()
        {
            _table = new IMSData(12, 12);
            Robot Robot1 = new Robot(0, 0, Direction.UP, 1000, 1000, 1);
            Robot Robot2 = new Robot(0, 10, Direction.UP, 1000, 1000, 1);
            _table.EntityData.RobotData.Add(Robot1);
            _table.EntityData.RobotData.Add(Robot2);
            Dictionary<Int32, Int32> asd = new Dictionary<Int32, Int32>() { { 2, 1 } };
            Dictionary<Int32, Int32> asd2 = new Dictionary<Int32, Int32>() { { 1, 1 } };
            Pod pod1 = new Pod(0, 9, asd);
            Pod pod2 = new Pod(0, 1, asd2);
            _table.EntityData.PodData.Add(pod1);
            _table.EntityData.PodData.Add(pod2);
            Destination dest1 = new Destination(11, 0, 1);
            Destination dest2 = new Destination(3, 11, 2);
            _table.EntityData.DestinationData.Add(dest1);
            _table.EntityData.DestinationData.Add(dest2);
            Dock dock1 = new Dock(4, 3);
            Dock dock2 = new Dock(6, 10);
            _table.EntityData.DockData.Add(dock1);
            _table.EntityData.DockData.Add(dock2);
            ConflictBasedSearch cbs = new ConflictBasedSearch(_table);
            return cbs.CheckConflicts();
        }
        [TestMethod]
        public void Robot1Pos1Test()
        {
            Dictionary<Robot, List<Pos>> routes = CreatTestTable();
            Assert.AreEqual(routes[_table.EntityData.RobotData[0]].Last().X, 0);
            Assert.AreEqual(routes[_table.EntityData.RobotData[0]].Last().Y, 9);
        }
        [TestMethod]
        public void Robot2Pos2Test()
        {
            Dictionary<Robot, List<Pos>> routes = CreatTestTable();
            Assert.AreEqual(routes[_table.EntityData.RobotData[1]].Last().X, 0);
            Assert.AreEqual(routes[_table.EntityData.RobotData[1]].Last().Y, 1);
        }
        [TestMethod]
        public void Robot1RouteTest()
        {
            Dictionary<Robot, List<Pos>> routes = CreatTestTable();
            Assert.AreEqual(routes[_table.EntityData.RobotData[0]].Count(), 23);
        }
        [TestMethod]
        public void Robot2RouteTest()
        {
            Dictionary<Robot, List<Pos>> routes = CreatTestTable();
            Assert.AreEqual(routes[_table.EntityData.RobotData[1]].Count(), 35);
        }
        #endregion





        [TestMethod]
        public void CreatorSizeTest()
        {
            int x = 10;
            int y = 12;
            _model.GenerateEmtyTableForSettingsWindow(x, y);
            Assert.AreEqual(_model.TempSizeX, x);
            Assert.AreEqual(_model.TempSizeY, y);
        }

        [TestMethod]
        public void EntityPlacementTest()
        {
            int x = 10;
            int y = 10;
            int capacity = 10;
            int id = 1;
            _model.GenerateEmtyTableForSettingsWindow(x, y);

            _model.ChangeField(0, 0, EntityType.Dock);
            Assert.AreEqual(_model.GetTemp(0, 0).Type, EntityType.Dock);

            _model.ChangeField(0, 0, EntityType.Robot, capacity, id);
            Assert.AreEqual(_model.GetTemp(0, 0).Type, EntityType.Robot);
            Assert.AreEqual(((Robot)_model.GetTemp(0, 0)).Capacity, capacity);
            Assert.AreEqual(_model.GetTemp(0, 0).Direction, Direction.UP);
            _model.RotateRobot(0, 0);
            Assert.AreEqual(_model.GetTemp(0, 0).Direction, Direction.RIGHT);
            _model.RotateRobot(0, 0);
            Assert.AreEqual(_model.GetTemp(0, 0).Direction, Direction.DOWN);
            _model.RotateRobot(0, 0);
            Assert.AreEqual(_model.GetTemp(0, 0).Direction, Direction.LEFT);
            _model.RotateRobot(0, 0);
            Assert.AreEqual(_model.GetTemp(0, 0).Direction, Direction.UP);

            _model.ChangeField(0, 0, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(0, 0).Type, EntityType.Pod);

            _model.ChangeField(0, 0, EntityType.Destination);
            Assert.AreEqual(_model.GetTemp(0, 0).Type, EntityType.Destination);

            _model.GenerateEmtyTableForSettingsWindow(_model.TempSizeX, _model.TempSizeY);
            Assert.AreEqual(_model.GetTemp(0, 0).Type, EntityType.Empty);
        }

        [TestMethod]
        public void ProductAssignmentTest()
        {
            int x = 10;
            int y = 10;
            _model.GenerateEmtyTableForSettingsWindow(x, y);
            _model.ChangeField(2, 2, EntityType.Pod);
            _model.ChangeField(3, 2, EntityType.Pod);
            _model.ChangeField(2, 3, EntityType.Destination);
            _model.AddProduct(2, 2, 3, 3, 5);
            Assert.AreEqual(((Pod)_model.GetTemp(2, 2)).Products.ContainsKey(5), true);
            Assert.AreEqual(((Pod)_model.GetTemp(3, 2)).Products.ContainsKey(5), true);
            Assert.AreEqual(((Destination)_model.GetTemp(2, 3)).ID, 5);
            _model.AddProduct(2, 2, 3, 3, 6);
            Assert.AreEqual(((Pod)_model.GetTemp(2, 2)).Products.ContainsKey(5), true);
            Assert.AreEqual(((Pod)_model.GetTemp(3, 2)).Products.ContainsKey(5), true);
            Assert.AreEqual(((Pod)_model.GetTemp(2, 2)).Products.ContainsKey(6), true);
            Assert.AreEqual(((Pod)_model.GetTemp(3, 2)).Products.ContainsKey(6), true);
            Assert.AreEqual(((Destination)_model.GetTemp(2, 3)).ID, 6);
        }

        [TestMethod]
        public void BasicRelocationTest()
        {
            int x = 10;
            int y = 10;
            _model.GenerateEmtyTableForSettingsWindow(x, y);

            _model.ChangeField(1, 1, EntityType.Pod);
            _model.ChangeField(1, 2, EntityType.Pod);
            _model.ChangeField(2, 1, EntityType.Pod);
            _model.ChangeField(2, 2, EntityType.Pod);
            _model.AddProduct(1, 1, 2, 2, 22);

            _model.RelocationAttempt(1, 1, 2, 2, 3, 3);

            Assert.AreEqual(_model.GetTemp(1, 1).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(1, 2).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(2, 1).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(2, 2).Type, EntityType.Empty);

            Assert.AreEqual(_model.GetTemp(3, 3).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(3, 4).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(4, 3).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(4, 4).Type, EntityType.Pod);

            Assert.AreEqual(((Pod)_model.GetTemp(3, 3)).Products.ContainsKey(22), true);
            Assert.AreEqual(((Pod)_model.GetTemp(3, 4)).Products.ContainsKey(22), true);
            Assert.AreEqual(((Pod)_model.GetTemp(4, 3)).Products.ContainsKey(22), true);
            Assert.AreEqual(((Pod)_model.GetTemp(4, 4)).Products.ContainsKey(22), true);
        }

        [TestMethod]
        public void RelocationSelectionTest()
        {
            int x = 10;
            int y = 10;
            _model.GenerateEmtyTableForSettingsWindow(x, y);

            _model.ChangeField(0, 0, EntityType.Pod);
            _model.ChangeField(0, 1, EntityType.Pod);
            _model.ChangeField(1, 0, EntityType.Pod);
            _model.ChangeField(1, 1, EntityType.Pod);

            _model.RelocationAttempt(1, 1, 0, 0, 3, 3);
            Assert.AreEqual(_model.GetTemp(0, 0).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(1, 0).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(0, 1).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(1, 1).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(2, 2).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(2, 3).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(3, 2).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(3, 3).Type, EntityType.Pod);

            _model.RelocationAttempt(2, 3, 3, 2, 0, 1);
            Assert.AreEqual(_model.GetTemp(0, 0).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(1, 0).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(0, 1).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(1, 1).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(2, 2).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(2, 3).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(3, 2).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(3, 3).Type, EntityType.Empty);

            _model.RelocationAttempt(1, 0, 0, 1, 3, 2);
            Assert.AreEqual(_model.GetTemp(0, 0).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(1, 0).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(0, 1).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(1, 1).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(2, 2).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(2, 3).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(3, 2).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(3, 3).Type, EntityType.Pod);
        }

        [TestMethod]
        public void NonPodRelocationTest()
        {
            int x = 10;
            int y = 10;
            _model.GenerateEmtyTableForSettingsWindow(x, y);

            _model.ChangeField(0, 0, EntityType.Pod);
            _model.ChangeField(0, 1, EntityType.Dock);
            _model.ChangeField(1, 0, EntityType.Destination);
            _model.ChangeField(1, 1, EntityType.Pod);

            _model.RelocationAttempt(0, 0, 1, 1, 2, 2);
            Assert.AreEqual(_model.GetTemp(2, 2).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(3, 3).Type, EntityType.Pod);
            Assert.AreEqual(_model.GetTemp(2, 3).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(3, 2).Type, EntityType.Empty);

            Assert.AreEqual(_model.GetTemp(0, 0).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(1, 1).Type, EntityType.Empty);
            Assert.AreEqual(_model.GetTemp(0, 1).Type, EntityType.Dock);
            Assert.AreEqual(_model.GetTemp(1, 0).Type, EntityType.Destination);
        }

        [TestMethod]
        public void FailedRelocationTest()
        {
            int x = 10;
            int y = 10;
            _model.GenerateEmtyTableForSettingsWindow(x, y);

            _model.ChangeField(0, 0, EntityType.Pod);
            _model.ChangeField(0, 1, EntityType.Pod);
            _model.ChangeField(1, 0, EntityType.Pod);
            _model.ChangeField(1, 1, EntityType.Pod);
            _model.ChangeField(2, 3, EntityType.Pod);
            _model.ChangeField(3, 2, EntityType.Pod);
            _model.ChangeField(2, 2, EntityType.Pod);
            _model.ChangeField(3, 3, EntityType.Pod);
            _model.AddProduct(0, 0, 1, 1, 10);
            _model.AddProduct(2, 2, 3, 3, 5);
            _model.RelocationAttempt(0, 0, 1, 1, 2, 2);

            Assert.AreEqual(((Pod)_model.GetTemp(0, 0)).Products.ContainsKey(10), true);
            Assert.AreEqual(((Pod)_model.GetTemp(1, 0)).Products.ContainsKey(10), true);
            Assert.AreEqual(((Pod)_model.GetTemp(0, 1)).Products.ContainsKey(10), true);
            Assert.AreEqual(((Pod)_model.GetTemp(1, 1)).Products.ContainsKey(10), true);

            Assert.AreEqual(((Pod)_model.GetTemp(2, 2)).Products.ContainsKey(5), true);
            Assert.AreEqual(((Pod)_model.GetTemp(3, 2)).Products.ContainsKey(5), true);
            Assert.AreEqual(((Pod)_model.GetTemp(2, 3)).Products.ContainsKey(5), true);
            Assert.AreEqual(((Pod)_model.GetTemp(3, 3)).Products.ContainsKey(5), true);
        }

        [TestMethod]
        public void PartialRelocationTest()
        {
            int x = 10;
            int y = 10;
            _model.GenerateEmtyTableForSettingsWindow(x, y);

            _model.ChangeField(0, 0, EntityType.Pod);
            _model.ChangeField(0, 1, EntityType.Pod);
            _model.ChangeField(1, 0, EntityType.Pod);
            _model.ChangeField(1, 1, EntityType.Pod);
            //_model.ChangeField(2, 3, EntityType.Pod);
            //_model.ChangeField(3, 2, EntityType.Pod);
            _model.ChangeField(2, 2, EntityType.Pod);
            _model.ChangeField(3, 3, EntityType.Pod);
            _model.AddProduct(0, 0, 1, 1, 10);
            _model.AddProduct(2, 2, 3, 3, 5);
            _model.RelocationAttempt(0, 0, 1, 1, 2, 2);

            Assert.AreEqual(((Pod)_model.GetTemp(0, 0)).Products.ContainsKey(10), true);
            //Assert.AreEqual(((Pod)_model.GetTemp(1, 0)).Products.ContainsKey(10), true);
            //Assert.AreEqual(((Pod)_model.GetTemp(0, 1)).Products.ContainsKey(10), true);
            Assert.AreEqual(((Pod)_model.GetTemp(1, 1)).Products.ContainsKey(10), true);

            Assert.AreEqual(((Pod)_model.GetTemp(2, 2)).Products.ContainsKey(5), true);
            Assert.AreEqual(((Pod)_model.GetTemp(3, 2)).Products.ContainsKey(10), true);
            Assert.AreEqual(((Pod)_model.GetTemp(2, 3)).Products.ContainsKey(10), true);
            Assert.AreEqual(((Pod)_model.GetTemp(3, 3)).Products.ContainsKey(5), true);
        }




        #region Event handlers
        private void Model_FieldChanged(object sender, RobotMovedEventArgs e)
        {
            //Assert.AreEqual(_model.SizeX, e.X);
            //Assert.AreEqual(_model.SizeX, e.Y);
        }

        private void Model_SpeedChanged(object sender, SpeedChangedEventArgs e)
        {
            Assert.AreEqual(_model.Speed, e.Speed);
        }


        #endregion

    }
}
