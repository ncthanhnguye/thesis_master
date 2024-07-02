import { Injectable } from '@angular/core';
import { Observable } from 'rxjs-compat/Observable';

@Injectable()
export class WebSocketService {
  public ws: WebSocket;

  public createObservableSocket(url: string): Observable<string> {
    this.ws = new WebSocket(url);

    return new Observable(observer => {
      this.ws.onmessage = event => observer.next(event.data);
      this.ws.onerror = event => observer.error(event);
      this.ws.onclose = event => observer.complete();
    });

  }

  public sendMessage(message: any, url: string) {
    var conn = new WebSocket(url);
    conn.onmessage = function(e){  };
    conn.onopen = () => conn.send(message);
  }
}