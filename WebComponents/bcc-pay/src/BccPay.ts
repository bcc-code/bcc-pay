import { html, LitElement, property } from 'lit-element';
import { applyStyles } from './SharedStyles';
import { startNetsPayment } from './NetsClient';
import { initPayment } from './BccPayClient';

export class BccPay extends LitElement {
  @property({ type: String }) item = 'Subscription';

  @property({ type: Number }) amount = 5;

  @property({ type: String }) currency = 'NOK';

  @property({ type: String }) country = 'NOR';

  paymentId = '';

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

  async displayErrorPage() {
    const firstScreenElement = document.getElementById(
      'first-screen'
    ) as HTMLElement;
    firstScreenElement.style.display = 'none';

    const errorScreenElement = document.getElementById(
      'payment-error-screen'
    ) as HTMLElement;
    errorScreenElement.style.display = 'block';
    await this.updateComplete;
    applyStyles();
  }

  reload() {
    window.location.reload();
  }

  async init() {
    this.paymentId = await initPayment(
      this.currency,
      this.country,
      this.amount
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
            @click="${() => startNetsPayment(this.paymentId)}"
          >
            PAY WITH NETS
          </button>
        </div>
        <div id="nets-payment-screen" style="display: none">
          <div class="card-subtitle">
            <h5>Pay with nets</h5>
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
      </div>
    `;
  }
}
