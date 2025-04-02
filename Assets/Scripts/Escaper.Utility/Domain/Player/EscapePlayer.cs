#nullable enable

using System;
using System.Collections.Generic;
using Escaper.Utility.Domain.Item;
using Escaper.Utility.Domain.Scene;

namespace Escaper.Utility.Domain.Player
{
    public class EscapePlayer
    {
        public EscapePlayer(
            EscapeScene lookAt,
            List<EscapeItem> items
        )
        {
            LookAt = lookAt;
            Items = new List<EscapeItem>(items);
            Items.AddRange(items);
        }

        public List<EscapeItem> Items { get; }
        public EscapeScene LookAt { get; }
        public int? HoldItemIndex;
        public EscapeItem? CurrentItem
        {
            get
            {
                if (HoldItemIndex == null)
                {
                    return null;
                }
                return Items[HoldItemIndex ?? 0];
            }
        }

        public void GetItem(EscapeItem item)
        {
            Items.Add(item);
        }

        public void HoldItem(int index)
        {
            if (index < 0 || index > Items.Count - 1)
            {
                throw new Exception("cannot hold item");
            }
            HoldItemIndex = index;
        }

        public void ReleaseItem()
        {
            HoldItemIndex = null;
        }

        public void UseItem(Action<EscapeItem> act, bool delete)
        {
            if (CurrentItem == null)
            {
                return;
            }
            act(CurrentItem);
            if (delete)
            {
                Items.Remove(CurrentItem);
                HoldItemIndex = null;
            }
        }
    }
}