using Rougelikeberry.Map;
using Rougelikeberry.Render;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleGame : MonoBehaviour
{
    public Console Map;
    public Console Inventory;
    public Console Stat;
    public Console Message;

    private static DungeonMap dunegonMap;
    private static Player player;
    private static CommandSystem commandSys;

    private bool active; 

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => RootConsole.s_Singleton.Initialized == true);
        active = true;
        Map.SetBackgroundColor(Palette.FloorBackground);
        Message.SetBackgroundColor(Swatch.DbDeepWater);
        Inventory.SetBackgroundColor(Swatch.DbWood);
        Stat.SetBackgroundColor(Swatch.DbOldStone);

        player = new Player();
        commandSys = new CommandSystem();

        MapGenerator mapGen = new MapGenerator(Map.GetWidth(), Map.GetHeight());
        dunegonMap = mapGen.CreateMap();
        dunegonMap.Draw(Map);
    }

    public void Update()
    {
        if (!active) { return; }

        bool renderNow = false;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            renderNow = commandSys.MovePlayer(Direction.Up);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            renderNow = commandSys.MovePlayer(Direction.Right);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            renderNow = commandSys.MovePlayer(Direction.Down);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            renderNow = commandSys.MovePlayer(Direction.Left);
        }

        if (renderNow)
        {
            dunegonMap.Draw(Map);
            player.Draw(Map, dunegonMap);
            dunegonMap.UpdatePlayerFOV();

            renderNow = false;
        }
    }


    #region Map
    public class DungeonMap : Map
    {
        public void Draw(Console console)
        {
            foreach (Block block in GetAllBlocks())
            {
                SetConsoleSymbolForBlock(console, block);
            }
        }

        private void SetConsoleSymbolForBlock(Console console, Block block)
        {
            if (!block.IsExplored)
            {
                return;
            }

            if (IsInFoV(block.X, block.Y))
            {
                if (block.IsWalkable)
                {
                    console.GetCell(block.X, block.Y).SetContent((int)'.', Palette.FloorBackgroundFov, Palette.FloorFov);
                }
                else
                {
                    console.GetCell(block.X, block.Y).SetContent((int)'#', Palette.WallBackgroundFov, Palette.WallFov);
                }
            }
            else
            {
                if (block.IsWalkable)
                {
                    console.GetCell(block.X, block.Y).SetContent((int)'.', Palette.FloorBackground, Palette.Floor);
                }
                else
                {
                    console.GetCell(block.X, block.Y).SetContent((int)'#', Palette.WallBackground, Palette.Wall);
                }
            }
        }

        public void UpdatePlayerFOV()
        {
            var player = ExampleGame.player;
            ComputeFoV(player.X, player.Y, player.Awareness, true);

            foreach (Block block in GetAllBlocks())
            {
                if (IsInFoV(block.X, block.Y))
                {
                    SetBlockProperties(block.X, block.Y, block.IsTransparent, block.IsWalkable, true);
                }
            }
        }

        // Returns true when able to place the Actor on the cell or false otherwise
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            // Only allow actor placement if the cell is walkable
            if (GetBlock(x, y).IsWalkable)
            {
                // The cell the actor was previously on is now walkable
                SetIsWalkable(actor.X, actor.Y, true);
                // Update the actor's position
                actor.X = x;
                actor.Y = y;
                // The new cell the actor is on is now not walkable
                SetIsWalkable(actor.X, actor.Y, false);
                // Don't forget to update the field of view if we just repositioned the player
                if (actor is Player)
                {
                    UpdatePlayerFOV();
                }
                return true;
            }
            return false;
        }

        // A helper method for setting the IsWalkable property on a Cell
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Block block = GetBlock(x, y) as Block;
            SetBlockProperties(block.X, block.Y, block.IsTransparent, isWalkable, block.IsExplored);
        }
    }

    public class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;

        private readonly DungeonMap _map;

        public MapGenerator(int width, int height)
        {
            _width = width;
            _height = height;
            _map = new DungeonMap();
        }

        public DungeonMap CreateMap()
        {
            _map.Initialize(_width, _height);
            foreach (Block block in _map.GetAllBlocks())
            {
                _map.SetBlockProperties(block.X, block.Y, true, true, true);
            }

            foreach (Block block in _map.GetBlocksInRows(0,_height-1))
            {
                _map.SetBlockProperties(block.X, block.Y, false, false, true);
            }

            foreach (Block block in _map.GetBlocksInColumns(0, _width-1))
            {
                _map.SetBlockProperties(block.X, block.Y, false, false, true);
            }
            return _map;
        }
    }
    #endregion

    #region Player
    public enum Direction
    {
        None = 0,
        DownLeft = 1,
        Down = 2,
        DownRight = 3,
        Left = 4,
        Center = 5,
        Right = 6,
        UpLeft = 7,
        Up = 8,
        UpRight = 9
    }

    public interface IActor
    {
        string Name { get; set; }
        int Awareness { get; set; }
    }

    public interface IDrawable
    {
        Color Color { get; set; }
        char Symbol { get; set; }
        int X { get; set; }
        int Y { get; set; }

        void Draw(Console console, IMap map);
    }

    public class Actor : IActor, IDrawable
    {
        // IActor
        public string Name { get; set; }
        public int Awareness { get; set; }

        // IDrawable
        public Color Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public void Draw(Console console, IMap map)
        {
            // Don't draw actors in cells that haven't been explored
            if (!map.GetBlock(X, Y).IsExplored)
            {
                return;
            }

            // Only draw the actor with the color and symbol when they are in field-of-view
            if (map.IsInFoV(X, Y))
            {
                console.GetCell(X, Y).SetContent(Symbol, Palette.FloorBackgroundFov, Color);
            }
            else
            {
                // When not in field-of-view just draw a normal floor
                console.GetCell(X, Y).SetContent(Symbol, Palette.FloorBackground, Palette.Floor);
            }
        }
    }

    public class Player : Actor
    {
        public Player()
        {
            Awareness = 15;
            Name = "Rogue";
            Color = Swatch.DbLight;
            Symbol = '@';
            X = 10;
            Y = 10;
        }
    }
    #endregion

    #region Systems
    public class CommandSystem
    {
        // Return value is true if the player was able to move
        // false when the player couldn't move, such as trying to move into a wall
        public bool MovePlayer(Direction direction)
        {
            int x = ExampleGame.player.X;
            int y = ExampleGame.player.Y;

            switch (direction)
            {
                case Direction.Up:
                    {
                        y = ExampleGame.player.Y - 1;
                        break;
                    }
                case Direction.Down:
                    {
                        y = ExampleGame.player.Y + 1;
                        break;
                    }
                case Direction.Left:
                    {
                        x = ExampleGame.player.X - 1;
                        break;
                    }
                case Direction.Right:
                    {
                        x = ExampleGame.player.X + 1;
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            if (ExampleGame.dunegonMap.SetActorPosition(ExampleGame.player, x, y))
            {
                return true;
            }

            return false;
        }
    }
    #endregion

    #region Colors
    public class Palette
    {
        public static Color FloorBackground = Color.black;
        public static Color Floor = Swatch.AlternateDarkest;
        public static Color FloorBackgroundFov = Swatch.DbDark;
        public static Color FloorFov = Swatch.Alternate;

        public static Color WallBackground = Swatch.SecondaryDarkest;
        public static Color Wall = Swatch.Secondary;
        public static Color WallBackgroundFov = Swatch.SecondaryDarker;
        public static Color WallFov = Swatch.SecondaryLighter;

        public static Color TextHeading = Swatch.DbLight;
    }

    public class Swatch
    {
        public static Color PrimaryLightest = EasyColor.Convert(110, 121, 119);
        public static Color PrimaryLighter = EasyColor.Convert(88, 100, 98);
        public static Color Primary = EasyColor.Convert(68, 82, 79);
        public static Color PrimaryDarker = EasyColor.Convert(48, 61, 59);
        public static Color PrimaryDarkest = EasyColor.Convert(29, 45, 42);

        public static Color SecondaryLightest = EasyColor.Convert(116, 120, 126);
        public static Color SecondaryLighter = EasyColor.Convert(93, 97, 105);
        public static Color Secondary = EasyColor.Convert(72, 77, 85);
        public static Color SecondaryDarker = EasyColor.Convert(51, 56, 64);
        public static Color SecondaryDarkest = EasyColor.Convert(31, 38, 47);

        public static Color AlternateLightest = EasyColor.Convert(190, 184, 174);
        public static Color AlternateLighter = EasyColor.Convert(158, 151, 138);
        public static Color Alternate = EasyColor.Convert(129, 121, 107);
        public static Color AlternateDarker = EasyColor.Convert(97, 89, 75);
        public static Color AlternateDarkest = EasyColor.Convert(71, 62, 45);

        public static Color ComplimentLightest = EasyColor.Convert(190, 180, 174);
        public static Color ComplimentLighter = EasyColor.Convert(158, 147, 138);
        public static Color Compliment = EasyColor.Convert(129, 116, 107);
        public static Color ComplimentDarker = EasyColor.Convert(97, 84, 75);
        public static Color ComplimentDarkest = EasyColor.Convert(71, 56, 45);

        public static Color DbDark = EasyColor.Convert(20, 12, 28);
        public static Color DbOldBlood = EasyColor.Convert(68, 36, 52);
        public static Color DbDeepWater = EasyColor.Convert(48, 52, 109);
        public static Color DbOldStone = EasyColor.Convert(78, 74, 78);
        public static Color DbWood = EasyColor.Convert(133, 76, 48);
        public static Color DbVegetation = EasyColor.Convert(52, 101, 36);
        public static Color DbBlood = EasyColor.Convert(208, 70, 72);
        public static Color DbStone = EasyColor.Convert(117, 113, 97);
        public static Color DbWater = EasyColor.Convert(89, 125, 206);
        public static Color DbBrightWood = EasyColor.Convert(210, 125, 44);
        public static Color DbMetal = EasyColor.Convert(133, 149, 161);
        public static Color DbGrass = EasyColor.Convert(109, 170, 44);
        public static Color DbSkin = EasyColor.Convert(210, 170, 153);
        public static Color DbSky = EasyColor.Convert(109, 194, 202);
        public static Color DbSun = EasyColor.Convert(218, 212, 94);
        public static Color DbLight = EasyColor.Convert(222, 238, 214);
    }

    public class EasyColor
    {
        public static Color Convert(float r, float g, float b)
        {
            r /= 255F;
            g /= 255;
            b /= 255;
            return new Color(r, g, b);
        }
    }
    #endregion
}
