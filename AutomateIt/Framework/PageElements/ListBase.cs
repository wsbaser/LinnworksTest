using System;
using System.Collections.Generic;
using System.Linq;
using automateit.SCSS;
using Natu.Utils.Extensions;
using NUnit.Framework;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public abstract class ListBase<T> : ContainerBase, IWebList<T> where T : IItem {
        protected virtual bool CacheItems => false;

        public ListBase(IPage parent, string rootScss = null)
            : base(parent, rootScss)
        {
        }

        #region IWebList<T> Members

        public abstract string ItemIdScss { get; }

        public virtual Selector ItemIdSelector => new Selector(ItemIdScss,FrameScss);

        public virtual List<string> GetIds() => Get.TextsFast(ItemIdScss, FrameBy).Select(t => t.Trim()).ToList();

        private List<T> _items;
        private Selector _selector;

        public void InvalidateItems() {
            _items = null;
        }

        public virtual List<T> GetDummyItems() {
            // TODO: Add support for selectors based on ID's containing single quote('). Currently we filter ID's that contains ' because can't build selectors using that ID's.
            var ids = GetIds().Where(id => !id.Contains("'"));
            return WebPageBuilder.CreateDummyItems<T>(this, ids);
        }

        public virtual int GetCount() => GetIds().Count;

        #endregion

        public override bool IsVisible() => Is.Visible(RootSelector, FrameBy);

        public virtual T FindRandom(Func<T, bool> filter = null)
        {
            var ids = GetIds();
            ids.Shuffle();
            if (ids.Count > 0)
            {
                if (filter == null)
                {
                    return GetItem(ids.First());
                }

                foreach (var id in ids)
                {
                    var item = GetItem(id);
                    if (filter.Invoke(item))
                    {
                        return item;
                    }
                }
            }

            return default(T);
        }

        public T FindSingle(Func<T, bool> filter)
        {
            var list = GetItems();
            return list.SingleOrDefault(filter);
        }

        public T FindFirstOrDefault(Func<T, bool> filter) {
            var list = GetItems();
            return list.FirstOrDefault(filter);
        }

        protected List<T> FindAll(Func<T, bool> filter)
		{
			var list = GetItems();
			return list.Where(filter).ToList();
		}

        public T RandomItem(Func<string, bool> excludeFunc) {
            var ids = GetIds();
            if (ids.Count > 0) {
                return GetItem(ids.RandomItem(excludeFunc));
            }
            return default(T);
        }

        public T RandomItem(List<string> excludeList=null) {
            // We can't build a selector with id, containing '
            excludeList = excludeList ?? new List<string>();
            var ids = GetIds().Where(id => !id.Contains('\'')).ToList();
            if (ids.Count > 0) {
                return GetItem(ids.RandomItem(excludeList));
            }
            return default(T);
        }

        public override bool IsEmpty() => GetCount() == 0;

        public virtual List<T> GetItems() {
            if (_items == null
                || !CacheItems) {
                // TODO: Add support for selectors based on ID's containing single quote('). Currently we filter ID's that contains ' because can't build selectors using that ID's.
                var ids = GetIds().Where(id => !id.Contains("'"));
                _items = WebPageBuilder.CreateItems<T>(this, ids);
            }
            return _items;
        }

        public T1 GetItem<T1>(string id)
            where T1 : IItem {
            object item = WebPageBuilder.CreateComponent<T>(this, id);
            return (T1)item;
        }

        public List<T1> GetItems<T1>()
            where T1 : IItem => GetItems().Cast<T1>().ToList();
        
        public virtual T GetItem(string id) => WebPageBuilder.CreateComponent<T>(this, id);

        public override void AssertIsNotEmpty() => CollectionAssert.IsNotEmpty(GetIds(), $"'{ComponentName}' is not empty");

        public override void AssertIsEmpty() => CollectionAssert.IsEmpty(GetIds(), $"'{ComponentName}' is empty");

		public void AssertListIsEquivalent(IList<string> list) => CollectionAssert.AreEquivalent(list, GetIds(), $"'{ComponentName}' contains incorrect items list.");

	    public virtual void AssertContainsIgnoringCase(params string[] expectedSubset) => AssertContainsIgnoringCase(expectedSubset.ToList());

        public virtual void AssertContainsIgnoringCase(List<string> expectedSubset)
        {
            expectedSubset = expectedSubset.Select(v => v.Trim().ToLower()).ToList();
            CollectionAssert.IsSubsetOf(
                expectedSubset, GetIds().Select(id => id.Trim().Replace(" > ", ">").ToLower()),
                "List does not contain expected values.");
        }
    }

}
