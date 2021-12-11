using Essentials.Helpers;
using Xunit;

namespace Essentials.Test.Extensions
{
    public class RandomizerTests
    {

        [Fact]
        public void ReturnsDoubleInRange()
        {
            for (int run = 0; run < 100; run++)
            {
                var random = Randomizer.GetDouble();
                Assert.InRange(random, 0, 1);
            }
        }

        [Fact]
        public void ReturnsIntInRange()
        {
            for (int run = 0; run < 100; run++)
            {
                var random = Randomizer.GetInt32();
                Assert.InRange(random, 0, int.MaxValue);
            }
        }

        [Fact]
        public void ReturnsIntInSmallRange()
        {
            for (int run = 0; run < 100; run++)
            {
                var random = Randomizer.GetInt32(upper: 10);
                Assert.InRange(random, 0, 10);
            }
        }

        [Fact]
        public void ReturnsIntInNegativeRange()
        {
            for (int run = 0; run < 100; run++)
            {
                var random = Randomizer.GetInt32(-100, 100);
                Assert.InRange(random, -100, 100);
            }
        }

    }
}
