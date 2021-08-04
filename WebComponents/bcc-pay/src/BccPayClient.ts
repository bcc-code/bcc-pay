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
  try {
    await fetch(`${server}/Payment`, {
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
  } catch (e) {
    return '';
  }
}