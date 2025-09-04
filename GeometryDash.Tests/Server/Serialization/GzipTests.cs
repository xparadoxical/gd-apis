namespace GeometryDash.Tests.Server.Serialization;
public class GzipTests
{
    static GzipTests()
    {
        var gzippedBase64dLevelString = """
            H4sIAAAAAAAACqVT243DMAxbyAVEPfLAfXWGDqABusINf5bpj-LgAg36ETEWRUpWkOfDjoZ0SU1opKVGJEBQApOeN-SWEJHcE4mocKTkkfhFDgvRzyzwvcW5tKgaCj4y0Sz9yujSSvr5ew9fepT8wreJtxeSK3vZ3tpcXPDqUu15hzUpCMJG8NYj3_cRdfI84Sh42EluRPoM4u4jkoWwiFXKMmWF7g1VLgQQdIDSSuli5IyccTSnmdPMg3JyZqx0Asc39qs_reAcgHkL6nSOCw7f4ada974SrXv2JtGfmVSt5D6Tratq05M0vCr-kS5L8g8dRrWWBwQAAA==
            """u8;

        var gzipped = new byte[Base64.GetMaxDecodedLength(gzippedBase64dLevelString.Length)];
        var written = Base64.Decode(gzippedBase64dLevelString, gzipped);
        var gzippedTrimmed = gzipped[..written];

        GzipData = gzippedTrimmed;
    }

    public static byte[] GzipData { get; }

    [Fact]
    public void GetDecompressedLength_Works()
    {
        Assert.Equal(1031u, Gzip.GetDecompressedLength(GzipData));
    }

    [Fact]
    public void Decompress_Works()
    {
        var outLength = Gzip.GetDecompressedLength(GzipData);
        var decompressed = new byte[outLength];

        var written = Gzip.Decompress(GzipData, decompressed);

        Assert.Equal(outLength, written);
        Assert.Equal("""
            kS38,1_40_2_125_3_255_11_255_12_255_13_255_4_-1_6_1000_7_1_15_1_18_0_8_1|1_0_2_102_3_255_11_255_12_255_13_255_4_-1_6_1001_7_1_15_1_18_0_8_1|1_0_2_102_3_255_11_255_12_255_13_255_4_-1_6_1009_7_1_15_1_18_0_8_1|1_255_2_255_3_255_11_255_12_255_13_255_4_-1_6_1002_5_1_7_1_15_1_18_0_8_1|1_40_2_125_3_255_11_255_12_255_13_255_4_-1_6_1013_7_1_15_1_18_0_8_1|1_40_2_125_3_255_11_255_12_255_13_255_4_-1_6_1014_7_1_15_1_18_0_8_1|1_125_2_125_3_255_11_255_12_255_13_255_4_-1_6_1005_5_1_7_1_15_1_18_0_8_1|1_0_2_255_3_255_11_255_12_255_13_255_4_-1_6_1006_5_1_7_1_15_1_18_0_8_1|1_255_2_255_3_255_11_255_12_255_13_255_4_-1_6_1004_7_1_15_1_18_0_8_1|,kA13,0,kA15,0,kA16,0,kA14,,kA6,0,kA7,0,kA25,0,kA17,0,kA18,0,kS39,0,kA2,0,kA3,0,kA8,0,kA4,0,kA9,0,kA10,0,kA22,0,kA23,0,kA24,0,kA27,1,kA40,1,kA41,1,kA42,1,kA28,0,kA29,0,kA31,1,kA32,1,kA36,0,kA43,0,kA44,0,kA45,1,kA46,0,kA33,1,kA34,1,kA35,0,kA37,1,kA38,1,kA39,1,kA19,0,kA26,0,kA20,0,kA21,0,kA11,0;1,1,2,105,3,15,155,1;1,1,2,225,3,75,155,1,21,1004;1,1,2,315,3,15,155,1,21,1004;1,1,2,405,3,15,155,1,21,1004;
            """u8, decompressed);
    }
}
