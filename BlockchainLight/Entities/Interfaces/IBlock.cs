using System;

namespace BlockchainLight.Entities.Interfaces
{
    public interface IBlock
    {
        public int Index { get; }

        public string PreviousHash { get; set; }

        public string Hash { get; set; }

        public string Data { get; set; }

        public DateTime TimeStamp { get; set; }

        public int Nonce { get; set; }
        
        public string CalculateHash();

        public void ChangeNonce();
    }
}
