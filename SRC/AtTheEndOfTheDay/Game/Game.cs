using System;
using System.IO;
using System.Linq;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using System.Reflection;
using System.Collections.Generic;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Game : IDisposable
    {
        #region Constants
        private const Single _OriginalAspectRatio = 961f / 510f;
        private const String _GameLabel = "Game]";
        private const String _UserLabel = "User]";
        private const String _GoalLabel = "Goal]";
        private const String _LevelLabel = "Level]";
        private static readonly Type[] _ItemTypes = typeof(Item).LoadSubTypes();
        private static readonly Type[] _GoalTypes = typeof(IGoal).LoadSubTypes();

        private readonly String _MaterialFolder;
        private readonly String _SignFolder;
        private readonly TgcScene _Scene;
        private readonly Level[] _Levels;
        private readonly Dx3D.Effect _LightShader = GuiController.Instance.Shaders.TgcMeshPointLightShader.Clone(GuiController.Instance.D3dDevice);
        private void _LoadShaders()
        {
            //Cargar variables shader de la luz
            _LightShader.SetValue("lightColor", ColorValue.FromColor(Color.White));
            _LightShader.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(GuiController.Instance.ThirdPersonCamera.getPosition()));
            _LightShader.SetValue("lightAttenuation", 1);
            //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            _LightShader.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            _LightShader.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            _LightShader.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            _LightShader.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            _LightShader.SetValue("materialSpecularExp", 10f);
        }
        #endregion Constants

        #region Constructors
        public Game(String mediaFolder)
        {
            _LoadShaders();
            _MaterialFolder = mediaFolder + "Texture\\Material\\";
            _SignFolder = mediaFolder + "Texture\\Sign\\";
            _Scene = new TgcSceneLoader().loadSceneFromFile(mediaFolder + "Mesh\\Items.xml");
            foreach (var mesh in _Scene.Meshes)
            {
                mesh.AutoTransformEnable = false;
                mesh.AutoUpdateBoundingBox = false;
            }
            var lvls = Directory.GetFiles(mediaFolder + "Level\\", "*.lvl");
            _Levels = new Level[lvls.Length];
            for (var i = 0; i < lvls.Length; i++)
                _Levels[i] = _NewLevel(lvls[i]);
            var screen = GuiController.Instance.Panel3d.Size;
            var cameraFix = _OriginalAspectRatio / ((Single)screen.Width / screen.Height);
            foreach (var level in _Levels)
                level.CameraPosition.Z *= cameraFix;
            _Levels[0].Load();
        }
        private Level _NewLevel(String lvlPath)
        {
            try
            {
                var lvl = File.ReadAllText(lvlPath);
                var sections = lvl.Split('[');
                var game = _NewItemList(sections, _GameLabel);
                var user = _NewItemList(sections, _UserLabel);
                var goal = _NewGoalList(sections, game, user);
                foreach (var item in game)
                    item.Init(game, user);
                foreach (var item in user)
                    item.Init(game, user);
                var levelSection = sections.FirstOrDefault(s => s.StartsWith(_LevelLabel));
                var props = levelSection.Substring(_LevelLabel.Length).Split('\n');
                return new Level(game, user, goal).LoadFieldsFromText(props);
            }
            catch (ArrayTypeMismatchException e) { throw e; }
            catch (Exception e) { throw new Exception("Wrong level file format.", e); }
        }
        private IList<Item> _NewItemList(String[] sections, String header)
        {
            return _ItemTypes.CreateFromTextStart<Item>(sections, header, this);
        }
        private static IEnumerable<IGoal> _NewGoalList(String[] sections, params Object[] parameters)
        {
            return _GoalTypes.CreateFromTextStart<IGoal>(sections, _GoalLabel, parameters).ToArray();
        }
        #endregion Constructors

        #region MeshMethods
        public TgcMesh GetMesh(String name)
        {
            return _Scene.getMeshByName(name);
        }
        public TgcMesh NewMesh(String father)
        {
            return GetMesh(father).clone(String.Empty);
        }
        public TgcMesh NewMesh(String father, Color color)
        {
            var mesh = NewMesh(father);
            mesh.setColor(color);
            return mesh;
        }
        public TgcMesh NewMesh(String father, String material)
        {
            var mesh = NewMesh(father + "Textured");
            mesh.changeDiffuseMaps(new TgcTexture[] {
                TgcTexture.createTexture(_MaterialFolder + material)
            });
            return mesh;
        }
        #endregion MeshMethods

        #region GamePlay
        private Int32 _LevelIndex = 0;
        public void Play(Single deltaTime)
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            var level = _Levels[_LevelIndex];
            if (level.IsComplete)
            {
                if (input.keyDown(Key.R))
                    level.Load();
                else if (input.keyDown(Key.Return))
                {
                    _LevelIndex++;
                    if (_LevelIndex == _Levels.Length)
                        _LevelIndex = 0;
                    _Levels[_LevelIndex].Load();
                }
            }
            else level.Play(deltaTime);
            level.SetCamera();
            level.SetLight(_LightShader);
            level.Render(_LightShader);
            _LightShader.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            _LightShader.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            _LightShader.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            //level.Render(_LightShader);
        }
        public void Dispose()
        {
            _Scene.disposeAll();
            foreach (var level in _Levels)
                level.Dispose();
        }
        #endregion GamePlay
    }
}
