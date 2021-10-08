import { requestHeaders, server, amount, isDevEnv } from './BccPay';

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
    disablePaymentButtons();
    startPaymentButtonsLoading();
    const url = `${server}/payment-configurations?countryCode=${country}&currencyCode=${currency}&paymentType=${paymentType}`;
    await fetch(url, {
      method: 'GET',
      headers: fetchHeaders,
    })
      .then(response => response.json())
      .then(json => {
        stopPaymentButtonsLoading();
        possibleConfigurations = json;
      });

    const paymentConfigurationsObject = JSON.parse(
      JSON.stringify(possibleConfigurations)
    );

    if (isDevEnv === true) {
      console.log('PossibleConfigurations country: ' + country);
      console.log(paymentConfigurationsObject);
    }
    paymentConfigurationsObject.paymentConfigurations.forEach(element => {
      element.providerDefinitionDetails.forEach(element => {
        enablePossiblePayment(
          element.paymentMethod,
          element.currency,
          element.id
        );
        providerDefinitionIdList[element.paymentMethod] = element.id;
      });
    });
    return possibleConfigurations;
  } catch (e) {
    console.log('getPaymentConfigurations exception: ' + e);
    return '';
  }
}

export async function enablePossiblePayment(
  paymentMethod: string,
  currency: string,
  paymentConfigurationId: string
) {
  const paymentButton = document.getElementById(
    paymentMethod
  ) as HTMLButtonElement;
  if (paymentButton) {
    paymentButton.disabled = false;
    if (currency !== 'NOK') {
      const exchangeResult = await calculateExchange(paymentConfigurationId);
      if (isDevEnv === true) {
        console.log('Exchange result is: ' + exchangeResult);
      }
      if (!paymentButton.textContent?.includes(exchangeResult)) {
        paymentButton.textContent += exchangeResult;
      }
    }
  }
}

export async function calculateExchange(
  paymentConfigurationId: string
): Promise<string> {
  let result = '';
  try {
    const fetchHeaders = new Headers();
    fetchHeaders.append('Content-Type', 'application/json');
    if (requestHeaders) {
      requestHeaders.forEach(requestHeaderObject => {
        fetchHeaders.append(requestHeaderObject.key, requestHeaderObject.value);
      });
    }

    const url = `${server}/payment-configurations/with-exchange?Amount=${amount}&FromCurrency=NOK&ToCurrency=EUR`;
    await fetch(url, {
      method: 'GET',
      headers: fetchHeaders,
    })
      .then(response => response.json())
      .then(json => {
        stopPaymentButtonsLoading();
        Array.from(json.providerDefinitionExchangeDefinition).forEach(
          (element: any) => {
            if (paymentConfigurationId === element.definitionId) {
              if (isDevEnv === true) {
                console.log('Calculate exchange result:');
                console.log(element);
              }
              result = `(${element.toAmount} ${element.toCurrency})`;
            }
          }
        );
      });
  } catch (e) {
    console.log('calculateExchange exception: ' + e);
  }
  return result;
}

export function disablePaymentButtons() {
  const paymentButtons = getPaymentButtons();

  Array.from(paymentButtons).forEach(element => {
    element.disabled = true;
  });
}

export function startPaymentButtonsLoading() {
  const paymentButtons = getPaymentButtons();

  Array.from(paymentButtons).forEach(element => {
    element.classList.add('payment-button--loading');
  });
}

export function stopPaymentButtonsLoading() {
  const paymentButtons = getPaymentButtons();

  Array.from(paymentButtons).forEach(element => {
    element.classList.remove('payment-button--loading');
  });
}

function getPaymentButtons(): HTMLCollectionOf<HTMLButtonElement> {
  return document.getElementsByClassName(
    'payment-button'
  ) as HTMLCollectionOf<HTMLButtonElement>;
}

export async function initPayment(
  currency: string,
  country: string,
  amount: number,
  server: string
): Promise<string> {
  amount = amount;
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
    disablePaymentButtons();
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
  return providerDefinitionIdList[paymentMethod];
}
