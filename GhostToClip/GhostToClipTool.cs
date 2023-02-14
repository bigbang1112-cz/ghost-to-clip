using GBX.NET.Engines.Game;
using GbxToolAPI;

namespace GhostToClip;

public class GhostToClipTool : ITool, IHasOutput<CGameCtnMediaClip>, IConfigurable<GhostToClipConfig>
{
    private readonly CGameCtnGhost ghost;

    public GhostToClipConfig Config { get; set; } = new();

    public GhostToClipTool(CGameCtnGhost ghost)
    {
        this.ghost = ghost ?? throw new ArgumentNullException(nameof(ghost));
    }

    public CGameCtnMediaClip Produce()
    {
        throw new NotImplementedException();
    }
}
