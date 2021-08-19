export function displayErrorPage() {
  hideFirstScreen();
  hideNetsPaymentScreen();
  showErrorPageScreen();
}

export function displayChangeUserDataPage() {
  hideNetsPaymentScreen();
  clearNetsIframe();
  showChangeUserDataScreen();
}

export function displayNetsPayment() {
  hideFirstScreen();
  showNetsPaymentScreen();
}

export function displayMolliePayment() {
  hideFirstScreen();
  hideNetsPaymentScreen();
}

export function displayFirstScreen() {
  hideNetsPaymentScreen();
  hideErrorPageScreen();
  showFirstScreen();
}

export function paymentCompleted() {
  hidePaymentProviderChangeElement();
  showPaymentCompletedScreen();
}

export function reloadPage() {
  window.location.reload();
}

export function closeComponent() {
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

function showNetsPaymentScreen() {
  const netsScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'block';
}

function hideNetsPaymentScreen() {
  const netsScreenElement = document.getElementById(
    'nets-payment-screen'
  ) as HTMLElement;
  netsScreenElement.style.display = 'none';
}

function showFirstScreen() {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'block';
}

function hideFirstScreen() {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'none';
}

function showChangeUserDataScreen() {
  const changeUserDataElement = document.getElementById(
    'change-user-data-screen'
  ) as HTMLElement;
  changeUserDataElement.style.display = 'block';
}

function hideChangeUserDataScreen() {
  const changeUserDataElement = document.getElementById(
    'change-user-data-screen'
  ) as HTMLElement;
  changeUserDataElement.style.display = 'none';
}

function showPaymentCompletedScreen() {
  const paymentCompletedElement = document.getElementById(
    'payment-completed-screen'
  ) as HTMLElement;
  paymentCompletedElement.style.display = 'block';
}

function hidePaymentCompletedScreen() {
  const paymentCompletedElement = document.getElementById(
    'payment-completed-screen'
  ) as HTMLElement;
  paymentCompletedElement.style.display = 'none';
}

function hidePaymentProviderChangeElement() {
  const paymentProviderChangeElement = document.getElementById(
    'payment-provider-change'
  ) as HTMLElement;
  paymentProviderChangeElement.style.display = 'none';
}

function clearNetsIframe() {
  const checkoutElement = document.getElementById(
    'checkout-container-div'
  ) as HTMLElement;
  checkoutElement.innerHTML = '';
}
