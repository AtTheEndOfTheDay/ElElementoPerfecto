using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Level : IGameComponent, IDisposable
    {
        #region Constants
        private static readonly TgcStaticSound _SoundNull = new TgcStaticSound();
        private static readonly Vector3 DefaultCameraPosition = Vector3Extension.Back * 200f;
        private static readonly Vector3 DefaultCameraTarget = Vector3.Empty;
        private static readonly Vector3 DefaultLightPosition = new Vector3(1f, 1f, -1f) * 500f;
        private const Single DefaultLightIntensity = 66f;
        private static readonly Vector3 DefaultPlanePoint = Vector3.Empty;
        private static readonly Vector3 DefaultPlaneNormal = Vector3Extension.Front;
        private static readonly Vector2 DefaultWinSignSize = new Vector2(113f, 56.5f);
        private static readonly Vector3 DefaultWinSignPosition = new Vector3(-25, 0, -10);
        private static readonly TgcStaticSound WinSound = Game.Current.GetSound("Win.wav", 0);
        #endregion Constants

        #region Constructors
        public Level()
        {
            _Stage = _Building;
            CameraPosition = DefaultCameraPosition;
            CameraTarget = DefaultCameraTarget;
            LightPosition = DefaultLightPosition;
            LightIntensity = DefaultLightIntensity;
            PlanePoint = DefaultPlanePoint;
            PlaneNormal = DefaultPlaneNormal;
        }
        #endregion Constructors

        #region Properties
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraTarget { get; set; }
        public Vector3 LightPosition { get; set; }
        public Single LightIntensity { get; set; }
        public Vector3 PlanePoint { get; set; }
        public Vector3 PlaneNormal { get; set; }
        public Plane Plane { get { return Plane.FromPointNormal(PlanePoint, PlaneNormal); } }
        public Single Order { get; set; }
        private String _Name = String.Empty;
        public String Name
        {
            get { return _Name; }
            set { _Name = value ?? String.Empty; }
        }
        private String _Description = String.Empty;
        public String Description
        {
            get { return _Description; }
            set { _Description = value ?? String.Empty; }
        }
        private String _Properties = String.Empty;
        public String Properties
        {
            get { return _Properties; }
            set { _Properties = value ?? String.Empty; }
        }
        private Menu _Menu = Menu.Null;
        public Menu Menu
        {
            get { return _Menu == Menu.Null ? null : _Menu; }
            set
            {
                if (_Menu == value) return;
                _Menu = value ?? Menu.Null;
                if (value != null && !_Items.Contains(_Menu))
                    _Items.Add(_Menu);
            }
        }
        private String _WinSignPath;
        public String WinSign
        {
            get { return _WinSignPath; }
            set
            {
                if (_WinSignPath == value) return;
                _WinSignPath = value;
                if (_WinSign.Texture != null)
                    _WinSign.Texture.dispose();
                if (String.IsNullOrWhiteSpace(value))
                    _WinSign.Texture = null;
                else
                {
                    try { _WinSign.Texture = Game.Current.GetSign(value); }
                    catch { _WinSign.Texture = null; }
                }
            }
        }
        private TexturedQuad _WinSign = new TexturedQuad()
        {
            Size = DefaultWinSignSize,
            Position = DefaultWinSignPosition,
        };
        public Vector2 WinSignSize
        {
            get { return _WinSign.Size; }
            set { _WinSign.Size = value; }
        }
        public Vector3 WinSignPosition
        {
            get { return _WinSign.Position; }
            set { _WinSign.Position = value; }
        }
        private TgcStaticSound _Sound = _SoundNull;
        private String _SoundPath;
        public String Sound
        {
            get { return _SoundPath; }
            set
            {
                if (_SoundPath == value) return;
                _SoundPath = value;
                if (_Sound != _SoundNull)
                    _Sound.dispose();
                if (String.IsNullOrWhiteSpace(value))
                    _Sound = _SoundNull;
                else
                {
                    try { _Sound = Game.Current.GetSound(value, -1500); }
                    catch { _Sound = _SoundNull; }
                }
            }
        }
        #endregion Properties

        #region Goals
        public IGoal[] Goals { get { return _Goals.ToArray(); } }
        private readonly IList<IGoal> _Goals = new List<IGoal>();
        public IGoal Add(IGoal goal)
        {
            _Goals.Add(goal);
            return goal;
        }
        public void Add(IEnumerable<IGoal> goals)
        {
            foreach (var goal in goals)
                Add(goal);
        }
        public IGoal Remove(IGoal goal)
        {
            _Goals.Remove(goal);
            return goal;
        }
        public void Remove(IEnumerable<IGoal> goals)
        {
            foreach (var goal in goals)
                Remove(goal);
        }
        #endregion Goals

        #region Items
        public Item[] Items { get { return _Items.ToArray(); } }
        private readonly IList<Item> _Items = new List<Item>();
        private readonly IList<Item> _Actives = new List<Item>();
        private readonly IList<Interactive> _Interactives = new List<Interactive>();
        public Item Add(Item item)
        {
            _Items.Add(item);
            var interactive = item as Interactive;
            if (interactive != null)
                _Interactives.Add(interactive);
            return item;
        }
        public void Add(IEnumerable<Item> items)
        {
            foreach (var item in items)
                Add(item);
        }
        public Item Remove(Item item)
        {
            _Items.Remove(item);
            _Actives.Remove(item);
            var interactive = item as Interactive;
            if (interactive != null)
                _Interactives.Remove(interactive);
            return item;
        }
        public void Remove(IEnumerable<Item> items)
        {
            foreach (var item in items)
                Remove(item);
        }
        #endregion Items

        #region GamePlay
        private Item _Selected = null;
        private Color _SelectedColor = Color.Green;
        private Action<Single> _Stage = null;
        public Boolean IsComplete { get; private set; }
        private readonly TgcPickingRay _PickingRay = new TgcPickingRay();

        public void Load()
        {
            _Sound.play(true);
            _Menu.Add(_Actives);
            Remove(_Actives.ToArray());
            RollBack();
        }
        public void RollBack()
        {
            _Stage = _Building;
            IsComplete = false;
            foreach (var item in _Items)
                item.LoadValues();
        }
        public void UnLoad()
        {
            _Sound.stop();
        }
        public void Play(Single deltaTime)
        {
            if (IsComplete) return;
            _StageControl();
            if (_Stage == null)
                _Menu.Animate(deltaTime);
            else _Stage(deltaTime);
        }
        public void SetCamera()
        {
            var camera = GuiController.Instance.ThirdPersonCamera;
            camera.setCamera(CameraTarget, CameraPosition.Y, CameraPosition.Z);
            camera.RotationY = CameraPosition.X;
            camera.Enable = true;
        }
        public void SetLight(Dx3D.Effect shader)
        {
            shader.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(LightPosition));
            shader.SetValue("lightIntensity", LightIntensity);
        }
        public void Render(Dx3D.Effect shader)
        {
            if (IsComplete)
            {
                _WinSign.Render();
            }
            foreach (var item in _Items)
                item.Render(shader);
            if (_Selected != null)
            {
                shader.SetValue("materialAmbientColor", ColorValue.FromColor(_SelectedColor));
                shader.SetValue("materialDiffuseColor", ColorValue.FromColor(_SelectedColor));
                shader.SetValue("materialSpecularColor", ColorValue.FromColor(_SelectedColor));
                _Selected.Render(shader);
            }
        }
        public void Dispose()
        {
            if (_WinSign.Texture != null)
                _WinSign.Texture.dispose();
            _Sound.dispose();
            foreach (var item in _Items)
                item.Dispose();
        }
        #endregion GamePlay

        #region StageControl
        private void _StageControl()
        {
            if (_Selected != null) return;
            var input = GuiController.Instance.D3dInput;
            if (input.keyPressed(Key.Space))
            {
                if (_Stage == _Building)
                    foreach (var item in _Items)
                        item.SaveValues();
                if (_Stage == _Simulation)
                    _Stage = null;
                else _Stage = _Simulation;
            }
            else if (input.keyPressed(Key.C))
            {
                if (_Stage == _Building) return;
                RollBack();

            }
        }

        private void _Building(Single deltaTime)
        {
            _Menu.Animate(deltaTime);
            if (_Selected == null)
                _Pick();
            else _Build(deltaTime);
        }

        private void _Simulation(Single deltaTime)
        {
            var collisions = new List<ItemCollision>();
            foreach (var item in _Items)
            {
                item.Animate(deltaTime);
                foreach (var interactive in _Interactives)
                {
                    item.Act(interactive, deltaTime);
                    var collision = item.Collide(interactive);
                    if (collision != null)
                        collisions.Add(collision);
                }
            }
            var i = 0;
            var reacted = true;
            var maxIterations = _Interactives.Count * 2;
            while (reacted && i++ < maxIterations)
            {
                reacted = false;
                foreach (var collision in collisions)
                    if (collision.Reaction(deltaTime))
                        reacted = true;
            }
            foreach (var interactive in _Interactives)
                interactive.Simulate(deltaTime);
            IsComplete = _Goals.All(goal => goal.IsMeet);
            if (IsComplete)
                WinSound.play();
        }

        private void _Pick()
        {
            var input = GuiController.Instance.D3dInput;
            var left = TgcD3dInput.MouseButtons.BUTTON_LEFT;
            var right = TgcD3dInput.MouseButtons.BUTTON_RIGHT;
            if (input.buttonPressed(left))
            {
                _PickingRay.updateRay();
                var ray = _PickingRay.Ray;
                var picked = _Actives.FirstOrDefault(i => i.Intercepts(ray));
                if (picked != null)
                    Remove(picked);
                else if (_Menu.Intercepts(ray))
                    picked = _Menu.Pick(ray);
                _Selected = picked;
            }
            else if (input.buttonPressed(right))
            {
                _PickingRay.updateRay();
                var ray = _PickingRay.Ray;
                var picked = _Actives.FirstOrDefault(i => i.Intercepts(ray));
                if (picked != null)
                    _Menu.Add(Remove(picked));
            }
        }

        private void _Build(Single deltaTime)
        {
            _PickingRay.updateRay();
            Single t; Vector3 position;
            TgcCollisionUtils.intersectRayPlane(_PickingRay.Ray, Plane, out t, out position);
            _Selected.Build(deltaTime);
            _Selected.Position = position;
            _SelectedColor = _Menu.Collides(_Selected) ? Color.Blue
                : (_Items.Any(i => i.Collides(_Selected)) ? Color.Red
                : Color.Green);
            var input = GuiController.Instance.D3dInput;
            var left = TgcD3dInput.MouseButtons.BUTTON_LEFT;
            if (input.buttonPressed(left))
            {
                if (_SelectedColor == Color.Blue)
                {
                    _Menu.Add(_Selected);
                    _Selected = null;
                }
                else if (_SelectedColor == Color.Green)
                {
                    _Actives.Add(Add(_Selected));
                    _Selected = null;
                }
            }
        }
        #endregion StageControl
    }
}
