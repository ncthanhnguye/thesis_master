import Swal from 'sweetalert2';
import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({ providedIn: 'root' })
export class AppSwal {
    constructor(
        private translate: TranslateService
    ) {
    }

    async showSuccess(msg: any, isShowCancelButton: any) {

        const selectOption = await Swal.fire({
            title: msg,
            html: '<style>.swal-container {z-index: 99999}</style>',
            //text: "You won't be able to revert this!",
            customClass: {
                popup: 'swal-warning'
            },
            type: 'success',
            showCancelButton: isShowCancelButton,
            confirmButtonColor: '#007bff',
            cancelButtonColor: '#dc3545',
            cancelButtonText: this.translate.instant('Cancel'),
            confirmButtonText: this.translate.instant('Ok'),
        }).then((result) => {
            if (result.value) {
                return true;
            } else { return false; }
        });

        return selectOption;
    }

    async showWarning(msg: any, isShowCancelButton: any) {

        const selectOption = await Swal.fire({
            title: msg,
            html: '<style>.swal-container {z-index: 99999}</style>',
            //text: "You won't be able to revert this!",
            customClass: {
                container: 'swal-container',
                popup: 'swal-warning'
            },
            type: 'warning',
            showCancelButton: isShowCancelButton,
            confirmButtonColor: '#007bff',
            cancelButtonColor: '#dc3545',
            cancelButtonText: this.translate.instant('Cancel'),
            confirmButtonText: this.translate.instant('Ok'),
        }).then((result) => {
            if (result.value) {
                return true;
            } else { return false; }
        });

        return selectOption;
    }

    async showError(msg: any) {

        const selectOption = await Swal.fire({
            title: msg,
            html: '<style>.swal-container {z-index: 99999; background-color: #FFFFFF}</style>',
            type: 'error',
            showCancelButton: false,
            confirmButtonColor: '#007bff',
            cancelButtonColor: '#dc3545',
            cancelButtonText: this.translate.instant('Cancel'),
            confirmButtonText: this.translate.instant('Ok'),
        }).then((result) => {
            if (result.value) {
                return true;
            } else { return false; }
        });

        return selectOption;
    }

    async showOptionAdvance(msg: any, isShowCancelButton: any, html: any) {

        const selectOption = await Swal.fire({
            title: msg,
            html: html,
            customClass: 'swal-wide',
            showCancelButton: isShowCancelButton,
            confirmButtonColor: '#007bff',
            cancelButtonColor: '#dc3545',
            cancelButtonText: this.translate.instant('Cancel'),
            confirmButtonText: this.translate.instant('Ok'),
        }).then((result) => {
            if (result.value) {
                return true;
            } else { return false; }
        });

        return selectOption;
    }
}
