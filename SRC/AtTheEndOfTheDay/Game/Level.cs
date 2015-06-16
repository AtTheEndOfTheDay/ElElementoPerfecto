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

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Level:GameComponent
    {
        #region Constants
        private static readonly Menu _MenuNull = new Menu(null);
        private static readonly Vector3 DefaultCameraPosition = Vector3Extension.Back * -200f;
        private static readonly Vector3 DefaultCameraTarget = Vector3.Empty;
        private static readonly Vector3 DefaultLightPosition = new Vector3(1f, 1f, -1f) * 500f;
        private const Single DefaultLightIntensity = 66f;
        private static readonly Vector3 DefaultPlanePoint = Vector3.Empty;
        private static readonly Vector3 DefaultPlaneNormal = Vector3Extension.Front;
        #endregion Constants

        #region Constructors
        public Level(Game game)
            : base(game)
        {
            _Stage = _Building;
            _Items = game;
            _Goals = goal;
            _Interactives = _Items.OfType<Interactive>().ToList();
            try
            {
                _Menu = game.OfType<Menu>().Single();
                _Menu.Add(user);
            }
            catch (Exception e) { throw new Exception("Menu not found.", e); }
            //TODO: pasarlo al .lvl y que el parser devuelva el string o directamente el TgcSprite mejor.
            _WinSign.Texture = TgcTexture.createTexture(sign);
            _WinSign.Scaling = new Vector2(0.5f, 0.5f);
            _WinSign.Position = new Vector2(300, 200);
        }
        #endregion Constructors

        #region Properties
        public Single Order { get; set; }
        public String _Name = String.Empty;
        public String Name
        {
            get { return _Name; }
            set { _Name = value ?? String.Empty; }
        }
        public String _Properties = String.Empty;
        public String Properties
        {
            get { return _Properties; }
            set { _Properties = value ?? String.Empty; }
        }
        public Menu _Menu = _MenuNull;
        public Menu Menu
        {
            get { return _Menu == _MenuNull ? null : _Menu; }
            set { _Menu = value ?? _MenuNull; }
        }
        #endregion Properties

        #region Fields
        private Item _Selected = null;
        private Color _SelectedColor = Color.Green;
        private Action<Single> _Stage = null;
        private TgcSprite _WinSign = new TgcSprite();
        public Boolean IsComplete { get; private set; }
        private readonly TgcPickingRay _PickingRay = new TgcPickingRay();

        public Vector3 CameraPosition = new Vector3(0f, 0f, -100f);
        public Vector3 CameraTarget = Vector3.Empty;
        public Vector3 LightPosition = new Vector3(100f, 100f, -100f);
        public Single LightIntensity = 66f;
        public Vector3 PlanePoint = Vector3.Empty;
        public Vector3 PlaneNormal = Vector3Extension.Front;
        public Plane Plane { get { return Plane.FromPointNormal(PlanePoint, PlaneNormal); } }
        #endregion Fields

        #region Lists
        public Goal[] Goals { get { return _Goals.ToArray(); } }
        private readonly IList<Goal> _Goals = new List<Goal>();
        public Goal Add(Goal goal)
        {
            _Goals.Add(goal);
            return goal;
        }
        public T Add<T>(T goals)
            where T : IEnumerable<Goal>
        {
            foreach (var goal in goals)
                Add(goal);
            return goals;
        }
        public Goal Remove(Goal goal)
        {
            _Goals.Remove(goal);
            return goal;
        }
        public T Remove<T>(T goals)
            where T : IEnumerable<Goal>
        {
            foreach (var goal in goals)
                Remove(goal);
            return goals;
        }

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
        public T Add<T>(T items)
            where T : IEnumerable<Item>
        {
            foreach (var item in items)
                Add(item);
            return items;
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
        public T Remove<T>(T items)
            where T : IEnumerable<Item>
        {
            foreach (var item in items)
                Remove(item);
            return items;
        }
        #endregion Lists

        #region GamePlay
        public void Load()
        {
            _Menu.Add(_Actives);
            Remove(_Actives);
            RollBack();
        }
        public void RollBack()
        {
            _Stage = _Building;
            IsComplete = false;
            foreach (var item in _Items)
                item.LoadValues();
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
                GuiController.Instance.Drawer2D.beginDrawSprite();
                _WinSign.render();
                GuiController.Instance.Drawer2D.endDrawSprite();
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
                RollBack();
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
