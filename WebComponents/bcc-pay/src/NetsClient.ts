import { NetsUser } from './NetsUser';
import { User } from './User';

var checkout: any;

export async function startNetsPayment(
  paymentId: string,
  user: User,
  server: string
): Promise<boolean> {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'none';

  const changeUserDataScreenElement = document.getElementById(
    'change-user-data-screen'
  ) as HTMLElement;
  changeUserDataScreenElement.style.display = 'none';

  const netsScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'block';

  const netsPaymentId = await initNetsPayment(paymentId, user, server);
  console.log('Nets payment id is: ' + netsPaymentId);

  return await processNetsPayment(netsPaymentId);
}

export async function initNetsPayment(
  paymentId: string,
  user: User,
  server: string
): Promise<string> {
  console.log('User in init nets payment: ' + JSON.stringify(user));

  const body: NetsUser = {
    paymentMethod: 'NetsCreditCard',
    email: user.email,
    phoneNumber: user.phoneNumber,
    firstName: user.firstName,
    lastName: user.lastName,
    addressLine1: user.addressLine1,
    addressLine2: user.addressLine2,
    city: user.city,
    postalCode: user.postalCode,
  };

  let netsPaymentId: string = '';
  await fetch(`https://${server}/Payment/${paymentId}/attempts`, {
    method: 'POST',
    body: JSON.stringify(body),
    headers: {
      'Content-Type': 'application/json',
    },
  })
    .then(response => response.json())
    .then(json => {
      netsPaymentId = json.paymentCheckoutId;
    });
  return netsPaymentId;
}

export async function processNetsPayment(paymentId: string): Promise<boolean> {
  if (paymentId) {
    const checkoutOptions = {
      checkoutKey: '#checkout-key#',
      paymentId: paymentId,
      containerId: 'checkout-container-div',
    };

    // @ts-ignore
    checkout = new Dibs.Checkout(checkoutOptions);
    await checkout.on('payment-completed', function (response: any) {
      console.log('Completed!' + JSON.stringify(response));
      return true;
    });

    console.log('Not completed!');
    return false;
  } else {
    console.log('Expected a paymentId');
    return false;
  }
}

export async function cleanupNetsPayment() {
  checkout.cleanup();
}
