using Raylib_cs;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.Cryptography;
using VillageBuilder;
using VillageBuilder.VillageBuilder;

/* TO DO
    2. ADD MENU AND GUI
    3. IMPLEMENT BUILDING 
    4. STEAL SOME MODELS
    5. AI
    6. MUSIC
    7. ENEMIES

*/
namespace VillageBuilder
{
    internal class Program
    {
        static List<Building> placedBuidlings = new();

        static BuildingType selectedBuilding = BuildingType.None;
        
        static void Main(string[] args)
        {
            var BuildingTypeValues = Enum.GetValues<BuildingType>();
            Random rng = new Random();
            Raylib.InitWindow(GameConfig.ScreenWidth, GameConfig.ScreenHeight, "Village Builder");
            ResourceManager.Init();
            Raylib.ToggleFullscreen();
            Raylib.SetTargetFPS(60);
            
            Camera2D camera = new Camera2D
            {
                target = new Vector2(GameConfig.GridWidth * GameConfig.TileSize / 2f, GameConfig.GridHeight * GameConfig.TileSize / 2f),
                offset = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f),
                rotation = 0f,
                zoom = 1f
            };
            Tile[,] grid = new Tile[GameConfig.GridWidth, GameConfig.GridHeight];
            for (int y = 0; y < GameConfig.GridHeight; y++)
            {
                for (int x = 0; x < GameConfig.GridWidth; x++)
                {
                    grid[x, y] = new Tile();
                    grid[x, y].Position.X = x;
                    grid[x, y].Position.Y = y;
                }
            }
            GenerateTerrain(grid);
            while (!Raylib.WindowShouldClose())
            {

                float camSpeed = 500 * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
                {
                    camera.target.Y -= camSpeed;
                }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
                {
                    camera.target.Y += camSpeed;
                }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
                {
                    camera.target.X -= camSpeed;
                }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
                {
                    camera.target.X += camSpeed;
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_Q))
                {
                    camera.zoom += 0.1f;
                }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_E))
                {
                    camera.zoom -= 0.1f;
                }
                camera.zoom = Math.Clamp(camera.zoom, 0.5f, 3.0f);
                camera.target.X = Math.Clamp(camera.target.X, GameConfig.ScreenWidth / 2f, GameConfig.GridWidth * GameConfig.TileSize - GameConfig.ScreenWidth / 2f);
                camera.target.Y = Math.Clamp(camera.target.Y, GameConfig.ScreenHeight / 2f, GameConfig.GridHeight * GameConfig.TileSize - GameConfig.ScreenHeight / 2f);
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RAYWHITE);
                Raylib.BeginMode2D(camera);

                for (int y = 0; y < GameConfig.GridHeight; y++)
                {
                    for (int x = 0; x < GameConfig.GridWidth; x++)
                    {
                        Rectangle tileRect = new Rectangle(x * GameConfig.TileSize, y * GameConfig.TileSize, GameConfig.TileSize, GameConfig.TileSize);
                        Tile tile = grid[x, y];

                        Color fill = tile.Type switch
                        {
                            TileType.Forest => Color.DARKGREEN,
                            TileType.Farm => Color.BEIGE,
                            TileType.Quarry => Color.DARKGRAY,
                            TileType.House => Color.BROWN,
                            TileType.Grassland => Color.GREEN,
                            TileType.Rock => Color.LIGHTGRAY,
                            TileType.Water => Color.SKYBLUE,
                            _ => Color.BLANK

                        };
                        
                        if (tile.Type != TileType.Empty)
                        {
                            Raylib.DrawRectangleRec(tileRect, fill);
                        }
                        Raylib.DrawRectangleLinesEx(tileRect, 1, Color.DARKGRAY);

                    }
                }
                foreach (var building in placedBuidlings)
                {
                    var pos = building.Position;
                    Rectangle rect = new Rectangle(pos.X * GameConfig.TileSize, pos.Y * GameConfig.TileSize, GameConfig.TileSize, GameConfig.TileSize);
                    Raylib.DrawRectangleRec(rect, Color.DARKBROWN); 
                    Raylib.DrawText(building.Type.ToString(), (int)rect.x + 5, (int)rect.y + 5, 10, Color.WHITE);
                }


                Raylib.EndMode2D();
                string resourceText =
                $"Wood: {ResourceManager.Get(ResourceType.Wood)}  " +
                $"Stone: {ResourceManager.Get(ResourceType.Stone)}  " +
                $"Food: {ResourceManager.Get(ResourceType.Food)}";

                int textWidth = Raylib.MeasureText(resourceText, 20);
                Raylib.DrawText(resourceText, GameConfig.ScreenWidth - textWidth - 20, 10, 20, Color.DARKBROWN);

                Raylib.DrawText("WASD to move | Q/E to zoom", 10, 10, 20, Color.DARKGRAY);
                int barHeight = 100;
                Rectangle uiBar = new Rectangle(0, GameConfig.ScreenHeight - barHeight, GameConfig.ScreenWidth, barHeight);
                Raylib.DrawRectangleRec(uiBar, Color.LIGHTGRAY);
                Raylib.DrawRectangleLinesEx(uiBar, 2, Color.DARKGRAY);

                
                for (int i = 0; i < BuildingTypeValues.Length; i++)
                {
                    int buttonWidth = 100;
                    int buttonHeight = 80;
                    int spacing = 20;
                    int x = -100 + i * (buttonWidth + spacing);
                    int y = GameConfig.ScreenHeight - barHeight + 10;

                    Rectangle buttonRect = new Rectangle(x, y, buttonWidth, buttonHeight);

                    Color buttonColor = selectedBuilding == (BuildingType)(i) ? Color.GREEN : Color.BEIGE;
                    Raylib.DrawRectangleRec(buttonRect, buttonColor);
                    Raylib.DrawRectangleLinesEx(buttonRect, 2, Color.BROWN);
                    Raylib.DrawText(BuildingTypeValues[i].ToString(), x + 10, y + 30, 20, Color.DARKBROWN);

                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON) &&
                        Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), buttonRect))
                    {

                        selectedBuilding = selectedBuilding == (BuildingType)(i ) ? selectedBuilding = BuildingType.None : selectedBuilding = (BuildingType)(i);
                    }
                }
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON) && selectedBuilding != BuildingType.None)
                {
                    Vector2 Pos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
                    int tileX = (int)(Pos.X / GameConfig.TileSize);
                    int tileY = (int)(Pos.Y / GameConfig.TileSize);
                    if(tileX >= 0 && tileX < GameConfig.GridWidth && tileY >= 0 && tileY < GameConfig.GridHeight)
                    {
                        Tile targetTile = grid[tileX, tileY];
                        if (targetTile.Type == TileType.Grassland && targetTile.Building == null)
                        {
                            Vector2Int position = new Vector2Int(tileX, tileY);
                            Building newBuilding = selectedBuilding switch
                            {
                                BuildingType.House => new House(position),
                                BuildingType.Farm => new Farm(position),
                                BuildingType.Quarry => new Quarry(position),
                                BuildingType.None => throw new Exception("Nemas vybrano"),
                                _ => throw new Exception("Not possible Man")


                            };
                            if (newBuilding != null)
                            {
                                targetTile.Building = newBuilding;
                                newBuilding.OnPlaced(grid);
                                placedBuidlings.Add(newBuilding);
                                
                            }
                        }
                            
                    }
                }
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
            
        }
        static private void GenerateTerrain(Tile[,] grid)
        {
            Random rng = new Random();
            foreach (var item in grid)
            {
                item.Type = TileType.Grassland;
            }
            
            for (int i = 0; i < 15; i++)
            {
                int lakeX = rng.Next(10, GameConfig.GridWidth - 10);
                int lakeY = rng.Next(10, GameConfig.GridHeight - 10);
                int radius = rng.Next(5, 12);
                GenerateBlob(grid, lakeX, lakeY, radius, TileType.Forest);
            }
            for (int i = 0; i < 11; i++)
            {
                int lakeX = rng.Next(10, GameConfig.GridWidth - 10);
                int lakeY = rng.Next(10, GameConfig.GridHeight - 10);
                int radius = rng.Next(5, 12);
                GenerateBlob(grid, lakeX, lakeY, radius, TileType.Rock);
            }
            for (int i = 0; i < 10; i++)
            {
                int lakeX = rng.Next(10, GameConfig.GridWidth - 10);
                int lakeY = rng.Next(10, GameConfig.GridHeight - 10);
                int radius = rng.Next(5, 12);
                GenerateBlob(grid, lakeX, lakeY, radius, TileType.Water);
            }
        }
        static void GenerateBlob(Tile[,] grid, int startX, int startY, int maxDepth, TileType type)
        {
            Random rng = new Random();
            Queue<(int x, int y, int depth)> queue = new Queue<(int x, int y, int depth)>();
            HashSet<(int, int)> visited = new HashSet<(int, int)>();

            queue.Enqueue((startX, startY, 0));
            visited.Add((startX, startY));

            while (queue.Count > 0)
            {
                var (x, y, depth) = queue.Dequeue();
                if (x < 0 || y < 0 || x >= GameConfig.GridWidth || y >= GameConfig.GridHeight)
                    continue;

               
                float spreadChance = 1f - (depth / (float)maxDepth);
                if (spreadChance <= 0) continue;

                // Actually set the tile
                grid[x, y].Type = type;

                // Try to spread to neighbors
                foreach (var (nx, ny) in TileHelper.GetNeighbors(x, y))
                {
                    if (!visited.Contains((nx, ny)) && rng.NextDouble() < spreadChance)
                    {
                        queue.Enqueue((nx, ny, depth + 1));
                        visited.Add((nx, ny));
                    }
                }
            }
        }
        static void TickBuildings()
        {
            foreach (var building in placedBuidlings)
            {
                building.Tick();
            }
        }
        

    }

}
