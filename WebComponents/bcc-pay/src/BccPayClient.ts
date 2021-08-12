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
    const url = `${server}/payment-configurations?countryCode=${country}`;
    await fetch(url, {
      method: 'GET',
      headers: fetchHeaders,
    })
      .then(response => response.json())
      .then(json => {
        possibleConfigurations = json;
        console.log(
          'Possible configurations: ' + JSON.stringify(possibleConfigurations)
        );
      });
    return possibleConfigurations;
  } catch (e) {
    console.log('getPaymentConfigurations exception: ' + e);

    return '';
  }
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
