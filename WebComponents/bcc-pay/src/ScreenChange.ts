export function displayErrorPage() {
  hideFirstScreen();
  hideNetsPaymentScreen();
  hideMolliePaymentScreen();
  showErrorPageScreen();
}

export function displayChangeUserDataPage() {
  hideNetsPaymentScreen();
  clearNetsIframe();
  showChangeUserDataScreen();
}

export function displayNetsPayment() {
  hideFirstScreen();
  hideMolliePaymentScreen();
  showNetsPaymentScreen();
}

export function displayMolliePayment() {
  hideFirstScreen();
  hideNetsPaymentScreen();
  showMolliePaymentScreen();
}

export function displayFirstScreen() {
  hideMolliePaymentScreen();
  hideNetsPaymentScreen();
  hideErrorPageScreen();
  showFirstScreen();
}

export function paymentCompleted() {
  hidePaymentProviderChangeElement();
  showPaymentCompletedScreen();
}

export function reload() {
  window.location.reload();
}

export function close() {
  const mainElement = document.getElementById('main-div') as HTMLElement;
  mainElement.style.display = 'none';
}

function showErrorPageScreen() {
  const errorScreenElement = document.getElementById(
    'payment-error-screen'
  ) as HTMLElement;
  errorScreenElement.style.display = 'block';
}

function hideErrorPageScreen() {
  const errorScreenElement = document.getElementById(
    'payment-error-screen'
  ) as HTMLElement;
  errorScreenElement.style.display = 'none';
}

function showMolliePaymentScreen() {
  const netsScreenElement = document.getElementById(
    'mollie-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'block';
}
function hideMolliePaymentScreen() {
  const molliePaymentScreenElement = document.getElementById(
    'mollie-payment-screen'
  ) as HTMLElement;
  molliePaymentScreenElement.style.display = 'none';
}

function hideNetsPaymentScreen() {
  const netsScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'none';
}

function hideFirstScreen() {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'none';
}

function showFirstScreen() {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'block';
}

function clearNetsIframe() {
  const checkoutElement = document.getElementById(
    'checkout-container-div'
  ) as HTMLElement;
  checkoutElement.innerHTML = '';
}

function showChangeUserDataScreen() {
  const changeUserDataElement = document.getElementById(
    'change-user-data-screen'
  ) as HTMLElement;
  changeUserDataElement.style.display = 'block';
}

function showPaymentCompletedScreen() {
  const paymentCompletedElement = document.getElementById(
    'payment-completed-screen'
  ) as HTMLElement;
  paymentCompletedElement.style.display = 'block';
}

function showNetsPaymentScreen() {
  const netsScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'block';
}

function hidePaymentProviderChangeElement() {
  const paymentProviderChangeElement = document.getElementById(
    'payment-provider-change'
  ) as HTMLElement;
  paymentProviderChangeElement.style.display = 'none';
}
