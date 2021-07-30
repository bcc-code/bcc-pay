export async function initNetsPayment(paymentId: string): Promise<string> {
  const body = {
    paymentMethod: 0,
    email: 'test@test.no',
    phoneNumber: '+47661626839',
    firstName: 'TestName',
    lastName: 'TestLastName',
    addressLine1: 'TestAddressLine1',
    addressLine2: 'TestAddressLine2',
    city: 'Oslo',
    postalCode: '0001',
  };

  let netsPaymentId: string = '';
  const res = await fetch(
    `https://localhost:5001/Payment/${paymentId}/attempts`,
    {
      method: 'POST',
      body: JSON.stringify(body),
      headers: {
        'Content-Type': 'application/json',
      },
    }
  )
    .then(response => response.json())
    .then(json => {
      console.log('parsed json', json);
      netsPaymentId = json.paymentCheckoutId;
    });
  return netsPaymentId;
}

export async function startNetsPayment(paymentId: string) {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'none';

  const netsScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'block';

  const netsPaymentId = await initNetsPayment(paymentId);
  console.log('Nets payment id is: ' + netsPaymentId);

  processNetsPayment(netsPaymentId);
}

export async function processNetsPayment(paymentId: string) {
  if (paymentId) {
    const checkoutOptions = {
      checkoutKey: '#checkout-key#',
      paymentId: paymentId,
      containerId: 'checkout-container-div',
    };

    // @ts-ignore
    const checkout = new Dibs.Checkout(checkoutOptions);
    checkout.on('payment-completed', function (response: any) {
      // window.location = 'completed.html';
      console.log('Completed!' + response);
    });
  } else {
    console.log('Expected a paymentId'); // No paymentId provided,
    // window.location = 'cart.html'; // go back to cart.html
    console.log('Not completed!');
  }
}
