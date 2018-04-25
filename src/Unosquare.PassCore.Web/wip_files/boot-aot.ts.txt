import { platformBrowser } from "@angular/platform-browser";
import { AppModuleNgFactory } from "aot/app/app.module.ngfactory";
import { enableProdMode } from '@angular/core';
declare var process;
if (process.env.ENV === 'production') {
    console.log("PROD MODE");
    enableProdMode();
}
platformBrowser().bootstrapModuleFactory(AppModuleNgFactory);