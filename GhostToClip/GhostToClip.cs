using GBX.NET.Engines.Game;
using GbxToolAPI;

namespace GhostToClip;

public class GhostToClip : Tool<GhostToClipConfig>, IHasOutput<CGameCtnMediaClip>
{
    private readonly CGameCtnGhost ghost;

    public override GhostToClipConfig Config { get; set; } = new();

    public GhostToClip(CGameCtnGhost ghost)
    {
        this.ghost = ghost ?? throw new ArgumentNullException(nameof(ghost));
    }

    public CGameCtnMediaClip Produce()
    {
        throw new NotImplementedException();
    }
}
