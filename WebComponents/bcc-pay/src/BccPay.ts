import { html, css, LitElement, property } from 'lit-element';

export class BccPay extends LitElement {
  static styles = css`
    @import url('https://fonts.googleapis.com/css2?family=Lato:wght@400;700;900&display=swap');
    * {
      margin: 0;
      padding: 0;
      box-sizing: border-box;
    }

    img {
      max-width: 100%;
      display: block;
    }

    a {
      text-decoration: none;
    }

    .flow > * + * {
      margin-top: var(--flow-spacer, var(--spacer));
    }

    html,
    body {
      width: 100%;
      height: 100%;
    }

    body {
      min-height: 100%;
      font-family: 'Lato', sans-serif;
      color: var(--neutral-900);
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      background-color: hsl(226, 74%, 97%);
    }

    main {
      display: grid;
      grid-gap: 30px;
      grid-template-columns: repeat(3, auto);
      grid-template-rows: repeat(2, auto);
    }

    section:nth-child(1) {
      grid-column: 1;
      align-self: center;
    }

    section:nth-child(2) {
      grid-row: 2;
      grid-column: 2 / span 2;
    }

    section:nth-child(3) {
      grid-column: 2;
      justify-self: end;
    }

    section:nth-child(4) {
      grid-column: 1;
      grid-row: 2;
    }

    .card-long {
      display: grid;
      grid-template-columns: repeat(8, 1fr);
      grid-template-rows: max-content auto;
      grid-row-gap: 1rem;
      align-content: center;
      max-width: 608px;
      background: #ffffff;
      box-shadow: 0px 24px 25px -10px rgba(182, 194, 240, 0.54);
      border-radius: 15px;
      padding: 35px 45px;
    }

    .card__title {
      grid-column: 1 / span 4;
      grid-row: 1;
    }

    .card__price {
      grid-row: 2;
      font-weight: 900;
      font-size: 1.375rem;
    }

    .card__btn {
      display: inline-block;
      grid-column: 7 / span 2;
      grid-row: 1;
      align-self: center;
      font-size: 0.875rem;
      text-transform: uppercase;
      background: hsl(258, 71%, 61%);
      color: white;
      padding: 15px 25px;
      border-radius: 5px;
      text-align: center;
      font-weight: 700;
    }

    .card-regular {
      display: grid;
      grid-template-columns: 1fr 100px;
      max-width: 429px;
      background: #ffffff;
      box-shadow: 0px 24px 25px -10px rgba(182, 194, 240, 0.54);
      grid-row-gap: 1rem;
      border-radius: 15px;
      padding: 55px;
    }

    .card-regular__sub {
      grid-column: 1 / span 2;
      font-size: 18px;
      text-transform: uppercase;
      color: var(--neutral-500);
    }

    card-regular__title {
      grid-row: 2;
      grid-column: 1 / span 2;
    }

    .card-regular__price {
      font-weight: 900;
      font-size: 1.375rem;
      place-self: center;
    }

    .card-regular__btn {
      margin-top: 1rem;
      justify-self: start;
      display: inline-block;
      font-size: 0.875rem;
      text-transform: uppercase;
      background: hsl(258, 71%, 61%);
      color: white;
      padding: 15px 25px;
      border-radius: 5px;
      text-align: center;
      font-weight: 700;
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

    .card-fill {
      display: grid;
      grid-template-columns: 1fr 60px;
      grid-template-rows: repeat(4, 1fr);
      max-width: 358px;
      box-shadow: 0px 24px 25px -10px rgb(182 194 240 / 54%);
      border-radius: 15px;
      padding: 50px 35px;
      background: hsl(258, 71%, 61%);
      color: white;
      box-shadow: 0 0 0 19px hsl(245, 64%, 94%);
    }

    .card-fill__sub {
      font-size: 18px;
      text-transform: uppercase;
    }

    .card-fill__price {
      font-size: 22px;
      font-weight: 900;
    }

    .card-fill__btn {
      display: inline-block;
      background: white;
      color: var(--clr-accent);
      grid-row: 4;
      justify-self: start;
      padding: 15px 25px;
      font-size: 14px;
      text-transform: uppercase;
      border-radius: 5px;
    }
  `;

  @property({ type: String }) item = 'Subscription';

  @property({ type: Number }) cost = 5;

  @property({ type: String }) currency = 'NOK';

  __increment() {
    this.cost += 1;
  }

  render() {
    return html`
      <div class="card-square">
        <div class="card-square__sub">
          <h5>you are paying for</h5>
        </div>
        <div class="card-square__title">
          <h3>${this.item}</h3>
        </div>
        <div class="card-square__price">
          <span class="card-square__tag">Price</span>
          <span class="card-square__cost">${this.cost} ${this.currency} </span>
        </div>
        <button class="card-square__btn" @click=${this.__increment}>
          PAY MORE
        </button>
        <a href="#" class="card-square__link">Learn More</a>
      </div>
    `;
  }
}
