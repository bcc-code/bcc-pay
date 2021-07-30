export async function initPayment(
  currency: string,
  country: string,
  amount: number
): Promise<string> {
  const body = {
    payerId: '123',
    currencyCode: 'NOK',
    countryCode: 'NOR',
    amount: 10,
  };
  let paymentId: string = '';
  await fetch('https://localhost:5001/Payment', {
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
