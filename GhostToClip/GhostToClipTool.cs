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
        var ghostBlock = GetGhostBlock();

        var ghostTrack = CGameCtnMediaTrack.Create()
            .WithName(ghost.GhostNickname ?? "Ghost")
            .WithBlocks(ghostBlock)
            .ForTMUF()
            .Build();

        var cameraBlock = CGameCtnMediaBlockCameraGame.Create()
            .WithTimeRange(TimeSingle.Zero, ghost.RaceTime.GetValueOrDefault(TimeInt32.FromSeconds(3)))
            .ForTM2020()
            .WithClipEntId(1)
            .Build();

        var cameraTrack = CGameCtnMediaTrack.Create()
            .WithName("Camera")
            .WithBlocks(cameraBlock)
            .ForTMUF()
            .Build();

        return CGameCtnMediaClip.Create()
            .WithTracks(ghostTrack, cameraTrack)
            .ForTMUF()
            .Build();
    }

    private CGameCtnMediaBlock GetGhostBlock() => Config.Game switch
    {
        EGame.Trackmania => GetTrackmaniaGhostBlock(),
        EGame.Trackmania2020 => GetTrackmania2020GhostBlock(),
        _ => throw new NotSupportedException($"Game {Config.Game} is not supported."),
    };

    private CGameCtnMediaBlockEntity GetTrackmania2020GhostBlock()
    {
        if (ghost.RecordData is null)
        {
            throw new InvalidOperationException("Ghost record data is null.");
        }

        return CGameCtnMediaBlockEntity.Create(ghost.RecordData)
            .WithNoticeRecords(new[] { 1, 0 })
            .ForTM2020()
            .WithKeys(new CGameCtnMediaBlockEntity.Key(),
            new CGameCtnMediaBlockEntity.Key
            {
                Time = ghost.RaceTime.GetValueOrDefault(TimeInt32.FromSeconds(3))
            })
            .Build();
    }

    private CGameCtnMediaBlockGhost GetTrackmaniaGhostBlock()
    {
        return CGameCtnMediaBlockGhost.Create(ghost)
            .ForTMUF()
            .EndingAt(ghost.RaceTime.GetValueOrDefault(TimeInt32.FromSeconds(3)) + TimeInt32.FromSeconds(3))
            .Build();
    }
}
