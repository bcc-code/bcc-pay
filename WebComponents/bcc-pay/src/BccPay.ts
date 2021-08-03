import { html, LitElement, property } from 'lit-element';
import { applyStyles } from './SharedStyles';
import { startNetsPayment } from './NetsClient';
import { initPayment } from './BccPayClient';
import { User } from './User';
import { MDCTextField } from '@material/textfield';

export class BccPay extends LitElement {
  @property({ type: String }) item = 'Subscription';
  @property({ type: Number }) amount = 5;
  @property({ type: String }) currency = 'NOK';
  @property({ type: String }) country = 'NOR';
  @property({ type: User }) user = {
    email: 'doe@test.no',
    phoneNumber: '+47661626839',
    firstName: 'John',
    lastName: 'Doe',
    addressLine1: 'TestAddressLine1',
    addressLine2: 'TestAddressLine2',
    city: 'Oslo',
    postalCode: '0001',
  };

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

  async displayErrorPage() {
    const firstScreenElement = document.getElementById(
      'first-screen'
    ) as HTMLElement;
    firstScreenElement.style.display = 'none';

    const errorScreenElement = document.getElementById(
      'payment-error-screen'
    ) as HTMLElement;
    errorScreenElement.style.display = 'block';
    // await this.updateComplete;
    // applyStyles();
  }

  changeUserData() {
    const errorScreenElement = document.getElementById(
      'nets-payment-screen'
    ) as HTMLElement;
    errorScreenElement.style.display = 'none';

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
      this.amount
    );

    if (this.paymentId === '') {
      this.displayErrorPage();
    }
    console.log('Bcc pay payment id: ' + this.paymentId);

    const textField = new MDCTextField(
      document.querySelector('.mdc-text-field') || new Element()
    );
  }

  updateUserData(paymentId: string, user: any) {
    console.log('User data updated' + JSON.stringify(user));
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
            @click="${() => startNetsPayment(this.paymentId, this.user)}"
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
                @click="${() => this.changeUserData()}"
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

          <label class="mdc-text-field mdc-text-field--filled">
            <span class="mdc-text-field__ripple"></span>
            <span class="mdc-floating-label" id="my-label">Email</span>
            <input
              type="text"
              class="mdc-text-field__input"
              aria-labelledby="my-label"
              value="${this.user.email}"
              @change="${(e: any) => {
                this.user.email = e.target.value;
                document.getElementById('user-email')!.innerHTML =
                  this.user.email;
              }}"
            />
            <span class="mdc-line-ripple"></span>
          </label>
          <br />

          <label class="mdc-text-field mdc-text-field--filled">
            <span class="mdc-text-field__ripple"></span>
            <span class="mdc-floating-label" id="my-label">Phone</span>
            <input
              type="text"
              class="mdc-text-field__input"
              aria-labelledby="my-label"
              value="${this.user.phoneNumber}"
              @change="${(e: any) => (this.user.phoneNumber = e.target.value)}"
            />
            <span class="mdc-line-ripple"></span>
          </label>
          <br />

          <label class="mdc-text-field mdc-text-field--filled">
            <span class="mdc-text-field__ripple"></span>
            <span class="mdc-floating-label" id="my-label">First Name</span>
            <input
              type="text"
              class="mdc-text-field__input"
              aria-labelledby="my-label"
              value="${this.user.firstName}"
              @change="${(e: any) => (this.user.firstName = e.target.value)}"
            />
            <span class="mdc-line-ripple"></span>
          </label>
          <br />

          <label class="mdc-text-field mdc-text-field--filled">
            <span class="mdc-text-field__ripple"></span>
            <span class="mdc-floating-label" id="my-label">Last Name</span>
            <input
              type="text"
              class="mdc-text-field__input"
              aria-labelledby="my-label"
              value="${this.user.lastName}"
              @change="${(e: any) => {
                this.user.lastName = e.target.value;
              }}"
            />
            <span class="mdc-line-ripple"></span>
          </label>
          <br />

          <label class="mdc-text-field mdc-text-field--filled">
            <span class="mdc-text-field__ripple"></span>
            <span class="mdc-floating-label" id="my-label">Address Line 1</span>
            <input
              type="text"
              class="mdc-text-field__input"
              aria-labelledby="my-label"
              value="${this.user.addressLine1}"
              @change="${(e: any) => (this.user.addressLine1 = e.target.value)}"
            />
            <span class="mdc-line-ripple"></span>
          </label>
          <br />

          <label class="mdc-text-field mdc-text-field--filled">
            <span class="mdc-text-field__ripple"></span>
            <span class="mdc-floating-label" id="my-label">Address Line 2</span>
            <input
              type="text"
              class="mdc-text-field__input"
              aria-labelledby="my-label"
              value="${this.user.addressLine2}"
              @change="${(e: any) => (this.user.addressLine2 = e.target.value)}"
            />
            <span class="mdc-line-ripple"></span>
          </label>
          <br />

          <label class="mdc-text-field mdc-text-field--filled">
            <span class="mdc-text-field__ripple"></span>
            <span class="mdc-floating-label" id="my-label">City</span>
            <input
              type="text"
              class="mdc-text-field__input"
              aria-labelledby="my-label"
              value="${this.user.city}"
              @change="${(e: any) => (this.user.city = e.target.value)}"
            />
            <span class="mdc-line-ripple"></span>
          </label>
          <br />

          <label class="mdc-text-field mdc-text-field--filled">
            <span class="mdc-text-field__ripple"></span>
            <span class="mdc-floating-label" id="my-label">Postal code</span>
            <input
              type="text"
              class="mdc-text-field__input"
              aria-labelledby="my-label"
              value="${this.user.postalCode}"
              @change="${(e: any) => (this.user.postalCode = e.target.value)}"
            />
            <span class="mdc-line-ripple"></span>
          </label>

          <button
            class="reload-button"
            @click="${() => startNetsPayment(this.paymentId, this.user)}"
          >
            CHANGE
          </button>
        </div>
      </div>
    `;
  }
}
