# \<bcc-pay>

This webcomponent follows the [open-wc](https://github.com/open-wc/open-wc) recommendation.

Install of component:
`npm install bcc-pay`

For angular module you have to add CUSTOM_ELEMENTS_SCHEMA from '@angular/core'

```JavaScript
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
@NgModule({
  declarations: [SampleComponent],
  imports: [CommonModule, SampleRoutingModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
```

Then you can use webComponent with one of options presented below, you have to always pass your netsCheckoutKey from nets admin panel:

```
> https://portal.dibspayment.eu/dashboard
> Company
> Integration
> Checkout key
```

Examples of using component

```html
<bcc-pay netsCheckoutKey="#netsCheckoutKey#" isDevEnv="true"></bcc-pay>

<bcc-pay
  item="Subscpription"
  currency="NOK"
  country="NOR"
  amount="10"
  server="https://localhost:5001"
  netsCheckoutKey="#netsCheckoutKey#"
></bcc-pay>
```

You can also pass user object to component:

JS

```JavaScript
const sampleUser = {
    email: 'doe@test.no',
    phoneNumber: '+47661626839',
    firstName: 'John',
    lastName: 'Doe',
    addressLine1: 'TestAddressLine1',
    addressLine2: 'TestAddressLine2',
    city: 'Oslo',
    postalCode: '0001',
  };
```

HTML

```html
<bcc-pay
  item="Subscpription"
  currency="NOK"
  country="NOR"
  amount="12"
  server="https://localhost:5001"
  user="${this.sampleUser}"
  netsCheckoutKey="#netsCheckoutKey#"
></bcc-pay>
```

You can also pass additional headers for requests:

Request model

```TypeScript
export class RequestHeader {
  key: string;
  value: string;
  }
```

Sample in JS

```Javascript
const requestHeaders = [{ key: 'new_key', value: 'new_value' }],
```

HTML

```html
<bcc-pay
  requestHeaders="${this.requestHeaders}"
  netsCheckoutKey="#netsCheckoutKey#"
  isDevEnv="true"
></bcc-pay>
```

List of possible parameters and default values:

```Typescript
  @property({ type: String }) item = 'Subscription';
  @property({ type: Number }) amount = 5;
  @property({ type: String }) currency = 'EUR';
  @property({ type: String }) country = 'DE';
  @property({ type: User }) user: User = {};
  @property({ type: String }) server = 'https://localhost:5001';
  @property({ type: String }) netsCheckoutKey = '#checkout_key#';
  @property({ type: Boolean }) isDevEnv: boolean = false;
  @property({ type: [RequestHeader] }) requestHeaders:
    | [RequestHeader]
    | undefined;
  @property({ type: String }) paymentId: string = '';
```
