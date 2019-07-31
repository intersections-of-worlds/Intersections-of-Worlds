using Unity.Mathematics;
namespace GameCore
{
    public struct RandomSeed
    {
        private uint seed;
        public static implicit operator uint(RandomSeed seed)
        {
            return seed.seed;
        }
        public Random GetRandom()
        {
            return new Random(seed);
        }
        public void Add(uint hash)
        {
            seed = (uint)new System.ValueTuple<uint, uint>(seed, hash).GetHashCode();
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