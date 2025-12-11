using FrontEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrontEnd.Services
{
    public class CartService
    {
        private List<Article> cartItems = new List<Article>();
        public event Action? OnChange;

        public void AddToCart(Article article)
        {
            cartItems.Add(article);
            NotifyStateChanged();
        }

        public void RemoveFromCart(Article article)
        {
            cartItems.Remove(article);
            NotifyStateChanged();
        }

        public List<Article> GetItems()
        {
            return cartItems;
        }

        public void ClearCart()
        {
            cartItems.Clear();
            NotifyStateChanged();
        }

        public float GetTotal()
        {
            return cartItems.Sum(item => item.Price);
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
