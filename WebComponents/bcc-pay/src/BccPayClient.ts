import { requestHeaders } from './BccPay';

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
