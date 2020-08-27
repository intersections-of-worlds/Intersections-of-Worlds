using Unity.Mathematics;
namespace GameCore
{
    public struct RandomSeed
    {
        private uint seed;
        public RandomSeed(uint seed)
        {
            this.seed = seed;
        }
        public static RandomSeed New()
        {
            return new RandomSeed((uint)System.DateTime.Now.Ticks.GetHashCode());
        }
        public static implicit operator uint(RandomSeed seed)
        {
            return seed.seed;
        }
        public static implicit operator int(RandomSeed seed)
        {
            return (int)seed.seed;
        }
        public Random GetRandom()
        {
            return new Random(seed);
        }
        public void Next()
        {
            this.seed = GetRandom().NextUInt();
        }
        public RandomSeed Add(int hash)
        {
            return new RandomSeed((uint)new System.ValueTuple<uint, int>(seed, hash).GetHashCode());
        }
    }
    public static class StringExtention
    {
        /// <summary>
        /// 获得靠谱的唯一Hash
        /// </summary>
        public static int GetHash(this string str)
        {
            int HashCode = 0;
            for(int i = 0; i < str.Length; i++)
            {
                HashCode = HashCode * 13131 + str[i].GetHashCode();
            }
            return (HashCode & 0x7FFFFFFF);
        }
    }

}