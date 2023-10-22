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
    public class CartItem
    {
        public Beer Beer { get; }
        public int Amount { get; private set; }

        public decimal TotalPrice { get => Amount * Beer.Price; }
        public void IncrementAmount()
        {
            Amount += 1;
        }

        public CartItem(Beer beer, int amount = 1)
        {
            Beer = beer;
            Amount = amount;
        }
    }
}
