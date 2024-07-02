import { Injectable } from '@angular/core';
import { HttpRequest } from '@angular/common/http';
@Injectable()
export class AuthService {
    cachedRequests: Array<HttpRequest<any>> = [];
    public getToken(): string {
        return localStorage.getItem('token');
    }
    public isAuthenticated(): boolean {
        // get the token
        const token = this.getToken();
        // return a boolean reflecting 
        // whether or not the token is expired
        return token !== null;
    }
    public collectFailedRequest(request): void {
        this.cachedRequests.push(request);
    }
    public retryFailedRequests(): void {
        // retry the requests. this method can
        // be called after the token is refreshed
    }
}