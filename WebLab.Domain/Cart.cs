// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebLab.Domain.Entities;

namespace WebLab.Domain
{
    public class Cart
    {
        public Dictionary<int, CartItem> CartItems { get; set; } = new();
        public virtual void AddToCart(Beer beer)
        {
            if (CartItems.ContainsKey(beer.Id))
            {
                CartItems[beer.Id].IncrementAmount();
            } else
            {
                CartItems.Add(beer.Id, new CartItem(beer));
            }
        }
        public virtual void RemoveItems(int id)
        {
            CartItems.Remove(id);
        }
        public virtual void ClearAll()
        {
            CartItems.Clear();
        }
        public int Count { get => CartItems.Sum(item => item.Value.Amount); }
        public decimal TotalPrice
        {
            get => CartItems.Sum(item => item.Value.TotalPrice);
        }
    }
}
