import { html, LitElement, property } from 'lit-element';
import { applyStyles } from './SharedStyles';
import { startNetsPayment } from './NetsClient';
import { getPaymentConfigurations, initPayment } from './BccPayClient';
import { User } from './User';
import {
  displayErrorPage,
  displayFirstScreen,
  closeComponent,
  reloadPage,
} from './ScreenChange';
import { RequestHeader } from './RequestHeader';
import { startMolliePayment } from './MollieClient';

export let isDevEnv: boolean;
export let requestHeaders: [RequestHeader] | undefined;
export let country: string;
export let mollieUrl: string;
export let primaryColor: string;
export let secondaryColor: string;
export let accentColor: string;

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
  @property({ type: String }) primaryColor: string = '#006fc2';
  @property({ type: String }) secondaryColor: string = 'white';
  @property({ type: String }) accentColor: string = '#bae1ff';

  loadNestScript() {
    isDevEnv = this.isDevEnv;
    requestHeaders = this.requestHeaders;
    country = this.country;
    primaryColor = this.primaryColor;
    secondaryColor = this.secondaryColor;
    accentColor = this.accentColor;

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
    if (this.paymentId === '' || this.paymentId === undefined) {
      this.paymentId = await initPayment(
        this.currency,
        this.country,
        this.amount,
        this.server
      );
    }
    getPaymentConfigurations(this.country, this.server);

    if (this.paymentId === '') {
      displayErrorPage();
    }

    if (isDevEnv === true) {
      console.log('Bcc pay payment id: ' + this.paymentId);
    }
  }

  async initMolliePayment() {
    mollieUrl = await startMolliePayment(
      this.paymentId,
      this.user,
      this.server
    );

    if (mollieUrl !== '' || mollieUrl !== null || mollieUrl !== undefined) {
      const mollyButton = document.getElementById(
        'mollie-payment-button'
      ) as HTMLButtonElement;
      mollyButton.disabled = false;
    }
  }

  render() {
    return html`
      <div style="display: none">
        ${this.loadNestScript()} ${this.applyCssStyles()} ${this.init()}
      </div>
      <div class="card-square" id="main-div">
        <div id="first-screen" class="screen">
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

          <div class="country-container">
            Current country: ${this.country}
            <div>
              <select
                class="country-select"
                value="${this.country}"
                @change="${(e: any) => (this.country = e.target.value)}"
              >
                <option value="">Change country</option>
                <option value="NOR">Norway</option>
                <option value="DE">Germany</option>
                <option value="UA">Ukraine</option>
                <option value="PL">Poland</option>
              </select>
            </div>
          </div>
          <button
            id="Nets"
            class="payment-button"
            @click="${() =>
              startNetsPayment(
                this.paymentId,
                this.user,
                this.server,
                this.netsCheckoutKey
              )}"
          >
            <span class="payment-button-text">CREDIT CARD</span>
          </button>
          <button
            id="Mollie"
            class="payment-button"
            @click="${() => this.initMolliePayment()}"
          >
            <span class="payment-button-text">GIROPAY</span>
          </button>
        </div>

        <div id="payment-error-screen" class="screen" style="display: none">
          <div class="card-subtitle">
            <h5>
              There is an issue with starting payment, please try again later.
            </h5>
          </div>
          <button class="reload-button" @click="${() => reloadPage()}">
            RELOAD
          </button>
        </div>

        <div id="payment-completed-screen" class="screen" style="display: none">
          <div class="card-subtitle">
            <h5>Payment success</h5>
          </div>
          <button class="reload-button" @click="${() => closeComponent()}">
            Close
          </button>
        </div>

        <div id="nets-payment-screen" class="screen" style="display: none">
          <div class="card-subtitle">
            <h5 id="payment-provider-change">
              Other payment method?
              <button
                class="link-button"
                @click="${() => displayFirstScreen()}"
              >
                Change provider
              </button>
            </h5>
          </div>
          <div id="checkout-container-div"></div>
        </div>

        <div id="change-user-data-screen" class="screen" style="display: none">
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
