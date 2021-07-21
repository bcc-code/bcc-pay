# Dashboard

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 12.1.2.

## Run project

`npm install`
`ng serve`

Navigate to `http://localhost:4200/`

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Deploy

There is dockerfile with nginx configured, sample cloudrun deploy:

`gcloud builds submit --tag gcr.io/[GcpProjectId]/bcc-pay-front-dev && gcloud run deploy bcc-pay-front-dev --image gcr.io/[GcpProjectId]/bcc-pay-front-dev --platform=managed --region=europe-west1 --project=[GcpProjectId] --allow-unauthenticated`

## Tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).
Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.
