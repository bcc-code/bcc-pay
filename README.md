# BCC Pay

Core components for a multi-provider payment gateway for BCC.

# Api

To run Api go to sample app /Api/BccPay.Core.Sample/ and type
`dotnet run`

# WebComponent

For more info about webcomponent go to /WebComponents/README.md file.

To use webComponent in application type:
`npm install bcc-pay`

You have to always pass your netsCheckoutKey from nets admin panel. Sample uses of component:

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

You can also pass user object to component:

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

<bcc-pay item="Subscpription" currency="NOK" country="NOR" amount="12" server="https://localhost:5001" user=${this.sampleUser} netsCheckoutKey="#netsCheckoutKey#"></bcc-pay>

# Admin panel

To run admin panel application go to /UI/dashboard/
`ng serve`

- [ ] Core API
- [ ] Tenant configuration framework
- [ ] Plugin / adapter framework
- [ ] Core UI Components (e.g. web components)
