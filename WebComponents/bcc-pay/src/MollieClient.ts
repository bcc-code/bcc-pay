import { User } from './User';
import { country, isDevEnv, requestHeaders } from './BccPay';
import { displayErrorPage } from './ScreenChange';

export async function startMolliePayment(
  paymentId: string,
  user: User,
  server: string,
  paymentConfigurationId: string,
  buttonId: string
): Promise<string> {
  const mollieButton = document.getElementById(buttonId);
  mollieButton!.classList.add('payment-button--loading');

  const mollieCheckoutUrl = await initMolliePayment(
    paymentId,
    user,
    server,
    paymentConfigurationId
  );
  if (isDevEnv === true) {
    console.log('Mollie checkoutUrl is: ' + mollieCheckoutUrl);
  }

  if (
    mollieCheckoutUrl === null ||
    mollieCheckoutUrl === '' ||
    mollieCheckoutUrl === undefined
  ) {
    displayErrorPage();
    return '';
  }

  mollieButton!.classList.remove('payment-button--loading');
  // window.location.replace(mollieCheckoutUrl);
  // window.open(mollieCheckoutUrl, '_self');
  window.open(mollieCheckoutUrl, '_system');

  return mollieCheckoutUrl;
}

export async function initMolliePayment(
  paymentId: string,
  user: User,
  server: string,
  paymentConfigurationId: string
): Promise<string> {
  const body = {
    providerDefinitionId: paymentConfigurationId,
    email: user.email === null ? undefined : user.email,
    phoneNumber: user.phoneNumber === null ? undefined : user.phoneNumber,
    firstName: user.firstName === null ? undefined : user.firstName,
    lastName: user.lastName === null ? undefined : user.lastName,
    addressLine1: user.addressLine1 === null ? undefined : user.addressLine1,
    addressLine2: user.addressLine2 === null ? undefined : user.addressLine2,
    city: user.city === null ? undefined : user.city,
    postalCode: user.postalCode === null ? undefined : user.postalCode,
    countryCode: country,
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
  if (mollieCheckoutUrl === undefined) displayErrorPage();
  return mollieCheckoutUrl;
}
