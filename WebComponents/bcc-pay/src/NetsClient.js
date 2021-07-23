function initPayment() {}

function handlePayment(paymentId) {
  const urlParams = new URLSearchParams(window.location.search);
  const paymentId = urlParams.get('paymentId');
  if (paymentId) {
    const checkoutOptions = {
      checkoutKey: '<YOUR_CHECKOUT_KEY>', // Replace!
      paymentId: paymentId,
      containerId: 'checkout-container-div',
    };
    const checkout = new Dibs.Checkout(checkoutOptions);
    checkout.on('payment-completed', function (response) {
      window.location = 'completed.html';
    });
  } else {
    console.log('Expected a paymentId'); // No paymentId provided,
    window.location = 'cart.html'; // go back to cart.html
  }
}
