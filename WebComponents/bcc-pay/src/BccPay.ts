import { html, LitElement, property } from 'lit-element';
import { applyStyles } from './SharedStyles';
import { loadNestScript, startNetsPayment } from './NetsClient';
import {
  disablePaymentButtons,
  getPaymentConfigurations,
  getPaymentConfigutraionIdForPaymentMethod,
  initPayment,
} from './BccPayClient';
import { User } from './User';
import {
  displayErrorPage,
  displayFirstScreen,
  closeComponent,
  reloadPage,
} from './ScreenChange';
import { RequestHeader } from './RequestHeader';
import { startMolliePayment } from './MollieClient';
import { isNullOrEmpty } from './Common';

export let isDevEnv: boolean;
export let requestHeaders: [RequestHeader] | undefined;
export let country: string;
export let mollieUrl: string;
export let primaryColor: string;
export let secondaryColor: string;
export let accentColor: string;
export let amountFixed: boolean;
export let server: string;
export let amount: number;
export let exchangeCurrency: string;

export class BccPay extends LitElement {
  @property({ type: String }) item = 'Subscription';
  @property({ type: Number }) amount = 0;
  @property({ type: String }) currency = 'NOK';
  @property({ type: String }) country = 'NOR';
  @property({ type: String }) paymentType = '';
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

  createRenderRoot() {
    return this;
  }

  async applyCssStyles() {
    await this.updateComplete;
    applyStyles();
  }

  async init() {
    isDevEnv = this.isDevEnv;
    requestHeaders = this.requestHeaders;
    country = this.country;
    primaryColor = this.primaryColor;
    secondaryColor = this.secondaryColor;
    accentColor = this.accentColor;
    amountFixed = this.amount !== 0;
    server = this.server;
    amount = this.amount;

    disablePaymentButtons();

    if (this.amount !== 0)
      getPaymentConfigurations(
        this.country,
        this.server,
        this.currency,
        this.paymentType
      );

    if (isNullOrEmpty(this.paymentId)) {
      await this.initBccPayPayment();
    }

    if (isNullOrEmpty(this.paymentId)) {
      displayErrorPage();
    } else {
      displayFirstScreen();
    }

    if (isDevEnv === true) {
      console.log('Bcc pay payment id: ' + this.paymentId);
    }

    (document.getElementById('country-select') as HTMLSelectElement).value =
      this.country;
    (document.getElementById('currency-select') as HTMLSelectElement).value =
      this.currency;
  }

  async initBccPayPayment() {
    this.paymentId = await initPayment(
      this.currency,
      this.country,
      this.amount,
      this.server
    );
  }

  async initMolliePayment(paymentConfigurationId: string, buttonId: string) {
    mollieUrl = await startMolliePayment(
      this.paymentId,
      this.user,
      this.server,
      paymentConfigurationId,
      buttonId
    );

    if (!isNullOrEmpty(mollieUrl)) {
      const paymentButton = document.getElementById(
        buttonId
      ) as HTMLButtonElement;
      paymentButton.disabled = false;
    }
  }

  render() {
    return html`
      <div style="display: none">
        ${loadNestScript()} ${this.applyCssStyles()} ${this.init()}
      </div>
      <div class="card-square" id="main-div">
        <div id="first-screen" class="screen">
          <div
            class="card-price"
            style="display: ${amountFixed ? 'none' : 'block'}"
          >
            <div>
              <div class="card-subtitle">
                <h5>Choose amount</h5>
              </div>
              <div class="card-title">
                <h3>${this.item}</h3>
              </div>
            </div>
            <div class="card-row">
              <span class="card-tag">Amount</span>
              <span class="card-cost"
                ><input
                  type="number"
                  id="amount-input"
                  class="amount-input"
                  name="tentacles"
                  min="1"
                  max="10000"
                  value="${this.amount}"
                  @focusout="${(e: any) => {
                    this.amount = e.target.value;
                    this.initBccPayPayment();
                  }}"
                />
                <select
                  id="currency-select"
                  class="currency-select"
                  @change="${(e: any) => (this.currency = e.target.value)}"
                  value="${this.currency}"
                >
                  <option selected="${this.currency}" value="NOK">NOK</option>
                  <option selected="${this.currency}" value="EUR">EUR</option>
                  <option selected="${this.currency}" value="PLN">PLN</option>
                </select>
              </span>
            </div>
          </div>
          <div
            class="card-price"
            style="display: ${!amountFixed ? 'none' : 'block'}"
          >
            <div>
              <div class="card-subtitle">
                <h5>you are paying for</h5>
              </div>
              <div class="card-title">
                <h3>${this.item}</h3>
              </div>
            </div>
            <div class="card-row">
              <span class="card-tag">Amount</span>
              <span class="card-cost">${this.amount} ${this.currency} </span>
            </div>
          </div>

          <div class="country-container">
            Choose your country:
            <select
              id="country-select"
              class="country-select"
              @change="${(e: any) => {
                if (e.target.value === 'NOR') {
                  this.currency = '';
                } else {
                  this.currency = 'EUR';
                }
                this.country = e.target.value;
              }}"
              value="${this.country}"
            >
              <option value="" selected disabled hidden>Change country</option>
              <option selected="${this.country}" value="NOR">
                Norway (NOK)
              </option>
              <option selected="${this.country}" value="DEU">
                Germany (EUR)
              </option>
              <option selected="${this.country}" value="UA">Ukraine</option>
              <option selected="${this.country}" value="PL">Poland</option>
              <option selected="${this.country}" value="NLD">
                Netherlands
              </option>
            </select>
          </div>
          <button
            id="CreditCardOrVipps"
            class="payment-button"
            disabled="true"
            @click="${e =>
              startNetsPayment(
                this.paymentId,
                this.user,
                this.server,
                this.netsCheckoutKey,
                getPaymentConfigutraionIdForPaymentMethod(e.target.id)
              )}"
          >
            <span class="payment-button-text">CREDIT CARD </span>
          </button>
          <button
            id="Giropay"
            class="payment-button"
            disabled="true"
            @click="${e =>
              this.initMolliePayment(
                getPaymentConfigutraionIdForPaymentMethod(e.target.id),
                e.target.id
              )}"
          >
            <span class="payment-button-text">GIROPAY</span>
          </button>
          <button
            id="iDeal"
            class="payment-button"
            disabled="true"
            @click="${e =>
              this.initMolliePayment(
                getPaymentConfigutraionIdForPaymentMethod(e.target.id),
                e.target.id
              )}"
          >
            <span class="payment-button-text">iDeal</span>
          </button>
        </div>

        <div id="payment-error-screen" class="screen" style="display: none">
          <div class="card-subtitle">
            <h5>
              There is an issue with starting payment, please try again later.
            </h5>
          </div>
          <button class="payment-button" @click="${() => reloadPage()}">
            RELOAD
          </button>
        </div>

        <div id="payment-completed-screen" class="screen" style="display: none">
          <div class="card-subtitle">
            <h5>Payment success</h5>
          </div>
          <button class="payment-button" @click="${() => closeComponent()}">
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
      </div>
    `;
  }
}
