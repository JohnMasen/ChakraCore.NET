using System.Collections.Generic;

namespace Chakra.NET.GC
{
    public class DelegateHolderManager
    {
        private List<DelegateHolder> holders = new List<DelegateHolder>();
        public DelegateHolder CreateHolder(bool allowInternalRelease, bool allowExternalRelease)
        {
            DelegateHolder result = new DelegateHolder(this, allowInternalRelease, allowExternalRelease);
            holders.Add(result);
            return result;
        }

        internal void ReleaseHolder(DelegateHolder holder)
        {
            holders.Remove(holder);
        }


    }
}
