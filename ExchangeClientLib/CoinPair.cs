namespace ExchangeClientLib
{
    public class CoinPair
    {
        public string Pair1 { get; set; }
        public string Pair2 { get; set; }
        public int Precision { get; private set; }

        public CoinPair(string pair1, string pair2, int precision)
        {
            Pair1 = pair1;
            Pair2 = pair2;
            Precision = precision;
        }

        public override string ToString()
        {
            return Pair1 + Pair2;
        }
    }
}