import { html, LitElement, property } from 'lit-element';
import { applyStyles } from './SharedStyles';
import { startNetsPayment } from './NetsClient';
import { getPaymentConfigurations, initPayment } from './BccPayClient';
import { User } from './User';
import {
  displayChangeUserDataPage,
  displayErrorPage,
  reload,
  close,
} from './ScreenChange';
import { RequestHeader } from './RequestHeader';
import { startMolliePayment } from './MollieClient';

export let isDevEnv: boolean;
export let requestHeaders: [RequestHeader] | undefined;
export let paymentConfigurationId: string;

export class BccPay extends LitElement {
  @property({ type: String }) item = 'Subscription';
  @property({ type: Number }) amount = 5;
  @property({ type: String }) currency = 'EUR';
  @property({ type: String }) country = 'NOR';
  @property({ type: User }) user: User = {};
  @property({ type: String }) server = 'https://localhost:5001';
  @property({ type: String }) netsCheckoutKey = '#checkout_key#';
  @property({ type: Boolean }) isDevEnv: boolean = false;
  @property({ type: [RequestHeader] }) requestHeaders:
    | [RequestHeader]
    | undefined;
  @property({ type: String }) paymentId: string = '';
  @property({ type: String }) paymentConfigurationId: string = 'nets-cc-eur';

  mollieUrl: string = '';

  loadNestScript() {
    isDevEnv = this.isDevEnv;
    requestHeaders = this.requestHeaders;
    paymentConfigurationId = this.paymentConfigurationId;

    let nestScript = document.createElement('script');
    if (isDevEnv === true) {
      nestScript.src =
        'https://test.checkout.dibspayment.eu/v1/checkout.js?v=1';
    } else {
      nestScript.src = 'https://checkout.dibspayment.eu/v1/checkout.js?v=1';
    }

    return nestScript;
  }

  createRenderRoot() {
    return this;
  }

  async applyCssStyles() {
    await this.updateComplete;
    applyStyles();
  }

  async init() {
    getPaymentConfigurations(this.country, this.server);

    if (this.paymentId === '' || this.paymentId === undefined) {
      this.paymentId = await initPayment(
        this.currency,
        this.country,
        this.amount,
        this.server
      );
    }

    if (this.paymentId === '') {
      displayErrorPage();
    }

    if (isDevEnv === true) {
      console.log('Bcc pay payment id: ' + this.paymentId);
    }
  }

  async initMolliePayment() {
    this.mollieUrl = await startMolliePayment(
      this.paymentId,
      this.user,
      this.server,
      this.netsCheckoutKey
    );
  }

  render() {
    return html`
      <div style="display: none">
        ${this.loadNestScript()} ${this.applyCssStyles()} ${this.init()}
      </div>
      <div class="card-square" id="main-div">
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

          Current country: ${this.country}
          <div>
            <select
              class="country-select"
              value="${this.country}"
              @change="${(e: any) => (this.country = e.target.value)}"
            >
              <option value="">Select your country</option>
              <option value="NOR">Norway</option>
              <option value="DE">Germany</option>
              <option value="UA">Ukraine</option>
              <option value="PL">Poland</option>
            </select>
          </div>
          <button
            id="nets-cc"
            class="payment-button"
            @click="${() =>
              startNetsPayment(
                this.paymentId,
                this.user,
                this.server,
                this.netsCheckoutKey
              )}"
          >
            PAY WITH NETS
          </button>
          <button
            id="mollie-giropay"
            class="payment-button"
            @click="${() => this.initMolliePayment()}"
          >
            PAY WITH MOLLIE
          </button>
        </div>

        <div id="payment-error-screen" style="display: none">
          <div class="card-subtitle">
            <h5>
              There is an issue with starting payment, please try again later.
            </h5>
          </div>
          <button class="reload-button" @click="${() => reload()}">
            RELOAD
          </button>
        </div>

        <div id="payment-completed-screen" style="display: none">
          <div class="card-subtitle">
            <h5>Payment success</h5>
          </div>
          <button class="reload-button" @click="${() => close()}">Close</button>
        </div>

        <div id="nets-payment-screen" style="display: none">
          <div class="card-subtitle">
            <h5 id="payment-issue">
              Issue with payment?
              <button
                class="link-button"
                @click="${() => displayChangeUserDataPage()}"
              >
                Edit personal data
              </button>
            </h5>
          </div>
          <div id="checkout-container-div"></div>
        </div>

        <div id="mollie-payment-screen" style="display: none">
          <div class="card-subtitle">
            <h5>Mollie payment</h5>
          </div>

          <button
            class="payment-button"
            @click="${() => console.log('Url: ' + this.mollieUrl)}"
          >
            Mollie payment page
          </button>

          <iframe
            id="mollie-iframe"
            src="${this.mollieUrl}"
            title="mollie-iframe"
          ></iframe>
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
              startNetsPayment(
                this.paymentId,
                this.user,
                this.server,
                this.netsCheckoutKey
              )}"
          >
            CHANGE
          </button>
        </div>
      </div>
    `;
  }
}
