export function displayErrorPage() {
  const firstScreenElement = document.getElementById(
    'first-screen'
  ) as HTMLElement;
  firstScreenElement.style.display = 'none';

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

export function reload() {
  window.location.reload();
}
