export function displayErrorPage() {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'none';

  const netsScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'none';

  const molliePaymentScreenElement = document.getElementById(
    'mollie-payment-screen'
  ) as HTMLElement;
  molliePaymentScreenElement.style.display = 'none';

  const errorScreenElement = document.getElementById(
    'payment-error-screen'
  ) as HTMLElement;
  errorScreenElement.style.display = 'block';
}

export function displayChangeUserDataPage() {
  const errorScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  errorScreenElement.style.display = 'none';

  const checkoutElement = document.getElementById(
    'checkout-container-div'
  ) as HTMLElement;
  checkoutElement.innerHTML = '';

  const changeUserDataElement = document.getElementById(
    'change-user-data-screen'
  ) as HTMLElement;
  changeUserDataElement.style.display = 'block';
}

export function paymentCompleted() {
  const errorScreenElement = document.getElementById(
    'payment-issue'
  ) as HTMLElement;
  errorScreenElement.style.display = 'none';

  const paymentCompletedElement = document.getElementById(
    'payment-completed-screen'
  ) as HTMLElement;
  paymentCompletedElement.style.display = 'block';
}

export function reload() {
  window.location.reload();
}

export function close() {
  const mainElement = document.getElementById('main-div') as HTMLElement;
  mainElement.style.display = 'none';
}

export function goToFirstScreen() {
  const netsPaymentScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  netsPaymentScreenElement.style.display = 'none';

  const molliePaymentScreenElement = document.getElementById(
    'mollie-payment-screen'
  ) as HTMLElement;
  molliePaymentScreenElement.style.display = 'none';

  const errorScreenElement = document.getElementById(
    'payment-issue'
  ) as HTMLElement;
  errorScreenElement.style.display = 'none';

  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'block';
}
