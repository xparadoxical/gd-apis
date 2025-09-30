using GeometryDash.Server.Serialization;

namespace GeometryDash.Server;

[Separator(Prop = ':', ListItem = '|')]
public partial class PagingInfo(uint results, uint pageIndex, uint pageSize) : ISerializable<PagingInfo>
{
    public PagingInfo() : this(0, 0, 0) { } //TODO record/primaryctor support

    [Index(1)]
    public uint Results { get; set; } = results;

    [Index(2)]
    public uint PageIndex { get; set; } = pageIndex;

    [Index(3)]
    public uint PageSize { get; set; } = pageSize;
}
