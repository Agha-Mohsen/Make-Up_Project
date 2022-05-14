using System;
using System.Collections.Generic;
using System.Linq;
using _01_MakeUpQuery.Contracts.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nancy.Json;
using ShopManagement.Application.Contracts.Order;

namespace ServiceHost.Pages
{
    public class CartModel : PageModel
    {
        public List<CartItem> CartItems;
        public const string CookieName = "cart-items";
        private readonly IProductQuery _productQuery;

        public CartModel(IProductQuery productQuery)
        {
            CartItems = new List<CartItem>();
            _productQuery = productQuery;
        }

        public void OnGet()
        {
            var serialize = new JavaScriptSerializer();
            var value = Request.Cookies[CookieName];
            var cartItems = serialize.Deserialize<List<CartItem>>(value);
            if (cartItems != null)
                foreach (var item in cartItems)
                    item.CalculateTotalItemPrice();
            else
            {
                cartItems = new List<CartItem>();
            }

            CartItems = _productQuery.CheckInventoryStatus(cartItems);
        }

        public IActionResult OnGetRemoveFromCart(long id)
        {
            var serialize = new JavaScriptSerializer();
            var value = Request.Cookies[CookieName];
            Response.Cookies.Delete(CookieName);
            var cartItems = serialize.Deserialize<List<CartItem>>(value);
            var itemToRemove = cartItems.FirstOrDefault(x => x.Id == id);
            cartItems.Remove(itemToRemove);
            var options = new CookieOptions { Expires = DateTime.Now.AddDays(2) };
            Response.Cookies.Append(CookieName, serialize.Serialize(cartItems), options);
            return RedirectToPage("/Cart");
        }

        public IActionResult OnGetGoToCheckOut()
        {
            var serialize = new JavaScriptSerializer();
            var value = Request.Cookies[CookieName];
            var cartItems = serialize.Deserialize<List<CartItem>>(value);
            if (cartItems != null)
                foreach (var item in cartItems)
                    item.CalculateTotalItemPrice();
            else
            {
                cartItems = new List<CartItem>();
            }

            CartItems = _productQuery.CheckInventoryStatus(cartItems);

            if (cartItems.Any(x => !x.IsInStock))
                return RedirectToPage("/Cart");

            return RedirectToPage("/Checkout");
        }
    }
}