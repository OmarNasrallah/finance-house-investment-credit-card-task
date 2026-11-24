# Investment-Backed Credit Card: Frontend

Angular 20, module-based (NgModules, `standalone: false`), Reactive Forms.

## Run

```bash
npm install
npm start          # http://localhost:4200
```

Start the backend first (`http://localhost:5080`). `/api` calls are proxied there via `proxy.conf.json`.

## Structure

```
src/app/
  app-module.ts                 root module (HttpClient)
  app-routing-module.ts         lazy-loads CreditCardModule
  core/                         API service + models
  credit-card/
    credit-card-module.ts       feature module
    dashboard/                  customer picker, data loading, manual job trigger
    portfolio-list/             step 1: view portfolios
    issue-card-form/            step 2: lien + issue (reactive form)
    card-list/                  step 3: use / repay / close card
    redeem-form/                step 4: redemption (shows the API's violation reason)
    audit-trail/                lien and card activity
```

Forms only validate input shape (required, > 0). Business rules are enforced by the API's Domain layer,
and its violation messages are shown as returned, so you can see the rules being enforced.
