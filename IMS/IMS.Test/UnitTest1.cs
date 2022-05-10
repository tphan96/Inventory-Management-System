using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
using IMS.Model;
using IMS.Persistence;
using System;
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
        //private PathFinder _sim;
        //private Mock<IIMSDataAccess> _mock;
        #endregion

        #region Initialization
        [TestInitialize]
        public void Initialize()
        {
            //j�t�kt�bla m�g kell ide
            //int totalEnergyConsumption = 0;
            //int timeStep = 0;
            //int sizeX = 12;
            //int sizeY = 12;
            //List<Pod> podData = new List<Pod>();
            //podData.Add(new Pod(4, 3, new Dictionary<int, int>()));
            //podData.Add(new Pod(4, 5, new Dictionary<int, int>()));
            //podData.Add(new Pod(4, 6, new Dictionary<int, int>()));
            //List<Destination> destinationData = new List<Destination>();
            //destinationData.Add(new Destination(11, 5, 1));

            //List<Dock> dockData = new List<Dock>();
            //dockData.Add(new Dock(0, 11));

            //List<Robot> robotData = new List<Robot>();
            //Robot robot = new Robot(0, 0, 0, 5, 5, 1, 1);
            //robotData.Add(robot);

            //List<RobotUnderPod> robotUnderPodData = new List<RobotUnderPod>();
            //Robot robot2 = new Robot(4, 4, 0, 5, 5, 1, 1);
            //Pod pod2 = new Pod(4, 4, new Dictionary<int, int>());
            //RobotUnderPod robotunder = new RobotUnderPod(4, 4, robot2, pod2);
            //_table = new IMSData(podData, destinationData, dockData, robotData, robotUnderPodData, sizeX, sizeY, timeStep, totalEnergyConsumption);

            //_mock = new Mock<IIMSDataAccess>();
            //_mock.Setup(mock => mock.LoadSimulationAsync(It.IsAny<String>())).Returns(() => Task.FromResult(_table));

            //_model = new IMSModel(_mock.Object);
            _dataAccess = new IMSDataAccess();
            _model = new IMSModel(_dataAccess);
            //_model.LoadSimulationAsync(It.IsAny<String>())).Returns(() => Task.FromResult(_table));
            //_sim = new PathFinder(_table);

            //_model.TimePassed += new EventHandler(Model_TimePassed);
            //_model.SpeedChanged += new EventHandler(Model_SpeedChanged);


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
            Assert.AreEqual(routes[_table.EntityData.RobotData[0]].Count(), 22);
        }
        [TestMethod]
        public void Robot2RouteTest()
        {
            Dictionary<Robot, List<Pos>> routes = CreatTestTable();
            Assert.AreEqual(routes[_table.EntityData.RobotData[1]].Count(), 34);
        }
        #endregion
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
