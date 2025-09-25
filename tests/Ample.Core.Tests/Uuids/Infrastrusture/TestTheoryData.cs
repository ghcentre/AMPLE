namespace Ample.Core.Tests.Uuids.Infrastrusture;

internal static class TestTheoryData
{
    public static readonly TheoryData<string, string> KnownGuids = new()
    {
        { "03020100-0504-0706-0809-0a0b0c0d0e0f", "AAECAwQFBgcICQoLDA0ODw" },
        { "00000000-0000-0000-0000-000000000000", "AAAAAAAAAAAAAAAAAAAAAA" },
        { "ffffffff-ffff-ffff-ffff-ffffffffffff", "_____________________w" },
        { "e17b6b67-9cba-4d6b-85aa-1094412734fa", "Z2t74bqca02FqhCUQSc0-g" },
        { "d51c50d4-f414-49fb-9e33-d97709b56ff7", "1FAc1RT0-0meM9l3CbVv9w" },
        { "0e9bc1bd-3aaa-43da-ae3c-2cfdf58fb25f", "vcGbDqo62kOuPCz99Y-yXw" },
        { "7045f127-7020-403b-802c-e61bc1342584", "J_FFcCBwO0CALOYbwTQlhA" },
        { "e05a2248-d16b-4298-8c61-0c521ff6b353", "SCJa4GvRmEKMYQxSH_azUw" },
        { "fcd9ef74-0b32-47e6-a118-99ad7b11f86f", "dO_Z_DIL5kehGJmtexH4bw" }
    };

    public static readonly string[] SequentialV7Guids =
    [
        "01997f91-e9db-74da-b3cc-3a3b5c36155b",
        "01997f92-39b5-7a50-9760-c0bcc5bd62a8",
        "01997f92-622c-7f25-8b01-b69229c8d50c",
        "01997f92-9ed6-7f34-b947-d021d11d83de",
        "01997f93-0888-7e0d-b28f-1e6f8978d7a6"
    ];
}