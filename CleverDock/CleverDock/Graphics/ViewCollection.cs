using CleverDock.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Graphics
{
    public class ViewCollection : IList<View>
    {
        private List<View> list;
        private View parent;

        public event EventHandler<ViewEventArgs> Added;
        public event EventHandler<ViewEventArgs> Removed;

        public ViewCollection(View parent)
        {
            list = new List<View>();
            this.parent = parent;
        }

        private void SetSuperView(View view)
        {
            view.Superview = parent;
            view.Scene = parent.Scene;
        }

        public void Add(View view)
        {
            list.Add(view);
            SetSuperView(view);
            if (Added != null)
                Added(this, new ViewEventArgs(view));
        }

        public bool Remove(View view)
        {
            var result = list.Remove(view);
            view.Superview = null;
            view.Scene = null;
            if (Removed != null)
                Removed(this, new ViewEventArgs(view));
            return result;
        }

        public int IndexOf(View view)
        {
            return list.IndexOf(view);
        }

        public void Insert(int index, View view)
        {
            list.Insert(index, view);
            SetSuperView(view);
            if (Added != null)
                Added(this, new ViewEventArgs(view));
        }

        public void RemoveAt(int index)
        {
            var item = list[index];
            Remove(item);
        }

        public View this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public void Clear()
        {
            foreach(var item in this)
                Remove(item);
        }

        public bool Contains(View item)
        {
            return list.Contains(item);
        }

        public void CopyTo(View[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count;  }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<View> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
