import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { Component, Inject } from '@angular/core';

@Component({
    selector: 'dialog-overview',
    templateUrl: './dialog.html'
})
export class DialogOverview {

    constructor(
        public dialogRef: MatDialogRef<DialogOverview>,
        @Inject(MAT_DIALOG_DATA) public data: any) { }

    close(): void {
        this.dialogRef.close();
    }
}