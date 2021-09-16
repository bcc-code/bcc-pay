import { css, unsafeCSS } from 'lit-element';
import { accentColor, primaryColor, secondaryColor } from './BccPay';

function createComponentStyle(): string {
  return css`
    :root {
      --primaryColor: ${unsafeCSS(primaryColor)};
      --secondaryColor: ${unsafeCSS(secondaryColor)};
      --accentColor: ${unsafeCSS(accentColor)};
    }

    .card-square {
      box-sizing: border-box;
      display: grid;
      grid-template-columns: 1fr;
      grid-auto-rows: max-content;
      grid-row-gap: 1rem;
      max-width: 400px;
      background: #ffffff;
      box-shadow: 0px 24px 25px -10px rgba(182, 194, 240, 0.54);
      border-radius: 15px;
      padding: 55px 74px;
      transition: all 0.2s;
    }

    .screen {
      transition: all 2s;
    }

    .card-title {
      font-size: 1.375rem;
      font-weight: 700;
    }

    .card-price {
      display: flex;
      flex-direction: column;
      align-items: baseline;
      justify-content: space-between;
      padding: 10px 10px;
      background-color: hsl(238, 90%, 99%);
      border: 1px solid hsl(227, 65%, 95%);
      border-radius: 10px;
    }

    .card-subtitle {
      font-size: 18px;
      text-transform: uppercase;
      color: var(--neutral-500);
    }

    .card-tag {
      font-size: 1.125rem;
      font-weight: 900;
    }

    .card-cost {
      display: flex;
      font-size: 1.125rem;
      font-weight: 900;
    }

    .card-row {
      display: flex;
      width: 100%;
      justify-content: space-between;
      padding-bottom: 6px;
    }

    .amount-input {
      font-size: 16px;
      transition: 0.6s;
      border: none;
      border-bottom: 1px solid #ccc;
      background-color: transparent;
      &:focus {
        outline: none;
        border-bottom: 1px solid #28a2a2;
      }
    }
    .currency-select {
      margin-left: 10px;
    }

    input[type='text'] {
      width: 100%;
      border: 2px solid #aaa;
      border-radius: 4px;
      margin: 8px 0;
      outline: none;
      padding: 8px;
      box-sizing: border-box;
      transition: 0.3s;
    }

    input[type='text']:focus {
      border-color: dodgerBlue;
      box-shadow: 0 0 8px 0 dodgerBlue;
    }

    .payment-button {
      font-size: 1rem;
      text-transform: uppercase;
      background: var(--primaryColor);
      color: white;
      padding: 12px;
      border-radius: 5px;
      text-align: center;
      font-weight: 700;
      margin-top: 1rem;
      position: relative;
      cursor: pointer;
      width: 100%;
    }
    .payment-button:disabled {
      background-color: gray;
      color: white;
    }

    .payment-button:active {
      background: white;
    }

    .payment-button-text {
      color: white;
      transition: all 0.2s;
    }

    .payment-button--loading .payment-button-text {
      visibility: hidden;
      opacity: 0;
    }

    .payment-button--loading::after {
      content: '';
      position: absolute;
      width: 16px;
      height: 16px;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      margin: auto;
      border: 4px solid transparent;
      border-top-color: #ffffff;
      border-radius: 50%;
      animation: button-loading-spinner 1s ease infinite;
    }

    @keyframes button-loading-spinner {
      from {
        transform: rotate(0turn);
      }

      to {
        transform: rotate(1turn);
      }
    }

    .reload-button {
      font-size: 1rem;
      text-transform: uppercase;
      background: var(--primaryColor);
      color: white;
      padding: 23px;
      border-radius: 5px;
      text-align: center;
      font-weight: 700;
      margin-top: 1rem;
    }

    .link-button {
      background: none !important;
      border: none;
      padding: 0 !important;
      /*optional*/
      font-family: arial, sans-serif;
      /*input has OS specific font-family*/
      color: #069;
      text-decoration: underline;
      cursor: pointer;
    }

    .country-container {
      display: flex;
      flex-direction: column;
      align-items: left;
      margin-top: 32px;
      margin-bottom: 24px;
      justify-content: space-between;
    }
    .country-select {
      margin-top: 8px;
    }

    select {
      width: 100%;
      font: 400 12px/1.3 sans-serif;
      -webkit-appearance: none;
      appearance: none;
      color: dimgray;
      border: 1px solid dimgray;
      line-height: 1;
      outline: 0;
      padding: 0.65em 2.5em 0.55em 0.75em;
      border-radius: 2px;
      background-color: var(--secondaryColor);
      background-image: linear-gradient(var(--accentColor), var(--accentColor)),
        linear-gradient(-135deg, transparent 50%, var(--accentColor) 50%),
        linear-gradient(-225deg, transparent 50%, var(--accentColor) 50%),
        linear-gradient(var(--accentColor) 42%, var(--primaryColor) 42%);
      background-repeat: no-repeat, no-repeat, no-repeat, no-repeat;
      background-size: 1px 100%, 20px 22px, 20px 22px, 20px 100%;
      background-position: right 20px center, right bottom, right bottom,
        right bottom;
    }

    .is-visible {
      display: block !important;
    }

    .is-hidden {
      display: none !important;
    }

    h5 {
      margin-top: 8px;
      margin-bottom: 8px;
    }
  `.toString();
}

export function applyStyles() {
  const styleElement = document.createElement('style');
  styleElement.innerHTML = createComponentStyle();
  document.head.appendChild(styleElement);
}
