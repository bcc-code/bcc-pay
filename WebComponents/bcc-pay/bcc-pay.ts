import { BccPay } from './src/BccPay.js';

window.customElements.define('bcc-pay', BccPay);

declare global {
  interface HTMLElementTagNameMap {
    'bcc-pay': BccPay;
  }
}
