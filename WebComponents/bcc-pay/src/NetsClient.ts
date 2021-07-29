export async function initNetsPayment(): Promise<string> {
  const body = {
    payerId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    currency: 'NOK',
    country: 'NOR',
    amount: 1000,
    paymentMethod: 'CreditCard',
    payer: {
      email: 'test@test.no',
      phoneNumber: '+47661626839',
      firstName: 'TestName',
      lastName: 'TestLastName',
      addressLine1: 'TestAddressLine1',
      addressLine2: 'TestAddressLine2',
      city: 'Oslo',
      postalCode: '0001',
    },
  };

  let paymentId: string = '';
  const res = await fetch('https://localhost:5001/Payment', {
    method: 'POST',
    body: JSON.stringify(body),
    headers: {
      'Content-Type': 'application/json',
    },
  })
    .then(response => response.json())
    .then(json => {
      console.log('parsed json', json);
      paymentId = json.paymentId;
    });
  return paymentId;
}

export async function startNetsPayment() {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'none';

  const netsScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'block';

  const paymentId = await initNetsPayment();
  console.log('Payment id is: ' + paymentId);

  processNetsPayment(paymentId);
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
