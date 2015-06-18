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
    internal static class Parser
    {
        private const String _LevelTag = "Level";
        private const String _GoalTag = "Goals";
        private const String _GameTag = "Game";
        private const String _UserTag = "User";
        private static readonly Type[] _LevelTypes;
        private static readonly Type[] _GoalTypes;
        private static readonly Type[] _ItemTypes;
        static Parser()
        {
            var components = typeof(IGameComponent).FindSubTypes();
            _GoalTypes = typeof(IGoal).FindSubTypes(components);
            _ItemTypes = typeof(Item).FindSubTypes(components);
        }

        public static void ParseLevels(String lvlPath, IList<Level> levels)
        {
            var reader = new XmlTextReader(lvlPath);
            while (reader.Read())
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (_LevelTag.IgnoreCaseEquals(reader.Name))
                            levels.Add(reader.ParseLevel());
                        break;
                }
        }
        private static Level ParseLevel(this XmlTextReader reader)
        {
            List<IGoal> goals = null;
            List<Item> game = null, user = null;
            Level level = new Level();
            try
            {
                reader.SetObjectProperties(level);
                while (reader.Read())
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (goals == null && _GoalTag.IgnoreCaseEquals(reader.Name))
                                goals = reader.ParseList<IGoal>(_GoalTypes, _GoalTag);
                            else if (game == null && _GameTag.IgnoreCaseEquals(reader.Name))
                                game = reader.ParseList<Item>(_ItemTypes, _GameTag);
                            else if (user == null && _UserTag.IgnoreCaseEquals(reader.Name))
                                user = reader.ParseList<Item>(_ItemTypes, _UserTag);
                            break;
                        case XmlNodeType.EndElement:
                            if (_LevelTag.IgnoreCaseEquals(reader.Name))
                                return level.LoadGoalsAndItems(goals, game, user);
                            break;
                    }
            }
            catch(Exception e) { }
            return level.LoadGoalsAndItems(goals, game, user);
        }
        private static Level LoadGoalsAndItems(this Level level, List<IGoal> goals, List<Item> game, List<Item> user)
        {
            Item[] items = { };
            if (game != null)
            {
                items = items.Concat(game).ToArray();
                level.Add(game);
                var menu = game.OfType<Menu>().FirstOrDefault();
                if (menu != null)
                {
                    level.Menu = menu;
                    if (user != null)
                    {
                        level.Menu.Add(user);
                        items = items.Concat(user).ToArray();
                    }
                }
            }
            if (goals != null)
            {
                level.Add(goals);
                foreach (var goal in goals)
                    goal.FindTargets(items);
            }
            foreach (var item in items)
            {
                item.FindSiblings(items);
                item.SaveValues();
            }
            return level;
        }
        private static List<T> ParseList<T>(this XmlTextReader reader, Type[] types, String endTag)
        {
            var list = new List<T>();
            try
            {
                while (reader.Read())
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            try
                            {
                                var t = types.FirstOrDefault(x => x.Name.IgnoreCaseEquals(reader.Name));
                                if (t == null) break;
                                list.Add((T)reader.SetObjectProperties(t.NewInstance()));
                            }
                            catch (Exception e) { }
                            break;
                        case XmlNodeType.EndElement:
                            if (endTag.IgnoreCaseEquals(reader.Name))
                                return list;
                            break;
                    }
            }
            catch (Exception e) { }
            return list;
        }
    }
}
