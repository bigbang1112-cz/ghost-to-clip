using GBX.NET.Engines.Game;
using GbxToolAPI;
using TmEssentials;

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
        var ghostMediaBlock = CGameCtnMediaBlockGhost.Create(ghost)
            .ForTMUF()
            .EndingAt(ghost.RaceTime.GetValueOrDefault(TimeInt32.FromSeconds(3)) + TimeInt32.FromSeconds(3))
            .Build();

        var track = CGameCtnMediaTrack.Create()
            .WithName(ghost.GhostNickname ?? "Unnamed")
            .WithBlocks(ghostMediaBlock)
            .ForTMUF()
            .Build();

        return CGameCtnMediaClip.Create()
            .WithTracks(track)
            .ForTMUF()
            .Build();
    }
}
