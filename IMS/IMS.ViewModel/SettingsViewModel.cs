﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Persistence.Entities;
using IMS.Model;
using IMS.ViewModel.Fields;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace IMS.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {

        #region Private Fields

        private IMSModel _model;
        private Dictionary<EntityType, String> _entityToColor;
        private Dictionary<EntityType, String> _entityToImg;
        private int _sizeX;
        private int _sizeY;
        private int _relocationStep;
        private int _x1;
        private int _x2;
        private int _y1;
        private int _y2;
        private string _fieldColor;
        private EntityType _selectedType;
        private int _selectedProduct;
        private int _selectionStep;
      

        #endregion

        #region Properties
        public DelegateCommand CreateSimulationCommand { get; private set; }
        public DelegateCommand ResetSimulationCommand { get; private set; }
        public DelegateCommand ViewField { get; private set; }

        public ObservableCollection<TableField> Fields { get; private set; }
        public DelegateCommand SetSizeCommand { get; private set; }
        public DelegateCommand ChangeColorCommand { get; private set; }
        public DelegateCommand SelectDockCommand { get; private set; }
        public DelegateCommand SelectPodCommand { get; private set; }
        public DelegateCommand SelectDestinationCommand { get; private set; }
        public DelegateCommand SelectRobotCommand { get; private set; }
        public DelegateCommand RelocationCommand { get; private set; }
        public DelegateCommand AddProductOrIDCommand { get; private set; }

        public EntityType SelectedType
        {
            get { return _selectedType; }
        }

        public Int32 SizeX { 
            get { return _sizeX; }
            set
            {
                _sizeX = value;
                OnPropertyChanged(nameof(SizeX));
            }
        }
        public Int32 SizeY { 
            get { return _sizeY; }
            set
            {
                _sizeY = value;
                OnPropertyChanged(nameof(SizeY));
            }
        }

        public Int32 SelectedProduct
        {
            get; set;
        }

        public Int32 SelectedCapacity
        {
            get; set;
        }

        public String FieldColor
        {
            get { return _fieldColor; }
            set
            {
                if( _fieldColor == "White") //ha a modelből az adott field empty
                {
                    _fieldColor = value;
                }
            }
        }

        

        #endregion

        #region Events
        public event EventHandler CreateSimulation;
        public event EventHandler ResetSimulation;
        public event EventHandler SetSimulationSize;
        public event EventHandler ColorChanged;
        public event EventHandler StartSimulation;
        #endregion


        #region constructor
        public SettingsViewModel(IMSModel model)
        {
            _relocationStep = 0;
            _selectionStep = 0;

            _entityToColor = new Dictionary<EntityType, String>();
            _entityToColor.Add(EntityType.Destination, "Green");
            _entityToColor.Add(EntityType.Robot, "Yellow");
            _entityToColor.Add(EntityType.Dock, "Blue");
            _entityToColor.Add(EntityType.Empty, "White");
            _entityToColor.Add(EntityType.Pod, "Gray");
            _entityToColor.Add(EntityType.RobotUnderPod, "Purple");

            _entityToImg = new Dictionary<EntityType, String>();
            _entityToImg.Add(EntityType.Robot, "/img/robot.png");
            _entityToImg.Add(EntityType.Pod, "/img/full-pod.png");
            _entityToImg.Add(EntityType.RobotUnderPod, "/img/robot-under-pod.png");
            _entityToImg.Add(EntityType.Destination, "/img/dest.png");
            _entityToImg.Add(EntityType.Dock, "/img/dock.png");
            _entityToImg.Add(EntityType.Empty, "/img/empty.png");

            Fields = new ObservableCollection<TableField>();

            _model = model;
            _model.SimulationCreated += new EventHandler<EventArgs>(Model_SimulationCreated);
            _model.FieldChanged_SVM += new EventHandler<RobotMovedEventArgs>(Model_FieldChanged_SVM);
            _model.SelectionChanged_SVM += new EventHandler<SelectionChangedEventArgs>(Model_SelectionChanged_SVM);

            SetSizeCommand = new DelegateCommand(x => OnGenerateEmptyTable());
            CreateSimulationCommand = new DelegateCommand(param => OnCreateSimulation());
            ResetSimulationCommand = new DelegateCommand(param => OnResetSimulation());
            ChangeColorCommand = new DelegateCommand(param => OnColorChanged());
            SelectRobotCommand = new DelegateCommand(x => _changeSelection(EntityType.Robot));
            SelectPodCommand = new DelegateCommand(x => _changeSelection(EntityType.Pod));
            SelectDestinationCommand = new DelegateCommand(x => _changeSelection(EntityType.Destination));
            SelectDockCommand = new DelegateCommand(x => _changeSelection(EntityType.Dock));

            RelocationCommand = new DelegateCommand(x => _startRelocation());

            AddProductOrIDCommand = new DelegateCommand(x => _startSelection());

            _fieldColor = "White";

        }
        #endregion

        #region Methods
        /// <summary>
        /// matches entities to their color
        /// </summary>
        /// <param name="type">type to convert to color</param>
        /// <returns></returns>
        private String EntityToColor(EntityType type)
        {
            return _entityToColor[type];
        }

        /// <summary>
        /// matches each entity to an image path
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private String EntityToImg(EntityType type)
        {
            string res = _entityToImg[type];
            return res;
        }

        /// <summary>
        /// matches a robot to corresponding image
        /// based on its rotation
        /// </summary>
        /// <param name="dir">the direction it faces</param>
        /// <returns></returns>
        private string RobotImage(String dir)
        {
            //LEFT, UP, RIGHT, DOWN, NONE 
            switch (dir)
            {
                case "DOWN":
                    return "/img/robot-left.png";
                case "UP":
                    return "/img/robot-right.png";
                case "RIGHT":
                    return "/img/robot-down.png";
                default:
                    return "/img/robot.png";
            }
        }

        /// <summary>
        /// matches RobotUnderPod to image based on its rotation
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private string RobotUnderPodImage(String dir)
        {
            //LEFT, UP, RIGHT, DOWN, NONE 
            switch (dir)
            {
                case "DOWN":
                    return "/img/robot-under-pod-left.png";
                case "UP":
                    return "/img/robot-under-pod-right.png";
                case "RIGHT":
                    return "/img/robot-under-pod-down.png";
                default:
                    return "/img/robot-under-pod.png";
            }
        }

        /// <summary>
        /// creates an empty viewmodel table (Fields)
        /// </summary>
        public void GenerateTable()
        {
            Fields.Clear();
            for (Int32 i = 0; i < SizeY; ++i)
            {
                for (Int32 j = 0; j < SizeX; ++j)
                {
                    Fields.Add(new TableField
                    {
                        X = j,
                        Y = i,
                        Color = EntityToColor(EntityType.Empty),
                        BorderColor = "Gray",
                        Direction = Direction.NONE.ToString(),
                        Number = i * SizeX + j,
                        BgImage = EntityToImg(EntityType.Empty),
                        PutFieldCommand = new DelegateCommand(param => PutEntity(Convert.ToInt32(param)))
                    });
                }
            }
            OnPropertyChanged(nameof(SizeX));
            OnPropertyChanged(nameof(SizeY));
        }

        /// <summary>
        /// syncs the model's table with a pre-existing
        /// viewmodel table (Fields)
        /// </summary>
        public void SetupTable()
        {
            foreach (TableField field in Fields)
            {
                field.Color = EntityToColor(_model[field.X, field.Y].Type);
                field.BorderColor = "Gray";
                field.Direction = _model[field.X, field.Y].Direction.ToString();
                field.Type = _model[field.X, field.Y].Type;
                if (field.Type == EntityType.Robot)
                {
                    field.BgImage = RobotImage(field.Direction);
                }
                else if (field.Type == EntityType.RobotUnderPod)
                {
                    field.BgImage = RobotUnderPodImage(field.Direction);
                }
                else
                {
                    field.BgImage = EntityToImg(_model[field.X, field.Y].Type);
                }
            }
        }

        /// <summary>
        /// called when one clicks on the creator's table
        /// other variables determine what's to be done
        /// </summary>
        /// <param name="ind">index of Field in Fields</param>
        private void PutEntity(Int32 ind)
        {
            TableField field = Fields[ind];
            if (_relocationStep == 0 && _selectionStep == 0)
            {
                if (_selectedType == EntityType.Robot && SelectedCapacity > 0)
                {
                    if (field.Type == EntityType.Robot)
                    {
                        _model.RotateRobot(field.X, field.Y);
                    }
                    else
                    {
                        _model.ChangeField(field.X, field.Y, _selectedType, SelectedCapacity, SelectedProduct);
                    }
                }
                else if (_selectedType == EntityType.Destination)
                {
                    _model.ChangeField(field.X, field.Y, _selectedType, SelectedCapacity, SelectedProduct);
                }
                else
                {
                    _model.ChangeField(field.X, field.Y, _selectedType);
                }
            }
            else if (_relocationStep == 1)
            {
                _x1 = field.X;
                _y1 = field.Y;
                ++_relocationStep;
                _model.Selection(_x1, _y1);
            }
            else if (_relocationStep == 2)
            {
                _x2 = field.X;
                _y2 = field.Y;
                ++_relocationStep;
                _model.Selection(_x2, _y2);
            }
            else if (_relocationStep == 3)
            {

                int x = field.X;
                int y = field.Y;

                _model.RelocationAttempt(_x1, _y1, _x2, _y2, x, y);


                _relocationStep = 0;

                _model.EndSelection();
            }
            else if (_selectionStep == 1)
            {
                _x1 = field.X;
                _y1 = field.Y;
                ++_selectionStep;
                _model.AddProductSelection(_x1, _y1);
            }
            else if (_selectionStep == 2)
            {
                _x2 = field.X;
                _y2 = field.Y;

                _model.AddProduct(_x1, _y1, _x2, _y2, SelectedProduct);

                _selectionStep = 0;
            }
        }

        /// <summary>
        /// when a simulation is created, generate a new table and fill it up with Empty
        /// entities
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_SimulationCreated(Object sender, EventArgs e)
        {
            GenerateTable();
            SetupTable();
        }
        /// <summary>
        /// when the model's representation of the creator table changes,
        /// make a visual update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_FieldChanged_SVM(Object sender, RobotMovedEventArgs e)
        {

            Fields[e.Y * _model.TempSizeX + e.X].Entity = _model.GetTemp(e.X, e.Y);
            Fields[e.Y * _model.TempSizeX + e.X].Type = _model.GetTemp(e.X, e.Y).Type;
            Fields[e.Y * _model.TempSizeX + e.X].Color = EntityToColor(_model.GetTemp(e.X, e.Y).Type);
        }
        /// <summary>
        /// highlight the border of selected regions in the creator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_SelectionChanged_SVM(Object sender, SelectionChangedEventArgs e)
        {
            Fields[e.Y * _model.TempSizeX + e.X].BorderColor = e.IsSelected ? "Red" : "Gray";
        }
        /// <summary>
        /// first Entity has been clicked with the Move option selected
        /// mark it with a variable and stop the Add
        /// </summary>
        private void _startRelocation()
        {
            _relocationStep = 1;
            _stopSelection();
        }

        /// <summary>
        /// stop the mass relocation of pods
        /// </summary>
        private void _stopRelocation()
        {
            _relocationStep = 0;
        }
        /// <summary>
        /// when the first corner of the product addition rectangle has been clicked,
        /// stop relocation (if it's ongoing)
        /// </summary>
        private void _startSelection()
        {
            _selectionStep = 1;
            _stopRelocation();
        }

        /// <summary>
        /// 
        /// </summary>
        private void _stopSelection()
        {
            _selectionStep = 0;
        }
        /// <summary>
        /// change the type of entity to be placed
        /// </summary>
        /// <param name="type"></param>
        private void _changeSelection(EntityType type)
        {
            _selectedType = type;
        }

        #endregion

        #region Event methods

        private void OnCreateSimulation()
        {
            
            if (CreateSimulation != null)
                CreateSimulation(this, EventArgs.Empty);
        }

        private void OnResetSimulation()
        {

            if (ResetSimulation != null)
                ResetSimulation(this, EventArgs.Empty);
        }

        private void OnGenerateEmptyTable() 
        {
            if (SetSimulationSize != null)
                SetSimulationSize(this, EventArgs.Empty);
        }

        private void OnColorChanged()
        {
            if (ColorChanged != null)
                ColorChanged(this, EventArgs.Empty);
        }

        #endregion

    }
}
