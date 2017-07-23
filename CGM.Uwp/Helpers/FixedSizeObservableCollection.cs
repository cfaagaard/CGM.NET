using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Uwp.Helpers
{
    public class FixedSizeObservableCollection<T> : ObservableCollection<T>
    {
        private readonly int maxSize;
        private readonly bool reversed;
        public FixedSizeObservableCollection(int maxSize) : this(maxSize, false)
        {

        }
        public FixedSizeObservableCollection(int maxSize, bool reversed)
        {
            this.maxSize = maxSize;
            this.reversed = reversed;
        }
        protected override void InsertItem(int index, T item)
        {
            if (reversed)
            {
                if (Count == maxSize)
                {
                    base.RemoveAt(Count - 1);
                    //index = 0;
                }
            }
            else
            {
                if (Count == maxSize)
                {
                    base.RemoveAt(0);
                    index -= 1;
                }
            }


            base.InsertItem(index, item);
        }
    }
}
