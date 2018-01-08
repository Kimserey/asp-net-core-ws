import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';

@Injectable()
export class WebsocketService {
  observable: Observable<MessageEvent>;
  private ws: WebSocket;

  connect(username, url): Observable<MessageEvent> {
    if (!this.observable) {
      this.observable = this.createObservable(username, url);
      console.log('Successfully connected: ' + url);
    }
    return this.observable;
  }

  send(message) {
    if (this.ws.readyState === WebSocket.OPEN) {
      this.ws.send(JSON.stringify(message));
    }
  }

  private createObservable(username, url): Subject<any> {
    this.ws = new WebSocket(`${url}?username=${username}`);

    return Observable.create(
      (obs: Observer<MessageEvent>) => {
        this.ws.onmessage = ev => {
          obs.next(Object.assign({}, ev, { data: JSON.parse(ev.data) }));
        };
        this.ws.onerror = obs.error.bind(obs);
        this.ws.onclose = obs.complete.bind(obs);
        return this.ws.close.bind(this.ws);
      }
    );
  }
}
