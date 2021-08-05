import { html, TemplateResult } from 'lit-html';
import '../bcc-pay.js';

export default {
  title: 'BccPay',
  component: 'bcc-pay',
  argTypes: {
    item: { control: 'text' },
    amount: { control: 'number' },
    currency: { control: 'text' },
    country: { control: 'text' },
    server: { control: 'text' },
    netsCheckoutKey: { control: 'text' },
    isDevEnv: { control: 'boolean' },
  },
};

interface Story<T> {
  (args: T): TemplateResult;
  args?: Partial<T>;
  argTypes?: Record<string, unknown>;
}

interface ArgTypes {
  item?: string;
  amount?: number;
  currency?: string;
  country?: string;
  server?: string;
  netsCheckoutKey?: string;
  isDevEnv?: boolean;
  slot?: TemplateResult;
}

const Template: Story<ArgTypes> = ({
  item = 'Subscription',
  amount = 5,
  currency = 'NOK',
  country = 'NOR',
  server = 'https://localhost:5001',
  netsCheckoutKey = '#netsCheckoutKey#',
  isDevEnv = true,
  slot,
}: ArgTypes) => html`
  <bcc-pay
    .item=${item}
    .amount=${amount}
    .currency=${currency}
    .country=${country}
    .server=${server}
  >
    ${slot}
  </bcc-pay>
`;

export const Regular = Template.bind({});

export const CustomTitle = Template.bind({});
CustomTitle.args = {
  item: 'New item',
};

export const SlottedContent = Template.bind({});
SlottedContent.args = {
  slot: html`<p>Slotted content</p>`,
};
SlottedContent.argTypes = {
  slot: { table: { disable: true } },
};
