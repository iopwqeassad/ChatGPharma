// Function to hide the preloader
function hidePreloader() {
  document.body.classList.add("preloader-deactive");
}

window.addEventListener('load', function () {
  setTimeout(hidePreloader, 1000);
});


// Scroll to the top of the document when the button is clicked
document.querySelector('.scroll-top').addEventListener('click', function () {
  window.scrollTo({
    top: 0,
    behavior: 'smooth'
  });
});


// Initialize Bootstrap Tooltip
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
  return new bootstrap.Tooltip(tooltipTriggerEl)
})




// Product Details Page

document.querySelectorAll('.thumbnail').forEach(thumbnail => {
  thumbnail.addEventListener('click', function () {
    const src = this.src;
    document.getElementById('modalImage').src = src;
  });
});

// Product Details increase count

function incrementQuantity() {
  let quantityInput = document.getElementById('quantity');
  quantityInput.value = parseInt(quantityInput.value) + 1;
}

function decrementQuantity() {
  let quantityInput = document.getElementById('quantity');
  if (parseInt(quantityInput.value) > 1) {
    quantityInput.value = parseInt(quantityInput.value) - 1;
  }
}

// Prevent Right Click
// Prevent right-click context menu
//     document.addEventListener('contextmenu', function(e) {
//       e.preventDefault();
//   });

// Prevent F12 key and other keyboard shortcuts
//   document.onkeydown = function(e) {
//       if(e.keyCode == 123) {
//           return false;
//       }
//       if(e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)) {
//           return false;
//       }
//       if(e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)) {
//           return false;
//       }
//       if(e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)) {
//           return false;
//       }
//   };



//   Scroll To Top Code

window.onscroll = function () {
  const scrollTopButton = document.querySelector('.scroll-top');
  if (document.body.scrollTop > 100 || document.documentElement.scrollTop > 100) {
    scrollTopButton.style.display = "block";
  } else {
    scrollTopButton.style.display = "none";
  }
};




// ------------------------------------------------------------
//document.addEventListener("DOMContentLoaded", function () {
//  const cartContainer = document.querySelector('.list-cart-products');
//  const cartSummary = document.querySelector('.cart-summary');
//  const cartActions = document.querySelector('.cart-actions');
//  const emptyCartMessage = document.querySelector('.empty-cart-message');
//  const totalAmountElement = document.getElementById('total-amount');
//  const checkoutBtn = document.getElementById('checkout-btn');

//  function updateCartBadge() {
//    const cart = JSON.parse(localStorage.getItem('cart')) || [];
//    const badge = document.querySelector('.badge');
//    badge.textContent = cart.length; 
//  }

//  function addToCart(button) {
//      const productDiv = event.target.closest('.holding');
//    const productId = button.getAttribute('data-product-id');
//    const productImage = productDiv.querySelector('.card-img-top').getAttribute('src');
//    const productName = productDiv.querySelector('.card-title').textContent.trim();
//    const productPriceText = productDiv.querySelector('.card-price').textContent.trim();
//    const productPrice = parseFloat(productPriceText.replace(/[^0-9.-]+/g, ""));

//    let cart = JSON.parse(localStorage.getItem('cart')) || [];

//    // Check if product is already in the cart
//    const existingProductIndex = cart.findIndex(item => item.id === productId);
//    if (existingProductIndex > -1) {
//      cart[existingProductIndex].quantity += 1;
//    } else {
//      cart.push({ id: productId, image: productImage, name: productName, price: productPrice, quantity: 1 });
//    }

//    localStorage.setItem('cart', JSON.stringify(cart));
//    updateCartBadge(); // This will only change if a new unique item is added
//  }

//  function renderCart() {
//    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    
//    if (cart.length === 0) {
//      cartContainer.style.display = 'none';
//      cartSummary.style.display = 'none';
//      cartActions.style.display = 'none';
//      emptyCartMessage.style.display = 'block';
//    } else {
//      cartContainer.style.display = 'block';
//      cartSummary.style.display = 'block';
//      cartActions.classList.remove('d-none');  

//      emptyCartMessage.style.display = 'none';
      
//      let totalAmount = 0;

//      cart.forEach(item => {
//        totalAmount += item.price * item.quantity;
       
//      });

//      totalAmountElement.textContent = `${totalAmount.toFixed(2)} جنيه`;
//    }
//    updateCartBadge();
//  }

//  function updateCart(productId, newQuantity) {
//    let cart = JSON.parse(localStorage.getItem('cart')) || [];
//    const productIndex = cart.findIndex(item => item.id === productId);

//    if (productIndex > -1) {
//      if (newQuantity > 0) {
//        cart[productIndex].quantity = newQuantity;
//      } else {
//        cart.splice(productIndex, 1);
//      }
//      localStorage.setItem('cart', JSON.stringify(cart));
//      renderCart();
//    }
    
//  }

//  // Event delegation for Add to Cart buttons on the shop page
//  document.addEventListener('click', function (event) {
//    if (event.target.classList.contains('Add-tocart')) {
//      addToCart(event.target);
//    }
//  });

//  // Event delegation for cart actions on the shopping cart page
//  if (cartContainer) {
//    cartContainer.addEventListener('click', function(event) {
//      const productElement = event.target.closest('.shopping-product');
//      if (!productElement) return;

//      const productId = productElement.dataset.productId;
//      const quantityInput = productElement.querySelector('.quantity-input');
//      let quantity = parseInt(quantityInput.value);

//      if (event.target.classList.contains('increment-btn')) {
//        updateCart(productId, quantity + 1);
//      } else if (event.target.classList.contains('decrement-btn')) {
//        updateCart(productId, quantity - 1);
//      } else if (event.target.closest('.delete-btn')) {
//        updateCart(productId, 0);
//      }
//    });
//  }



//  // Initial render
//  updateCartBadge();
//  if (cartContainer) {
//    renderCart();
//  }
//});


 //localStorage.clear();
