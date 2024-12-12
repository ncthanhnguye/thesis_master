import { Injectable } from '@angular/core';
import * as mammoth from 'mammoth';
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

    async readDocx(rawFile: any, convertType: 'html' | 'text'): Promise<string> {
        const reader: FileReader = new FileReader();
        return new Promise((resolve, reject) => {
            reader.onerror = () => {
                reader.abort();
                reject(new DOMException('Problem parsing input file.'));
            };
    
            reader.onload = async (e: any) => {
                const arrayBuffer: ArrayBuffer = e.target.result;
                try {
                    if (convertType === 'html') {
                        const result = await mammoth.convertToHtml({ arrayBuffer });
                        resolve(result.value); // Trả về dữ liệu dạng HTML
                    } else if (convertType === 'text') {
                        const result = await mammoth.extractRawText({ arrayBuffer });
                        resolve(result.value); // Trả về dữ liệu dạng văn bản thuần
                    } else {
                        reject(new Error('Invalid convertType specified. Use "html" or "text".'));
                    }
                } catch (error) {
                    reject(new DOMException('Problem parsing .docx file.'));
                }
            };
            reader.readAsArrayBuffer(rawFile);
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
