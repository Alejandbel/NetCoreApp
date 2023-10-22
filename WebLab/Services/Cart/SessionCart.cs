// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;
using WebLab.Domain.Entities;
using WebLab.Extensions;

namespace WebLab.Services.Cart
{
    public class SessionCart : Domain.Cart
    {
        [JsonIgnore]
        private ISession? _session;

        public static Domain.Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>().HttpContext?.Session;
            SessionCart cart = session?.Get<SessionCart>("Cart") ?? new SessionCart();
            cart._session = session;
            return cart;
        }

        public override void AddToCart(Beer beer)
        {
            base.AddToCart(beer);
            _session.Set("Cart", this);
        }

        public override void RemoveItems(int id)
        {
            base.RemoveItems(id);
            _session.Set("Cart", this);
        }

        public override void ClearAll()
        {
            base.ClearAll();
            _session.Remove("Cart");
        }

    }
}
