using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Dx3D = Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.AtTheEndOfTheDay.ThePerfectElement
{
    internal static class Parser
    {
        private const String _DescriptionTag = "Description";
        private const String _LevelTag = "Level";
        private const String _GoalTag = "Goals";
        private const String _GameTag = "Game";
        private const String _UserTag = "User";
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
                            if (_DescriptionTag.IgnoreCaseEquals(reader.Name))
                                level.SetDescription(reader);
                            else if (goals == null && _GoalTag.IgnoreCaseEquals(reader.Name))
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
        private static Level SetDescription(this Level level, XmlTextReader reader)
        {
            try
            {
                var description = String.Empty;
                while (reader.Read())
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Text:
                            description += reader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            if (_DescriptionTag.IgnoreCaseEquals(reader.Name))
                            {
                                level.Description = description;
                                return level;
                            }
                            break;
                    }
            }
            catch (Exception e) { }
            return level;
        }
        private const BindingFlags _BindingFlags = BindingFlags.Public | BindingFlags.Instance;
        private static T SetObjectProperties<T>(this XmlTextReader reader, T instance, Type type = null)
        {
            try
            {
                type = type ?? instance.GetType();
                while (reader.MoveToNextAttribute())
                    if (!String.IsNullOrWhiteSpace(reader.Value))
                        try
                        {
                            var prop = type.GetProperty(reader.Name, _BindingFlags);
                            var value = prop.PropertyType.IsArray
                                ? reader.Value.ParseArray(prop.PropertyType.GetElementType())
                                : reader.Value.ParseValue();
                            prop.SetValue(instance, value, null);
                        }
                        catch (Exception e) { }
            }
            catch (Exception e) { }
            return instance;
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
