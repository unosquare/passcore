import { AppModule } from './app/app.module';
import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

enableProdMode();
fetch('api/password').then(response => response.json()).then(config => {
  window['lang'] = config.recaptcha.languageCode;
  platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.log(err));
});
