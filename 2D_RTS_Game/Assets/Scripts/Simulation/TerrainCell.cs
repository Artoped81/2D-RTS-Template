using RTSTemplate.Core;

namespace RTSTemplate.Simulation
{
    public struct TerrainCell
    {
        public MovementDomain Domain;
        public bool Occupied;

        public TerrainCell(MovementDomain domain)
        {
            Domain = domain;
            Occupied = false;
        }
    }
}
