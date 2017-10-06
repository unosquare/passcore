import { MD_DIALOG_DATA, MdDialogRef } from '@angular/material';
import { Component, Inject } from '@angular/core';

@Component({
    selector: 'dialog-overview',
    templateUrl: '../views/dialog.html'
})
export class DialogOverview {

    constructor(
        public dialogRef: MdDialogRef<DialogOverview>,
        @Inject(MD_DIALOG_DATA) public data: any) { }

    close(): void {
        this.dialogRef.close();
    }
}