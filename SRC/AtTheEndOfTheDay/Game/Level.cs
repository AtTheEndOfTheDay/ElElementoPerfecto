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

//TgcTexture madera = TgcTexture.createTexture(alumnoTextureFolder() + "Madera.jpg");
//TgcTexture metal = TgcTexture.createTexture(alumnoTextureFolder() + "Metal.jpg");
//TgcTexture metal2 = TgcTexture.createTexture(alumnoTextureFolder() + "METAL_t.jpg");
//TgcTexture laja = TgcTexture.createTexture(alumnoTextureFolder() + "Laja.jpg");
//TgcTexture texturaCannon = TgcTexture.createTexture(alumnoTextureFolder() + "Cannon.png");
//TgcTexture texturaMagnet = TgcTexture.createTexture(alumnoTextureFolder() + "Magnet.png");
//TgcTexture texturaSpring = TgcTexture.createTexture(alumnoTextureFolder() + "Spring.png");
//TgcTexture texturaPasarDeNivel = TgcTexture.createTexture(alumnoTextureFolder() + "PasasteDeNivel.jpg");
//TgcTexture texturaGanaste = TgcTexture.createTexture(alumnoTextureFolder() + "Ganaste!!!.jpg");

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Level
    {
        #region Constructors
        public Level(IList<Item> game, IList<Item> user, IEnumerable<IGoal> goal)
        {
            _Stage = _Building;
            _Game = game;
            _Goal = goal;
            _Interactives = _Game.OfType<Interactive>().ToList();
            try
            {
                _Menu = game.OfType<Menu>().Single();
                _Menu.Add(user);
            }
            catch (Exception e) { throw new Exception("Menu not found.", e); }
        }
        #endregion Constructors

        #region Fields
        private Item _Selected = null;
        private Color _SelectedColor = Color.Green;
        private Action<Single> _Stage = null;
        public Boolean IsComplete { get; private set; }
        private readonly TgcPickingRay _PickingRay = new TgcPickingRay();

        public Vector3 CameraPosition = new Vector3(0f, 0f, -100f);
        public Vector3 CameraTarget = Vector3.Empty;
        public Vector3 LightPosition = new Vector3(100f, 100f, -100f);
        public Single LightIntensity = 66f;
        public Vector3 PlanePoint = Vector3.Empty;
        public Vector3 PlaneNormal = Vector3Extension.Front;
        private Plane _Plane { get { return Plane.FromPointNormal(PlanePoint, PlaneNormal); } }
        #endregion Fields

        #region Lists
        private readonly Menu _Menu;
        private readonly IList<Item> _Game;
        private readonly IEnumerable<IGoal> _Goal;
        private readonly IList<Item> _Actives = new List<Item>();
        private readonly IList<Interactive> _Interactives;
        private void _Activate(Item item)
        {
            item.SaveValues();
            _Game.Add(item);
            _Actives.Add(item);
            var interactive = item as Interactive;
            if (interactive != null)
                _Interactives.Add(interactive);
        }
        private void _Deactivate(Item item)
        {
            _Game.Remove(item);
            _Actives.Remove(item);
            var interactive = item as Interactive;
            if (interactive != null)
                _Interactives.Remove(interactive);
        }
        private void _DeactivateAll()
        {
            _Menu.Add(_Actives);
            IList interactive = (IList)_Interactives;
            foreach (var item in _Actives)
            {
                _Game.Remove(item);
                interactive.Remove(item);
            }
            _Actives.Clear();
        }
        #endregion Lists

        #region GamePlay
        public void Load()
        {
            IsComplete = false;
            _DeactivateAll();
            _RollBack();
        }
        public void Play(Single deltaTime)
        {
            if (IsComplete) return;
            _StageControl();
            if (_Stage == null) return;
            _Stage(deltaTime);
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
            foreach (var item in _Game)
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
            foreach (var item in _Game)
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
                    foreach (var item in _Game)
                        item.SaveValues();
                if (_Stage == _Simulation)
                    _Stage = null;
                else _Stage = _Simulation;
            }
            else if (input.keyPressed(Key.C))
                _RollBack();
        }
        private void _RollBack()
        {
            _Stage = _Building;
            foreach (var item in _Game)
                item.LoadValues();
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
            foreach (var item in _Game)
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
            IsComplete = _Goal.All(goal => goal.IsMeet);
        }

        private void _Pick()
        {
            var input = GuiController.Instance.D3dInput;
            var left = TgcD3dInput.MouseButtons.BUTTON_LEFT;
            if (input.buttonPressed(left))
            {
                _PickingRay.updateRay();
                var ray = _PickingRay.Ray;
                var picked = _Actives.FirstOrDefault(i => i.Intercepts(ray));
                if (picked != null)
                    _Deactivate(picked);
                else if (_Menu.Intercepts(ray))
                    picked = _Menu.Pick(ray);
                _Selected = picked;
            }
        }

        private void _Build(Single deltaTime)
        {
            _PickingRay.updateRay();
            float t; Vector3 position;
            TgcCollisionUtils.intersectRayPlane(_PickingRay.Ray, _Plane, out t, out position);
            _Selected.Build(deltaTime);
            _Selected.Position = position;
            _SelectedColor = _Menu.Collides(_Selected) ? Color.Blue
                : (_Game.Any(i => i.Collides(_Selected)) ? Color.Red
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
                    _Activate(_Selected);
                    _Selected = null;
                }
            }
        }
        #endregion StageControl
    }
}
