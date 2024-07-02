import { Injectable } from '@angular/core';
import * as XLSX from 'xlsx';

@Injectable({
    providedIn: 'root'
})
export class AppFile {
    constructor() {

    }

    async readXLSX(rawFile: any) {
        const reader: FileReader = new FileReader();
        return new Promise((resolve, reject) => {
            reader.onerror = () => {
                reader.abort();
                reject(new DOMException('Problem parsing input file.'));
            };

            reader.onload = (e: any) => {
                const bstr: string = e.target.result;
                const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });
                const wsname: string = wb.SheetNames[0];
                const ws: XLSX.WorkSheet = wb.Sheets[wsname];
                const data = XLSX.utils.sheet_to_json(ws, { header: 1 });
                resolve(data);
            };
            reader.readAsBinaryString(rawFile);
        });
    }

    async readImage(rawFile: any) {

        const reader: FileReader = new FileReader();
        return new Promise((resolve, reject) => {
            reader.onerror = () => {
                reader.abort();
                reject(new DOMException('Problem parsing input file.'));
            };

            reader.onload = (e: any) => {
                const bstr: string = e.target.result;
                resolve(bstr);
            };
            reader.readAsDataURL(rawFile);
        });
    }

    async readFile(rawFile: any) {
        
        const reader: FileReader = new FileReader();
        return new Promise((resolve, reject) => {
            reader.onerror = () => {
                reader.abort();
                reject(new DOMException('Problem parsing input file.'));
            };

            reader.onload = (e: any) => {
                const bstr: string = e.target.result;
                resolve(bstr);
            };
            reader.readAsBinaryString(rawFile);
        });
    }
    
    async read(rawFile: any) {
        
        const reader: FileReader = new FileReader();
        return new Promise((resolve, reject) => {
            reader.onerror = () => {
                reader.abort();
                reject(new DOMException('Problem parsing input file.'));
            };

            reader.onload = (e: any) => {
                const bstr: string = e.target.result;
                resolve(bstr);
            };
            reader.readAsText(rawFile);
        });
    }
}
