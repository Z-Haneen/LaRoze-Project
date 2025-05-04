// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function addToCart(productName, price) {
    // Assume you have a way to get the productId, e.g., via data attributes or a mapping
    // For demo, hardcode a productId or fetch it dynamically
    var productId = 1; // Replace with actual productId
    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        data: { productId: productId, quantity: 1 },
        success: function (response) {
            if (response.success) {
                alert(response.message);
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert('Error adding to cart.');
        }
    });
}