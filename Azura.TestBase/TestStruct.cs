using System;

namespace Azura.TestBase
{
    [Azura]
    public struct TestStruct
    {
        [Azura] public long LongValue;
        [Azura] public ushort? UshortValue;
        [Azura] public Guid? Guid;
        [Azura] public TestSubStruct Sub;
        [Azura] public TestSubStruct? Sub2;
    }
}
