# BCC Pay

Core components for a multi-provider payment gateway for BCC.

# Api

To run Api go to sample app /Api/BccPay.Core.Sample/ and type
`dotnet run`

# WebComponent

To run webComponent in dev mode go to /webComponent/bcc-pay/
`npm run storybook`

To use webComponent in external application you have to upgrade version number in package.json and publish component with command:
`npm publish`

In actual application type:
`npm install bcc-pay`

Then you can use webComponent with one of options presented below:

<bcc-pay></bcc-pay>

<bcc-pay item="Subscpription" currency="NOK" country="NOR" amount="10" server="http://localhost:5001"></bcc-pay>

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

<bcc-pay item="Subscpription" currency="NOK" country="NOR" amount="12" server="https://localhost:5001" user=${this.sampleUser}></bcc-pay>

# Admin panel

To run admin panel application go to /UI/dashboard/
`ng serve`

- [ ] Core API
- [ ] Tenant configuration framework
- [ ] Plugin / adapter framework
- [ ] Core UI Components (e.g. web components)
