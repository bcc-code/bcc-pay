# BCC Pay

Core components for a multi-provider payment gateway for BCC.

# Api

To run Api go to sample app /Api/BccPay.Core.Sample/ and type
`dotnet restore dotnet run`

# WebComponent

For more info about webcomponent go to /WebComponents/README.md file.

To use webComponent in application type:

`npm install bcc-pay@latest`

You have to always pass netsCheckoutKey and amount:

```html
<bcc-pay netsCheckoutKey="#netsCheckoutKey#" amount="12"></bcc-pay>

<bcc-pay
  item="Subscpription"
  currency="NOK"
  country="NOR"
  amount="10"
  server="https://localhost:5001"
  netsCheckoutKey="#netsCheckoutKey#"
  isDevEnv="true"
></bcc-pay>
```

Full list of properties you can pass to WebComponent with default values:

```JavaScript
  @property({ type: String }) item = 'Subscription';
  @property({ type: Number }) amount = 0;
  @property({ type: String }) currency = 'NOK';
  @property({ type: String }) country = 'NOR';
  @property({ type: String }) paymentType = '';
  @property({ type: User }) user: User = {};
  @property({ type: String }) server = 'https://localhost:5001';
  @property({ type: String }) netsCheckoutKey = '#checkout_key#';
  @property({ type: Boolean }) isDevEnv: boolean = false;
  @property({ type: [RequestHeader] }) requestHeaders:
    | [RequestHeader]
    | undefined;
  @property({ type: String }) paymentId: string = '';
  @property({ type: String }) primaryColor: string = '#006fc2';
  @property({ type: String }) secondaryColor: string = 'white';
  @property({ type: String }) accentColor: string = '#bae1ff';
```

# Admin panel

To run admin panel application go to /UI/dashboard/

```
npm install
ng serve
```

- [ ] Core API
- [ ] Tenant configuration framework
- [ ] Plugin / adapter framework
- [ ] Core UI Components (e.g. web components)
