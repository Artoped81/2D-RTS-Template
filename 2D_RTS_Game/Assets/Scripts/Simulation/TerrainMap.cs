using RTSTemplate.Core;

namespace RTSTemplate.Simulation
{
    public class TerrainMap
    {
        public int Width { get; }
        public int Height { get; }

        private readonly TerrainCell[,] cells;

        public TerrainMap(int width, int height, MovementDomain defaultDomain = MovementDomain.Land)
        {
            Width = width;
            Height = height;
            cells = new TerrainCell[width, height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    cells[x, y] = new TerrainCell(defaultDomain);
        }

        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        public TerrainCell GetCell(int x, int y) => cells[x, y];

        public void SetDomain(int x, int y, MovementDomain domain)
        {
            cells[x, y].Domain = domain;
        }

        public void SetOccupied(int x, int y, bool occupied)
        {
            if (!IsInBounds(x, y)) return;
            cells[x, y].Occupied = occupied;
        }

        public bool IsPassable(MovementDomain travelerDomain, int x, int y)
        {
            if (!IsInBounds(x, y) || cells[x, y].Occupied) return false;

            return travelerDomain == MovementDomain.Air || cells[x, y].Domain == travelerDomain;
        }
    }
}
