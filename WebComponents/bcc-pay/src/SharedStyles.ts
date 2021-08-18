import { css, unsafeCSS } from 'lit-element';
import { primaryColor, secondaryColor } from './BccPay';

export const componentStyle = css`
  :root {
    --baseFg: dimgray;
    --baseBg: white;
    --accentFg: #006fc2;
    --accentBg: #bae1ff;
    --primaryColor: ${unsafeCSS('hsl(258, 71%, 61%)')};
    --secondaryColor: ${unsafeCSS('')};
  }

  .card-square {
    box-sizing: border-box;
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

  .card-title {
    font-size: 1.375rem;
    font-weight: 700;
  }

  .card-price {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 27px 30px;
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
    font-size: 1.125rem;
    font-weight: 900;
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
  }
  .payment-button:disabled {
    background-color: #cccccc;
    color: #666666;
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
    align-items: center;
    margin-top: 12px;
    justify-content: space-between;
  }
  .country-select {
  }

  select {
    font: 400 12px/1.3 sans-serif;
    -webkit-appearance: none;
    appearance: none;
    color: var(--baseFg);
    border: 1px solid var(--baseFg);
    line-height: 1;
    outline: 0;
    padding: 0.65em 2.5em 0.55em 0.75em;
    border-radius: 2px;
    background-color: var(--baseBg);
    background-image: linear-gradient(var(--baseFg), var(--baseFg)),
      linear-gradient(-135deg, transparent 50%, var(--accentBg) 50%),
      linear-gradient(-225deg, transparent 50%, var(--accentBg) 50%),
      linear-gradient(var(--accentBg) 42%, var(--accentFg) 42%);
    background-repeat: no-repeat, no-repeat, no-repeat, no-repeat;
    background-size: 1px 100%, 20px 22px, 20px 22px, 20px 100%;
    background-position: right 20px center, right bottom, right bottom,
      right bottom;
  }
`;

export function applyStyles() {
  const styleElement = document.createElement('style');
  styleElement.innerHTML = componentStyle.toString();
  document.head.appendChild(styleElement);
}
