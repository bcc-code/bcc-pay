class BccPay extends HTMLElement {
  connectedCallback() {
    this.innerHTML = `<h1>Hello from BCC PAY</h1>`;
  }
}

customElements.define("bcc-pay", BccPay);
