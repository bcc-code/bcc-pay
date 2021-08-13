import { User } from './User';
import { isDevEnv, paymentConfigurationId, requestHeaders } from './BccPay';
import { displayErrorPage, paymentCompleted } from './ScreenChange';

var checkout: any;

export async function startMolliePayment(
  paymentId: string,
  user: User,
  server: string,
  checkoutKey: string
): Promise<string> {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'none';

  const changeUserDataScreenElement = document.getElementById(
    'change-user-data-screen'
  ) as HTMLElement;
  changeUserDataScreenElement.style.display = 'none';

  const netsScreenElement = document.getElementById(
    'mollie-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'block';

  const mollieCheckoutUrl = await initMolliePayment(paymentId, user, server);
  if (isDevEnv === true) {
    console.log('Mollie checkoutUrl is: ' + mollieCheckoutUrl);
  }

  if (mollieCheckoutUrl === null || mollieCheckoutUrl === '') {
    displayErrorPage();
    return '';
  }

  return mollieCheckoutUrl;
}

export async function initMolliePayment(
  paymentId: string,
  user: User,
  server: string
): Promise<string> {
  const body = {
    paymentConfigurationId: 'mollie-giropay-eur',
    email: user.email === null ? undefined : user.email,
    phoneNumber: user.phoneNumber === null ? undefined : user.phoneNumber,
    firstName: user.firstName === null ? undefined : user.firstName,
    lastName: user.lastName === null ? undefined : user.lastName,
    addressLine1: user.addressLine1 === null ? undefined : user.addressLine1,
    addressLine2: user.addressLine2 === null ? undefined : user.addressLine2,
    city: user.city === null ? undefined : user.city,
    postalCode: user.postalCode === null ? undefined : user.postalCode,
  };

  const fetchHeaders = new Headers();
  fetchHeaders.append('Content-Type', 'application/json');
  if (requestHeaders) {
    requestHeaders.forEach(requestHeaderObject => {
      fetchHeaders.append(requestHeaderObject.key, requestHeaderObject.value);
    });
  }

  let mollieCheckoutUrl: string = '';
  try {
    await fetch(`${server}/Payment/${paymentId}/attempts`, {
      method: 'POST',
      body: JSON.stringify(body),
      headers: fetchHeaders,
    })
      .then(response => response.json())
      .then(json => {
        mollieCheckoutUrl = json.checkoutUrl;
      });
  } catch (e) {
    displayErrorPage();
  }
  return mollieCheckoutUrl;
}

export async function cleanupNetsPayment() {
  checkout.cleanup();
}
