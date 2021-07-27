// import axios from 'axios';

export async function initPayment() {
  const body = {
    payerId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    currency: 'NOK',
    country: 'NOR',
    amount: 1000,
    paymentMethod: 'CreditCard',
    privatePerson: {
      email: 'test@test.no',
      phoneNumberPrefix: '+47',
      phoneNumberBody: '661626839',
      firstName: 'TestName',
      lastName: 'TestLastName',
      addressLine1: 'TestAddressLine1',
      addressLine2: 'TestAddressLine2',
      city: 'Oslo',
      postalCode: '0001',
    },
  };

  const response = await axios.post(`https://localhost:5001/Payment`, body);
  console.log('Response is: ' + JSON.stringify(response));
}

export function handlePayment(paymentId) {
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

export async function initNetsPayment() {
  const body = {
    payerId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    currency: 'NOK',
    country: 'NOR',
    amount: 1000,
    paymentMethod: 'CreditCard',
    privatePerson: {
      email: 'test@test.no',
      phoneNumberPrefix: '+47',
      phoneNumberBody: '661626839',
      firstName: 'TestName',
      lastName: 'TestLastName',
      addressLine1: 'TestAddressLine1',
      addressLine2: 'TestAddressLine2',
      city: 'Oslo',
      postalCode: '0001',
    },
  };

  var options = new RequestOptions();

  options.headers = new Headers();
  options.headers.append('Content-Type', 'application/json');

  let res = await fetch({
    method: 'POST',
    body: JSON.stringify(body),
  });

  var request = new XMLHttpRequest();

  const response = await axios.post(`https://localhost:5001/Payment`, body);
  console.log('Response is: ' + JSON.stringify(response));
}
