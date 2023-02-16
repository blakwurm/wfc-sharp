using System;
using System.Linq;
using NUnit.Framework;
using WfcHost;

namespace WfcTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestConstraints()
        {
            var (wfc, cards) = Utils.BuildWfc();
            var ids = cards.Select(x=> x.Id).ToArray();
            
            var ideal6_1 = ids[14] | ids[12] | ids[15] | ids[13] | ids[11] | ids[9] | ids[10] | ids[8];
            var actual6_1 = cards[6].GetConstraint(1);
            Assert.True(ideal6_1 == actual6_1);
            
            var ideal7_3 = ids[4] | ids[12] | ids[5] | ids[13] | ids[1] | ids[9] | ids[0] | ids[8];
            var actual7_3 = cards[7].GetConstraint(3);
            Assert.True(ideal7_3 == actual7_3);
        }

        [Test]
        public void TestIndices()
        {
            var (wfc, cards) = Utils.BuildWfc();
            var key = new[]
            {
                4, 6, 14, 12,
                5, 7, 15, 13,
                1, 3, 11, 9,
                0, 2, 10, 8
            };
            
            for(var i = 0; i < 16; i++)
            {
                Assert.True(cards[key[i]].Id == (ulong)1 << i);
            }
        }
    }
}