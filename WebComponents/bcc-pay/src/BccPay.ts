import { html, css, LitElement, property } from 'lit-element';
export class BccPay extends LitElement {
  static styles = css`
    * {
      margin: 0;
      padding: 0;
      box-sizing: border-box;
    }

    .card-square {
      display: grid;
      grid-template-columns: 1fr;
      grid-auto-rows: max-content;
      grid-row-gap: 1rem;
      max-width: 478px;
      background: #ffffff;
      box-shadow: 0px 24px 25px -10px rgba(182, 194, 240, 0.54);
      border-radius: 15px;
      padding: 55px 74px;
    }

    .card-square__sub {
      font-size: 18px;
      text-transform: uppercase;
      color: var(--neutral-500);
    }

    .card-square__title {
      font-size: 1.375rem;
      font-weight: 700;
    }

    .card-square__price {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 27px 30px;
      background-color: hsl(238, 90%, 99%);
      border: 1px solid hsl(227, 65%, 95%);
      border-radius: 10px;
    }

    .card-square__price span {
      font-size: 1.125rem;
      font-weight: 900;
    }

    .card-square__btn {
      font-size: 1rem;
      text-transform: uppercase;
      background: hsl(258, 71%, 61%);
      color: white;
      padding: 23px;
      border-radius: 5px;
      text-align: center;
      font-weight: 700;
      margin-top: 1rem;
    }

    .card-square__link {
      text-align: center;
      font-size: 1rem;
      font-weight: 700;
      color: hsl(258, 71%, 61%);
      padding: 20px 0;
    }
  `;

  @property({ type: String }) item = 'Subscription';

  @property({ type: Number }) cost = 5;

  @property({ type: String }) currency = 'NOK';

  __increment() {
    this.cost += 1;
  }

  async startNetsPayment() {
    const firstScreenElement = this.shadowRoot?.getElementById(
      'first-screen'
    ) as HTMLElement;
    firstScreenElement.style.display = 'none';

    const netsScreenElement = this.shadowRoot?.getElementById(
      'nets-payment-screen'
    ) as HTMLElement;
    netsScreenElement.style.display = 'block';

    await initNetsPayment();
  }

  render() {
    return html`
      <div class="card-square">
        <div id="first-screen">
          <div class="card-square__sub">
            <h5>you are paying for</h5>
          </div>
          <div class="card-square__title">
            <h3>${this.item}</h3>
          </div>
          <div class="card-square__price">
            <span class="card-square__tag">Price</span>
            <span class="card-square__cost"
              >${this.cost} ${this.currency}
            </span>
          </div>
          <button class="card-square__btn" @click=${this.startNetsPayment}>
            PAY WITH NETS
          </button>
        </div>
        <div id="nets-payment-screen" style="display: none">
          NETS PAYMENT:

          <h1>Checkout</h1>
          <div id="checkout-container-div">
            <!-- checkout iframe will be embedded here -->
          </div>
          <script src="https://test.checkout.dibspayment.eu/v1/checkout.js?v=1"></script>
          <script src="nets.js"></script>
        </div>
      </div>
    `;
  }
}

export async function initNetsPayment() {
  const body = {
    payerId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    currency: 'NOK',
    country: 'NOR',
    amount: 1000,
    paymentMethod: 'CreditCard',
    privatePerson: {
      email: 'test@test.no',
      phoneNumberPrefix: '+47',
      phoneNumberBody: '661626839',
      firstName: 'TestName',
      lastName: 'TestLastName',
      addressLine1: 'TestAddressLine1',
      addressLine2: 'TestAddressLine2',
      city: 'Oslo',
      postalCode: '0001',
    },
  };

  const res = await fetch('https://localhost:5001/Payment', {
    method: 'POST',
    body: JSON.stringify(body),
    headers: {
      'Content-Type': 'application/json',
    },
  });

  console.log('Response is: ' + JSON.stringify(res));
}
