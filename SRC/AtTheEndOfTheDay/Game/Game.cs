using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    public class Game : IDisposable
    {
        private Game() { }//Singleton
        public static Game Current = new Game();

        #region Constants
        private const Single _OriginalAspectRatio = 961f / 510f;
        private readonly Dx3D.Effect _ToonShader = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "AtTheEndOfTheDay\\Mesh\\ToonShading.fx");
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

        #region GraphsConfig
        public Boolean IsMeshVisible { get; set; }
        public Boolean IsColliderVisible { get; set; }
        public Boolean IsToonShaderEnabled { get; set; }
        public Boolean IsTemporalEffectEnabled { get; set; }
        private void _GraphsConfig()
        {
            var gui = GuiController.Instance;
            var input = gui.D3dInput;
            if (input.keyPressed(Key.F6))
                IsToonShaderEnabled = !IsToonShaderEnabled;
            else if (input.keyPressed(Key.F7))
                IsTemporalEffectEnabled = !IsTemporalEffectEnabled;
            else if (input.keyPressed(Key.F8))
                IsMeshVisible = !IsMeshVisible;
            else if (input.keyPressed(Key.F9))
                IsColliderVisible = !IsColliderVisible;
            else if (input.keyPressed(Key.F10))
                gui.FpsCounterEnable = !gui.FpsCounterEnable;
            else if (input.keyPressed(Key.F11))
                gui.AxisLines.Enable = !gui.AxisLines.Enable;
            else if (input.keyPressed(Key.F12))
            {
                var rs = gui.D3dDevice.RenderState;
                rs.FillMode = rs.FillMode == FillMode.WireFrame
                    ? FillMode.Solid
                    : FillMode.WireFrame;
            }
        }
        #endregion GraphsConfig

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
        private Single _CameraFix;
        private String[] _Paths;
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
                mesh.AlphaBlendEnable = true;
                mesh.AutoTransformEnable = false;
                mesh.AutoUpdateBoundingBox = false;
                //mesh.Technique = "NormalMap";
            }
            _ToonShader.Technique = "NormalMap";
            var screen = GuiController.Instance.Panel3d.Size;
            _CameraFix = _OriginalAspectRatio / ((Single)screen.Width / screen.Height);
            _Paths = Directory.GetFiles(mediaFolder + "Level\\", "*.xml", SearchOption.AllDirectories);
            _Levels = new Level[_Paths.Length];
            _LoadLevelThread = new Thread(_LoadLevelThreadHandler);
            _LoadLevelThread.Start();
            //TODO:Cambiar cartel

            InitLoadingSign();
            
        }

        private TexturedQuad _LoadSign = new TexturedQuad()
        {
            Size = new Vector2(270, 150),
            Position = new Vector3(0, 0, -10),
        };

        private TexturedQuad _LoadWord = new TexturedQuad();

        private TgcQuad _BlackQuad = new TgcQuad()
        { 
            Center = Vector3.Empty,
            Normal = Vector3Extension.Back,
            Size = new Vector2(1000, 1000),
            Color = Color.Black,
            
        };
        private AnimatedQuad[] _LoadingAnimations = new AnimatedQuad[6];

        private void InitLoadingSign()
        {
            Vector2 auxSize;
            _LoadSign.Texture = GetSign("Loading9.png");
            _LoadWord.Texture = GetSign("LoadingWord.png");
            var camera = GuiController.Instance.ThirdPersonCamera;
            camera.setCamera(Vector3.Empty, 0, -200);
            camera.Enable = true;

            _BlackQuad.updateValues();

            auxSize = new Vector2(35, 15);

            _LoadWord.Size = new Vector2(45, 10);
            _LoadWord.Position = new Vector3(_LoadWord.Size.X / 2 + (auxSize.X * (-3)), -40, -10);

            for (int i = 0; i < 6; i++)
            {
                _LoadingAnimations[i] = new AnimatedQuad()
                {
                    Texture = Current.GetParticle("RedArrows.png"),
                    FrameSize = new Size(512, 256),

                    Size = auxSize,
                    Position = new Vector3(auxSize.X / 2 + (auxSize.X * (i - 3)), -55, -10),
                    FirstFrame = 7,
                    CurrentFrame = 7,
                    FrameRate = 3,
                    TotalFrames = 0,
                };

                _LoadingAnimations[i].Start();
            }

        }


        private Thread _LoadLevelThread;
        private void _LoadLevelThreadHandler()
        {
            var firstLevels = new List<Level>();
            Parser.ParseLevels(_Paths[0], firstLevels);
            if (firstLevels.Count > 0)
            {
                var level = _Levels[0] = firstLevels[0];
                level.Load();
                level.CameraPosition = level.CameraPosition.MultZ(_CameraFix);
            }
            for (var i = 1; i < _Levels.Length; i++)
            {
                var levels = new List<Level>();
                Parser.ParseLevels(_Paths[i], levels);
                if (levels.Count > 0)
                {
                    var level = _Levels[i] = levels[0];
                    level.CameraPosition = level.CameraPosition.MultZ(_CameraFix);
                }
            }
            _LoadLevelThread.Abort();
        }
        private Int32 _LevelIndex = 0;
        public void Play(Single deltaTime)
        {
            _GraphsConfig();
            _LvlHack();
            var level = _Levels[_LevelIndex];
            if (level == null)
            {
                _BlackQuad.render();
                _LoadSign.Render();
                _LoadWord.Render();
                for (int i = 0; i < 6; i++)
                {
                    _LoadingAnimations[i].Update(deltaTime);
                    _LoadingAnimations[i].Render();
                }
                return;
            }
            var input = GuiController.Instance.D3dInput;
            if (level.IsComplete)
            {
                if (input.keyDown(Key.R))
                    level.RollBack();
                else if (input.keyDown(Key.Return))
                    _SetLevel(_NextIndex);
            }
            else level.Play(deltaTime);
            level.SetCamera();
            _LightShader.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(level.LightPosition));
            _LightShader.SetValue("lightIntensity", level.LightIntensity);
            _LightShader.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            _LightShader.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            _LightShader.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            level.Render(IsToonShaderEnabled ? _ToonShader : _LightShader);
            //level.Render(_ToonShader);
            //level.Render(_LightShader);
        }
        public void SetColor(Dx3D.Effect shader, Color color)
        {
            if (shader == _LightShader)
            {
                shader.SetValue("materialAmbientColor", ColorValue.FromColor(color));
                shader.SetValue("materialDiffuseColor", ColorValue.FromColor(color));
                shader.SetValue("materialSpecularColor", ColorValue.FromColor(color));
            }
        }
        public void SetAlpha(Dx3D.Effect shader, Single alpha)
        {
            if (shader == _LightShader)
            {
                var color = shader.GetValueColor(shader.GetParameter(null, "materialDiffuseColor"));
                color.Alpha = alpha;
                shader.SetValue("materialDiffuseColor", color);
            }
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

            //_BlackQuad.dispose();
            for (int i = 0; i < 4; i++)
            {
                _LoadingAnimations[i].Dispose();
            }

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
