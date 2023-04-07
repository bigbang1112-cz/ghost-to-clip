using GBX.NET.Engines.Game;
using GbxToolAPI;
using System.Runtime.InteropServices.JavaScript;
using TmEssentials;

namespace GhostToClip;

[ToolName("Ghost to Clip")]
[ToolDescription("Converts ghost to MediaTracker clip for variety of purposes.")]
public class GhostToClipTool : ITool, IHasOutput<NodeFile<CGameCtnMediaClip>>, IConfigurable<GhostToClipConfig>
{
    private readonly IEnumerable<CGameCtnGhost> ghosts;

    public GhostToClipConfig Config { get; set; } = new();

    public GhostToClipTool(IEnumerable<CGameCtnGhost> ghosts)
    {
        this.ghosts = ghosts ?? throw new ArgumentNullException(nameof(ghosts));
    }

    public NodeFile<CGameCtnMediaClip> Produce()
    {
        var tracks = new List<CGameCtnMediaTrack>();

        var longestGhostTime = TimeInt32.Zero;

        foreach (var ghost in ghosts)
        {
            var ghostTrack = CGameCtnMediaTrack.Create()
                .WithName("Ghost")
                .WithBlocks(GetGhostBlock(ghost))
                .ForTMUF()
                .Build();

            tracks.Add(ghostTrack);

            var raceTime = ghost.RaceTime.GetValueOrDefault(TimeInt32.FromSeconds(3));

            if (raceTime > longestGhostTime)
            {
                longestGhostTime = raceTime;
            }
        }

        var cameraBlock = CGameCtnMediaBlockCameraGame.Create()
            .WithTimeRange(TimeSingle.Zero, longestGhostTime)
            .ForTM2020()
            .WithClipEntId(1)
            .Build();

        var cameraTrack = CGameCtnMediaTrack.Create()
            .WithName("Camera")
            .WithBlocks(cameraBlock)
            .ForTMUF()
            .Build();

        tracks.Add(cameraTrack);

        var clip = CGameCtnMediaClip.Create()
            .WithTracks(tracks)
            .ForTMUF()
            .Build();

        var firstGhost = ghosts.First();

        var mapName = firstGhost.Validate_ChallengeUid ?? "unknownmap";
        var time = firstGhost.RaceTime.ToTmString(useApostrophe: true) ?? "unfinished";
        var author = firstGhost.GhostNickname ?? firstGhost.GhostLogin ?? "unnamed";

        var pureFileName = $"GhostToClip_{TextFormatter.Deformat(mapName)}_{time}_{TextFormatter.Deformat(author)}.Clip.Gbx";
        var validFileName = string.Join("_", pureFileName.Split(Path.GetInvalidFileNameChars()));

        var forManiaPlanet = GameVersion.IsManiaPlanet(firstGhost);
        var dir = forManiaPlanet ? "Replays/Clips/GhostToClip" : "Tracks/GhostToClip";

        return new(clip, $"{dir}/{validFileName}", forManiaPlanet);
    }

    private CGameCtnMediaBlock GetGhostBlock(CGameCtnGhost ghost) => Config.Game switch
    {
        EGame.Trackmania => GetTrackmaniaGhostBlock(ghost),
        EGame.Trackmania2020 => GetTrackmania2020GhostBlock(ghost),
        _ => throw new NotSupportedException($"Game {Config.Game} is not supported."),
    };

    private static CGameCtnMediaBlockEntity GetTrackmania2020GhostBlock(CGameCtnGhost ghost)
    {
        if (ghost.RecordData is null)
        {
            throw new InvalidOperationException("Ghost record data is null.");
        }

        return CGameCtnMediaBlockEntity.Create(ghost.RecordData)
            .WithNoticeRecords(Enumerable.Range(0, ghost.RecordData.EntList.Count).ToArray())
            .ForTM2020()
            .WithKeys(new CGameCtnMediaBlockEntity.Key(),
            new CGameCtnMediaBlockEntity.Key
            {
                Time = ghost.RaceTime.GetValueOrDefault(TimeInt32.FromSeconds(3))
            })
            .WithGhostName(ghost.GhostNickname ?? "Ghost")
            .Build();
    }

    private static CGameCtnMediaBlockGhost GetTrackmaniaGhostBlock(CGameCtnGhost ghost)
    {
        return CGameCtnMediaBlockGhost.Create(ghost)
            .ForTMUF()
            .EndingAt(ghost.RaceTime.GetValueOrDefault(TimeInt32.FromSeconds(3)) + TimeInt32.FromSeconds(3))
            .Build();
    }
}
