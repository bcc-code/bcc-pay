import { html, LitElement, property } from 'lit-element';
import { applyStyles } from './SharedStyles';
import { startNetsPayment } from './NetsClient';

export class BccPay extends LitElement {
  @property({ type: String }) item = 'Subscription';

  @property({ type: Number }) cost = 5;

  @property({ type: String }) currency = 'NOK';

  loadNestScript() {
    let script = document.createElement('script');
    script.src = 'https://test.checkout.dibspayment.eu/v1/checkout.js?v=1';
    return script;
  }

  async initPayment(): Promise<string> {
    const body = {
      payerId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
      currency: 'NOK',
      country: 'NOR',
      amount: 100,
    };

    let paymentId: string = '';
    await fetch('https://localhost:5001/Payment', {
      method: 'POST',
      body: JSON.stringify(body),
      headers: {
        'Content-Type': 'application/json',
      },
    })
      .then(response => response.json())
      .then(json => {
        console.log('parsed json', json);
        paymentId = json.paymentId;
      });
    return paymentId;
  }

  createRenderRoot() {
    return this;
  }

  async applyStyles() {
    await this.updateComplete;
    applyStyles();
  }

  render() {
    return html`
      <div style="display: none">
        ${this.loadNestScript()} ${this.applyStyles()}
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
            <span class="card-cost">${this.cost} ${this.currency} </span>
          </div>
          <button class="nets-button" @click=${startNetsPayment}>
            PAY WITH NETS
          </button>
        </div>
        <div id="nets-payment-screen" style="display: none">
          <div class="card-subtitle">
            <h5>Pay with nets</h5>
          </div>
          <div id="checkout-container-div"></div>
        </div>
      </div>
    `;
  }
}
