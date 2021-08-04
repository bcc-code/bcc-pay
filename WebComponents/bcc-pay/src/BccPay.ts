import { html, LitElement, property } from 'lit-element';
import { applyStyles } from './SharedStyles';
import { startNetsPayment } from './NetsClient';
import { initPayment } from './BccPayClient';
import { User } from './User';

export class BccPay extends LitElement {
  @property({ type: String }) item = 'Subscription';
  @property({ type: Number }) amount = 5;
  @property({ type: String }) currency = 'NOK';
  @property({ type: String }) country = 'NOR';
  @property({ type: User }) user: User = {
    email: 'doe@test.no',
    firstName: 'John',
    lastName: 'Doe',
  };
  @property({ type: String }) server = 'https://localhost:5001';
  @property({ type: String }) checkoutKey = 'https://localhost:5001';
  @property({ type: Boolean }) isDevEnv: boolean = false;

  paymentId: string = '';

  loadNestScript() {
    let script = document.createElement('script');
    script.src = 'https://test.checkout.dibspayment.eu/v1/checkout.js?v=1';
    return script;
  }

  createRenderRoot() {
    return this;
  }

  async applyStyles() {
    await this.updateComplete;
    applyStyles();
  }

  displayErrorPage() {
    const firstScreenElement = document.getElementById(
      'first-screen'
    ) as HTMLElement;
    firstScreenElement.style.display = 'none';

    const errorScreenElement = document.getElementById(
      'payment-error-screen'
    ) as HTMLElement;
    errorScreenElement.style.display = 'block';
  }

  displayChangeUserDataPage() {
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

  reload() {
    window.location.reload();
  }

  async init() {
    this.paymentId = await initPayment(
      this.currency,
      this.country,
      this.amount,
      this.server
    );

    if (this.paymentId === '') {
      this.displayErrorPage();
    }
    console.log('Bcc pay payment id: ' + this.paymentId);
  }

  render() {
    return html`
      <div style="display: none">
        ${this.loadNestScript()} ${this.applyStyles()} ${this.init()}
      </div>
      <div class="card-square">
        <div id="first-screen">
          <div class="card-subtitle">
            <h5>you are paying for</h5>
          </div>
          <div class="card-title">
            <h3>${this.item}</h3>
          </div>
          <div class="card-price">
            <span class="card-tag">Price</span>
            <span class="card-cost">${this.amount} ${this.currency} </span>
          </div>
          <button
            class="nets-button"
            @click="${() =>
              startNetsPayment(this.paymentId, this.user, this.server)}"
          >
            PAY WITH NETS
          </button>
        </div>
        <div id="nets-payment-screen" style="display: none">
          <div class="card-subtitle">
            <h5>
              Nets payment as: <b id="user-email"> ${this.user.email}</b>
              <button
                class="link-button"
                @click="${() => this.displayChangeUserDataPage()}"
              >
                change data
              </button>
            </h5>
          </div>
          <div id="checkout-container-div"></div>
        </div>

        <div id="payment-error-screen" style="display: none">
          <div class="card-subtitle">
            <h5>
              There is an issue with starting payment, please try again later.
            </h5>
          </div>
          <button class="reload-button" @click="${() => this.reload()}">
            RELOAD
          </button>
        </div>

        <div id="change-user-data-screen" style="display: none">
          <div class="card-subtitle">
            <h5>Changing user data:</h5>
          </div>

          <div>
            <span>Email</span>
            <input
              type="text"
              value="${this.user.email}"
              @change="${(e: any) => (this.user.email = e.target.value)}"
            />
          </div>

          <div>
            <span>Phone</span>
            <input
              type="text"
              value="${this.user.phoneNumber}"
              @change="${(e: any) => (this.user.phoneNumber = e.target.value)}"
            />
          </div>

          <div>
            <span>First Name</span>
            <input
              type="text"
              value="${this.user.firstName}"
              @change="${(e: any) => (this.user.firstName = e.target.value)}"
            />
          </div>

          <div>
            <span>Last Name</span>
            <input
              type="text"
              value="${this.user.lastName}"
              @change="${(e: any) => (this.user.lastName = e.target.value)}"
            />
          </div>

          <div>
            <span>Address Line 1</span>
            <input
              type="text"
              value="${this.user.addressLine1}"
              @change="${(e: any) => (this.user.addressLine1 = e.target.value)}"
            />
          </div>

          <div>
            <span>Address Line 2</span>
            <input
              type="text"
              value="${this.user.addressLine2}"
              @change="${(e: any) => (this.user.addressLine2 = e.target.value)}"
            />
          </div>

          <div>
            <span>City</span>
            <input
              type="text"
              value="${this.user.city}"
              @change="${(e: any) => (this.user.city = e.target.value)}"
            />
          </div>

          <div>
            <span>Postal code</span>
            <input
              type="text"
              value="${this.user.postalCode}"
              @change="${(e: any) => (this.user.postalCode = e.target.value)}"
            />
          </div>

          <button
            class="reload-button"
            @click="${() =>
              startNetsPayment(this.paymentId, this.user, this.server)}"
          >
            CHANGE
          </button>
        </div>
      </div>
    `;
  }
}
