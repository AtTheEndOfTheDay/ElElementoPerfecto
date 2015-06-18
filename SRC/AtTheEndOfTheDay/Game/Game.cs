using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Drawing;
using System.Threading;
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
        private Game() { }//Singleton
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
        public Boolean IsMeshVisible { get; set; }
        public Boolean IsColliderVisible { get; set; }
        public Boolean IsToonShaderEnabled { get; set; }
        private Single _CameraFix;
        private String[] _Paths;
        private Level[] _Levels;
        private TexturedQuad _LoadSign = new TexturedQuad()
        {
            Size = new Vector2(113f, 56.5f),
            Position = new Vector3(-25, 0, -10),
        };
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
            var screen = GuiController.Instance.Panel3d.Size;
            _CameraFix = _OriginalAspectRatio / ((Single)screen.Width / screen.Height);
            _Paths = Directory.GetFiles(mediaFolder + "Level\\", "*.xml", SearchOption.AllDirectories);
            _Levels = new Level[_Paths.Length];
            _LoadLevelThread = new Thread(_LoadLevelThreadHandler);
            _LoadLevelThread.Start();
            //TODO:Cambiar cartel
            _LoadSign.Texture = GetSign("Win.png");
        }
        private Thread _LoadLevelThread;
        private void _LoadLevelThreadHandler()
        {
            var firstLevels = new List<Level>();
            Parser.ParseLevels(_Paths[0], firstLevels);
            if (firstLevels.Count > 0)
            {
                _Levels[0] = firstLevels[0];
                _Levels[0].Load();
            }
            for (var i = 1; i < _Levels.Length; i++)
            {
                var levels = new List<Level>();
                Parser.ParseLevels(_Paths[i], levels);
                if (levels.Count > 0)
                    _Levels[i] = levels[0];
            }
            _LoadLevelThread.Abort();
        }
        private Int32 _LevelIndex = 0;
        public void Play(Single deltaTime)
        {
            _LvlHack();
            var level = _Levels[_LevelIndex];
            if (level == null)
            {
                _LoadSign.Render();
                return;
            }
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (level.IsComplete)
            {
                if (input.keyDown(Key.R))
                    level.RollBack();
                else if (input.keyDown(Key.Return))
                    _SetLevel(_NextIndex);
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
            _LoadLevelThread.Abort();
            _LoadSign.Dispose();
            _Scene.disposeAll();
            foreach (var level in _Levels)
                if (level != null)
                    level.Dispose();
            _LevelIndex = 0;
            _Levels = null;
            _Paths = null;
        }
        private void _LvlHack()
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyPressed(Key.F2))
                _SetLevel(_NextIndex);
            else if (input.keyPressed(Key.F1))
                _SetLevel(_PrevIndex);
        }
        private void _SetLevel(Int32 index)
        {
            var newLevel = _Levels[index];
            if (newLevel == null) return;
            var prevLevel = _Levels[_LevelIndex];
            if (prevLevel != null)
                prevLevel.UnLoad();
            _LevelIndex = index;
            newLevel.Load();
        }
        private Int32 _NextIndex
        {
            get
            {
                var next = _LevelIndex + 1;
                return next == _Levels.Length ? 0 : next;
            }
        }
        private Int32 _PrevIndex
        {
            get
            {
                var prev = _LevelIndex - 1;
                return prev == -1 ? _Levels.Length - 1 : prev;
            }
        }
        #endregion GamePlay
    }
}
