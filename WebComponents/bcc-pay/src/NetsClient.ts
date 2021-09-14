import { User } from './User';
import { country, isDevEnv, requestHeaders } from './BccPay';
import {
  displayErrorPage,
  paymentCompleted,
  displayNetsPayment,
} from './ScreenChange';

var checkout: any;

export async function startNetsPayment(
  paymentId: string,
  user: User,
  server: string,
  checkoutKey: string,
  paymentConfigurationId: string
): Promise<boolean> {
  const netsButton = document.getElementById('Nets');
  if (netsButton) {
    netsButton!.classList.add('payment-button--loading');
  }
  const netsPaymentId = await initNetsPayment(
    paymentId,
    user,
    server,
    paymentConfigurationId
  );
  if (netsButton) {
    netsButton!.classList.remove('payment-button--loading');
  }
  displayNetsPayment();

  if (isDevEnv === true) {
    console.log('Nets payment id is: ' + netsPaymentId);
  }

  if (
    netsPaymentId === null ||
    netsPaymentId === undefined ||
    netsPaymentId === ''
  ) {
    displayErrorPage();
    return false;
  }
  return await processNetsPayment(netsPaymentId, checkoutKey);
}

export async function initNetsPayment(
  paymentId: string,
  user: User,
  server: string,
  paymentConfigurationId: string
): Promise<string> {
  console.log('Nets payment country: ' + country);
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

  let netsPaymentId: string = '';
  try {
    await fetch(`${server}/Payment/${paymentId}/attempts`, {
      method: 'POST',
      body: JSON.stringify(body),
      headers: fetchHeaders,
    })
      .then(response => response.json())
      .then(json => {
        netsPaymentId = json.paymentCheckoutId;
      });
  } catch (e) {
    displayErrorPage();
  }
  return netsPaymentId;
}

export async function processNetsPayment(
  paymentId: string,
  checkoutKey: string
): Promise<boolean> {
  if (paymentId) {
    const checkoutOptions = {
      checkoutKey: checkoutKey,
      paymentId: paymentId,
      containerId: 'checkout-container-div',
    };

    // @ts-ignore
    checkout = new Dibs.Checkout(checkoutOptions);
    await checkout.on('payment-completed', function (response: any) {
      if (isDevEnv === true) {
        console.log('Completed!' + JSON.stringify(response));
      }
      paymentCompleted();
      return true;
    });

    if (isDevEnv === true) {
      console.log('Not completed!');
    }
    return false;
  } else {
    if (isDevEnv === true) {
      console.log('Expected a paymentId');
    }
    return false;
  }
}

export async function cleanupNetsPayment() {
  checkout.cleanup();
}
