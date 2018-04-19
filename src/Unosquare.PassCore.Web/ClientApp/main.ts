import 'core-js/library'; // Provides polyfills
import 'zone.js/dist/zone';  // Included with Angular CLI.
import { AppModule } from './app/app.module';
import { enableProdMode } from '@angular/core';
import { platformBrowser } from '@angular/platform-browser';

// enableProdMode();
const platform = platformBrowser();
platform.bootstrapModule(AppModule);