import { html, fixture, expect } from '@open-wc/testing';

import { BccPay } from '../src/BccPay.js';
import '../bcc-pay.js';

describe('BccPay', () => {
  it('has a default title "Hey there" and counter 5', async () => {
    const el = await fixture<BccPay>(html`<bcc-pay></bcc-pay>`);

    expect(el.item).to.equal('Subscription');
    expect(el.cost).to.equal(5);
  });

  it('increases the counter on button click', async () => {
    const el = await fixture<BccPay>(html`<bcc-pay></bcc-pay>`);
    el.shadowRoot!.querySelector('button')!.click();

    expect(el.cost).to.equal(6);
  });

  it('can override the title via attribute', async () => {
    const el = await fixture<BccPay>(html`<bcc-pay currency="USD"></bcc-pay>`);

    expect(el.title).to.equal('USD');
  });

  it('passes the a11y audit', async () => {
    const el = await fixture<BccPay>(html`<bcc-pay></bcc-pay>`);

    await expect(el).shadowDom.to.be.accessible();
  });
});
