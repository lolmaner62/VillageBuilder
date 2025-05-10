using Raylib_cs;
using System.Net.Http.Headers;
using System.Numerics;
using VillageBuilder;

/* TO DO
    1. IMPROVE TERRAIN GEN
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
        static float camSpeed = 500 * Raylib.GetFrameTime();
        static void Main(string[] args)
        {
            Random rng = new Random();
            Raylib.InitWindow(GameConfig.ScreenWidth, GameConfig.ScreenHeight, "Village Builder");
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
                }
            }
            GenerateTerrain(grid);
            while (!Raylib.WindowShouldClose())
            {
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
                
                Raylib.EndMode2D();
                Raylib.DrawText("WASD to move | Q/E to zoom", 10, 10, 20, Color.DARKGRAY);
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
            for (int i = 0; i < 10; i++)
            {
                int lakeX = rng.Next(10, GameConfig.GridWidth - 10);
                int lakeY = rng.Next(10, GameConfig.GridHeight - 10);
                int radius = rng.Next(3, 7);
                GenerateLake(grid, lakeX, lakeY, radius);
            }
            for (int i = 0; i < 12; i++)
            {
                int lakeX = rng.Next(10, GameConfig.GridWidth - 10);
                int lakeY = rng.Next(10, GameConfig.GridHeight - 10);
                int radius = rng.Next(3, 7);
                GenerateForest(grid, lakeX, lakeY, radius);
            }
            for (int i = 0; i < 9; i++)
            {
                int lakeX = rng.Next(10, GameConfig.GridWidth - 10);
                int lakeY = rng.Next(10, GameConfig.GridHeight - 10);
                int radius = rng.Next(3, 7);
                GenerateRocks(grid, lakeX, lakeY, radius);
            }

        }
        static void GenerateLake(Tile[,] grid, int centerX, int centerY, int radius)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int gx = centerX + x;
                    int gy = centerY + y;

                    if (gx >= 0 && gx < GameConfig.GridWidth && gy >= 0 && gy < GameConfig.GridHeight)
                    {
                        if (x * x + y * y <= radius * radius)
                            grid[gx, gy].Type = TileType.Water;
                    }
                }
            }
        }
        static void GenerateRocks(Tile[,] grid, int centerX, int centerY, int radius)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int gx = centerX + x;
                    int gy = centerY + y;

                    if (gx >= 0 && gx < GameConfig.GridWidth && gy >= 0 && gy < GameConfig.GridHeight)
                    {
                        if (x * x + y * y <= radius * radius)
                            grid[gx, gy].Type = TileType.Rock;
                    }
                }
            }
        }
        static void GenerateForest(Tile[,] grid, int centerX, int centerY, int radius)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int gx = centerX + x;
                    int gy = centerY + y;

                    if (gx >= 0 && gx < GameConfig.GridWidth && gy >= 0 && gy < GameConfig.GridHeight)
                    {
                        if (x * x + y * y <= radius * radius)
                            grid[gx, gy].Type = TileType.Forest;
                    }
                }
            }
        }

    }

}
