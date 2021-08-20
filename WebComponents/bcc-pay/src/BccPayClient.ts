import { requestHeaders } from './BccPay';

export async function getPaymentConfigurations(
  country: string,
  server: string
) {
  let possibleConfigurations: string = '';

  const fetchHeaders = new Headers();
  fetchHeaders.append('Content-Type', 'application/json');
  if (requestHeaders) {
    requestHeaders.forEach(requestHeaderObject => {
      fetchHeaders.append(requestHeaderObject.key, requestHeaderObject.value);
    });
  }

  try {
    disablePayments();
    const url = `${server}/payment-configurations?countryCode=${country}`;
    await fetch(url, {
      method: 'GET',
      headers: fetchHeaders,
    })
      .then(response => response.json())
      .then(json => {
        possibleConfigurations = json;
        console.log(
          'Possible configurations for country ' +
            country +
            ' : ' +
            JSON.stringify(possibleConfigurations)
        );
      });

    const possibleConfigurationsObject = JSON.parse(
      JSON.stringify(possibleConfigurations)
    );
    possibleConfigurationsObject.forEach(element => {
      enablePossiblePayments(element.provider);
    });
    return possibleConfigurations;
  } catch (e) {
    console.log('getPaymentConfigurations exception: ' + e);

    return '';
  }
}

export function enablePossiblePayments(provider: string) {
  const paymentButton = document.getElementById(provider) as HTMLButtonElement;
  if (paymentButton) {
    paymentButton.disabled = false;
  }
}

export function disablePayments() {
  const netsButton = document.getElementById('Nets') as HTMLButtonElement;
  netsButton.disabled = true;

  const mollyButton = document.getElementById('Mollie') as HTMLButtonElement;
  mollyButton.disabled = true;
}

export async function initPayment(
  currency: string,
  country: string,
  amount: number,
  server: string
): Promise<string> {
  const body = {
    payerId: '123',
    currencyCode: currency,
    countryCode: country,
    amount: amount,
    description: 'Test description',
  };
  let paymentId: string = '';

  const fetchHeaders = new Headers();
  fetchHeaders.append('Content-Type', 'application/json');
  if (requestHeaders) {
    requestHeaders.forEach(requestHeaderObject => {
      fetchHeaders.append(requestHeaderObject.key, requestHeaderObject.value);
    });
  }

  try {
    disablePayments();
    await fetch(`${server}/Payment`, {
      method: 'POST',
      body: JSON.stringify(body),
      headers: fetchHeaders,
    })
      .then(response => response.json())
      .then(json => {
        paymentId = json.paymentId;
      });

    return paymentId;
  } catch (e) {
    return '';
  }
}
