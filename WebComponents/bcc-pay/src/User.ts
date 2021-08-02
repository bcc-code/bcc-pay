export class User {
  email: string;
  phoneNumber: string;
  firstName: string;
  lastName: string;
  addressLine1: string;
  addressLine2: string;
  city: string;
  postalCode: string;

  constructor(
    paymentMethod: string,
    email: string,
    phoneNumber: string,
    firstName: string,
    lastName: string,
    addressLine1: string,
    addressLine2: string,
    city: string,
    postalCode: string
  ) {
    this.email = email;
    this.phoneNumber = phoneNumber;
    this.firstName = firstName;
    this.lastName = lastName;
    this.addressLine1 = addressLine1;
    this.addressLine2 = addressLine2;
    this.city = city;
    this.postalCode = postalCode;
  }
}
