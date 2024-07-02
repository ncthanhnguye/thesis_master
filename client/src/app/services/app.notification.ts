import { NotificationService } from '@progress/kendo-angular-notification';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class Notification {

    private hideAfter = 2000;

    constructor(
        private notificationService: NotificationService
    ) {}

    showSuccess(msg: any) {
        this.notificationService.show({
            content: msg,
            position: { horizontal: 'right', vertical: 'bottom' },
            animation: { type: 'fade', duration: 800 },
            type: { style: 'success', icon: true },
            hideAfter: this.hideAfter,
        });
    }
    showError(msg: any) {
        this.notificationService.show({
            content: msg,
            position: { horizontal: 'right', vertical: 'bottom' },
            animation: { type: 'fade', duration: 800 },
            type: { style: 'error', icon: true },
            hideAfter: this.hideAfter,
        });
    }
}
