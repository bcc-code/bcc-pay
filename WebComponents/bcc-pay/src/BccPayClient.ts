import { requestHeaders } from './BccPay';

let providerDefinitionIdList = {};

export async function getPaymentConfigurations(
  country: string,
  server: string,
  currency: string,
  paymentType: string
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
    const url = `${server}/payment-configurations?countryCode=${country}&currencyCode=${currency}&paymentType=${paymentType}`;
    await fetch(url, {
      method: 'GET',
      headers: fetchHeaders,
    })
      .then(response => response.json())
      .then(json => {
        possibleConfigurations = json;
      });

    const paymentConfigurationsObject = JSON.parse(
      JSON.stringify(possibleConfigurations)
    );

    console.log('PossibleConfigurations country: ' + country);
    console.log(paymentConfigurationsObject);
    paymentConfigurationsObject.paymentConfigurations.forEach(element => {
      element.providerDefinitionDetails.forEach(element => {
        enablePossiblePayments(element.paymentMethod);
        providerDefinitionIdList[element.paymentMethod] = element.id;
      });
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
  const creditCardButton = document.getElementById(
    'CreditCardOrVipps'
  ) as HTMLButtonElement;
  if (creditCardButton) {
    creditCardButton.disabled = true;
  }

  const giropayButton = document.getElementById('Giropay') as HTMLButtonElement;
  if (giropayButton) {
    giropayButton.disabled = true;
  }

  const idealButton = document.getElementById('iDeal') as HTMLButtonElement;
  if (idealButton) {
    idealButton.disabled = true;
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

export function getPaymentConfigutraionIdForPaymentMethod(
  paymentMethod: string
): string {
  console.log('Getting payment config id for method: ' + paymentMethod);
  return providerDefinitionIdList[paymentMethod];
}
