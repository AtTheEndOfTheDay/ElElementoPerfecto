using System;
using System.IO;
using System.Xml;
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
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Game : IDisposable
    {
        private Game() { }
        public static Game Current = new Game();

        #region Constants
        private const Single _OriginalAspectRatio = 961f / 510f;
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
        public Boolean IsMeshVisible { get; set; }
        public Boolean IsColliderVisible { get; set; }
        private Level[] _Levels;
        public void Init(String mediaFolder)
        {
            if (_Levels != null) Dispose();
            _LoadShaders();
            _MaterialFolder = mediaFolder + "Texture\\Material\\";
            _ParticleFolder = mediaFolder + "Texture\\Particles\\";
            _SignFolder = mediaFolder + "Texture\\Sign\\";
            _SoundFolder = mediaFolder + "Sound\\";

            _Scene = new TgcSceneLoader().loadSceneFromFile(mediaFolder + "Mesh\\Items.xml");
            foreach (var mesh in _Scene.Meshes)
            {
                mesh.AutoTransformEnable = false;
                mesh.AutoUpdateBoundingBox = false;
            }
            var lvlPaths = Directory.GetFiles(mediaFolder + "Level\\", "*.xml", SearchOption.AllDirectories);
            var levels = new List<Level>();
            foreach (var lvlPath in lvlPaths)
                Parser.ParseLevels(lvlPath, levels);
            _Levels = levels.OrderBy(l => l.Order).ToArray();
            var screen = GuiController.Instance.Panel3d.Size;
            var cameraFix = _OriginalAspectRatio / ((Single)screen.Width / screen.Height);
            foreach (var level in _Levels)
                level.CameraPosition = level.CameraPosition.MultZ(cameraFix);
            _Levels[0].Load();
        }
        #endregion Constructors

        #region Media
        private String _MaterialFolder;
        public TgcTexture GetMaterial(String materialFile)
        {
            return TgcTexture.createTexture(_MaterialFolder + materialFile);
        }
        private String _ParticleFolder;
        public TgcTexture GetParticle(String particleFile)
        {
            return TgcTexture.createTexture(_ParticleFolder + particleFile);
        }
        private String _SignFolder;
        public TgcTexture GetSign(String signFile)
        {
            return TgcTexture.createTexture(_SignFolder + signFile);
        }
        private String _SoundFolder;
        public TgcStaticSound GetSound(String soundFile, int volume)
        {
            var sound = new TgcStaticSound();
            sound.loadSound(_SoundFolder + soundFile, volume);
            return sound;
        }
        private TgcScene _Scene;
        public TgcMesh GetMesh(String name)
        {
            return _Scene.getMeshByName(name);
        }
        public TgcMesh NewMesh(String father)
        {
            return GetMesh(father).clone(String.Empty);
        }
        #endregion Media

        #region GamePlay
        private Int32 _LevelIndex = 0;
        public void Play(Single deltaTime)
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            var level = _Levels[_LevelIndex];
            _LvlHack(input, level);
            if (level.IsComplete)
            {
                if (input.keyDown(Key.R))
                    level.RollBack();
                else if (input.keyDown(Key.Return))
                    advanceLevel(level);
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
            _Levels = null;
        }
        private void advanceLevel(Level level)
        {
            _LevelIndex++;
            if (_LevelIndex == _Levels.Length)
                _LevelIndex = 0;
            level.UnLoad();
            level = _Levels[_LevelIndex];
            level.Load();
        }
        private void backwardLevel(Level level)
        {
            _LevelIndex--;
            if (_LevelIndex == -1)
                _LevelIndex = _Levels.Length - 1;
            level.UnLoad();
            level = _Levels[_LevelIndex];
            level.Load();
        }
        private void _LvlHack(TgcD3dInput input, Level level)
        {
            if (input.keyPressed(Key.F2))
                advanceLevel(level);
            else if (input.keyPressed(Key.F1))
                backwardLevel(level);
        }
        #endregion GamePlay
    }
}
