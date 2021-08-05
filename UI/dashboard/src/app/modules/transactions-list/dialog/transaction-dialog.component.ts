import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import axios from 'axios';
import { environment } from '../../../../environments/environment';

export interface DialogData {
  paymentId: string;
}

@Component({
  selector: 'transaction-dialog',
  templateUrl: './transaction-dialog.html',
  styleUrls: ['./transactions-dialog.scss'],
})
export class TransactionDialog {
  paymentDetails: any;
  constructor(
    public dialogRef: MatDialogRef<TransactionDialog>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {}

  async ngOnInit() {
    const url = `${environment.bccPayUrl}/Payment/${this.data.paymentId}`;
    this.paymentDetails = await axios.get(url);
  }

  onCloseClick(): void {
    this.dialogRef.close();
  }
}
