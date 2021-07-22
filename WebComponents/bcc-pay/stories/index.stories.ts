import { html, TemplateResult } from 'lit-html';
import '../bcc-pay.js';

export default {
  title: 'BccPay',
  component: 'bcc-pay',
  argTypes: {
    item: { control: 'text' },
    cost: { control: 'number' },
    currency: { control: 'text' },
    server: { control: 'text' },
  },
};

interface Story<T> {
  (args: T): TemplateResult;
  args?: Partial<T>;
  argTypes?: Record<string, unknown>;
}

interface ArgTypes {
  item?: string;
  cost?: number;
  currency?: string;
  server?: string;
  slot?: TemplateResult;
}

const Template: Story<ArgTypes> = ({
  item = 'Subscription',
  cost = 5,
  currency = 'NOK',
  server = 'http://localhost:3000',
  slot,
}: ArgTypes) => html`
  <bcc-pay .item=${item} .cost=${cost} .currency=${currency} .server=${server}>
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
